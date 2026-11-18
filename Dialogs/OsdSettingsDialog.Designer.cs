namespace TrayTemps
{
    partial class OsdSettingsDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromThemeChanges();
                DisposeAppearanceFonts();
                if (colorDialog != null)
                    colorDialog.Dispose();
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OsdSettingsDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.settingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.metricLabelsInfoIcon = new System.Windows.Forms.Label();
            this.hotkeyValue = new System.Windows.Forms.TextBox();
            this.backgroundColor = new System.Windows.Forms.Button();
            this.itemOrder = new System.Windows.Forms.TreeView();
            this.orderUp = new System.Windows.Forms.Button();
            this.orderDown = new System.Windows.Forms.Button();
            this.columnsValue = new System.Windows.Forms.ComboBox();
            this.fpsLabelValueSpacingLabel = new System.Windows.Forms.Label();
            this.fpsLabelValueSpacing = new System.Windows.Forms.ComboBox();
            this.fpsRefreshIntervalValue = new System.Windows.Forms.ComboBox();
            this.externalSpacingLabel = new System.Windows.Forms.Label();
            this.externalSpacingValue = new System.Windows.Forms.ComboBox();
            this.cornerRoundnessLabel = new System.Windows.Forms.Label();
            this.cornerRoundnessValue = new System.Windows.Forms.ComboBox();
            this.customLabelContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.labelUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.labelEditSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.labelCut = new System.Windows.Forms.ToolStripMenuItem();
            this.labelCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.labelPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.labelDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.labelSelectionSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.labelSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.labelIconSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.insertIconMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.labelResetSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.resetLabelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metricContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editLabelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGroupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetMetricLabelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outerBorder = new System.Windows.Forms.Panel();
            this.resizeGrip = new TrayTemps.WindowResizeGripPanel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.contentPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pageTabs = new TrayTemps.BorderlessTabControl();
            this.presetsPage = new System.Windows.Forms.TabPage();
            this.presetsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.presetsTitle = new System.Windows.Forms.Label();
            this.presetsHelp = new System.Windows.Forms.Label();
            this.presetList = new System.Windows.Forms.ComboBox();
            this.loadEmbeddedPreset = new System.Windows.Forms.Button();
            this.loadPresetFile = new System.Windows.Forms.Button();
            this.savePresetFile = new System.Windows.Forms.Button();
            this.metricsPage = new System.Windows.Forms.TabPage();
            this.metricsPageLayout = new System.Windows.Forms.TableLayoutPanel();
            this.metricsCard = new System.Windows.Forms.Panel();
            this.displayedHardwareLabel = new System.Windows.Forms.Label();
            this.metricsListHeader = new System.Windows.Forms.Panel();
            this.metricNameHeader = new System.Windows.Forms.Label();
            this.metricLabelHeader = new System.Windows.Forms.Label();
            this.metricGroupHeader = new System.Windows.Forms.Label();
            this.metricsList = new TrayTemps.OsdMetricsListView();
            this.metricNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.metricLabelColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.metricGroupColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.metricLabelEditor = new System.Windows.Forms.TextBox();
            this.metricRowImages = new System.Windows.Forms.ImageList(this.components);
            this.metricListHint = new System.Windows.Forms.Label();
            this.hotkeyCard = new System.Windows.Forms.Panel();
            this.hotkeyLabel = new System.Windows.Forms.Label();
            this.hotkeyEnabled = new System.Windows.Forms.CheckBox();
            this.appearancePage = new System.Windows.Forms.TabPage();
            this.appearanceCard = new System.Windows.Forms.Panel();
            this.appearanceLayout = new System.Windows.Forms.TableLayoutPanel();
            this.appearanceGrid = new System.Windows.Forms.DataGridView();
            this.appearanceMetricColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.appearanceFamilyColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.appearanceSizeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.appearanceLabelColorColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.appearanceValueColorColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.appearanceFooter = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundOpacityValueLabel = new System.Windows.Forms.Label();
            this.backgroundOpacityValue = new System.Windows.Forms.TrackBar();
            this.backgroundColorLayout = new System.Windows.Forms.TableLayoutPanel();
            this.backgroundColorLabel = new System.Windows.Forms.Label();
            this.opacityLabel = new System.Windows.Forms.Label();
            this.opacityValueLabel = new System.Windows.Forms.Label();
            this.opacityValue = new System.Windows.Forms.TrackBar();
            this.layoutPage = new System.Windows.Forms.TabPage();
            this.layoutPageLayout = new System.Windows.Forms.TableLayoutPanel();
            this.layoutCard = new System.Windows.Forms.Panel();
            this.orderLabel = new System.Windows.Forms.Label();
            this.displayOrderLabel = new System.Windows.Forms.Label();
            this.groupFieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.positionLabel = new System.Windows.Forms.Label();
            this.columnsLabel = new System.Windows.Forms.Label();
            this.positionValue = new System.Windows.Forms.ComboBox();
            this.spacingCard = new System.Windows.Forms.Panel();
            this.spacingTitle = new System.Windows.Forms.Label();
            this.spacingLayout = new System.Windows.Forms.TableLayoutPanel();
            this.frameTimeGraphSizeLabel = new System.Windows.Forms.Label();
            this.frameTimeGraphSizeLayout = new System.Windows.Forms.TableLayoutPanel();
            this.frameTimeGraphWidthValue = new System.Windows.Forms.ComboBox();
            this.frameTimeGraphSizeSeparator = new System.Windows.Forms.Label();
            this.frameTimeGraphHeightValue = new System.Windows.Forms.ComboBox();
            this.groupedValuesGapLabel = new System.Windows.Forms.Label();
            this.groupedValuesGapPercent = new System.Windows.Forms.ComboBox();
            this.screenMarginLabel = new System.Windows.Forms.Label();
            this.screenMarginValue = new System.Windows.Forms.ComboBox();
            this.rowsGapHeader = new System.Windows.Forms.Label();
            this.rowsSpacing = new System.Windows.Forms.ComboBox();
            this.columnsGapHeader = new System.Windows.Forms.Label();
            this.columnsSpacing = new System.Windows.Forms.ComboBox();
            this.fpsRefreshIntervalLabel = new System.Windows.Forms.Label();
            this.bottomBar = new System.Windows.Forms.Panel();
            this.saveBtn = new System.Windows.Forms.Button();
            this.titleBar = new System.Windows.Forms.Panel();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.exitBtn = new System.Windows.Forms.Button();
            this.navigationPanel = new System.Windows.Forms.Panel();
            this.layoutNavButton = new System.Windows.Forms.Button();
            this.appearanceNavButton = new System.Windows.Forms.Button();
            this.metricsNavButton = new System.Windows.Forms.Button();
            this.presetsNavButton = new System.Windows.Forms.Button();
            this.accentLine = new System.Windows.Forms.Panel();
            this.navigationTitle = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.customLabelContextMenu.SuspendLayout();
            this.metricContextMenu.SuspendLayout();
            this.outerBorder.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.pageTabs.SuspendLayout();
            this.presetsPage.SuspendLayout();
            this.presetsLayout.SuspendLayout();
            this.metricsPage.SuspendLayout();
            this.metricsPageLayout.SuspendLayout();
            this.metricsCard.SuspendLayout();
            this.metricsListHeader.SuspendLayout();
            this.metricsList.SuspendLayout();
            this.hotkeyCard.SuspendLayout();
            this.appearancePage.SuspendLayout();
            this.appearanceCard.SuspendLayout();
            this.appearanceLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appearanceGrid)).BeginInit();
            this.appearanceFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundOpacityValue)).BeginInit();
            this.backgroundColorLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opacityValue)).BeginInit();
            this.layoutPage.SuspendLayout();
            this.layoutPageLayout.SuspendLayout();
            this.layoutCard.SuspendLayout();
            this.groupFieldsLayout.SuspendLayout();
            this.spacingCard.SuspendLayout();
            this.spacingLayout.SuspendLayout();
            this.frameTimeGraphSizeLayout.SuspendLayout();
            this.bottomBar.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.presetsNavButton.SuspendLayout();
            this.SuspendLayout();
            //
            // metricLabelsInfoIcon
            //
            this.metricLabelsInfoIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metricLabelsInfoIcon.BackColor = System.Drawing.Color.Transparent;
            this.metricLabelsInfoIcon.Cursor = System.Windows.Forms.Cursors.Help;
            this.metricLabelsInfoIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metricLabelsInfoIcon.Location = new System.Drawing.Point(582, 5);
            this.metricLabelsInfoIcon.Margin = new System.Windows.Forms.Padding(0);
            this.metricLabelsInfoIcon.Name = "metricLabelsInfoIcon";
            this.metricLabelsInfoIcon.Size = new System.Drawing.Size(40, 32);
            this.metricLabelsInfoIcon.TabIndex = 9;
            this.metricLabelsInfoIcon.Tag = "TrayTemps.Font.System";
            this.metricLabelsInfoIcon.Text = "ⓘ";
            this.metricLabelsInfoIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.settingsToolTip.SetToolTip(this.metricLabelsInfoIcon, "Double-click Label or Group to edit; F2 edits Label. Right-click to paste or inse" +
        "rt icons.");
            //
            // hotkeyValue
            //
            this.hotkeyValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hotkeyValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.hotkeyValue.Location = new System.Drawing.Point(214, 51);
            this.hotkeyValue.Margin = new System.Windows.Forms.Padding(2);
            this.hotkeyValue.Name = "hotkeyValue";
            this.hotkeyValue.ReadOnly = true;
            this.hotkeyValue.ShortcutsEnabled = false;
            this.hotkeyValue.Size = new System.Drawing.Size(408, 19);
            this.hotkeyValue.TabIndex = 1;
            this.hotkeyValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.settingsToolTip.SetToolTip(this.hotkeyValue, "Sets the shortcut that toggles OSD visibility.");
            this.hotkeyValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyValue_KeyDown);
            //
            // backgroundColor
            //
            this.backgroundColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.backgroundColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backgroundColor.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backgroundColor.Location = new System.Drawing.Point(130, 2);
            this.backgroundColor.Margin = new System.Windows.Forms.Padding(2);
            this.backgroundColor.Name = "backgroundColor";
            this.backgroundColor.Size = new System.Drawing.Size(48, 35);
            this.backgroundColor.TabIndex = 7;
            this.backgroundColor.Tag = "TrayTemps.Font.System";
            this.backgroundColor.Text = "🎨";
            this.settingsToolTip.SetToolTip(this.backgroundColor, "Choose the OSD background color.");
            this.backgroundColor.UseVisualStyleBackColor = false;
            this.backgroundColor.Click += new System.EventHandler(this.BackgroundColor_Click);
            //
            // itemOrder
            //
            this.itemOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemOrder.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.itemOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemOrder.HideSelection = false;
            this.itemOrder.ItemHeight = 26;
            this.itemOrder.Location = new System.Drawing.Point(18, 75);
            this.itemOrder.Margin = new System.Windows.Forms.Padding(2);
            this.itemOrder.Name = "itemOrder";
            this.itemOrder.Size = new System.Drawing.Size(429, 178);
            this.itemOrder.TabIndex = 2;
            this.settingsToolTip.SetToolTip(this.itemOrder, "Groups start collapsed. Select an item, then use Up/Down to change its order.");
            this.itemOrder.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.ItemOrder_DrawNode);
            this.itemOrder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ItemOrder_AfterSelect);
            this.itemOrder.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ItemOrder_NodeMouseClick);
            //
            // orderUp
            //
            this.orderUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orderUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.orderUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderUp.Location = new System.Drawing.Point(457, 79);
            this.orderUp.Margin = new System.Windows.Forms.Padding(2);
            this.orderUp.Name = "orderUp";
            this.orderUp.Size = new System.Drawing.Size(160, 32);
            this.orderUp.TabIndex = 3;
            this.orderUp.Text = "Up";
            this.settingsToolTip.SetToolTip(this.orderUp, "Move the selected item earlier.");
            this.orderUp.UseVisualStyleBackColor = true;
            this.orderUp.Click += new System.EventHandler(this.OrderUp_Click);
            //
            // orderDown
            //
            this.orderDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orderDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.orderDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderDown.Location = new System.Drawing.Point(457, 117);
            this.orderDown.Margin = new System.Windows.Forms.Padding(2);
            this.orderDown.Name = "orderDown";
            this.orderDown.Size = new System.Drawing.Size(160, 32);
            this.orderDown.TabIndex = 4;
            this.orderDown.Text = "Down";
            this.settingsToolTip.SetToolTip(this.orderDown, "Move the selected item later.");
            this.orderDown.UseVisualStyleBackColor = true;
            this.orderDown.Click += new System.EventHandler(this.OrderDown_Click);
            //
            // columnsValue
            //
            this.columnsValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.columnsValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnsValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.columnsValue.FormattingEnabled = true;
            this.columnsValue.Location = new System.Drawing.Point(545, 3);
            this.columnsValue.Margin = new System.Windows.Forms.Padding(2);
            this.columnsValue.Name = "columnsValue";
            this.columnsValue.Size = new System.Drawing.Size(59, 21);
            this.columnsValue.TabIndex = 1;
            this.settingsToolTip.SetToolTip(this.columnsValue, "Sets the number of OSD columns.");
            this.columnsValue.SelectedIndexChanged += new System.EventHandler(this.ColumnsValue_SelectedIndexChanged);
            //
            // fpsLabelValueSpacingLabel
            //
            this.fpsLabelValueSpacingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fpsLabelValueSpacingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.fpsLabelValueSpacingLabel.Location = new System.Drawing.Point(457, 210);
            this.fpsLabelValueSpacingLabel.Margin = new System.Windows.Forms.Padding(2);
            this.fpsLabelValueSpacingLabel.Name = "fpsLabelValueSpacingLabel";
            this.fpsLabelValueSpacingLabel.Size = new System.Drawing.Size(160, 20);
            this.fpsLabelValueSpacingLabel.TabIndex = 10;
            this.fpsLabelValueSpacingLabel.Text = "Label/Value Gap:";
            this.fpsLabelValueSpacingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsToolTip.SetToolTip(this.fpsLabelValueSpacingLabel, "Gap for the selected metric or group.");
            //
            // fpsLabelValueSpacing
            //
            this.fpsLabelValueSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fpsLabelValueSpacing.DropDownHeight = 200;
            this.fpsLabelValueSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fpsLabelValueSpacing.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.fpsLabelValueSpacing.FormattingEnabled = true;
            this.fpsLabelValueSpacing.IntegralHeight = false;
            this.fpsLabelValueSpacing.Location = new System.Drawing.Point(457, 230);
            this.fpsLabelValueSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.fpsLabelValueSpacing.Name = "fpsLabelValueSpacing";
            this.fpsLabelValueSpacing.Size = new System.Drawing.Size(160, 21);
            this.fpsLabelValueSpacing.TabIndex = 5;
            this.settingsToolTip.SetToolTip(this.fpsLabelValueSpacing, "Gap for the selected metric or group.");
            this.fpsLabelValueSpacing.SelectedIndexChanged += new System.EventHandler(this.MetricLabelGapChanged);
            //
            // fpsRefreshIntervalValue
            //
            this.fpsRefreshIntervalValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpsRefreshIntervalValue.DropDownHeight = 200;
            this.fpsRefreshIntervalValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fpsRefreshIntervalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.fpsRefreshIntervalValue.FormattingEnabled = true;
            this.fpsRefreshIntervalValue.IntegralHeight = false;
            this.fpsRefreshIntervalValue.Location = new System.Drawing.Point(435, 62);
            this.fpsRefreshIntervalValue.Margin = new System.Windows.Forms.Padding(2);
            this.fpsRefreshIntervalValue.Name = "fpsRefreshIntervalValue";
            this.fpsRefreshIntervalValue.Size = new System.Drawing.Size(169, 21);
            this.fpsRefreshIntervalValue.TabIndex = 4;
            this.settingsToolTip.SetToolTip(this.fpsRefreshIntervalValue, "Frame metrics refresh interval in seconds. Hardware polling is separate.");
            this.fpsRefreshIntervalValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // externalSpacingLabel
            //
            this.externalSpacingLabel.AutoSize = true;
            this.externalSpacingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.externalSpacingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.externalSpacingLabel.Location = new System.Drawing.Point(2, 60);
            this.externalSpacingLabel.Margin = new System.Windows.Forms.Padding(2);
            this.externalSpacingLabel.Name = "externalSpacingLabel";
            this.externalSpacingLabel.Size = new System.Drawing.Size(126, 25);
            this.externalSpacingLabel.TabIndex = 12;
            this.externalSpacingLabel.Text = "External spacing:";
            this.externalSpacingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsToolTip.SetToolTip(this.externalSpacingLabel, "Offsets the OSD from the selected monitor edges.");
            //
            // externalSpacingValue
            //
            this.externalSpacingValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.externalSpacingValue.DropDownHeight = 200;
            this.externalSpacingValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.externalSpacingValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.externalSpacingValue.FormattingEnabled = true;
            this.externalSpacingValue.IntegralHeight = false;
            this.externalSpacingValue.Location = new System.Drawing.Point(132, 62);
            this.externalSpacingValue.Margin = new System.Windows.Forms.Padding(2);
            this.externalSpacingValue.Name = "externalSpacingValue";
            this.externalSpacingValue.Size = new System.Drawing.Size(169, 21);
            this.externalSpacingValue.TabIndex = 6;
            this.settingsToolTip.SetToolTip(this.externalSpacingValue, "Offset from the selected monitor edges.");
            this.externalSpacingValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // cornerRoundnessLabel
            //
            this.cornerRoundnessLabel.AutoSize = true;
            this.cornerRoundnessLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cornerRoundnessLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.cornerRoundnessLabel.Location = new System.Drawing.Point(305, 31);
            this.cornerRoundnessLabel.Margin = new System.Windows.Forms.Padding(2);
            this.cornerRoundnessLabel.Name = "cornerRoundnessLabel";
            this.cornerRoundnessLabel.Size = new System.Drawing.Size(126, 25);
            this.cornerRoundnessLabel.TabIndex = 13;
            this.cornerRoundnessLabel.Text = "Corner roundness:";
            this.cornerRoundnessLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.settingsToolTip.SetToolTip(this.cornerRoundnessLabel, "Rounds the OSD background corners.");
            //
            // cornerRoundnessValue
            //
            this.cornerRoundnessValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cornerRoundnessValue.DropDownHeight = 200;
            this.cornerRoundnessValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cornerRoundnessValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.cornerRoundnessValue.FormattingEnabled = true;
            this.cornerRoundnessValue.IntegralHeight = false;
            this.cornerRoundnessValue.Location = new System.Drawing.Point(435, 33);
            this.cornerRoundnessValue.Margin = new System.Windows.Forms.Padding(2);
            this.cornerRoundnessValue.Name = "cornerRoundnessValue";
            this.cornerRoundnessValue.Size = new System.Drawing.Size(169, 21);
            this.cornerRoundnessValue.TabIndex = 7;
            this.settingsToolTip.SetToolTip(this.cornerRoundnessValue, "Background corner radius in logical pixels.");
            this.cornerRoundnessValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // customLabelContextMenu
            //
            this.customLabelContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelUndo,
            this.labelEditSeparator,
            this.labelCut,
            this.labelCopy,
            this.labelPaste,
            this.labelDelete,
            this.labelSelectionSeparator,
            this.labelSelectAll,
            this.labelIconSeparator,
            this.insertIconMenu,
            this.labelResetSeparator,
            this.resetLabelMenuItem});
            this.customLabelContextMenu.Name = "customLabelContextMenu";
            this.customLabelContextMenu.Size = new System.Drawing.Size(131, 204);
            this.customLabelContextMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.LabelMenu_Closed);
            this.customLabelContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.CustomLabelContextMenu_Opening);
            //
            // labelUndo
            //
            this.labelUndo.Name = "labelUndo";
            this.labelUndo.Size = new System.Drawing.Size(130, 22);
            this.labelUndo.Text = "Undo";
            this.labelUndo.Click += new System.EventHandler(this.LabelCommand_Click);
            //
            // labelEditSeparator
            //
            this.labelEditSeparator.Name = "labelEditSeparator";
            this.labelEditSeparator.Size = new System.Drawing.Size(127, 6);
            //
            // labelCut
            //
            this.labelCut.Name = "labelCut";
            this.labelCut.Size = new System.Drawing.Size(130, 22);
            this.labelCut.Text = "Cut";
            this.labelCut.Click += new System.EventHandler(this.LabelCommand_Click);
            //
            // labelCopy
            //
            this.labelCopy.Name = "labelCopy";
            this.labelCopy.Size = new System.Drawing.Size(130, 22);
            this.labelCopy.Text = "Copy";
            this.labelCopy.Click += new System.EventHandler(this.LabelCommand_Click);
            //
            // labelPaste
            //
            this.labelPaste.Name = "labelPaste";
            this.labelPaste.Size = new System.Drawing.Size(130, 22);
            this.labelPaste.Text = "Paste";
            this.labelPaste.Click += new System.EventHandler(this.LabelCommand_Click);
            //
            // labelDelete
            //
            this.labelDelete.Name = "labelDelete";
            this.labelDelete.Size = new System.Drawing.Size(130, 22);
            this.labelDelete.Text = "Delete";
            this.labelDelete.Click += new System.EventHandler(this.LabelCommand_Click);
            //
            // labelSelectionSeparator
            //
            this.labelSelectionSeparator.Name = "labelSelectionSeparator";
            this.labelSelectionSeparator.Size = new System.Drawing.Size(127, 6);
            //
            // labelSelectAll
            //
            this.labelSelectAll.Name = "labelSelectAll";
            this.labelSelectAll.Size = new System.Drawing.Size(130, 22);
            this.labelSelectAll.Text = "Select All";
            this.labelSelectAll.Click += new System.EventHandler(this.LabelCommand_Click);
            //
            // labelIconSeparator
            //
            this.labelIconSeparator.Name = "labelIconSeparator";
            this.labelIconSeparator.Size = new System.Drawing.Size(127, 6);
            //
            // insertIconMenu
            //
            this.insertIconMenu.Name = "insertIconMenu";
            this.insertIconMenu.Size = new System.Drawing.Size(130, 22);
            this.insertIconMenu.Text = "Insert icon";
            //
            // labelResetSeparator
            //
            this.labelResetSeparator.Name = "labelResetSeparator";
            this.labelResetSeparator.Size = new System.Drawing.Size(127, 6);
            //
            // resetLabelMenuItem
            //
            this.resetLabelMenuItem.Name = "resetLabelMenuItem";
            this.resetLabelMenuItem.Size = new System.Drawing.Size(130, 22);
            this.resetLabelMenuItem.Text = "Reset label";
            this.resetLabelMenuItem.Click += new System.EventHandler(this.ResetLabelMenu_Click);
            //
            // metricContextMenu
            //
            this.metricContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editLabelMenuItem,
            this.editGroupMenuItem,
            this.resetMetricLabelMenuItem});
            this.metricContextMenu.Name = "metricContextMenu";
            this.metricContextMenu.Size = new System.Drawing.Size(131, 70);
            this.metricContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.MetricMenu_Opening);
            //
            // editLabelMenuItem
            //
            this.editLabelMenuItem.Name = "editLabelMenuItem";
            this.editLabelMenuItem.Size = new System.Drawing.Size(130, 22);
            this.editLabelMenuItem.Text = "Edit label";
            this.editLabelMenuItem.Click += new System.EventHandler(this.EditLabelMenu_Click);
            //
            // editGroupMenuItem
            //
            this.editGroupMenuItem.Name = "editGroupMenuItem";
            this.editGroupMenuItem.Size = new System.Drawing.Size(130, 22);
            this.editGroupMenuItem.Text = "Edit group";
            this.editGroupMenuItem.Click += new System.EventHandler(this.EditGroupMenu_Click);
            //
            // resetMetricLabelMenuItem
            //
            this.resetMetricLabelMenuItem.Name = "resetMetricLabelMenuItem";
            this.resetMetricLabelMenuItem.Size = new System.Drawing.Size(130, 22);
            this.resetMetricLabelMenuItem.Text = "Reset label";
            this.resetMetricLabelMenuItem.Click += new System.EventHandler(this.ResetLabelMenu_Click);
            //
            // outerBorder
            //
            this.outerBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.outerBorder.Controls.Add(this.resizeGrip);
            this.outerBorder.Controls.Add(this.mainPanel);
            this.outerBorder.Controls.Add(this.navigationPanel);
            this.outerBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerBorder.Location = new System.Drawing.Point(0, 0);
            this.outerBorder.Name = "outerBorder";
            this.outerBorder.Padding = new System.Windows.Forms.Padding(1);
            this.outerBorder.Size = new System.Drawing.Size(800, 590);
            this.outerBorder.TabIndex = 0;
            //
            // resizeGrip
            //
            this.resizeGrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeGrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.resizeGrip.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeGrip.Location = new System.Drawing.Point(779, 569);
            this.resizeGrip.Name = "resizeGrip";
            this.resizeGrip.Size = new System.Drawing.Size(20, 20);
            this.resizeGrip.TabIndex = 2;
            //
            // mainPanel
            //
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.mainPanel.Controls.Add(this.contentPanel);
            this.mainPanel.Controls.Add(this.bottomBar);
            this.mainPanel.Controls.Add(this.titleBar);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(131, 1);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(668, 588);
            this.mainPanel.TabIndex = 1;
            //
            // contentPanel
            //
            this.contentPanel.ColumnCount = 1;
            this.contentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentPanel.Controls.Add(this.pageTabs, 0, 0);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 55);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(15);
            this.contentPanel.RowCount = 1;
            this.contentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentPanel.Size = new System.Drawing.Size(668, 483);
            this.contentPanel.TabIndex = 1;
            //
            // pageTabs
            //
            this.pageTabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.pageTabs.Controls.Add(this.presetsPage);
            this.pageTabs.Controls.Add(this.metricsPage);
            this.pageTabs.Controls.Add(this.appearancePage);
            this.pageTabs.Controls.Add(this.layoutPage);
            this.pageTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.pageTabs.ItemSize = new System.Drawing.Size(1, 1);
            this.pageTabs.Location = new System.Drawing.Point(15, 15);
            this.pageTabs.Margin = new System.Windows.Forms.Padding(0);
            this.pageTabs.Multiline = true;
            this.pageTabs.Name = "pageTabs";
            this.pageTabs.Padding = new System.Drawing.Point(0, 0);
            this.pageTabs.SelectedIndex = 0;
            this.pageTabs.Size = new System.Drawing.Size(638, 453);
            this.pageTabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.pageTabs.TabIndex = 0;
            this.pageTabs.TabStop = false;
            this.pageTabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.PageTabs_DrawItem);
            //
            // presetsPage
            //
            this.presetsPage.AutoScroll = true;
            this.presetsPage.Controls.Add(this.presetsLayout);
            this.presetsPage.Location = new System.Drawing.Point(0, 0);
            this.presetsPage.Name = "presetsPage";
            this.presetsPage.Size = new System.Drawing.Size(638, 453);
            this.presetsPage.TabIndex = 0;
            this.presetsPage.Text = "Presets";
            //
            // presetsLayout
            //
            this.presetsLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.presetsLayout.ColumnCount = 2;
            this.presetsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.presetsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.presetsLayout.Controls.Add(this.presetsTitle, 0, 0);
            this.presetsLayout.Controls.Add(this.presetsHelp, 0, 1);
            this.presetsLayout.Controls.Add(this.presetList, 0, 2);
            this.presetsLayout.Controls.Add(this.loadEmbeddedPreset, 1, 2);
            this.presetsLayout.Controls.Add(this.loadPresetFile, 0, 3);
            this.presetsLayout.Controls.Add(this.savePresetFile, 1, 3);
            this.presetsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.presetsLayout.Location = new System.Drawing.Point(0, 0);
            this.presetsLayout.Name = "presetsLayout";
            this.presetsLayout.Padding = new System.Windows.Forms.Padding(16);
            this.presetsLayout.RowCount = 5;
            this.presetsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.presetsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.presetsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.presetsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.presetsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.presetsLayout.Size = new System.Drawing.Size(638, 453);
            this.presetsLayout.TabIndex = 0;
            //
            // presetsTitle
            //
            this.presetsLayout.SetColumnSpan(this.presetsTitle, 2);
            this.presetsTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.presetsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.presetsTitle.Location = new System.Drawing.Point(16, 16);
            this.presetsTitle.Margin = new System.Windows.Forms.Padding(0);
            this.presetsTitle.Name = "presetsTitle";
            this.presetsTitle.Size = new System.Drawing.Size(606, 24);
            this.presetsTitle.TabIndex = 0;
            this.presetsTitle.Tag = "TrayTemps.Font.ExtraBold";
            this.presetsTitle.Text = "OSD Presets";
            this.presetsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // presetsHelp
            //
            this.presetsLayout.SetColumnSpan(this.presetsHelp, 2);
            this.presetsHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.presetsHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.presetsHelp.Location = new System.Drawing.Point(16, 40);
            this.presetsHelp.Margin = new System.Windows.Forms.Padding(0);
            this.presetsHelp.Name = "presetsHelp";
            this.presetsHelp.Size = new System.Drawing.Size(606, 76);
            this.presetsHelp.TabIndex = 1;
            this.presetsHelp.Text = resources.GetString("presetsHelp.Text");
            //
            // presetList
            //
            this.presetList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.presetList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.presetList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.presetList.Location = new System.Drawing.Point(16, 124);
            this.presetList.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.presetList.Name = "presetList";
            this.presetList.Size = new System.Drawing.Size(387, 21);
            this.presetList.TabIndex = 0;
            //
            // loadEmbeddedPreset
            //
            this.loadEmbeddedPreset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadEmbeddedPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadEmbeddedPreset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.loadEmbeddedPreset.Location = new System.Drawing.Point(409, 119);
            this.loadEmbeddedPreset.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.loadEmbeddedPreset.Name = "loadEmbeddedPreset";
            this.loadEmbeddedPreset.Size = new System.Drawing.Size(213, 32);
            this.loadEmbeddedPreset.TabIndex = 1;
            this.loadEmbeddedPreset.Text = "Load selected";
            this.loadEmbeddedPreset.UseVisualStyleBackColor = false;
            this.loadEmbeddedPreset.Click += new System.EventHandler(this.LoadEmbeddedPreset_Click);
            //
            // loadPresetFile
            //
            this.loadPresetFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadPresetFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadPresetFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.loadPresetFile.Location = new System.Drawing.Point(16, 157);
            this.loadPresetFile.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.loadPresetFile.Name = "loadPresetFile";
            this.loadPresetFile.Size = new System.Drawing.Size(387, 32);
            this.loadPresetFile.TabIndex = 2;
            this.loadPresetFile.Text = "Load file...";
            this.loadPresetFile.UseVisualStyleBackColor = false;
            this.loadPresetFile.Click += new System.EventHandler(this.LoadPresetFile_Click);
            //
            // savePresetFile
            //
            this.savePresetFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.savePresetFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.savePresetFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.savePresetFile.Location = new System.Drawing.Point(409, 157);
            this.savePresetFile.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.savePresetFile.Name = "savePresetFile";
            this.savePresetFile.Size = new System.Drawing.Size(213, 32);
            this.savePresetFile.TabIndex = 3;
            this.savePresetFile.Text = "Save current...";
            this.savePresetFile.UseVisualStyleBackColor = false;
            this.savePresetFile.Click += new System.EventHandler(this.SavePresetFile_Click);
            //
            // metricsPage
            //
            this.metricsPage.AutoScroll = true;
            this.metricsPage.Controls.Add(this.metricsPageLayout);
            this.metricsPage.Location = new System.Drawing.Point(0, 0);
            this.metricsPage.Name = "metricsPage";
            this.metricsPage.Size = new System.Drawing.Size(638, 453);
            this.metricsPage.TabIndex = 1;
            this.metricsPage.Text = "Metrics";
            this.metricsPage.UseVisualStyleBackColor = true;
            //
            // metricsPageLayout
            //
            this.metricsPageLayout.ColumnCount = 1;
            this.metricsPageLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.metricsPageLayout.Controls.Add(this.metricsCard, 0, 0);
            this.metricsPageLayout.Controls.Add(this.hotkeyCard, 0, 1);
            this.metricsPageLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricsPageLayout.Location = new System.Drawing.Point(0, 0);
            this.metricsPageLayout.Margin = new System.Windows.Forms.Padding(0);
            this.metricsPageLayout.Name = "metricsPageLayout";
            this.metricsPageLayout.RowCount = 2;
            this.metricsPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.metricsPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.metricsPageLayout.Size = new System.Drawing.Size(638, 453);
            this.metricsPageLayout.TabIndex = 0;
            //
            // metricsCard
            //
            this.metricsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.metricsCard.Controls.Add(this.metricLabelsInfoIcon);
            this.metricsCard.Controls.Add(this.displayedHardwareLabel);
            this.metricsCard.Controls.Add(this.metricsListHeader);
            this.metricsCard.Controls.Add(this.metricsList);
            this.metricsCard.Controls.Add(this.metricListHint);
            this.metricsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricsCard.Location = new System.Drawing.Point(0, 0);
            this.metricsCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.metricsCard.Name = "metricsCard";
            this.metricsCard.Size = new System.Drawing.Size(638, 355);
            this.metricsCard.TabIndex = 0;
            //
            // displayedHardwareLabel
            //
            this.displayedHardwareLabel.AutoSize = true;
            this.displayedHardwareLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayedHardwareLabel.Location = new System.Drawing.Point(16, 14);
            this.displayedHardwareLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.displayedHardwareLabel.Name = "displayedHardwareLabel";
            this.displayedHardwareLabel.Size = new System.Drawing.Size(248, 15);
            this.displayedHardwareLabel.TabIndex = 0;
            this.displayedHardwareLabel.Tag = "TrayTemps.Font.ExtraBold";
            this.displayedHardwareLabel.Text = "Displayed metrics, labels and groups:";
            //
            // metricsListHeader
            //
            this.metricsListHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metricsListHeader.Controls.Add(this.metricNameHeader);
            this.metricsListHeader.Controls.Add(this.metricLabelHeader);
            this.metricsListHeader.Controls.Add(this.metricGroupHeader);
            this.metricsListHeader.Location = new System.Drawing.Point(16, 38);
            this.metricsListHeader.Name = "metricsListHeader";
            this.metricsListHeader.Size = new System.Drawing.Size(606, 30);
            this.metricsListHeader.TabIndex = 1;
            //
            // metricNameHeader
            //
            this.metricNameHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metricNameHeader.Location = new System.Drawing.Point(0, 0);
            this.metricNameHeader.Name = "metricNameHeader";
            this.metricNameHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.metricNameHeader.Size = new System.Drawing.Size(265, 30);
            this.metricNameHeader.TabIndex = 0;
            this.metricNameHeader.Tag = "TrayTemps.Font.ExtraBold";
            this.metricNameHeader.Text = "Metric";
            this.metricNameHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // metricLabelHeader
            //
            this.metricLabelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metricLabelHeader.Location = new System.Drawing.Point(265, 0);
            this.metricLabelHeader.Name = "metricLabelHeader";
            this.metricLabelHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.metricLabelHeader.Size = new System.Drawing.Size(341, 30);
            this.metricLabelHeader.TabIndex = 1;
            this.metricLabelHeader.Tag = "TrayTemps.Font.ExtraBold";
            this.metricLabelHeader.Text = "Label";
            this.metricLabelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // metricGroupHeader
            //
            this.metricGroupHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.metricGroupHeader.Location = new System.Drawing.Point(0, 0);
            this.metricGroupHeader.Name = "metricGroupHeader";
            this.metricGroupHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.metricGroupHeader.Size = new System.Drawing.Size(180, 30);
            this.metricGroupHeader.TabIndex = 2;
            this.metricGroupHeader.Tag = "TrayTemps.Font.ExtraBold";
            this.metricGroupHeader.Text = "Group";
            this.metricGroupHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // metricsList
            //
            this.metricsList.AccessibleName = "Displayed metrics and editable labels";
            this.metricsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metricsList.CheckBoxes = true;
            this.metricsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.metricNameColumn,
            this.metricLabelColumn,
            this.metricGroupColumn});
            this.metricsList.ContextMenuStrip = this.metricContextMenu;
            this.metricsList.Controls.Add(this.metricLabelEditor);
            this.metricsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metricsList.FullRowSelect = true;
            this.metricsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.metricsList.HideSelection = false;
            this.metricsList.Location = new System.Drawing.Point(16, 68);
            this.metricsList.MultiSelect = false;
            this.metricsList.Name = "metricsList";
            this.metricsList.ShowItemToolTips = true;
            this.metricsList.Size = new System.Drawing.Size(606, 240);
            this.metricsList.SmallImageList = this.metricRowImages;
            this.metricsList.TabIndex = 1;
            this.metricsList.UseCompatibleStateImageBehavior = false;
            this.metricsList.View = System.Windows.Forms.View.Details;
            this.metricsList.ViewChanging += new System.EventHandler(this.MetricsList_ViewChanging);
            this.metricsList.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.MetricsList_ColumnWidthChanged);
            this.metricsList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.MetricsList_ItemCheck);
            this.metricsList.SelectedIndexChanged += new System.EventHandler(this.MetricsList_SelectedIndexChanged);
            this.metricsList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MetricsList_MouseDoubleClick);
            this.metricsList.Resize += new System.EventHandler(this.MetricsList_Resize);
            //
            // metricNameColumn
            //
            this.metricNameColumn.Text = "Metric";
            this.metricNameColumn.Width = 265;
            //
            // metricLabelColumn
            //
            this.metricLabelColumn.Text = "Label";
            this.metricLabelColumn.Width = 341;
            //
            // metricGroupColumn
            //
            this.metricGroupColumn.Text = "Group";
            this.metricGroupColumn.Width = 180;
            //
            // metricLabelEditor
            //
            this.metricLabelEditor.AccessibleName = "Selected metric label";
            this.metricLabelEditor.ContextMenuStrip = this.customLabelContextMenu;
            this.metricLabelEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.metricLabelEditor.Location = new System.Drawing.Point(0, 0);
            this.metricLabelEditor.MaxLength = 40;
            this.metricLabelEditor.Name = "metricLabelEditor";
            this.metricLabelEditor.Size = new System.Drawing.Size(100, 21);
            this.metricLabelEditor.TabIndex = 0;
            this.metricLabelEditor.Visible = false;
            this.metricLabelEditor.TextChanged += new System.EventHandler(this.MetricLabelEditor_TextChanged);
            this.metricLabelEditor.Leave += new System.EventHandler(this.MetricLabelEditor_Leave);
            //
            // metricRowImages
            //
            this.metricRowImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.metricRowImages.ImageSize = new System.Drawing.Size(1, 25);
            this.metricRowImages.TransparentColor = System.Drawing.Color.Transparent;
            //
            // metricListHint
            //
            this.metricListHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metricListHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.metricListHint.Location = new System.Drawing.Point(16, 314);
            this.metricListHint.Name = "metricListHint";
            this.metricListHint.Size = new System.Drawing.Size(606, 32);
            this.metricListHint.TabIndex = 2;
            this.metricListHint.Text = "Double-click Label or Group. F2 edits the label; Tab switches cells. Empty group " +
    "= separate.";
            //
            // hotkeyCard
            //
            this.hotkeyCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.hotkeyCard.Controls.Add(this.hotkeyLabel);
            this.hotkeyCard.Controls.Add(this.hotkeyEnabled);
            this.hotkeyCard.Controls.Add(this.hotkeyValue);
            this.hotkeyCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hotkeyCard.Location = new System.Drawing.Point(0, 363);
            this.hotkeyCard.Margin = new System.Windows.Forms.Padding(0);
            this.hotkeyCard.Name = "hotkeyCard";
            this.hotkeyCard.Size = new System.Drawing.Size(638, 90);
            this.hotkeyCard.TabIndex = 2;
            //
            // hotkeyLabel
            //
            this.hotkeyLabel.AutoSize = true;
            this.hotkeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hotkeyLabel.Location = new System.Drawing.Point(16, 14);
            this.hotkeyLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.hotkeyLabel.Name = "hotkeyLabel";
            this.hotkeyLabel.Size = new System.Drawing.Size(300, 15);
            this.hotkeyLabel.TabIndex = 0;
            this.hotkeyLabel.Tag = "TrayTemps.Font.ExtraBold";
            this.hotkeyLabel.Text = "Global hotkey (press Ctrl, Shift, or Alt + a key):";
            //
            // hotkeyEnabled
            //
            this.hotkeyEnabled.AutoSize = true;
            this.hotkeyEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.hotkeyEnabled.Location = new System.Drawing.Point(18, 52);
            this.hotkeyEnabled.Margin = new System.Windows.Forms.Padding(2);
            this.hotkeyEnabled.Name = "hotkeyEnabled";
            this.hotkeyEnabled.Size = new System.Drawing.Size(122, 17);
            this.hotkeyEnabled.TabIndex = 0;
            this.hotkeyEnabled.Text = "Enable OSD hotkey:";
            this.hotkeyEnabled.UseVisualStyleBackColor = true;
            //
            // appearancePage
            //
            this.appearancePage.AutoScroll = true;
            this.appearancePage.Controls.Add(this.appearanceCard);
            this.appearancePage.Location = new System.Drawing.Point(0, 0);
            this.appearancePage.Name = "appearancePage";
            this.appearancePage.Size = new System.Drawing.Size(638, 453);
            this.appearancePage.TabIndex = 2;
            this.appearancePage.Text = "Appearance";
            this.appearancePage.UseVisualStyleBackColor = true;
            //
            // appearanceCard
            //
            this.appearanceCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.appearanceCard.Controls.Add(this.appearanceLayout);
            this.appearanceCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appearanceCard.Location = new System.Drawing.Point(0, 0);
            this.appearanceCard.Margin = new System.Windows.Forms.Padding(0);
            this.appearanceCard.Name = "appearanceCard";
            this.appearanceCard.Size = new System.Drawing.Size(638, 453);
            this.appearanceCard.TabIndex = 0;
            //
            // appearanceLayout
            //
            this.appearanceLayout.ColumnCount = 1;
            this.appearanceLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.appearanceLayout.Controls.Add(this.appearanceGrid, 0, 0);
            this.appearanceLayout.Controls.Add(this.appearanceFooter, 0, 1);
            this.appearanceLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appearanceLayout.Location = new System.Drawing.Point(0, 0);
            this.appearanceLayout.Name = "appearanceLayout";
            this.appearanceLayout.Padding = new System.Windows.Forms.Padding(12);
            this.appearanceLayout.RowCount = 2;
            this.appearanceLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.appearanceLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.appearanceLayout.Size = new System.Drawing.Size(638, 453);
            this.appearanceLayout.TabIndex = 0;
            //
            // appearanceGrid
            //
            this.appearanceGrid.AllowUserToAddRows = false;
            this.appearanceGrid.AllowUserToDeleteRows = false;
            this.appearanceGrid.AllowUserToResizeRows = false;
            this.appearanceGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.appearanceGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.appearanceGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.appearanceGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(6, 4, 0, 4);
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.appearanceGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.appearanceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.appearanceGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.appearanceMetricColumn,
            this.appearanceFamilyColumn,
            this.appearanceSizeColumn,
            this.appearanceLabelColorColumn,
            this.appearanceValueColorColumn});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.appearanceGrid.DefaultCellStyle = dataGridViewCellStyle9;
            this.appearanceGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appearanceGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.appearanceGrid.EnableHeadersVisualStyles = false;
            this.appearanceGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.appearanceGrid.Location = new System.Drawing.Point(15, 15);
            this.appearanceGrid.MultiSelect = false;
            this.appearanceGrid.Name = "appearanceGrid";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.appearanceGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.appearanceGrid.RowHeadersVisible = false;
            this.appearanceGrid.RowTemplate.Height = 26;
            this.appearanceGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.appearanceGrid.Size = new System.Drawing.Size(608, 339);
            this.appearanceGrid.TabIndex = 0;
            this.appearanceGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AppearanceGrid_CellContentClick);
            this.appearanceGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.AppearanceGrid_CellPainting);
            this.appearanceGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.AppearanceGrid_CellValueChanged);
            this.appearanceGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.AppearanceGrid_CurrentCellDirtyStateChanged);
            this.appearanceGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.AppearanceGrid_DataError);
            this.appearanceGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.AppearanceGrid_EditingControlShowing);
            this.appearanceGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.AppearanceGrid_Paint);
            this.appearanceGrid.Enter += new System.EventHandler(this.AppearanceGrid_FocusChanged);
            this.appearanceGrid.Leave += new System.EventHandler(this.AppearanceGrid_FocusChanged);
            //
            // appearanceMetricColumn
            //
            this.appearanceMetricColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.appearanceMetricColumn.HeaderText = "Metric";
            this.appearanceMetricColumn.MinimumWidth = 140;
            this.appearanceMetricColumn.Name = "appearanceMetricColumn";
            this.appearanceMetricColumn.ReadOnly = true;
            this.appearanceMetricColumn.Width = 140;
            //
            // appearanceFamilyColumn
            //
            this.appearanceFamilyColumn.DisplayStyleForCurrentCellOnly = true;
            this.appearanceFamilyColumn.DropDownWidth = 260;
            this.appearanceFamilyColumn.FillWeight = 150F;
            this.appearanceFamilyColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.appearanceFamilyColumn.HeaderText = "Font Family";
            this.appearanceFamilyColumn.MinimumWidth = 160;
            this.appearanceFamilyColumn.Name = "appearanceFamilyColumn";
            //
            // appearanceSizeColumn
            //
            this.appearanceSizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.appearanceSizeColumn.DisplayStyleForCurrentCellOnly = true;
            this.appearanceSizeColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.appearanceSizeColumn.HeaderText = "Font Size";
            this.appearanceSizeColumn.MinimumWidth = 70;
            this.appearanceSizeColumn.Name = "appearanceSizeColumn";
            this.appearanceSizeColumn.Width = 70;
            //
            // appearanceLabelColorColumn
            //
            this.appearanceLabelColorColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI Emoji", 9F);
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.appearanceLabelColorColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.appearanceLabelColorColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.appearanceLabelColorColumn.HeaderText = "Label Color";
            this.appearanceLabelColorColumn.MinimumWidth = 62;
            this.appearanceLabelColorColumn.Name = "appearanceLabelColorColumn";
            this.appearanceLabelColorColumn.Width = 62;
            //
            // appearanceValueColorColumn
            //
            this.appearanceValueColorColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI Emoji", 9F);
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.appearanceValueColorColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.appearanceValueColorColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.appearanceValueColorColumn.HeaderText = "Value Color";
            this.appearanceValueColorColumn.MinimumWidth = 62;
            this.appearanceValueColorColumn.Name = "appearanceValueColorColumn";
            this.appearanceValueColorColumn.Width = 62;
            //
            // appearanceFooter
            //
            this.appearanceFooter.ColumnCount = 4;
            this.appearanceFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.appearanceFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.appearanceFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.appearanceFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.appearanceFooter.Controls.Add(this.label1, 0, 0);
            this.appearanceFooter.Controls.Add(this.backgroundOpacityValueLabel, 1, 0);
            this.appearanceFooter.Controls.Add(this.backgroundOpacityValue, 2, 0);
            this.appearanceFooter.Controls.Add(this.backgroundColorLayout, 3, 0);
            this.appearanceFooter.Controls.Add(this.opacityLabel, 0, 1);
            this.appearanceFooter.Controls.Add(this.opacityValueLabel, 1, 1);
            this.appearanceFooter.Controls.Add(this.opacityValue, 2, 1);
            this.appearanceFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appearanceFooter.Location = new System.Drawing.Point(15, 360);
            this.appearanceFooter.Name = "appearanceFooter";
            this.appearanceFooter.RowCount = 2;
            this.appearanceFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.appearanceFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.appearanceFooter.Size = new System.Drawing.Size(608, 78);
            this.appearanceFooter.TabIndex = 1;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 39);
            this.label1.TabIndex = 13;
            this.label1.Text = "Background transparency:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // backgroundOpacityValueLabel
            //
            this.backgroundOpacityValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundOpacityValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.backgroundOpacityValueLabel.Location = new System.Drawing.Point(172, 0);
            this.backgroundOpacityValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.backgroundOpacityValueLabel.Name = "backgroundOpacityValueLabel";
            this.backgroundOpacityValueLabel.Size = new System.Drawing.Size(44, 39);
            this.backgroundOpacityValueLabel.TabIndex = 10;
            this.backgroundOpacityValueLabel.Text = "0%";
            this.backgroundOpacityValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // backgroundOpacityValue
            //
            this.backgroundOpacityValue.AutoSize = false;
            this.backgroundOpacityValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundOpacityValue.Location = new System.Drawing.Point(220, 2);
            this.backgroundOpacityValue.Margin = new System.Windows.Forms.Padding(2);
            this.backgroundOpacityValue.Maximum = 100;
            this.backgroundOpacityValue.Name = "backgroundOpacityValue";
            this.backgroundOpacityValue.Size = new System.Drawing.Size(206, 35);
            this.backgroundOpacityValue.TabIndex = 8;
            this.backgroundOpacityValue.TickFrequency = 10;
            this.backgroundOpacityValue.ValueChanged += new System.EventHandler(this.BackgroundOpacityValue_ValueChanged);
            //
            // backgroundColorLayout
            //
            this.backgroundColorLayout.ColumnCount = 2;
            this.backgroundColorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.backgroundColorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.backgroundColorLayout.Controls.Add(this.backgroundColorLabel, 0, 0);
            this.backgroundColorLayout.Controls.Add(this.backgroundColor, 1, 0);
            this.backgroundColorLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundColorLayout.Location = new System.Drawing.Point(428, 0);
            this.backgroundColorLayout.Margin = new System.Windows.Forms.Padding(0);
            this.backgroundColorLayout.Name = "backgroundColorLayout";
            this.backgroundColorLayout.RowCount = 1;
            this.backgroundColorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.backgroundColorLayout.Size = new System.Drawing.Size(180, 39);
            this.backgroundColorLayout.TabIndex = 9;
            //
            // backgroundColorLabel
            //
            this.backgroundColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.backgroundColorLabel.Location = new System.Drawing.Point(2, 2);
            this.backgroundColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.backgroundColorLabel.Name = "backgroundColorLabel";
            this.backgroundColorLabel.Size = new System.Drawing.Size(124, 35);
            this.backgroundColorLabel.TabIndex = 9;
            this.backgroundColorLabel.Text = "Background color:";
            this.backgroundColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // opacityLabel
            //
            this.opacityLabel.AutoSize = true;
            this.opacityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opacityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.opacityLabel.Location = new System.Drawing.Point(2, 39);
            this.opacityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.opacityLabel.Name = "opacityLabel";
            this.opacityLabel.Size = new System.Drawing.Size(166, 39);
            this.opacityLabel.TabIndex = 3;
            this.opacityLabel.Text = "OSD transparency:";
            this.opacityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // opacityValueLabel
            //
            this.opacityValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opacityValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.opacityValueLabel.Location = new System.Drawing.Point(172, 39);
            this.opacityValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.opacityValueLabel.Name = "opacityValueLabel";
            this.opacityValueLabel.Size = new System.Drawing.Size(44, 39);
            this.opacityValueLabel.TabIndex = 4;
            this.opacityValueLabel.Text = "10%";
            this.opacityValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // opacityValue
            //
            this.opacityValue.AutoSize = false;
            this.opacityValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opacityValue.Location = new System.Drawing.Point(220, 41);
            this.opacityValue.Margin = new System.Windows.Forms.Padding(2);
            this.opacityValue.Maximum = 80;
            this.opacityValue.Name = "opacityValue";
            this.opacityValue.Size = new System.Drawing.Size(206, 35);
            this.opacityValue.TabIndex = 2;
            this.opacityValue.TickFrequency = 10;
            this.opacityValue.Value = 10;
            this.opacityValue.ValueChanged += new System.EventHandler(this.OpacityValue_ValueChanged);
            //
            // layoutPage
            //
            this.layoutPage.AutoScroll = true;
            this.layoutPage.AutoScrollMinSize = new System.Drawing.Size(0, 448);
            this.layoutPage.Controls.Add(this.layoutPageLayout);
            this.layoutPage.Location = new System.Drawing.Point(0, 0);
            this.layoutPage.Name = "layoutPage";
            this.layoutPage.Size = new System.Drawing.Size(638, 453);
            this.layoutPage.TabIndex = 3;
            this.layoutPage.Text = "Layout";
            this.layoutPage.UseVisualStyleBackColor = true;
            //
            // layoutPageLayout
            //
            this.layoutPageLayout.ColumnCount = 1;
            this.layoutPageLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPageLayout.Controls.Add(this.layoutCard, 0, 0);
            this.layoutPageLayout.Controls.Add(this.spacingCard, 0, 1);
            this.layoutPageLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPageLayout.Location = new System.Drawing.Point(0, 0);
            this.layoutPageLayout.Margin = new System.Windows.Forms.Padding(0);
            this.layoutPageLayout.Name = "layoutPageLayout";
            this.layoutPageLayout.RowCount = 2;
            this.layoutPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.layoutPageLayout.Size = new System.Drawing.Size(638, 453);
            this.layoutPageLayout.TabIndex = 0;
            //
            // layoutCard
            //
            this.layoutCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.layoutCard.Controls.Add(this.orderLabel);
            this.layoutCard.Controls.Add(this.displayOrderLabel);
            this.layoutCard.Controls.Add(this.itemOrder);
            this.layoutCard.Controls.Add(this.orderUp);
            this.layoutCard.Controls.Add(this.orderDown);
            this.layoutCard.Controls.Add(this.groupFieldsLayout);
            this.layoutCard.Controls.Add(this.fpsLabelValueSpacingLabel);
            this.layoutCard.Controls.Add(this.fpsLabelValueSpacing);
            this.layoutCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutCard.Location = new System.Drawing.Point(0, 0);
            this.layoutCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.layoutCard.MinimumSize = new System.Drawing.Size(0, 292);
            this.layoutCard.Name = "layoutCard";
            this.layoutCard.Size = new System.Drawing.Size(638, 292);
            this.layoutCard.TabIndex = 1;
            //
            // orderLabel
            //
            this.orderLabel.AutoSize = true;
            this.orderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderLabel.Location = new System.Drawing.Point(16, 16);
            this.orderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(133, 15);
            this.orderLabel.TabIndex = 0;
            this.orderLabel.Tag = "TrayTemps.Font.ExtraBold";
            this.orderLabel.Text = "Position and layout:";
            //
            // displayOrderLabel
            //
            this.displayOrderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.displayOrderLabel.AutoSize = true;
            this.displayOrderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.displayOrderLabel.Location = new System.Drawing.Point(457, 156);
            this.displayOrderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.displayOrderLabel.Name = "displayOrderLabel";
            this.displayOrderLabel.Size = new System.Drawing.Size(114, 13);
            this.displayOrderLabel.TabIndex = 3;
            this.displayOrderLabel.Text = "* Display order / priority";
            //
            // groupFieldsLayout
            //
            this.groupFieldsLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupFieldsLayout.ColumnCount = 4;
            this.groupFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.groupFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.groupFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.groupFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.groupFieldsLayout.Controls.Add(this.positionLabel, 0, 0);
            this.groupFieldsLayout.Controls.Add(this.columnsValue, 3, 0);
            this.groupFieldsLayout.Controls.Add(this.columnsLabel, 2, 0);
            this.groupFieldsLayout.Controls.Add(this.positionValue, 1, 0);
            this.groupFieldsLayout.Location = new System.Drawing.Point(16, 40);
            this.groupFieldsLayout.Name = "groupFieldsLayout";
            this.groupFieldsLayout.RowCount = 1;
            this.groupFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.groupFieldsLayout.Size = new System.Drawing.Size(606, 28);
            this.groupFieldsLayout.TabIndex = 5;
            //
            // positionLabel
            //
            this.positionLabel.AutoSize = true;
            this.positionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.positionLabel.Location = new System.Drawing.Point(2, 0);
            this.positionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.Size = new System.Drawing.Size(86, 28);
            this.positionLabel.TabIndex = 1;
            this.positionLabel.Text = "Position:";
            this.positionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // columnsLabel
            //
            this.columnsLabel.AutoSize = true;
            this.columnsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.columnsLabel.Location = new System.Drawing.Point(364, 0);
            this.columnsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.columnsLabel.Name = "columnsLabel";
            this.columnsLabel.Size = new System.Drawing.Size(177, 28);
            this.columnsLabel.TabIndex = 2;
            this.columnsLabel.Text = "No. of columns:";
            this.columnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // positionValue
            //
            this.positionValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.positionValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.positionValue.FormattingEnabled = true;
            this.positionValue.Items.AddRange(new object[] {
            "Top left",
            "Top center",
            "Top right",
            "Center left",
            "Center",
            "Center right",
            "Bottom left",
            "Bottom center",
            "Bottom right"});
            this.positionValue.Location = new System.Drawing.Point(92, 2);
            this.positionValue.Margin = new System.Windows.Forms.Padding(2);
            this.positionValue.Name = "positionValue";
            this.positionValue.Size = new System.Drawing.Size(268, 21);
            this.positionValue.TabIndex = 0;
            this.positionValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // spacingCard
            //
            this.spacingCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.spacingCard.Controls.Add(this.spacingTitle);
            this.spacingCard.Controls.Add(this.spacingLayout);
            this.spacingCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spacingCard.Location = new System.Drawing.Point(0, 278);
            this.spacingCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.spacingCard.Name = "spacingCard";
            this.spacingCard.Size = new System.Drawing.Size(638, 167);
            this.spacingCard.TabIndex = 1;
            //
            // spacingTitle
            //
            this.spacingTitle.AutoSize = true;
            this.spacingTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spacingTitle.Location = new System.Drawing.Point(16, 14);
            this.spacingTitle.Name = "spacingTitle";
            this.spacingTitle.Size = new System.Drawing.Size(140, 15);
            this.spacingTitle.TabIndex = 0;
            this.spacingTitle.Tag = "TrayTemps.Font.ExtraBold";
            this.spacingTitle.Text = "Spacing and refresh:";
            //
            // spacingLayout
            //
            this.spacingLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spacingLayout.ColumnCount = 4;
            this.spacingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.spacingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.spacingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.spacingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.spacingLayout.Controls.Add(this.screenMarginLabel, 0, 0);
            this.spacingLayout.Controls.Add(this.screenMarginValue, 1, 0);
            this.spacingLayout.Controls.Add(this.rowsGapHeader, 0, 1);
            this.spacingLayout.Controls.Add(this.rowsSpacing, 1, 1);
            this.spacingLayout.Controls.Add(this.columnsGapHeader, 2, 0);
            this.spacingLayout.Controls.Add(this.columnsSpacing, 3, 0);
            this.spacingLayout.Controls.Add(this.fpsRefreshIntervalLabel, 2, 2);
            this.spacingLayout.Controls.Add(this.fpsRefreshIntervalValue, 3, 2);
            this.spacingLayout.Controls.Add(this.externalSpacingLabel, 0, 2);
            this.spacingLayout.Controls.Add(this.externalSpacingValue, 1, 2);
            this.spacingLayout.Controls.Add(this.cornerRoundnessLabel, 2, 1);
            this.spacingLayout.Controls.Add(this.cornerRoundnessValue, 3, 1);
            this.spacingLayout.Controls.Add(this.groupedValuesGapLabel, 0, 3);
            this.spacingLayout.Controls.Add(this.groupedValuesGapPercent, 1, 3);
            this.spacingLayout.Controls.Add(this.frameTimeGraphSizeLabel, 2, 3);
            this.spacingLayout.Controls.Add(this.frameTimeGraphSizeLayout, 3, 3);
            this.spacingLayout.Location = new System.Drawing.Point(16, 35);
            this.spacingLayout.Margin = new System.Windows.Forms.Padding(0);
            this.spacingLayout.Name = "spacingLayout";
            this.spacingLayout.RowCount = 4;
            this.spacingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.spacingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.spacingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00063F));
            this.spacingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.spacingLayout.Size = new System.Drawing.Size(606, 118);
            this.spacingLayout.TabIndex = 1;
            //
            // frameTimeGraphSizeLabel
            //
            this.frameTimeGraphSizeLabel.AutoSize = true;
            this.frameTimeGraphSizeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameTimeGraphSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.frameTimeGraphSizeLabel.Location = new System.Drawing.Point(305, 89);
            this.frameTimeGraphSizeLabel.Margin = new System.Windows.Forms.Padding(2);
            this.frameTimeGraphSizeLabel.Name = "frameTimeGraphSizeLabel";
            this.frameTimeGraphSizeLabel.Size = new System.Drawing.Size(126, 27);
            this.frameTimeGraphSizeLabel.TabIndex = 15;
            this.frameTimeGraphSizeLabel.Text = "Graph size:";
            this.frameTimeGraphSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.settingsToolTip.SetToolTip(this.frameTimeGraphSizeLabel, "Frametime graph width x height in pixels at 100% display scaling (default 160 x 42). Scales with DPI and fits the available screen space.");
            //
            // frameTimeGraphSizeLayout
            //
            this.frameTimeGraphSizeLayout.ColumnCount = 3;
            this.frameTimeGraphSizeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.frameTimeGraphSizeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 14F));
            this.frameTimeGraphSizeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.frameTimeGraphSizeLayout.Controls.Add(this.frameTimeGraphWidthValue, 0, 0);
            this.frameTimeGraphSizeLayout.Controls.Add(this.frameTimeGraphSizeSeparator, 1, 0);
            this.frameTimeGraphSizeLayout.Controls.Add(this.frameTimeGraphHeightValue, 2, 0);
            this.frameTimeGraphSizeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameTimeGraphSizeLayout.Location = new System.Drawing.Point(433, 87);
            this.frameTimeGraphSizeLayout.Margin = new System.Windows.Forms.Padding(0);
            this.frameTimeGraphSizeLayout.Name = "frameTimeGraphSizeLayout";
            this.frameTimeGraphSizeLayout.RowCount = 1;
            this.frameTimeGraphSizeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.frameTimeGraphSizeLayout.Size = new System.Drawing.Size(173, 31);
            this.frameTimeGraphSizeLayout.TabIndex = 11;
            //
            // frameTimeGraphWidthValue
            //
            this.frameTimeGraphWidthValue.AccessibleName = "Frametime graph width (pixels)";
            this.frameTimeGraphWidthValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameTimeGraphWidthValue.DropDownHeight = 200;
            this.frameTimeGraphWidthValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameTimeGraphWidthValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.frameTimeGraphWidthValue.FormattingEnabled = true;
            this.frameTimeGraphWidthValue.IntegralHeight = false;
            this.frameTimeGraphWidthValue.Location = new System.Drawing.Point(2, 2);
            this.frameTimeGraphWidthValue.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.frameTimeGraphWidthValue.Name = "frameTimeGraphWidthValue";
            this.frameTimeGraphWidthValue.Size = new System.Drawing.Size(77, 21);
            this.frameTimeGraphWidthValue.TabIndex = 0;
            this.settingsToolTip.SetToolTip(this.frameTimeGraphWidthValue, "Frametime graph width in pixels at 100% display scaling (80-800; default 160). Scales with DPI and fits the available screen space.");
            this.frameTimeGraphWidthValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // frameTimeGraphSizeSeparator
            //
            this.frameTimeGraphSizeSeparator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameTimeGraphSizeSeparator.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.frameTimeGraphSizeSeparator.Location = new System.Drawing.Point(79, 0);
            this.frameTimeGraphSizeSeparator.Margin = new System.Windows.Forms.Padding(0);
            this.frameTimeGraphSizeSeparator.Name = "frameTimeGraphSizeSeparator";
            this.frameTimeGraphSizeSeparator.Size = new System.Drawing.Size(14, 31);
            this.frameTimeGraphSizeSeparator.TabIndex = 1;
            this.frameTimeGraphSizeSeparator.Text = "\u00D7";
            this.frameTimeGraphSizeSeparator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // frameTimeGraphHeightValue
            //
            this.frameTimeGraphHeightValue.AccessibleName = "Frametime graph height (pixels)";
            this.frameTimeGraphHeightValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameTimeGraphHeightValue.DropDownHeight = 200;
            this.frameTimeGraphHeightValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameTimeGraphHeightValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.frameTimeGraphHeightValue.FormattingEnabled = true;
            this.frameTimeGraphHeightValue.IntegralHeight = false;
            this.frameTimeGraphHeightValue.Location = new System.Drawing.Point(93, 2);
            this.frameTimeGraphHeightValue.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.frameTimeGraphHeightValue.Name = "frameTimeGraphHeightValue";
            this.frameTimeGraphHeightValue.Size = new System.Drawing.Size(78, 21);
            this.frameTimeGraphHeightValue.TabIndex = 2;
            this.settingsToolTip.SetToolTip(this.frameTimeGraphHeightValue, "Frametime graph height in pixels at 100% display scaling (20-400; default 42). Scales with DPI and fits the available screen space.");
            this.frameTimeGraphHeightValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // groupedValuesGapLabel
            //
            this.groupedValuesGapLabel.AutoSize = true;
            this.groupedValuesGapLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupedValuesGapLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.groupedValuesGapLabel.Location = new System.Drawing.Point(2, 89);
            this.groupedValuesGapLabel.Margin = new System.Windows.Forms.Padding(2);
            this.groupedValuesGapLabel.Name = "groupedValuesGapLabel";
            this.groupedValuesGapLabel.Size = new System.Drawing.Size(126, 27);
            this.groupedValuesGapLabel.TabIndex = 14;
            this.groupedValuesGapLabel.Text = "Grouped values gap:";
            this.groupedValuesGapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsToolTip.SetToolTip(this.groupedValuesGapLabel, "100% = one-space minimum. Increases gaps between all grouped value slots, including FPS and 1% Low.");
            //
            // groupedValuesGapPercent
            //
            this.groupedValuesGapPercent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupedValuesGapPercent.DropDownHeight = 200;
            this.groupedValuesGapPercent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupedValuesGapPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.groupedValuesGapPercent.FormattingEnabled = true;
            this.groupedValuesGapPercent.IntegralHeight = false;
            this.groupedValuesGapPercent.Location = new System.Drawing.Point(132, 92);
            this.groupedValuesGapPercent.Margin = new System.Windows.Forms.Padding(2);
            this.groupedValuesGapPercent.Name = "groupedValuesGapPercent";
            this.groupedValuesGapPercent.Size = new System.Drawing.Size(169, 21);
            this.groupedValuesGapPercent.TabIndex = 10;
            this.settingsToolTip.SetToolTip(this.groupedValuesGapPercent, "100% = one-space minimum. Increases gaps between all grouped value slots, including FPS and 1% Low.");
            this.groupedValuesGapPercent.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            this.groupedValuesGapPercent.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.GroupedValuesGapPercent_Format);
            //
            // screenMarginLabel
            //
            this.screenMarginLabel.AutoSize = true;
            this.screenMarginLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenMarginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.screenMarginLabel.Location = new System.Drawing.Point(2, 2);
            this.screenMarginLabel.Margin = new System.Windows.Forms.Padding(2);
            this.screenMarginLabel.Name = "screenMarginLabel";
            this.screenMarginLabel.Size = new System.Drawing.Size(126, 25);
            this.screenMarginLabel.TabIndex = 10;
            this.screenMarginLabel.Text = "Internal spacing:";
            this.screenMarginLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // screenMarginValue
            //
            this.screenMarginValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenMarginValue.DropDownHeight = 200;
            this.screenMarginValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.screenMarginValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.screenMarginValue.FormattingEnabled = true;
            this.screenMarginValue.IntegralHeight = false;
            this.screenMarginValue.Location = new System.Drawing.Point(132, 4);
            this.screenMarginValue.Margin = new System.Windows.Forms.Padding(2);
            this.screenMarginValue.Name = "screenMarginValue";
            this.screenMarginValue.Size = new System.Drawing.Size(169, 21);
            this.screenMarginValue.TabIndex = 0;
            this.screenMarginValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // rowsGapHeader
            //
            this.rowsGapHeader.AutoSize = true;
            this.rowsGapHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rowsGapHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.rowsGapHeader.Location = new System.Drawing.Point(2, 31);
            this.rowsGapHeader.Margin = new System.Windows.Forms.Padding(2);
            this.rowsGapHeader.Name = "rowsGapHeader";
            this.rowsGapHeader.Size = new System.Drawing.Size(126, 25);
            this.rowsGapHeader.TabIndex = 21;
            this.rowsGapHeader.Text = "Row spacing:";
            this.rowsGapHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // rowsSpacing
            //
            this.rowsSpacing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rowsSpacing.DropDownHeight = 200;
            this.rowsSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rowsSpacing.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.rowsSpacing.FormattingEnabled = true;
            this.rowsSpacing.IntegralHeight = false;
            this.rowsSpacing.Location = new System.Drawing.Point(132, 33);
            this.rowsSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.rowsSpacing.Name = "rowsSpacing";
            this.rowsSpacing.Size = new System.Drawing.Size(169, 21);
            this.rowsSpacing.TabIndex = 2;
            this.rowsSpacing.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // columnsGapHeader
            //
            this.columnsGapHeader.AutoSize = true;
            this.columnsGapHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsGapHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.columnsGapHeader.Location = new System.Drawing.Point(305, 2);
            this.columnsGapHeader.Margin = new System.Windows.Forms.Padding(2);
            this.columnsGapHeader.Name = "columnsGapHeader";
            this.columnsGapHeader.Size = new System.Drawing.Size(126, 25);
            this.columnsGapHeader.TabIndex = 22;
            this.columnsGapHeader.Text = "Column spacing:";
            this.columnsGapHeader.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // columnsSpacing
            //
            this.columnsSpacing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsSpacing.DropDownHeight = 200;
            this.columnsSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnsSpacing.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.columnsSpacing.FormattingEnabled = true;
            this.columnsSpacing.IntegralHeight = false;
            this.columnsSpacing.Location = new System.Drawing.Point(435, 4);
            this.columnsSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.columnsSpacing.Name = "columnsSpacing";
            this.columnsSpacing.Size = new System.Drawing.Size(169, 21);
            this.columnsSpacing.TabIndex = 3;
            this.columnsSpacing.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            //
            // fpsRefreshIntervalLabel
            //
            this.fpsRefreshIntervalLabel.AutoSize = true;
            this.fpsRefreshIntervalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpsRefreshIntervalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.fpsRefreshIntervalLabel.Location = new System.Drawing.Point(305, 60);
            this.fpsRefreshIntervalLabel.Margin = new System.Windows.Forms.Padding(2);
            this.fpsRefreshIntervalLabel.Name = "fpsRefreshIntervalLabel";
            this.fpsRefreshIntervalLabel.Size = new System.Drawing.Size(126, 25);
            this.fpsRefreshIntervalLabel.TabIndex = 8;
            this.fpsRefreshIntervalLabel.Text = "FPS refresh (s):";
            this.fpsRefreshIntervalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // bottomBar
            //
            this.bottomBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.bottomBar.Controls.Add(this.saveBtn);
            this.bottomBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomBar.Location = new System.Drawing.Point(0, 538);
            this.bottomBar.Name = "bottomBar";
            this.bottomBar.Padding = new System.Windows.Forms.Padding(15, 10, 30, 10);
            this.bottomBar.Size = new System.Drawing.Size(668, 50);
            this.bottomBar.TabIndex = 2;
            //
            // saveBtn
            //
            this.saveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.saveBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBtn.ForeColor = System.Drawing.Color.White;
            this.saveBtn.Location = new System.Drawing.Point(506, 10);
            this.saveBtn.Margin = new System.Windows.Forms.Padding(0);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(132, 30);
            this.saveBtn.TabIndex = 2;
            this.saveBtn.Tag = "TrayTemps.Font.ExtraBold";
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            //
            // titleBar
            //
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.titleBar.Controls.Add(this.subtitleLabel);
            this.titleBar.Controls.Add(this.exitBtn);
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.titleBar.Size = new System.Drawing.Size(668, 55);
            this.titleBar.TabIndex = 0;
            this.titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseDown);
            this.titleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseMove);
            this.titleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseUp);
            //
            // subtitleLabel
            //
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.subtitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.subtitleLabel.Location = new System.Drawing.Point(14, 0);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(604, 55);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "On-screen display configuration";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.subtitleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseDown);
            this.subtitleLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseMove);
            this.subtitleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseUp);
            //
            // exitBtn
            //
            this.exitBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.exitBtn.ForeColor = System.Drawing.Color.White;
            this.exitBtn.Location = new System.Drawing.Point(618, 0);
            this.exitBtn.Margin = new System.Windows.Forms.Padding(0);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(50, 55);
            this.exitBtn.TabIndex = 3;
            this.exitBtn.TabStop = false;
            this.exitBtn.Tag = "TrayTemps.Font.ExtraBold";
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            //
            // navigationPanel
            //
            this.navigationPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.navigationPanel.Controls.Add(this.layoutNavButton);
            this.navigationPanel.Controls.Add(this.appearanceNavButton);
            this.navigationPanel.Controls.Add(this.metricsNavButton);
            this.navigationPanel.Controls.Add(this.presetsNavButton);
            this.navigationPanel.Controls.Add(this.navigationTitle);
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.navigationPanel.Location = new System.Drawing.Point(1, 1);
            this.navigationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Size = new System.Drawing.Size(130, 588);
            this.navigationPanel.TabIndex = 0;
            this.navigationPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseDown);
            this.navigationPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseMove);
            this.navigationPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseUp);
            //
            // layoutNavButton
            //
            this.layoutNavButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.layoutNavButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutNavButton.FlatAppearance.BorderSize = 0;
            this.layoutNavButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.layoutNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutNavButton.Location = new System.Drawing.Point(0, 190);
            this.layoutNavButton.Name = "layoutNavButton";
            this.layoutNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.layoutNavButton.Size = new System.Drawing.Size(130, 45);
            this.layoutNavButton.TabIndex = 4;
            this.layoutNavButton.Tag = "TrayTemps.Font.ExtraBold";
            this.layoutNavButton.Text = "Layout";
            this.layoutNavButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.layoutNavButton.UseVisualStyleBackColor = false;
            this.layoutNavButton.Click += new System.EventHandler(this.LayoutNavButton_Click);
            //
            // appearanceNavButton
            //
            this.appearanceNavButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.appearanceNavButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.appearanceNavButton.FlatAppearance.BorderSize = 0;
            this.appearanceNavButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.appearanceNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appearanceNavButton.Location = new System.Drawing.Point(0, 145);
            this.appearanceNavButton.Name = "appearanceNavButton";
            this.appearanceNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.appearanceNavButton.Size = new System.Drawing.Size(130, 45);
            this.appearanceNavButton.TabIndex = 3;
            this.appearanceNavButton.Tag = "TrayTemps.Font.ExtraBold";
            this.appearanceNavButton.Text = "Appearance";
            this.appearanceNavButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.appearanceNavButton.UseVisualStyleBackColor = false;
            this.appearanceNavButton.Click += new System.EventHandler(this.AppearanceNavButton_Click);
            //
            // metricsNavButton
            //
            this.metricsNavButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.metricsNavButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.metricsNavButton.FlatAppearance.BorderSize = 0;
            this.metricsNavButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.metricsNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metricsNavButton.Location = new System.Drawing.Point(0, 100);
            this.metricsNavButton.Name = "metricsNavButton";
            this.metricsNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.metricsNavButton.Size = new System.Drawing.Size(130, 45);
            this.metricsNavButton.TabIndex = 2;
            this.metricsNavButton.Tag = "TrayTemps.Font.ExtraBold";
            this.metricsNavButton.Text = "Metrics";
            this.metricsNavButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metricsNavButton.UseVisualStyleBackColor = false;
            this.metricsNavButton.Click += new System.EventHandler(this.MetricsNavButton_Click);
            //
            // presetsNavButton
            //
            this.presetsNavButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.presetsNavButton.Controls.Add(this.accentLine);
            this.presetsNavButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.presetsNavButton.FlatAppearance.BorderSize = 0;
            this.presetsNavButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.presetsNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.presetsNavButton.Location = new System.Drawing.Point(0, 55);
            this.presetsNavButton.Name = "presetsNavButton";
            this.presetsNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.presetsNavButton.Size = new System.Drawing.Size(130, 45);
            this.presetsNavButton.TabIndex = 1;
            this.presetsNavButton.Tag = "TrayTemps.Font.ExtraBold";
            this.presetsNavButton.Text = "Presets";
            this.presetsNavButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.presetsNavButton.UseVisualStyleBackColor = false;
            this.presetsNavButton.Click += new System.EventHandler(this.PresetsNavButton_Click);
            //
            // accentLine
            //
            this.accentLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.accentLine.Dock = System.Windows.Forms.DockStyle.Left;
            this.accentLine.Location = new System.Drawing.Point(0, 0);
            this.accentLine.Name = "accentLine";
            this.accentLine.Size = new System.Drawing.Size(3, 45);
            this.accentLine.TabIndex = 4;
            //
            // navigationTitle
            //
            this.navigationTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.navigationTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navigationTitle.ForeColor = System.Drawing.Color.DarkGray;
            this.navigationTitle.Location = new System.Drawing.Point(0, 0);
            this.navigationTitle.Name = "navigationTitle";
            this.navigationTitle.Padding = new System.Windows.Forms.Padding(15, 18, 0, 0);
            this.navigationTitle.Size = new System.Drawing.Size(130, 55);
            this.navigationTitle.TabIndex = 0;
            this.navigationTitle.Tag = "TrayTemps.Font.ExtraBold";
            this.navigationTitle.Text = "OSD Settings";
            this.navigationTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseDown);
            this.navigationTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseMove);
            this.navigationTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseUp);
            //
            // OsdSettingsDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(800, 590);
            this.Controls.Add(this.outerBorder);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(720, 480);
            this.Name = "OsdSettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OSD Settings";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseUp);
            this.customLabelContextMenu.ResumeLayout(false);
            this.metricContextMenu.ResumeLayout(false);
            this.outerBorder.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.pageTabs.ResumeLayout(false);
            this.presetsPage.ResumeLayout(false);
            this.presetsLayout.ResumeLayout(false);
            this.metricsPage.ResumeLayout(false);
            this.metricsPageLayout.ResumeLayout(false);
            this.metricsCard.ResumeLayout(false);
            this.metricsCard.PerformLayout();
            this.metricsListHeader.ResumeLayout(false);
            this.metricsList.ResumeLayout(false);
            this.metricsList.PerformLayout();
            this.hotkeyCard.ResumeLayout(false);
            this.hotkeyCard.PerformLayout();
            this.appearancePage.ResumeLayout(false);
            this.appearanceCard.ResumeLayout(false);
            this.appearanceLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.appearanceGrid)).EndInit();
            this.appearanceFooter.ResumeLayout(false);
            this.appearanceFooter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundOpacityValue)).EndInit();
            this.backgroundColorLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.opacityValue)).EndInit();
            this.layoutPage.ResumeLayout(false);
            this.layoutPageLayout.ResumeLayout(false);
            this.layoutCard.ResumeLayout(false);
            this.layoutCard.PerformLayout();
            this.groupFieldsLayout.ResumeLayout(false);
            this.groupFieldsLayout.PerformLayout();
            this.spacingCard.ResumeLayout(false);
            this.spacingCard.PerformLayout();
            this.spacingLayout.ResumeLayout(false);
            this.spacingLayout.PerformLayout();
            this.frameTimeGraphSizeLayout.ResumeLayout(false);
            this.bottomBar.ResumeLayout(false);
            this.titleBar.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.presetsNavButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel outerBorder;
        private WindowResizeGripPanel resizeGrip;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel titleBar;
        private System.Windows.Forms.Panel bottomBar;
        private System.Windows.Forms.Panel navigationPanel;
        private System.Windows.Forms.Button metricsNavButton;
        private System.Windows.Forms.Button appearanceNavButton;
        private System.Windows.Forms.Button layoutNavButton;
        private System.Windows.Forms.Panel accentLine;
        private System.Windows.Forms.TableLayoutPanel contentPanel;
        private BorderlessTabControl pageTabs;
        private System.Windows.Forms.TabPage metricsPage;
        private System.Windows.Forms.TableLayoutPanel metricsPageLayout;
        private System.Windows.Forms.TabPage appearancePage;
        private System.Windows.Forms.TabPage layoutPage;
        private System.Windows.Forms.TableLayoutPanel layoutPageLayout;
        private System.Windows.Forms.Panel metricsCard;
        private OsdMetricsListView metricsList;
        private System.Windows.Forms.ColumnHeader metricNameColumn;
        private System.Windows.Forms.ColumnHeader metricLabelColumn;
        private System.Windows.Forms.Panel metricsListHeader;
        private System.Windows.Forms.Label metricNameHeader;
        private System.Windows.Forms.Label metricLabelHeader;
        private System.Windows.Forms.Label metricListHint;
        private System.Windows.Forms.TextBox metricLabelEditor;
        private System.Windows.Forms.Panel appearanceCard;
        private System.Windows.Forms.Panel layoutCard;
        private System.Windows.Forms.Panel spacingCard;
        private System.Windows.Forms.Label spacingTitle;
        private System.Windows.Forms.TableLayoutPanel spacingLayout;
        private System.Windows.Forms.Label frameTimeGraphSizeLabel;
        private System.Windows.Forms.TableLayoutPanel frameTimeGraphSizeLayout;
        private System.Windows.Forms.ComboBox frameTimeGraphWidthValue;
        private System.Windows.Forms.Label frameTimeGraphSizeSeparator;
        private System.Windows.Forms.ComboBox frameTimeGraphHeightValue;
        private System.Windows.Forms.Label groupedValuesGapLabel;
        private System.Windows.Forms.ComboBox groupedValuesGapPercent;
        private System.Windows.Forms.Panel hotkeyCard;
        private System.Windows.Forms.Label displayOrderLabel;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Label positionLabel;
        private System.Windows.Forms.ComboBox positionValue;
        private System.Windows.Forms.Label screenMarginLabel;
        private System.Windows.Forms.ComboBox screenMarginValue;
        private System.Windows.Forms.Label opacityLabel;
        private System.Windows.Forms.TrackBar opacityValue;
        private System.Windows.Forms.Label opacityValueLabel;
        private System.Windows.Forms.Label backgroundColorLabel;
        private System.Windows.Forms.Button backgroundColor;
        private System.Windows.Forms.TrackBar backgroundOpacityValue;
        private System.Windows.Forms.Label backgroundOpacityValueLabel;
        private System.Windows.Forms.Label fpsRefreshIntervalLabel;
        private System.Windows.Forms.ComboBox fpsRefreshIntervalValue;
        private System.Windows.Forms.Label fpsLabelValueSpacingLabel;
        private System.Windows.Forms.ComboBox fpsLabelValueSpacing;
        private System.Windows.Forms.Label externalSpacingLabel;
        private System.Windows.Forms.ComboBox externalSpacingValue;
        private System.Windows.Forms.Label cornerRoundnessLabel;
        private System.Windows.Forms.ComboBox cornerRoundnessValue;
        private System.Windows.Forms.Label columnsLabel;
        private System.Windows.Forms.TabPage presetsPage;
        private System.Windows.Forms.TableLayoutPanel presetsLayout;
        private System.Windows.Forms.Label presetsTitle;
        private System.Windows.Forms.Label presetsHelp;
        private System.Windows.Forms.ComboBox presetList;
        private System.Windows.Forms.Button loadEmbeddedPreset;
        private System.Windows.Forms.Button loadPresetFile;
        private System.Windows.Forms.Button savePresetFile;
        private System.Windows.Forms.Button presetsNavButton;
        private System.Windows.Forms.ComboBox columnsValue;
        private System.Windows.Forms.Label orderLabel;
        private System.Windows.Forms.TreeView itemOrder;
        private System.Windows.Forms.Button orderUp;
        private System.Windows.Forms.Button orderDown;
        private System.Windows.Forms.Label hotkeyLabel;
        private System.Windows.Forms.CheckBox hotkeyEnabled;
        private System.Windows.Forms.TextBox hotkeyValue;
        private System.Windows.Forms.Label displayedHardwareLabel;
        private System.Windows.Forms.Label metricLabelsInfoIcon;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.DataGridView appearanceGrid;
        private System.Windows.Forms.TableLayoutPanel appearanceLayout;
        private System.Windows.Forms.TableLayoutPanel appearanceFooter;
        private System.Windows.Forms.TableLayoutPanel backgroundColorLayout;
        private System.Windows.Forms.ImageList metricRowImages;
        private System.Windows.Forms.ColumnHeader metricGroupColumn;
        private System.Windows.Forms.Label metricGroupHeader;
        private System.Windows.Forms.TableLayoutPanel groupFieldsLayout;
        private System.Windows.Forms.ContextMenuStrip metricContextMenu;
        private System.Windows.Forms.ToolStripMenuItem labelUndo;
        private System.Windows.Forms.ToolStripMenuItem labelCut;
        private System.Windows.Forms.ToolStripMenuItem labelCopy;
        private System.Windows.Forms.ToolStripMenuItem labelPaste;
        private System.Windows.Forms.ToolStripMenuItem labelDelete;
        private System.Windows.Forms.ToolStripMenuItem labelSelectAll;
        private System.Windows.Forms.ToolStripMenuItem insertIconMenu;
        private System.Windows.Forms.ToolStripMenuItem resetLabelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editLabelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editGroupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetMetricLabelMenuItem;
        private System.Windows.Forms.ToolStripSeparator labelEditSeparator;
        private System.Windows.Forms.ToolStripSeparator labelSelectionSeparator;
        private System.Windows.Forms.ToolStripSeparator labelIconSeparator;
        private System.Windows.Forms.ToolStripSeparator labelResetSeparator;
        private System.Windows.Forms.ToolTip settingsToolTip;
        private System.Windows.Forms.ContextMenuStrip customLabelContextMenu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox rowsSpacing;
        private System.Windows.Forms.ComboBox columnsSpacing;
        private System.Windows.Forms.Label columnsGapHeader;
        private System.Windows.Forms.Label rowsGapHeader;
        private System.Windows.Forms.Label navigationTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn appearanceMetricColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn appearanceFamilyColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn appearanceSizeColumn;
        private System.Windows.Forms.DataGridViewButtonColumn appearanceLabelColorColumn;
        private System.Windows.Forms.DataGridViewButtonColumn appearanceValueColorColumn;
    }
}
