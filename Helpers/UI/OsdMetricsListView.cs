using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrayTemps
{
    // Keep native checkbox behavior for visibility, but make label double-clicks
    // edit-only. Native ListView otherwise also toggles the checked state.
    [System.ComponentModel.ToolboxItem(false)]
    public sealed class OsdMetricsListView : ListView
    {
        public event EventHandler ViewChanging;

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == 0x0114 || message.Msg == 0x0115 || message.Msg == 0x020A)
                ViewChanging?.Invoke(this, EventArgs.Empty);
            if (message.Msg == 0x0203)
            {
                var point = new Point((short)message.LParam.ToInt64(), (short)(message.LParam.ToInt64() >> 16));
                var hit = HitTest(point);
                if (hit.Item != null && hit.Item.SubItems.Count > 1 && hit.SubItem != hit.Item.SubItems[0])
                {
                    hit.Item.Selected = true; hit.Item.Focused = true;
                    OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, point.X, point.Y, 0));
                    return;
                }
            }
            base.WndProc(ref message);
        }
    }
}
