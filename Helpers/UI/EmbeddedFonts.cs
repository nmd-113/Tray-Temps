using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrayTemps
{
    internal static class EmbeddedFonts
    {
        private const string BookResourceName = "TrayTemps.Resources.bunken_book.ttf";
        private const string BoldResourceName = "TrayTemps.Resources.bunken_bold.ttf";

        private static readonly object SyncRoot = new object();
        private static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        private static readonly List<IntPtr> FontMemory = new List<IntPtr>();
        private static readonly List<IntPtr> FontResourceHandles = new List<IntPtr>();
        private static bool _initialized;

        public static FontFamily Book { get; private set; }
        public static FontFamily Bold { get; private set; }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(
            IntPtr pbFont,
            uint cbFont,
            IntPtr pdv,
            ref uint pcFonts);

        [DllImport("gdi32.dll")]
        private static extern bool RemoveFontMemResourceEx(IntPtr handle);

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (SyncRoot)
            {
                if (_initialized)
                    return;

                Book = LoadFontFamily(BookResourceName);
                Bold = LoadFontFamily(BoldResourceName);

                AppDomain.CurrentDomain.ProcessExit += (s, e) => ReleaseFontMemory();
                AppDomain.CurrentDomain.DomainUnload += (s, e) => ReleaseFontMemory();

                _initialized = true;
            }
        }

        public static void ApplyTo(Control root)
        {
            if (root == null)
                return;

            Initialize();
            ApplyFont(root);

            foreach (Control child in root.Controls)
                ApplyTo(child);

            if (root.ContextMenuStrip != null)
                ApplyTo(root.ContextMenuStrip);
        }

        public static void ApplyTo(ToolStrip toolStrip)
        {
            if (toolStrip == null)
                return;

            Initialize();
            // Safely replace the ToolStrip font and dispose the previous font if it is not shared.
            Font oldFont = toolStrip.Font;
            Font newFont = CreateReplacementFont(oldFont, false);
            toolStrip.Font = newFont;
            if (oldFont != null && !ReferenceEquals(oldFont, newFont) && !ReferenceEquals(oldFont, toolStrip.Parent?.Font))
                oldFont.Dispose();

            foreach (ToolStripItem item in toolStrip.Items)
                ApplyTo(item);
        }

        private static void ApplyTo(ToolStripItem item)
        {
            if (item == null)
                return;

            // Safely replace the ToolStripItem font and dispose the previous font if it is not shared.
            Font oldFont = item.Font;
            Font newFont = CreateReplacementFont(oldFont, false);
            item.Font = newFont;
            if (oldFont != null && !ReferenceEquals(oldFont, newFont) && !ReferenceEquals(oldFont, item.Owner?.Font))
                oldFont.Dispose();

            if (item is ToolStripDropDownItem dropDownItem)
            {
                foreach (ToolStripItem child in dropDownItem.DropDownItems)
                    ApplyTo(child);
            }
        }

        private static void ApplyFont(Control control)
        {
            if (control == null) return;
            bool preserveSystemFont = IsSystemSpecialCase(control);
            // Capture the old font, assign the replacement, then dispose the old font if it is safe to do so.
            Font oldFont = control.Font;
            Font newFont = CreateReplacementFont(oldFont, preserveSystemFont);
            control.Font = newFont;
            if (!preserveSystemFont && oldFont != null && !ReferenceEquals(oldFont, newFont) && !ReferenceEquals(oldFont, control.Parent?.Font))
                oldFont.Dispose();
        }

        private static Font CreateReplacementFont(Font source, bool preserveSystemFont)
        {
            if (source == null || source.FontFamily == null)
                return source;

            if (preserveSystemFont)
                return source;

            FontFamily family = source.Style.HasFlag(FontStyle.Bold) ? Bold : Book;

            return new Font(
                family,
                source.Size,
                source.Style,
                source.Unit,
                source.GdiCharSet,
                source.GdiVerticalFont);
        }

        private static bool IsSystemSpecialCase(Control control)
        {
            if (control is RichTextBox || control is TextBoxBase)
            {
                string name = control.Font?.FontFamily?.Name ?? string.Empty;
                return name.Equals("Consolas", StringComparison.OrdinalIgnoreCase);
            }

            string controlName = control.Name ?? string.Empty;

            if (controlName.Equals("iconLabel", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static FontFamily LoadFontFamily(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Font resource not found: " + resourceName);

                byte[] fontData = new byte[stream.Length];
                int offset = 0;
                while (offset < fontData.Length)
                {
                    int read = stream.Read(fontData, offset, fontData.Length - offset);
                    if (read == 0)
                        break;
                    offset += read;
                }

                IntPtr fontPointer = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPointer, fontData.Length);
                FontCollection.AddMemoryFont(fontPointer, fontData.Length);

                uint fontCount = 0;
                IntPtr fontResourceHandle = AddFontMemResourceEx(
                    fontPointer,
                    (uint)fontData.Length,
                    IntPtr.Zero,
                    ref fontCount);

                if (fontResourceHandle != IntPtr.Zero)
                    FontResourceHandles.Add(fontResourceHandle);

                FontMemory.Add(fontPointer);

                return FontCollection.Families[FontCollection.Families.Length - 1];
            }
        }

        private static void ReleaseFontMemory()
        {
            lock (SyncRoot)
            {
                foreach (IntPtr handle in FontResourceHandles)
                {
                    if (handle != IntPtr.Zero)
                        RemoveFontMemResourceEx(handle);
                }

                FontResourceHandles.Clear();

                foreach (IntPtr pointer in FontMemory)
                {
                    if (pointer != IntPtr.Zero)
                        Marshal.FreeCoTaskMem(pointer);
                }

                FontMemory.Clear();
                FontCollection.Dispose();
            }
        }
    }
}
