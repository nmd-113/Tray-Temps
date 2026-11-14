using System;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class FpsLimiterNoticesDialog : Form
    {
        internal FpsLimiterNoticesDialog()
        {
            InitializeComponent();
        }

        internal FpsLimiterNoticesDialog(string notices, MainForm owner) : this()
        {
            noticeText.Text = (notices ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Replace("\n", Environment.NewLine);
            var theme = ThemePalette.Create(owner != null && owner.IsLightModeEnabled);
            BackColor = theme.WindowBack;
            layout.BackColor = theme.WindowBack;
            noticeText.BackColor = theme.InputBack;
            noticeText.ForeColor = theme.Text;
            close.BackColor = theme.ButtonBack;
            close.ForeColor = theme.BodyText;
            close.FlatAppearance.BorderColor = theme.Border;
            close.FlatAppearance.MouseOverBackColor = theme.ButtonHover;
            close.FlatAppearance.MouseDownBackColor = theme.ButtonDown;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            noticeText.Select(0, 0);
            noticeText.ScrollToCaret();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!EmbeddedFonts.IsDesignTime(this))
                WindowCornerHelper.ApplyRoundedCorners(Handle);
        }
    }
}
