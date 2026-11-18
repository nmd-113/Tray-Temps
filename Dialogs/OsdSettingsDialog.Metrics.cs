using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class OsdSettingsDialog
    {
        // Dialog presentation only; Layout continues to follow the saved ItemOrder.
        private static readonly int[] MetricDisplayOrder = Enumerable.Range(0, OsdMetricDefinitions.Keys.Length)
            .OrderBy(index => OsdMetricDefinitions.Keys[index] == "CurrentTime" ? 1 : 0).ToArray();
        private OsdConfiguration _metricDraft = new OsdConfiguration();
        private bool _loadingMetricList, _loadingLabelEditor, _endingLabelEdit, _loadingAppearance;
        private bool _handlingMetricCheck;
        private string _editingMetricKey, _originalLabelText;
        private int _editingColumn = 1;
        private Color _disabledMetricColor = Color.Gray;
        private Font _appearanceBodyFont, _appearanceHeaderFont;

        private void InitializeAppearanceFonts()
        {
            // Preserve Designer sizes and map its bold header style to the embedded extra-bold face.
            EmbeddedFonts.Initialize();
            Font designerHeaderFont = appearanceGrid.ColumnHeadersDefaultCellStyle.Font ?? appearanceGrid.Font;
            _appearanceBodyFont = EmbeddedFonts.CreateUiFont(appearanceGrid.DefaultCellStyle.Font ?? appearanceGrid.Font);
            _appearanceHeaderFont = new Font(EmbeddedFonts.ExtraBold, designerHeaderFont.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void DisposeAppearanceFonts()
        {
            _appearanceBodyFont?.Dispose(); _appearanceBodyFont = null;
            _appearanceHeaderFont?.Dispose(); _appearanceHeaderFont = null;
        }

        private void PopulateMetricList()
        {
            _loadingMetricList = true; metricsList.BeginUpdate();
            try
            {
                metricsList.Items.Clear();
                foreach (int index in MetricDisplayOrder)
                {
                    string key = OsdMetricDefinitions.Keys[index];
                    var item = new ListViewItem(OsdMetricDefinitions.Names[index]) { Tag = key, Checked = OsdMetricDefinitions.IsVisible(_metricDraft, key) };
                    item.SubItems.Add(OsdMetricDefinitions.ResolveLabel(_metricDraft, key));
                    item.SubItems.Add(OsdMetricDefinitions.Group(_metricDraft, key));
                    metricsList.Items.Add(item);
                }
                RefreshMetricRows(); ResizeMetricColumns();
            }
            finally { metricsList.EndUpdate(); _loadingMetricList = false; }
        }

        private void MetricMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = metricsList.SelectedItems.Count != 1;
        private void EditLabelMenu_Click(object sender, EventArgs e) => BeginMetricCellEdit(1);
        private void EditGroupMenu_Click(object sender, EventArgs e) => BeginMetricCellEdit(2);
        private void ResetLabelMenu_Click(object sender, EventArgs e) => ResetSelectedMetricLabel();
        private void LabelMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (IsHandleCreated && !IsDisposed && !Disposing) BeginInvoke((MethodInvoker)(() =>
            {
                if (!IsDisposed && !Disposing && metricLabelEditor.Visible && !metricLabelEditor.Focused) EndMetricLabelEdit(true);
            }));
        }

        private void RefreshMetricRows()
        {
            if (_handlingMetricCheck) return;
            foreach (ListViewItem item in metricsList.Items)
            {
                string key = (string)item.Tag;
                if (item.SubItems.Count < 3) continue;
                item.SubItems[1].Text = OsdMetricDefinitions.ResolveLabel(_metricDraft, key);
                item.SubItems[2].Text = OsdMetricDefinitions.Group(_metricDraft, key);
                item.ForeColor = metricsList.ForeColor;
                item.ToolTipText = key == "FrameTimeGraph" ? "Last 10 seconds; peaks preserved. Standalone row; no group. History recording is independent." :
                    key == "AverageFps" ? "Rolling 10-second FPS average; warms up after 1 second. Set its gap in Layout's Display Order list." :
                    key == "FrameTime" ? "Mean frame interval over the FPS refresh window, in ms. Independent of history recording." :
                    "Edit Label or Group. Group names are OSD captions; empty = separate metric.";
            }
        }

        private void MetricsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loadingMetricList || _handlingMetricCheck || !_configurationLoaded || !metricsList.Created) return;
            string key = (string)metricsList.Items[e.Index].Tag;
            if (OsdMetricDefinitions.IsVisible(_metricDraft, key) == (e.NewValue == CheckState.Checked)) return;
            _handlingMetricCheck = true;
            try
            {
                EndMetricLabelEdit(true);
                OsdMetricDefinitions.SetVisible(_metricDraft, key, e.NewValue == CheckState.Checked);
                UpdateHardwareVisibility(); RebuildItemOrder(); PreviewVisualSettings();
            }
            finally { _handlingMetricCheck = false; }
            if (IsHandleCreated && !IsDisposed && !Disposing)
                BeginInvoke((MethodInvoker)(() => { if (!IsDisposed && !Disposing) RefreshMetricRows(); }));
        }

        private void MetricsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hit = metricsList.HitTest(e.Location);
            if (hit.Item != null && hit.SubItem != hit.Item.SubItems[0])
                BeginMetricCellEdit(hit.Item.SubItems.IndexOf(hit.SubItem));
        }
        private void MetricsList_SelectedIndexChanged(object sender, EventArgs e) => EndMetricLabelEdit(true);
        private void MetricsList_ViewChanging(object sender, EventArgs e) => EndMetricLabelEdit(true);
        private void MetricsList_Resize(object sender, EventArgs e) { EndMetricLabelEdit(true); ResizeMetricColumns(); }
        private void MetricsList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) => EndMetricLabelEdit(true);

        private void ResizeMetricColumns()
        {
            if (metricsList.Columns.Count != 3) return;
            int width = Math.Max(100, metricsList.ClientSize.Width - 4);
            metricNameColumn.Width = width * 38 / 100;
            metricLabelColumn.Width = width * 32 / 100;
            metricGroupColumn.Width = width - metricNameColumn.Width - metricLabelColumn.Width;
            metricNameHeader.Width = metricNameColumn.Width;
            metricLabelHeader.Left = metricNameHeader.Right; metricLabelHeader.Width = metricLabelColumn.Width;
            metricGroupHeader.Left = metricLabelHeader.Right; metricGroupHeader.Width = metricGroupColumn.Width;
        }

        private void BeginMetricCellEdit(int column)
        {
            EndMetricLabelEdit(true);
            if (metricsList.SelectedItems.Count != 1 || column < 1 || column > 2) return;
            var item = metricsList.SelectedItems[0]; item.EnsureVisible();
            if (column == 2 && (string)item.Tag == "FrameTimeGraph") return;
            _editingMetricKey = (string)item.Tag; _editingColumn = column;
            _originalLabelText = item.SubItems[column].Text;
            _loadingLabelEditor = true;
            try
            {
                metricLabelEditor.Text = _originalLabelText; metricLabelEditor.ClearUndo();
                Rectangle bounds = item.SubItems[column].Bounds;
                metricLabelEditor.Bounds = new Rectangle(bounds.X, bounds.Y, Math.Max(20, bounds.Width), Math.Max(bounds.Height, metricLabelEditor.PreferredHeight));
                metricLabelEditor.Visible = true; metricLabelEditor.BringToFront(); metricLabelEditor.Focus(); metricLabelEditor.SelectAll();
            }
            finally { _loadingLabelEditor = false; }
        }

        private void ApplyCellEdit(OsdConfiguration config)
        {
            if (_editingMetricKey == null || metricLabelEditor.Text == _originalLabelText) return;
            if (_editingColumn == 1)
            {
                config.LabelOverrides[_editingMetricKey] = OsdMetricDefinitions.NormalizeLabel(metricLabelEditor.Text, "");
                config.GroupedLabelOverrides.Remove(_editingMetricKey);
            }
            else config.MetricGroups[_editingMetricKey] = OsdMetricDefinitions.SingleLine(metricLabelEditor.Text, 40, true);
        }
        private void EndMetricLabelEdit(bool commit)
        {
            if (_endingLabelEdit || _editingMetricKey == null) return;
            _endingLabelEdit = true;
            try
            {
                if (commit) ApplyCellEdit(_metricDraft);
                _editingMetricKey = null; metricLabelEditor.Visible = false;
                RefreshMetricRows(); RebuildItemOrder(); PreviewVisualSettings();
            }
            finally { _endingLabelEdit = false; }
        }
        private void ResetSelectedMetricLabel()
        {
            string key = _editingMetricKey ?? (metricsList.SelectedItems.Count == 1 ? (string)metricsList.SelectedItems[0].Tag : null);
            if (key == null) return;
            EndMetricLabelEdit(false); _metricDraft.LabelOverrides.Remove(key); _metricDraft.GroupedLabelOverrides.Remove(key);
            RefreshMetricRows(); PreviewVisualSettings();
        }
        private bool HandleMetricEditorKey(Keys keyData)
        {
            if (metricLabelEditor.Visible)
            {
                if (keyData == Keys.Enter || keyData == Keys.Escape) { EndMetricLabelEdit(keyData == Keys.Enter); metricsList.Focus(); return true; }
                if (keyData == Keys.Tab) { int next = _editingColumn == 1 ? 2 : 1; EndMetricLabelEdit(true); BeginMetricCellEdit(next); return true; }
            }
            else if (metricsList.Focused && keyData == Keys.F2) { BeginMetricCellEdit(1); return true; }
            return false;
        }
        private void MetricLabelEditor_TextChanged(object sender, EventArgs e) { if (!_loadingLabelEditor) PreviewVisualSettings(); }
        private void MetricLabelEditor_Leave(object sender, EventArgs e) { if (!customLabelContextMenu.Visible) EndMetricLabelEdit(true); }

        private void PopulateAppearance()
        {
            _loadingAppearance = true;
            appearanceGrid.SuspendLayout();
            try
            {
                appearanceGrid.Rows.Clear();
                appearanceFamilyColumn.Items.Clear();
                appearanceSizeColumn.Items.Clear();
                appearanceFamilyColumn.Items.AddRange(OsdFontHelper.GetAvailableFamilyNames());
                foreach (string family in _metricDraft.MetricStyles.Values.Select(style => style.FontFamily).Distinct())
                    if (!appearanceFamilyColumn.Items.Contains(family)) appearanceFamilyColumn.Items.Add(family);
                for (decimal size = 6; size <= 48; size += 0.5m) appearanceSizeColumn.Items.Add(size.ToString("0.#", CultureInfo.InvariantCulture));
                foreach (var style in _metricDraft.MetricStyles.Values)
                {
                    string size = style.FontSize.ToString(CultureInfo.InvariantCulture);
                    if (!appearanceSizeColumn.Items.Contains(size)) appearanceSizeColumn.Items.Add(size);
                }
                foreach (int i in MetricDisplayOrder)
                {
                    string key = OsdMetricDefinitions.Keys[i]; var style = _metricDraft.MetricStyles[key];
                    int row = appearanceGrid.Rows.Add(OsdMetricDefinitions.Names[i], style.FontFamily, style.FontSize.ToString(CultureInfo.InvariantCulture), "🎨", "🎨");
                    appearanceGrid.Rows[row].Cells[1].ToolTipText = style.FontFamily;
                    appearanceGrid.Rows[row].Tag = key; RefreshColorCells(appearanceGrid.Rows[row], style);
                }
            }
            finally { appearanceGrid.ResumeLayout(true); _loadingAppearance = false; }
        }

        private bool TryCommitAppearance()
        {
            bool committed = appearanceGrid.EndEdit();
            var invalid = appearanceGrid.Rows.Cast<DataGridViewRow>()
                .SelectMany(row => row.Cells.Cast<DataGridViewCell>()).FirstOrDefault(cell => cell.ErrorText.Length > 0);
            if (committed && invalid == null) return true;
            ShowPage(OsdSettingsPage.Appearance);
            if (invalid != null) appearanceGrid.CurrentCell = invalid;
            appearanceGrid.Focus();
            return false;
        }
        private static void RefreshColorCells(DataGridViewRow row, OsdMetricStyle style)
        {
            for (int column = 3; column <= 4; column++)
            {
                var cell = row.Cells[column]; Color color = Color.FromArgb(column == 3 ? style.LabelColor : style.ValueColor);
                cell.Style.BackColor = color; cell.Style.ForeColor = GetReadableForeground(color);
                cell.Style.SelectionBackColor = color; cell.Style.SelectionForeColor = cell.Style.ForeColor;
                cell.ToolTipText = (column == 3 ? "Label" : "Value") + " color (RGB " + color.R + ", " + color.G + ", " + color.B + ")";
            }
        }
        private void AppearanceGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        { if (appearanceGrid.IsCurrentCellDirty) appearanceGrid.CommitEdit(DataGridViewDataErrorContexts.Commit); }
        private void AppearanceGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (!(e.Control is ComboBox editor) || !(appearanceGrid.CurrentCell is DataGridViewComboBoxCell cell)) return;
            // IntegralHeight can expand the native popup to the screen height for long lists.
            editor.IntegralHeight = false;
            editor.MaxDropDownItems = cell.MaxDropDownItems;
            editor.DropDownHeight = editor.ItemHeight * Math.Min(editor.MaxDropDownItems, Math.Max(1, editor.Items.Count)) + 2;
        }
        private void AppearanceGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Keep invalid cell input in the editor; the saved draft retains its valid value.
            e.Cancel = true; e.ThrowException = false;
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                appearanceGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Choose an available font or size.";
        }
        private void AppearanceGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loadingAppearance || e.RowIndex < 0 || e.ColumnIndex < 1 || e.ColumnIndex > 2 || !_configurationLoaded) return;
            var row = appearanceGrid.Rows[e.RowIndex]; var style = _metricDraft.MetricStyles[(string)row.Tag];
            var choices = (DataGridViewComboBoxColumn)appearanceGrid.Columns[e.ColumnIndex];
            if (!choices.Items.Contains(row.Cells[e.ColumnIndex].Value))
            {
                row.Cells[e.ColumnIndex].ErrorText = "Choose an available font or size.";
                return;
            }
            row.Cells[e.ColumnIndex].ErrorText = "";
            if (e.ColumnIndex == 1)
            {
                style.FontFamily = Convert.ToString(row.Cells[1].Value);
                row.Cells[1].ToolTipText = style.FontFamily;
            }
            else if (float.TryParse(Convert.ToString(row.Cells[2].Value), NumberStyles.Float, CultureInfo.InvariantCulture, out float size))
                style.FontSize = Math.Max(6, Math.Min(48, size));
            PreviewVisualSettings();
        }
        private void AppearanceGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 3) return;
            appearanceGrid.EndEdit();
            var row = appearanceGrid.Rows[e.RowIndex]; var style = _metricDraft.MetricStyles[(string)row.Tag];
            colorDialog.Color = Color.FromArgb(e.ColumnIndex == 3 ? style.LabelColor : style.ValueColor);
            if (colorDialog.ShowDialog(this) != DialogResult.OK) return;
            if (e.ColumnIndex == 3) style.LabelColor = colorDialog.Color.ToArgb(); else style.ValueColor = colorDialog.Color.ToArgb();
            RefreshColorCells(row, style); PreviewVisualSettings();
        }
        private void ApplyAppearanceTheme(ThemePalette theme)
        {
            appearanceLayout.BackColor = appearanceFooter.BackColor = backgroundColorLayout.BackColor = theme.SurfaceBack;
            appearanceGrid.BackgroundColor = theme.InputBack; appearanceGrid.GridColor = theme.Border;
            appearanceGrid.DefaultCellStyle.BackColor = theme.InputBack; appearanceGrid.DefaultCellStyle.ForeColor = theme.Text;
            UpdateAppearanceSelectionColors();
            appearanceGrid.ColumnHeadersDefaultCellStyle.BackColor = theme.BarBack; appearanceGrid.ColumnHeadersDefaultCellStyle.ForeColor = theme.TitleText;
            foreach (Label label in appearanceFooter.Controls.OfType<Label>()) { label.ForeColor = theme.Text; label.BackColor = theme.SurfaceBack; }
            backgroundColorLabel.ForeColor = theme.Text; backgroundColorLabel.BackColor = theme.SurfaceBack;
        }
        private void UpdateAppearanceSelectionColors()
        {
            appearanceGrid.DefaultCellStyle.SelectionBackColor = appearanceGrid.ContainsFocus ? SystemColors.Highlight : SystemColors.Control;
            appearanceGrid.DefaultCellStyle.SelectionForeColor = appearanceGrid.ContainsFocus ? SystemColors.HighlightText : SystemColors.ControlText;
        }
        private void AppearanceGrid_FocusChanged(object sender, EventArgs e) => UpdateAppearanceSelectionColors();
        private void AppearanceGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 3)
            {
                // The cell padding belongs to the row, not to the color swatch button.
                var rowStyle = appearanceGrid.DefaultCellStyle;
                Color back = (e.State & DataGridViewElementStates.Selected) != 0 ? rowStyle.SelectionBackColor : rowStyle.BackColor;
                using (var brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, e.CellBounds);
                var padding = e.CellStyle.Padding;
                Rectangle buttonBounds = Rectangle.FromLTRB(e.CellBounds.Left + padding.Left, e.CellBounds.Top + padding.Top,
                    e.CellBounds.Right - padding.Right, e.CellBounds.Bottom - padding.Bottom);
                if (buttonBounds.Width > 0 && buttonBounds.Height > 0)
                    using (var brush = new SolidBrush(e.CellStyle.BackColor)) e.Graphics.FillRectangle(brush, buttonBounds);
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~(DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground));
                e.Handled = true;
                return;
            }
            if (e.RowIndex != -1 || e.ColumnIndex < 0) return;
            // The standard header bevel/grid differs from the flat Metrics header.
            var style = appearanceGrid.ColumnHeadersDefaultCellStyle;
            using (var brush = new SolidBrush(style.BackColor)) e.Graphics.FillRectangle(brush, e.CellBounds);
            Rectangle textBounds = Rectangle.FromLTRB(e.CellBounds.Left + style.Padding.Left, e.CellBounds.Top,
                e.CellBounds.Right - 2, e.CellBounds.Bottom);
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.FormattedValue), style.Font ?? appearanceGrid.Font,
                textBounds, style.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping);
            e.Handled = true;
        }
        private void AppearanceGrid_Paint(object sender, PaintEventArgs e)
        {
            int top = appearanceGrid.ColumnHeadersVisible ? appearanceGrid.ColumnHeadersHeight : 0;
            if (appearanceGrid.ClientSize.Width > 0 && appearanceGrid.ClientSize.Height > top)
                ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0, top, appearanceGrid.ClientSize.Width,
                    appearanceGrid.ClientSize.Height - top), Border3DStyle.Sunken);
        }
        private void UpdateHardwareVisibility()
        {
            bool fps = OsdMetricDefinitions.NeedsFrames(_metricDraft);
            fpsRefreshIntervalLabel.Enabled = fpsRefreshIntervalValue.Enabled = fps;
            UpdateSelectedMetricGap();
        }
        private string SelectedGapKey() => itemOrder.SelectedNode?.Tag as string;
        private void ItemOrder_AfterSelect(object sender, TreeViewEventArgs e) => UpdateSelectedMetricGap();
        private void ItemOrder_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.X < itemOrder.ClientSize.Width - 78 || e.Node.Parent != null) return;
            if (!ReferenceEquals(itemOrder.SelectedNode, e.Node)) itemOrder.SelectedNode = e.Node;
            if (!fpsLabelValueSpacing.Enabled) return;
            fpsLabelValueSpacing.Focus();
            fpsLabelValueSpacing.DroppedDown = true;
        }
        private void ItemOrder_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // Leave the native expand/collapse buttons and hierarchy lines visible.
            var bounds = Rectangle.FromLTRB(e.Bounds.Left, e.Bounds.Top, itemOrder.ClientSize.Width, e.Bounds.Bottom);
            bool selected = (e.State & TreeNodeStates.Selected) != 0;
            Color back = selected ? SystemColors.Highlight : itemOrder.BackColor;
            Color fore = selected ? SystemColors.HighlightText : e.Node.ForeColor;
            using (var brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, bounds);
            int gapColumnLeft = itemOrder.ClientSize.Width - 78;
            var labelBounds = Rectangle.FromLTRB(e.Bounds.Left, e.Bounds.Top, Math.Max(e.Bounds.Left, gapColumnLeft - 6), e.Bounds.Bottom);
            TextRenderer.DrawText(e.Graphics, e.Node.Text, itemOrder.Font, labelBounds, fore,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            string value = GetOrderGapText(e.Node);
            TextRenderer.DrawText(e.Graphics, value, itemOrder.Font,
                Rectangle.FromLTRB(gapColumnLeft, e.Bounds.Top, itemOrder.ClientSize.Width - 4, e.Bounds.Bottom), fore,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
        private string GetOrderGapText(TreeNode node)
        {
            if (node.Parent != null) return "—";
            string key = node.Tag as string;
            if (string.IsNullOrEmpty(key)) return "—";
            int gap = _metricDraft.MetricLabelValueGaps != null && _metricDraft.MetricLabelValueGaps.TryGetValue(key, out int saved)
                ? saved : _metricDraft.LabelValueSpacing;
            return gap + " px";
        }
        private void UpdateSelectedMetricGap()
        {
            string key = itemOrder.SelectedNode?.Parent == null ? SelectedGapKey() : null;
            bool enabled = key != null;
            fpsLabelValueSpacingLabel.Enabled = fpsLabelValueSpacing.Enabled = enabled;
            itemOrder.Invalidate();
            if (!enabled) return;
            int gap = _metricDraft.MetricLabelValueGaps.TryGetValue(key, out int saved) ? saved : _metricDraft.LabelValueSpacing;
            _loadingLabelEditor = true; try { SelectNumericOption(fpsLabelValueSpacing, gap); } finally { _loadingLabelEditor = false; }
        }
        private void MetricLabelGapChanged(object sender, EventArgs e)
        {
            if (_loadingLabelEditor || !_configurationLoaded) return;
            string key = SelectedGapKey(); if (key == null) return;
            _metricDraft.MetricLabelValueGaps[key] = (int)GetNumericOptionValue(fpsLabelValueSpacing, 10);
            itemOrder.Invalidate();
            PreviewVisualSettings();
        }
        private void RebuildItemOrder()
        {
            if (_separateItemOrder == null) return;
            string selected = itemOrder.SelectedNode?.Tag as string;
            var expandedGroups = new HashSet<string>(itemOrder.Nodes.Cast<TreeNode>()
                .Where(node => node.IsExpanded && node.Tag is string tag && tag.StartsWith("Group:", StringComparison.Ordinal))
                .Select(node => (string)node.Tag), StringComparer.OrdinalIgnoreCase);
            itemOrder.BeginUpdate();
            try
            {
                itemOrder.Nodes.Clear();
                var groups = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);
                foreach (var kind in _separateItemOrder)
                {
                    string key = kind.ToString(); string group = OsdMetricDefinitions.Group(_metricDraft, key);
                    var node = new TreeNode(OsdItemOrderHelper.GetDisplayName(kind)) { Tag = key,
                        ForeColor = OsdMetricDefinitions.IsVisible(_metricDraft, key) ? itemOrder.ForeColor : _disabledMetricColor };
                    if (group.Length == 0) itemOrder.Nodes.Add(node);
                    else
                    {
                        if (!groups.TryGetValue(group, out var parent))
                        {
                            parent = new TreeNode(group + " (group)") { Tag = "Group:" + group };
                            itemOrder.Nodes.Add(parent); groups[group] = parent;
                        }
                        parent.Nodes.Add(node);
                    }
                }
                foreach (TreeNode parent in itemOrder.Nodes)
                {
                    if ((string)parent.Tag == selected) itemOrder.SelectedNode = parent;
                    foreach (TreeNode child in parent.Nodes)
                    {
                        if ((string)child.Tag != selected) continue;
                        itemOrder.SelectedNode = expandedGroups.Contains((string)parent.Tag) ? child : parent;
                    }
                }
                if (itemOrder.SelectedNode == null && itemOrder.Nodes.Count > 0) itemOrder.SelectedNode = itemOrder.Nodes[0];
                foreach (TreeNode parent in itemOrder.Nodes)
                {
                    string tag = parent.Tag as string;
                    if (tag == null || !tag.StartsWith("Group:", StringComparison.Ordinal)) continue;
                    if (expandedGroups.Contains(tag)) parent.Expand(); else parent.Collapse();
                }
                RefreshOrderColors();
            }
            finally { itemOrder.EndUpdate(); }
            UpdateColumnOptions();
        }
        private void RefreshOrderColors()
        {
            foreach (TreeNode node in itemOrder.Nodes) RefreshOrderNodeColor(node);
        }
        private void RefreshOrderNodeColor(TreeNode node)
        {
            string key = node.Tag as string;
            bool enabled = node.Nodes.Count > 0
                ? node.Nodes.Cast<TreeNode>().Any(child => OsdMetricDefinitions.IsVisible(_metricDraft, child.Tag as string))
                : OsdMetricDefinitions.IsVisible(_metricDraft, key);
            node.ForeColor = enabled ? itemOrder.ForeColor : _disabledMetricColor;
            foreach (TreeNode child in node.Nodes) RefreshOrderNodeColor(child);
        }
        private void OrderUp_Click(object sender, EventArgs e) => MoveSelectedItem(-1);
        private void OrderDown_Click(object sender, EventArgs e) => MoveSelectedItem(1);
        private void MoveSelectedItem(int offset)
        {
            TreeNode node = itemOrder.SelectedNode;
            if (node == null) return;
            TreeNodeCollection siblings = node.Parent == null ? itemOrder.Nodes : node.Parent.Nodes;
            int next = node.Index + offset;
            if (next < 0 || next >= siblings.Count) return;
            node.Remove(); siblings.Insert(next, node); itemOrder.SelectedNode = node;
            _separateItemOrder = new List<OsdItemKind>();
            foreach (TreeNode parent in itemOrder.Nodes)
            {
                if (parent.Nodes.Count == 0) _separateItemOrder.Add((OsdItemKind)Enum.Parse(typeof(OsdItemKind), (string)parent.Tag));
                else foreach (TreeNode child in parent.Nodes) _separateItemOrder.Add((OsdItemKind)Enum.Parse(typeof(OsdItemKind), (string)child.Tag));
            }
            PreviewVisualSettings();
        }
    }
}
