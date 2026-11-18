using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using FpsLimiter.Core;
using TrayTemps;

internal static class AppAuditTests
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int _checks;
    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong(IntPtr window, int index);
    private sealed class Host : MainForm { protected override void OnLoad(EventArgs e) { } }
    private static T Find<T>(Control root, string name) where T : Control => (T)root.Controls.Find(name, true).Single();
    private static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        Console.WriteLine("PASS " + (++_checks) + ": " + message);
    }
    private static Task Refresh(HardwareDetailsDialog dialog) =>
        (Task)typeof(HardwareDetailsDialog).GetMethod("UpdateLiveSensorsAsync", Private).Invoke(dialog, null);
    private static void StopTimer(HardwareDetailsDialog dialog) =>
        ((Timer)typeof(HardwareDetailsDialog).GetField("_liveTimer", Private).GetValue(dialog)).Stop();

    internal static async Task Run()
    {
        _checks = 0;
        UpdateResponses();
        using (var host = new Host())
        {
            host.Show();
            foreach (bool light in new[] { false, true })
            {
                Find<CheckBox>(host, "lightModeSwitch").Checked = light;
                foreach (int count in new[] { 0, 64 })
                {
                    var config = new FpsLimiterConfiguration();
                    for (int i = 0; i < count; i++) config.GameLimits.Add(new GameLimit { ExecutablePath = @"C:\Games\Game" + i + ".exe" });
                    using (var dialog = new FpsLimiterSettingsDialog(host, config, null))
                    {
                        dialog.Show(host);
                        foreach (Size size in new[] { new Size(650, 500), new Size(920, 650), new Size(650, 500) })
                        {
                            dialog.Size = size;
                            await Task.Delay(30);
                            var games = Find<ListView>(dialog, "games");
                            Check((GetWindowLong(games.Handle, -16) & 0x100000) == 0,
                                "profile list fits without horizontal scrolling: " + count + " profiles, " + size + ", light=" + light);
                            var path = Find<Label>(dialog, "pathHeader");
                            var cap = Find<Label>(dialog, "capHeader");
                            Check(path.Left == games.Columns[0].Width && cap.Left == games.Columns[0].Width + games.Columns[1].Width &&
                                games.Items.Count == count,
                                "profile captions remain aligned with every data column after resizing");
                        }
                        dialog.Close();
                    }
                }
            }
            await LiveSensorUpdates(host);
            host.Close();
        }
        Console.WriteLine("ALL APP AUDIT CHECKS PASSED: " + _checks);
    }

    private static void UpdateResponses()
    {
        Check(UpdateCheckHelper.TryGetLatestGitHubTag("[null,42,{}, {\"name\":42}, {\"name\":null}, {\"name\":\"v2.4.0.0\"}, {\"name\":\"v2.5.1.0\"}]",
            out Version version, out string tag) && version == new Version(2, 5, 1, 0) && tag == "v2.5.1.0",
            "malformed tag entries cannot crash update checking or hide a valid release");
        Check(!UpdateCheckHelper.TryGetLatestGitHubTag("[null,{}, {\"name\":false}, {\"name\":\"preview\"}]", out version, out tag) &&
            version == null && tag == null, "responses without a valid version return no release");
        Check(!UpdateCheckHelper.TryGetLatestGitHubTag("{\"message\":\"Service unavailable\"}", out version, out tag),
            "service-error objects are handled without treating them as release lists");
    }

    private static async Task LiveSensorUpdates(Form host)
    {
        int calls = 0;
        bool stopping = false;
        bool fail = false;
        string sample = "CPU temperature: 55 C\r\nCPU load: 12%";
        using (var dialog = new HardwareDetailsDialog("Example CPU", "CPU", "Example specifications",
            () => { calls++; if (fail) throw new InvalidOperationException("Controlled sensor failure"); return Task.FromResult(sample); }, () => stopping))
        {
            dialog.Show(host);
            await Task.Delay(30);
            StopTimer(dialog);
            var box = Find<RichTextBox>(dialog, "_liveBox");
            Check(calls == 1 && box.Text.Replace("\r\n", "\n") == sample.Replace("\r\n", "\n"),
                "showing hardware details immediately reads the first live sample");
            var subtitle = Find<Label>(dialog, "_subtitleLabel");
            Check(subtitle.Parent.ClientRectangle.Contains(subtitle.Bounds), "hardware subtitle fits entirely inside its title bar");
            box.Select(4, 11);
            sample = "CPU temperature: 56 C\r\nCPU load: 13%";
            await Refresh(dialog);
            Check(box.Text.Replace("\r\n", "\n") == sample.Replace("\r\n", "\n") && box.SelectionStart == 4 && box.SelectionLength == 11,
                "changed sensor readings preserve the user's text selection");
            int textChanges = 0;
            box.TextChanged += (s, e) => textChanges++;
            await Refresh(dialog);
            Check(textChanges == 0 && box.SelectionLength == 11,
                "unchanged readings avoid replacing the native text buffer or clearing its selection");
            sample = "CPU";
            await Refresh(dialog);
            Check(box.Text == sample && box.SelectionStart <= box.TextLength &&
                box.SelectionStart + box.SelectionLength <= box.TextLength,
                "shorter readings clamp the saved selection to the new text");
            fail = true;
            await Refresh(dialog);
            Check(box.Text == "Error reading sensors.", "a failed sensor query displays its unavailable state");
            fail = false;
            await Refresh(dialog);
            Check(box.Text == sample, "the same sample can recover after a query error without stale text-cache suppression");
            int before = calls;
            dialog.WindowState = FormWindowState.Minimized;
            await Refresh(dialog);
            Check(calls == before, "minimized hardware dialogs perform no live sensor reads");
            dialog.WindowState = FormWindowState.Normal;
            dialog.Hide();
            await Refresh(dialog);
            Check(calls == before, "hidden hardware dialogs perform no live sensor reads");
            dialog.Show(host);
            await Refresh(dialog);
            Check(calls == before + 1, "visible hardware dialogs resume live reads");
            stopping = true;
            await Refresh(dialog);
            Check(calls == before + 1, "shutdown prevents a new live query from starting");
            dialog.Close();
            await Refresh(dialog);
            Check(calls == before + 1, "closed hardware dialogs perform no further reads");
        }

        var pending = new TaskCompletionSource<string>();
        using (var dialog = new HardwareDetailsDialog("Pending CPU", "CPU", "Specifications", () => pending.Task))
        {
            dialog.Show(host);
            await Task.Delay(30);
            StopTimer(dialog);
            dialog.Close();
            pending.SetResult("late sample");
            await Task.Delay(30);
            Check(dialog.IsDisposed, "a pending live query completes safely after the hardware window closes");
        }
    }
}
