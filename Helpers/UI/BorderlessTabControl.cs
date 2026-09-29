using System.Windows.Forms;

namespace TrayTemps
{
    /// <summary>
    /// A tab host for custom navigation surfaces.  The native tab control still
    /// provides designer support, page ownership, and keyboard navigation, but
    /// does not reserve an inset client rectangle for an invisible tab strip.
    /// </summary>
    public sealed class BorderlessTabControl : TabControl
    {
        private const int TcmAdjustRect = 0x1328;

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == TcmAdjustRect)
            {
                message.Result = (System.IntPtr)1;
                return;
            }

            base.WndProc(ref message);
        }
    }
}
