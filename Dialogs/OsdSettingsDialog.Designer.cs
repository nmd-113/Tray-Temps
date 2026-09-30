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
            this.settingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.fpsRefreshIntervalValue = new System.Windows.Forms.ComboBox();
            this.fpsLabelValueSpacingLabel = new System.Windows.Forms.Label();
            this.fpsLabelValueSpacing = new System.Windows.Forms.ComboBox();
            this.combineTemperatureAndUsage = new System.Windows.Forms.CheckBox();
            this.columnsValue = new System.Windows.Forms.ComboBox();
            this.labelValueSpacing = new System.Windows.Forms.ComboBox();
            this.hotkeyValue = new System.Windows.Forms.TextBox();
            this.spacingHeader = new System.Windows.Forms.Label();
            this.outerBorder = new System.Windows.Forms.Panel();
            this.resizeGrip = new TrayTemps.WindowResizeGripPanel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.contentPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pageTabs = new TrayTemps.BorderlessTabControl();
            this.metricsPage = new System.Windows.Forms.TabPage();
            this.metricsPageLayout = new System.Windows.Forms.TableLayoutPanel();
            this.metricsCard = new System.Windows.Forms.Panel();
            this.displayedHardwareLabel = new System.Windows.Forms.Label();
            this.showCpu = new System.Windows.Forms.CheckBox();
            this.showGpu = new System.Windows.Forms.CheckBox();
            this.showCpuUsage = new System.Windows.Forms.CheckBox();
            this.showGpuUsage = new System.Windows.Forms.CheckBox();
            this.showRamUsage = new System.Windows.Forms.CheckBox();
            this.showVramUsage = new System.Windows.Forms.CheckBox();
            this.showFps = new System.Windows.Forms.CheckBox();
            this.fpsRefreshIntervalLabel = new System.Windows.Forms.Label();
            this.labelsCard = new System.Windows.Forms.Panel();
            this.labelsTitle = new System.Windows.Forms.Label();
            this.customLabelsEnabled = new System.Windows.Forms.CheckBox();
            this.customLabelsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.customCpuLabelCaption = new System.Windows.Forms.Label();
            this.customCpuLabel = new System.Windows.Forms.TextBox();
            this.customGpuLabelCaption = new System.Windows.Forms.Label();
            this.customGpuLabel = new System.Windows.Forms.TextBox();
            this.customCpuUsageLabelCaption = new System.Windows.Forms.Label();
            this.customCpuUsageLabel = new System.Windows.Forms.TextBox();
            this.customGpuUsageLabelCaption = new System.Windows.Forms.Label();
            this.customGpuUsageLabel = new System.Windows.Forms.TextBox();
            this.customRamLabelCaption = new System.Windows.Forms.Label();
            this.customRamLabel = new System.Windows.Forms.TextBox();
            this.customVramLabelCaption = new System.Windows.Forms.Label();
            this.customVramLabel = new System.Windows.Forms.TextBox();
            this.customFpsLabelCaption = new System.Windows.Forms.Label();
            this.customFpsLabel = new System.Windows.Forms.TextBox();
            this.appearancePage = new System.Windows.Forms.TabPage();
            this.appearanceCard = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.fontColorsLabel = new System.Windows.Forms.Label();
            this.fontFamilyLabel = new System.Windows.Forms.Label();
            this.fontFamilyValue = new System.Windows.Forms.ComboBox();
            this.fontLabel = new System.Windows.Forms.Label();
            this.fontSizeValue = new System.Windows.Forms.ComboBox();
            this.opacityLabel = new System.Windows.Forms.Label();
            this.opacityValue = new System.Windows.Forms.TrackBar();
            this.opacityValueLabel = new System.Windows.Forms.Label();
            this.backgroundOpacityValue = new System.Windows.Forms.TrackBar();
            this.backgroundOpacityValueLabel = new System.Windows.Forms.Label();
            this.fontColorsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.cpuFontColorLabel = new System.Windows.Forms.Label();
            this.cpuFontColor = new System.Windows.Forms.Button();
            this.gpuFontColorLabel = new System.Windows.Forms.Label();
            this.gpuFontColor = new System.Windows.Forms.Button();
            this.fpsFontColorLabel = new System.Windows.Forms.Label();
            this.fpsFontColor = new System.Windows.Forms.Button();
            this.ramFontColorLabel = new System.Windows.Forms.Label();
            this.ramFontColor = new System.Windows.Forms.Button();
            this.vramFontColorLabel = new System.Windows.Forms.Label();
            this.vramFontColor = new System.Windows.Forms.Button();
            this.backgroundColorLabel = new System.Windows.Forms.Label();
            this.backgroundColor = new System.Windows.Forms.Button();
            this.layoutPage = new System.Windows.Forms.TabPage();
            this.layoutPageLayout = new System.Windows.Forms.TableLayoutPanel();
            this.layoutCard = new System.Windows.Forms.Panel();
            this.orderLabel = new System.Windows.Forms.Label();
            this.positionLabel = new System.Windows.Forms.Label();
            this.positionValue = new System.Windows.Forms.ComboBox();
            this.columnsLabel = new System.Windows.Forms.Label();
            this.displayOrderLabel = new System.Windows.Forms.Label();
            this.itemOrder = new System.Windows.Forms.ListBox();
            this.orderUp = new System.Windows.Forms.Button();
            this.orderDown = new System.Windows.Forms.Button();
            this.spacingCard = new System.Windows.Forms.Panel();
            this.spacingTitle = new System.Windows.Forms.Label();
            this.spacingLayout = new System.Windows.Forms.TableLayoutPanel();
            this.screenMarginLabel = new System.Windows.Forms.Label();
            this.screenMarginValue = new System.Windows.Forms.ComboBox();
            this.rowsGapHeader = new System.Windows.Forms.Label();
            this.rowsSpacing = new System.Windows.Forms.ComboBox();
            this.columnsGapHeader = new System.Windows.Forms.Label();
            this.columnsSpacing = new System.Windows.Forms.ComboBox();
            this.hotkeyCard = new System.Windows.Forms.Panel();
            this.hotkeyLabel = new System.Windows.Forms.Label();
            this.hotkeyEnabled = new System.Windows.Forms.CheckBox();
            this.bottomBar = new System.Windows.Forms.Panel();
            this.saveBtn = new System.Windows.Forms.Button();
            this.titleBar = new System.Windows.Forms.Panel();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.exitBtn = new System.Windows.Forms.Button();
            this.navigationPanel = new System.Windows.Forms.Panel();
            this.layoutNavButton = new System.Windows.Forms.Button();
            this.appearanceNavButton = new System.Windows.Forms.Button();
            this.metricsNavButton = new System.Windows.Forms.Button();
            this.navigationTitle = new System.Windows.Forms.Label();
            this.accentLine = new System.Windows.Forms.Panel();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.outerBorder.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.pageTabs.SuspendLayout();
            this.metricsPage.SuspendLayout();
            this.metricsPageLayout.SuspendLayout();
            this.metricsCard.SuspendLayout();
            this.labelsCard.SuspendLayout();
            this.customLabelsLayout.SuspendLayout();
            this.appearancePage.SuspendLayout();
            this.appearanceCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opacityValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundOpacityValue)).BeginInit();
            this.fontColorsLayout.SuspendLayout();
            this.layoutPage.SuspendLayout();
            this.layoutPageLayout.SuspendLayout();
            this.layoutCard.SuspendLayout();
            this.spacingCard.SuspendLayout();
            this.spacingLayout.SuspendLayout();
            this.hotkeyCard.SuspendLayout();
            this.bottomBar.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fpsRefreshIntervalValue
            // 
            this.fpsRefreshIntervalValue.DropDownHeight = 200;
            this.fpsRefreshIntervalValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fpsRefreshIntervalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsRefreshIntervalValue.FormattingEnabled = true;
            this.fpsRefreshIntervalValue.IntegralHeight = false;
            this.fpsRefreshIntervalValue.Location = new System.Drawing.Point(130, 166);
            this.fpsRefreshIntervalValue.Margin = new System.Windows.Forms.Padding(2);
            this.fpsRefreshIntervalValue.Name = "fpsRefreshIntervalValue";
            this.fpsRefreshIntervalValue.Size = new System.Drawing.Size(93, 21);
            this.fpsRefreshIntervalValue.TabIndex = 9;
            this.settingsToolTip.SetToolTip(this.fpsRefreshIntervalValue, "Changes only FPS display refresh; hardware sensor polling is unaffected.");
            this.fpsRefreshIntervalValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // fpsLabelValueSpacingLabel
            // 
            this.fpsLabelValueSpacingLabel.AutoSize = true;
            this.fpsLabelValueSpacingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsLabelValueSpacingLabel.Location = new System.Drawing.Point(244, 170);
            this.fpsLabelValueSpacingLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fpsLabelValueSpacingLabel.Name = "fpsLabelValueSpacingLabel";
            this.fpsLabelValueSpacingLabel.Size = new System.Drawing.Size(104, 13);
            this.fpsLabelValueSpacingLabel.TabIndex = 10;
            this.fpsLabelValueSpacingLabel.Text = "FPS label/value gap";
            this.settingsToolTip.SetToolTip(this.fpsLabelValueSpacingLabel, "Controls spacing between the FPS label and value only.");
            // 
            // fpsLabelValueSpacing
            // 
            this.fpsLabelValueSpacing.DropDownHeight = 200;
            this.fpsLabelValueSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fpsLabelValueSpacing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsLabelValueSpacing.FormattingEnabled = true;
            this.fpsLabelValueSpacing.IntegralHeight = false;
            this.fpsLabelValueSpacing.Location = new System.Drawing.Point(365, 166);
            this.fpsLabelValueSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.fpsLabelValueSpacing.Name = "fpsLabelValueSpacing";
            this.fpsLabelValueSpacing.Size = new System.Drawing.Size(93, 21);
            this.fpsLabelValueSpacing.TabIndex = 11;
            this.settingsToolTip.SetToolTip(this.fpsLabelValueSpacing, "Controls spacing between the FPS label and value only.");
            this.fpsLabelValueSpacing.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // combineTemperatureAndUsage
            // 
            this.combineTemperatureAndUsage.AutoSize = true;
            this.combineTemperatureAndUsage.Location = new System.Drawing.Point(18, 100);
            this.combineTemperatureAndUsage.Margin = new System.Windows.Forms.Padding(2);
            this.combineTemperatureAndUsage.Name = "combineTemperatureAndUsage";
            this.combineTemperatureAndUsage.Size = new System.Drawing.Size(171, 20);
            this.combineTemperatureAndUsage.TabIndex = 2;
            this.combineTemperatureAndUsage.Text = "Combine temps + usage";
            this.settingsToolTip.SetToolTip(this.combineTemperatureAndUsage, "Places each CPU/GPU temperature and usage value together in one OSD entry.");
            this.combineTemperatureAndUsage.UseVisualStyleBackColor = true;
            this.combineTemperatureAndUsage.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // columnsValue
            // 
            this.columnsValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.columnsValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnsValue.FormattingEnabled = true;
            this.columnsValue.Location = new System.Drawing.Point(563, 44);
            this.columnsValue.Margin = new System.Windows.Forms.Padding(2);
            this.columnsValue.Name = "columnsValue";
            this.columnsValue.Size = new System.Drawing.Size(54, 24);
            this.columnsValue.TabIndex = 1;
            this.settingsToolTip.SetToolTip(this.columnsValue, "Controls how enabled OSD metrics are distributed across columns.");
            this.columnsValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // labelValueSpacing
            // 
            this.labelValueSpacing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelValueSpacing.DropDownHeight = 200;
            this.labelValueSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.labelValueSpacing.FormattingEnabled = true;
            this.labelValueSpacing.IntegralHeight = false;
            this.labelValueSpacing.Location = new System.Drawing.Point(435, 2);
            this.labelValueSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.labelValueSpacing.Name = "labelValueSpacing";
            this.labelValueSpacing.Size = new System.Drawing.Size(169, 24);
            this.labelValueSpacing.TabIndex = 2;
            this.settingsToolTip.SetToolTip(this.labelValueSpacing, "Adjusts the gap between each metric label and its value.");
            this.labelValueSpacing.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // hotkeyValue
            // 
            this.hotkeyValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hotkeyValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hotkeyValue.Location = new System.Drawing.Point(162, 50);
            this.hotkeyValue.Margin = new System.Windows.Forms.Padding(2);
            this.hotkeyValue.Name = "hotkeyValue";
            this.hotkeyValue.ReadOnly = true;
            this.hotkeyValue.ShortcutsEnabled = false;
            this.hotkeyValue.Size = new System.Drawing.Size(454, 20);
            this.hotkeyValue.TabIndex = 1;
            this.hotkeyValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.settingsToolTip.SetToolTip(this.hotkeyValue, "Sets the shortcut that toggles OSD visibility.");
            this.hotkeyValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyValue_KeyDown);
            // 
            // spacingHeader
            // 
            this.spacingHeader.AutoSize = true;
            this.spacingHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spacingHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spacingHeader.Location = new System.Drawing.Point(305, 2);
            this.spacingHeader.Margin = new System.Windows.Forms.Padding(2);
            this.spacingHeader.Name = "spacingHeader";
            this.spacingHeader.Size = new System.Drawing.Size(126, 25);
            this.spacingHeader.TabIndex = 19;
            this.spacingHeader.Text = "Label/value spacing";
            this.spacingHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsToolTip.SetToolTip(this.spacingHeader, "Controls label/value spacing for all OSD items except FPS.");
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
            // metricsPage
            // 
            this.metricsPage.AutoScroll = true;
            this.metricsPage.Controls.Add(this.metricsPageLayout);
            this.metricsPage.Location = new System.Drawing.Point(0, 0);
            this.metricsPage.Name = "metricsPage";
            this.metricsPage.Size = new System.Drawing.Size(638, 453);
            this.metricsPage.TabIndex = 0;
            this.metricsPage.Text = "Metrics";
            this.metricsPage.UseVisualStyleBackColor = true;
            // 
            // metricsPageLayout
            // 
            this.metricsPageLayout.ColumnCount = 1;
            this.metricsPageLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.metricsPageLayout.Controls.Add(this.metricsCard, 0, 0);
            this.metricsPageLayout.Controls.Add(this.labelsCard, 0, 1);
            this.metricsPageLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.metricsPageLayout.Location = new System.Drawing.Point(0, 0);
            this.metricsPageLayout.Margin = new System.Windows.Forms.Padding(0);
            this.metricsPageLayout.Name = "metricsPageLayout";
            this.metricsPageLayout.RowCount = 2;
            this.metricsPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 216F));
            this.metricsPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.metricsPageLayout.Size = new System.Drawing.Size(638, 406);
            this.metricsPageLayout.TabIndex = 0;
            // 
            // metricsCard
            // 
            this.metricsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.metricsCard.Controls.Add(this.displayedHardwareLabel);
            this.metricsCard.Controls.Add(this.showCpu);
            this.metricsCard.Controls.Add(this.showGpu);
            this.metricsCard.Controls.Add(this.showCpuUsage);
            this.metricsCard.Controls.Add(this.showGpuUsage);
            this.metricsCard.Controls.Add(this.showRamUsage);
            this.metricsCard.Controls.Add(this.showVramUsage);
            this.metricsCard.Controls.Add(this.showFps);
            this.metricsCard.Controls.Add(this.fpsRefreshIntervalLabel);
            this.metricsCard.Controls.Add(this.fpsRefreshIntervalValue);
            this.metricsCard.Controls.Add(this.fpsLabelValueSpacingLabel);
            this.metricsCard.Controls.Add(this.fpsLabelValueSpacing);
            this.metricsCard.Controls.Add(this.combineTemperatureAndUsage);
            this.metricsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metricsCard.Location = new System.Drawing.Point(0, 0);
            this.metricsCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.metricsCard.Name = "metricsCard";
            this.metricsCard.Size = new System.Drawing.Size(638, 208);
            this.metricsCard.TabIndex = 0;
            // 
            // displayedHardwareLabel
            // 
            this.displayedHardwareLabel.AutoSize = true;
            this.displayedHardwareLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.displayedHardwareLabel.Location = new System.Drawing.Point(16, 14);
            this.displayedHardwareLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.displayedHardwareLabel.Name = "displayedHardwareLabel";
            this.displayedHardwareLabel.Size = new System.Drawing.Size(132, 16);
            this.displayedHardwareLabel.TabIndex = 0;
            this.displayedHardwareLabel.Text = "Displayed metrics";
            // 
            // showCpu
            // 
            this.showCpu.AutoSize = true;
            this.showCpu.Checked = true;
            this.showCpu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showCpu.Location = new System.Drawing.Point(18, 42);
            this.showCpu.Margin = new System.Windows.Forms.Padding(2);
            this.showCpu.Name = "showCpu";
            this.showCpu.Size = new System.Drawing.Size(135, 20);
            this.showCpu.TabIndex = 0;
            this.showCpu.Text = "Show CPU in OSD";
            this.showCpu.UseVisualStyleBackColor = true;
            this.showCpu.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // showGpu
            // 
            this.showGpu.AutoSize = true;
            this.showGpu.Checked = true;
            this.showGpu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGpu.Location = new System.Drawing.Point(18, 71);
            this.showGpu.Margin = new System.Windows.Forms.Padding(2);
            this.showGpu.Name = "showGpu";
            this.showGpu.Size = new System.Drawing.Size(136, 20);
            this.showGpu.TabIndex = 1;
            this.showGpu.Text = "Show GPU in OSD";
            this.showGpu.UseVisualStyleBackColor = true;
            this.showGpu.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // showCpuUsage
            // 
            this.showCpuUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showCpuUsage.AutoSize = true;
            this.showCpuUsage.Location = new System.Drawing.Point(365, 42);
            this.showCpuUsage.Margin = new System.Windows.Forms.Padding(2);
            this.showCpuUsage.Name = "showCpuUsage";
            this.showCpuUsage.Size = new System.Drawing.Size(131, 20);
            this.showCpuUsage.TabIndex = 3;
            this.showCpuUsage.Text = "Show CPU usage";
            this.showCpuUsage.UseVisualStyleBackColor = true;
            this.showCpuUsage.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // showGpuUsage
            // 
            this.showGpuUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showGpuUsage.AutoSize = true;
            this.showGpuUsage.Location = new System.Drawing.Point(365, 71);
            this.showGpuUsage.Margin = new System.Windows.Forms.Padding(2);
            this.showGpuUsage.Name = "showGpuUsage";
            this.showGpuUsage.Size = new System.Drawing.Size(132, 20);
            this.showGpuUsage.TabIndex = 4;
            this.showGpuUsage.Text = "Show GPU usage";
            this.showGpuUsage.UseVisualStyleBackColor = true;
            this.showGpuUsage.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // showRamUsage
            // 
            this.showRamUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showRamUsage.AutoSize = true;
            this.showRamUsage.Location = new System.Drawing.Point(365, 100);
            this.showRamUsage.Margin = new System.Windows.Forms.Padding(2);
            this.showRamUsage.Name = "showRamUsage";
            this.showRamUsage.Size = new System.Drawing.Size(133, 20);
            this.showRamUsage.TabIndex = 5;
            this.showRamUsage.Text = "Show RAM usage";
            this.showRamUsage.UseVisualStyleBackColor = true;
            this.showRamUsage.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // showVramUsage
            // 
            this.showVramUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showVramUsage.AutoSize = true;
            this.showVramUsage.Location = new System.Drawing.Point(365, 129);
            this.showVramUsage.Margin = new System.Windows.Forms.Padding(2);
            this.showVramUsage.Name = "showVramUsage";
            this.showVramUsage.Size = new System.Drawing.Size(142, 20);
            this.showVramUsage.TabIndex = 6;
            this.showVramUsage.Text = "Show VRAM usage";
            this.showVramUsage.UseVisualStyleBackColor = true;
            this.showVramUsage.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // showFps
            // 
            this.showFps.AutoSize = true;
            this.showFps.Location = new System.Drawing.Point(18, 129);
            this.showFps.Margin = new System.Windows.Forms.Padding(2);
            this.showFps.Name = "showFps";
            this.showFps.Size = new System.Drawing.Size(88, 20);
            this.showFps.TabIndex = 7;
            this.showFps.Text = "Show FPS";
            this.showFps.UseVisualStyleBackColor = true;
            this.showFps.CheckedChanged += new System.EventHandler(this.HardwareVisibility_CheckedChanged);
            // 
            // fpsRefreshIntervalLabel
            // 
            this.fpsRefreshIntervalLabel.AutoSize = true;
            this.fpsRefreshIntervalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsRefreshIntervalLabel.Location = new System.Drawing.Point(18, 170);
            this.fpsRefreshIntervalLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fpsRefreshIntervalLabel.Name = "fpsRefreshIntervalLabel";
            this.fpsRefreshIntervalLabel.Size = new System.Drawing.Size(99, 13);
            this.fpsRefreshIntervalLabel.TabIndex = 8;
            this.fpsRefreshIntervalLabel.Text = "FPS refresh interval";
            // 
            // labelsCard
            // 
            this.labelsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelsCard.Controls.Add(this.labelsTitle);
            this.labelsCard.Controls.Add(this.customLabelsEnabled);
            this.labelsCard.Controls.Add(this.customLabelsLayout);
            this.labelsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelsCard.Location = new System.Drawing.Point(0, 216);
            this.labelsCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelsCard.Name = "labelsCard";
            this.labelsCard.Size = new System.Drawing.Size(638, 182);
            this.labelsCard.TabIndex = 1;
            // 
            // labelsTitle
            // 
            this.labelsTitle.AutoSize = true;
            this.labelsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.labelsTitle.Location = new System.Drawing.Point(16, 14);
            this.labelsTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelsTitle.Name = "labelsTitle";
            this.labelsTitle.Size = new System.Drawing.Size(134, 16);
            this.labelsTitle.TabIndex = 0;
            this.labelsTitle.Text = "Labels and names";
            // 
            // customLabelsEnabled
            // 
            this.customLabelsEnabled.AutoSize = true;
            this.customLabelsEnabled.Location = new System.Drawing.Point(18, 41);
            this.customLabelsEnabled.Margin = new System.Windows.Forms.Padding(2);
            this.customLabelsEnabled.Name = "customLabelsEnabled";
            this.customLabelsEnabled.Size = new System.Drawing.Size(111, 20);
            this.customLabelsEnabled.TabIndex = 0;
            this.customLabelsEnabled.Text = "Custom labels";
            this.customLabelsEnabled.UseVisualStyleBackColor = true;
            this.customLabelsEnabled.CheckedChanged += new System.EventHandler(this.CustomLabelsEnabled_CheckedChanged);
            // 
            // customLabelsLayout
            // 
            this.customLabelsLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customLabelsLayout.ColumnCount = 4;
            this.customLabelsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.customLabelsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.customLabelsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.customLabelsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.customLabelsLayout.Controls.Add(this.customCpuLabelCaption, 0, 0);
            this.customLabelsLayout.Controls.Add(this.customCpuLabel, 1, 0);
            this.customLabelsLayout.Controls.Add(this.customGpuLabelCaption, 2, 0);
            this.customLabelsLayout.Controls.Add(this.customGpuLabel, 3, 0);
            this.customLabelsLayout.Controls.Add(this.customCpuUsageLabelCaption, 0, 1);
            this.customLabelsLayout.Controls.Add(this.customCpuUsageLabel, 1, 1);
            this.customLabelsLayout.Controls.Add(this.customGpuUsageLabelCaption, 2, 1);
            this.customLabelsLayout.Controls.Add(this.customGpuUsageLabel, 3, 1);
            this.customLabelsLayout.Controls.Add(this.customRamLabelCaption, 0, 2);
            this.customLabelsLayout.Controls.Add(this.customRamLabel, 1, 2);
            this.customLabelsLayout.Controls.Add(this.customVramLabelCaption, 2, 2);
            this.customLabelsLayout.Controls.Add(this.customVramLabel, 3, 2);
            this.customLabelsLayout.Controls.Add(this.customFpsLabelCaption, 0, 3);
            this.customLabelsLayout.Controls.Add(this.customFpsLabel, 1, 3);
            this.customLabelsLayout.Location = new System.Drawing.Point(16, 65);
            this.customLabelsLayout.Margin = new System.Windows.Forms.Padding(2);
            this.customLabelsLayout.Name = "customLabelsLayout";
            this.customLabelsLayout.RowCount = 4;
            this.customLabelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.customLabelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.customLabelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.customLabelsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.customLabelsLayout.Size = new System.Drawing.Size(606, 108);
            this.customLabelsLayout.TabIndex = 3;
            // 
            // customCpuLabelCaption
            // 
            this.customCpuLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customCpuLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customCpuLabelCaption.Location = new System.Drawing.Point(2, 2);
            this.customCpuLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customCpuLabelCaption.Name = "customCpuLabelCaption";
            this.customCpuLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customCpuLabelCaption.TabIndex = 0;
            this.customCpuLabelCaption.Text = "CPU Temp";
            this.customCpuLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customCpuLabel
            // 
            this.customCpuLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customCpuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customCpuLabel.Location = new System.Drawing.Point(82, 3);
            this.customCpuLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customCpuLabel.MaxLength = 40;
            this.customCpuLabel.Name = "customCpuLabel";
            this.customCpuLabel.Size = new System.Drawing.Size(219, 20);
            this.customCpuLabel.TabIndex = 1;
            this.customCpuLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // customGpuLabelCaption
            // 
            this.customGpuLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customGpuLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customGpuLabelCaption.Location = new System.Drawing.Point(305, 2);
            this.customGpuLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customGpuLabelCaption.Name = "customGpuLabelCaption";
            this.customGpuLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customGpuLabelCaption.TabIndex = 3;
            this.customGpuLabelCaption.Text = "GPU Temp";
            this.customGpuLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customGpuLabel
            // 
            this.customGpuLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customGpuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customGpuLabel.Location = new System.Drawing.Point(385, 3);
            this.customGpuLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customGpuLabel.MaxLength = 40;
            this.customGpuLabel.Name = "customGpuLabel";
            this.customGpuLabel.Size = new System.Drawing.Size(219, 20);
            this.customGpuLabel.TabIndex = 3;
            this.customGpuLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // customCpuUsageLabelCaption
            // 
            this.customCpuUsageLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customCpuUsageLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customCpuUsageLabelCaption.Location = new System.Drawing.Point(2, 29);
            this.customCpuUsageLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customCpuUsageLabelCaption.Name = "customCpuUsageLabelCaption";
            this.customCpuUsageLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customCpuUsageLabelCaption.TabIndex = 6;
            this.customCpuUsageLabelCaption.Text = "CPU Load";
            this.customCpuUsageLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customCpuUsageLabel
            // 
            this.customCpuUsageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customCpuUsageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customCpuUsageLabel.Location = new System.Drawing.Point(82, 30);
            this.customCpuUsageLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customCpuUsageLabel.MaxLength = 40;
            this.customCpuUsageLabel.Name = "customCpuUsageLabel";
            this.customCpuUsageLabel.Size = new System.Drawing.Size(219, 20);
            this.customCpuUsageLabel.TabIndex = 5;
            this.customCpuUsageLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // customGpuUsageLabelCaption
            // 
            this.customGpuUsageLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customGpuUsageLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customGpuUsageLabelCaption.Location = new System.Drawing.Point(305, 29);
            this.customGpuUsageLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customGpuUsageLabelCaption.Name = "customGpuUsageLabelCaption";
            this.customGpuUsageLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customGpuUsageLabelCaption.TabIndex = 9;
            this.customGpuUsageLabelCaption.Text = "GPU Load";
            this.customGpuUsageLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customGpuUsageLabel
            // 
            this.customGpuUsageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customGpuUsageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customGpuUsageLabel.Location = new System.Drawing.Point(385, 30);
            this.customGpuUsageLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customGpuUsageLabel.MaxLength = 40;
            this.customGpuUsageLabel.Name = "customGpuUsageLabel";
            this.customGpuUsageLabel.Size = new System.Drawing.Size(219, 20);
            this.customGpuUsageLabel.TabIndex = 7;
            this.customGpuUsageLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // customRamLabelCaption
            // 
            this.customRamLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customRamLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customRamLabelCaption.Location = new System.Drawing.Point(2, 56);
            this.customRamLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customRamLabelCaption.Name = "customRamLabelCaption";
            this.customRamLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customRamLabelCaption.TabIndex = 12;
            this.customRamLabelCaption.Text = "RAM Use";
            this.customRamLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customRamLabel
            // 
            this.customRamLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customRamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customRamLabel.Location = new System.Drawing.Point(82, 57);
            this.customRamLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customRamLabel.MaxLength = 40;
            this.customRamLabel.Name = "customRamLabel";
            this.customRamLabel.Size = new System.Drawing.Size(219, 20);
            this.customRamLabel.TabIndex = 9;
            this.customRamLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // customVramLabelCaption
            // 
            this.customVramLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customVramLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customVramLabelCaption.Location = new System.Drawing.Point(305, 56);
            this.customVramLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customVramLabelCaption.Name = "customVramLabelCaption";
            this.customVramLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customVramLabelCaption.TabIndex = 15;
            this.customVramLabelCaption.Text = "VRAM Use";
            this.customVramLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customVramLabel
            // 
            this.customVramLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customVramLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customVramLabel.Location = new System.Drawing.Point(385, 57);
            this.customVramLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customVramLabel.MaxLength = 40;
            this.customVramLabel.Name = "customVramLabel";
            this.customVramLabel.Size = new System.Drawing.Size(219, 20);
            this.customVramLabel.TabIndex = 11;
            this.customVramLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // customFpsLabelCaption
            // 
            this.customFpsLabelCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customFpsLabelCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customFpsLabelCaption.Location = new System.Drawing.Point(2, 83);
            this.customFpsLabelCaption.Margin = new System.Windows.Forms.Padding(2);
            this.customFpsLabelCaption.Name = "customFpsLabelCaption";
            this.customFpsLabelCaption.Size = new System.Drawing.Size(76, 23);
            this.customFpsLabelCaption.TabIndex = 18;
            this.customFpsLabelCaption.Text = "FPS";
            this.customFpsLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customFpsLabel
            // 
            this.customFpsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customFpsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customFpsLabel.Location = new System.Drawing.Point(82, 84);
            this.customFpsLabel.Margin = new System.Windows.Forms.Padding(2);
            this.customFpsLabel.MaxLength = 40;
            this.customFpsLabel.Name = "customFpsLabel";
            this.customFpsLabel.Size = new System.Drawing.Size(219, 20);
            this.customFpsLabel.TabIndex = 13;
            this.customFpsLabel.TextChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // appearancePage
            // 
            this.appearancePage.AutoScroll = true;
            this.appearancePage.Controls.Add(this.appearanceCard);
            this.appearancePage.Location = new System.Drawing.Point(0, 0);
            this.appearancePage.Name = "appearancePage";
            this.appearancePage.Size = new System.Drawing.Size(638, 453);
            this.appearancePage.TabIndex = 1;
            this.appearancePage.Text = "Appearance";
            this.appearancePage.UseVisualStyleBackColor = true;
            // 
            // appearanceCard
            // 
            this.appearanceCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.appearanceCard.Controls.Add(this.label1);
            this.appearanceCard.Controls.Add(this.fontColorsLabel);
            this.appearanceCard.Controls.Add(this.fontFamilyLabel);
            this.appearanceCard.Controls.Add(this.fontFamilyValue);
            this.appearanceCard.Controls.Add(this.fontLabel);
            this.appearanceCard.Controls.Add(this.fontSizeValue);
            this.appearanceCard.Controls.Add(this.opacityLabel);
            this.appearanceCard.Controls.Add(this.opacityValue);
            this.appearanceCard.Controls.Add(this.opacityValueLabel);
            this.appearanceCard.Controls.Add(this.backgroundOpacityValue);
            this.appearanceCard.Controls.Add(this.backgroundOpacityValueLabel);
            this.appearanceCard.Controls.Add(this.fontColorsLayout);
            this.appearanceCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.appearanceCard.Location = new System.Drawing.Point(0, 0);
            this.appearanceCard.Margin = new System.Windows.Forms.Padding(0);
            this.appearanceCard.Name = "appearanceCard";
            this.appearanceCard.Size = new System.Drawing.Size(638, 260);
            this.appearanceCard.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 218);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "BG Transparency";
            // 
            // fontColorsLabel
            // 
            this.fontColorsLabel.AutoSize = true;
            this.fontColorsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.fontColorsLabel.Location = new System.Drawing.Point(16, 14);
            this.fontColorsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fontColorsLabel.Name = "fontColorsLabel";
            this.fontColorsLabel.Size = new System.Drawing.Size(92, 16);
            this.fontColorsLabel.TabIndex = 0;
            this.fontColorsLabel.Text = "Appearance";
            // 
            // fontFamilyLabel
            // 
            this.fontFamilyLabel.AutoSize = true;
            this.fontFamilyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontFamilyLabel.Location = new System.Drawing.Point(16, 46);
            this.fontFamilyLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fontFamilyLabel.Name = "fontFamilyLabel";
            this.fontFamilyLabel.Size = new System.Drawing.Size(57, 13);
            this.fontFamilyLabel.TabIndex = 1;
            this.fontFamilyLabel.Text = "Font family";
            // 
            // fontFamilyValue
            // 
            this.fontFamilyValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontFamilyValue.DropDownHeight = 200;
            this.fontFamilyValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontFamilyValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontFamilyValue.FormattingEnabled = true;
            this.fontFamilyValue.IntegralHeight = false;
            this.fontFamilyValue.Location = new System.Drawing.Point(84, 42);
            this.fontFamilyValue.Margin = new System.Windows.Forms.Padding(2);
            this.fontFamilyValue.Name = "fontFamilyValue";
            this.fontFamilyValue.Size = new System.Drawing.Size(384, 21);
            this.fontFamilyValue.TabIndex = 0;
            this.fontFamilyValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // fontLabel
            // 
            this.fontLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fontLabel.AutoSize = true;
            this.fontLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontLabel.Location = new System.Drawing.Point(510, 46);
            this.fontLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(27, 13);
            this.fontLabel.TabIndex = 2;
            this.fontLabel.Text = "Size";
            // 
            // fontSizeValue
            // 
            this.fontSizeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSizeValue.DropDownHeight = 200;
            this.fontSizeValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontSizeValue.FormattingEnabled = true;
            this.fontSizeValue.IntegralHeight = false;
            this.fontSizeValue.Location = new System.Drawing.Point(548, 40);
            this.fontSizeValue.Margin = new System.Windows.Forms.Padding(2);
            this.fontSizeValue.Name = "fontSizeValue";
            this.fontSizeValue.Size = new System.Drawing.Size(69, 24);
            this.fontSizeValue.TabIndex = 1;
            this.fontSizeValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // opacityLabel
            // 
            this.opacityLabel.AutoSize = true;
            this.opacityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opacityLabel.Location = new System.Drawing.Point(16, 82);
            this.opacityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.opacityLabel.Name = "opacityLabel";
            this.opacityLabel.Size = new System.Drawing.Size(92, 13);
            this.opacityLabel.TabIndex = 3;
            this.opacityLabel.Text = "Text transparency";
            // 
            // opacityValue
            // 
            this.opacityValue.AutoSize = false;
            this.opacityValue.Location = new System.Drawing.Point(154, 74);
            this.opacityValue.Margin = new System.Windows.Forms.Padding(2);
            this.opacityValue.Maximum = 100;
            this.opacityValue.Minimum = 20;
            this.opacityValue.Name = "opacityValue";
            this.opacityValue.Size = new System.Drawing.Size(125, 30);
            this.opacityValue.TabIndex = 2;
            this.opacityValue.TickFrequency = 10;
            this.opacityValue.Value = 90;
            this.opacityValue.ValueChanged += new System.EventHandler(this.OpacityValue_ValueChanged);
            // 
            // opacityValueLabel
            // 
            this.opacityValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opacityValueLabel.Location = new System.Drawing.Point(107, 78);
            this.opacityValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.opacityValueLabel.Name = "opacityValueLabel";
            this.opacityValueLabel.Size = new System.Drawing.Size(46, 22);
            this.opacityValueLabel.TabIndex = 4;
            this.opacityValueLabel.Text = "90%";
            this.opacityValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // backgroundOpacityValue
            // 
            this.backgroundOpacityValue.AutoSize = false;
            this.backgroundOpacityValue.Location = new System.Drawing.Point(154, 210);
            this.backgroundOpacityValue.Margin = new System.Windows.Forms.Padding(2);
            this.backgroundOpacityValue.Maximum = 100;
            this.backgroundOpacityValue.Name = "backgroundOpacityValue";
            this.backgroundOpacityValue.Size = new System.Drawing.Size(123, 30);
            this.backgroundOpacityValue.TabIndex = 8;
            this.backgroundOpacityValue.TickFrequency = 10;
            this.backgroundOpacityValue.Value = 100;
            this.backgroundOpacityValue.ValueChanged += new System.EventHandler(this.BackgroundOpacityValue_ValueChanged);
            // 
            // backgroundOpacityValueLabel
            // 
            this.backgroundOpacityValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backgroundOpacityValueLabel.Location = new System.Drawing.Point(107, 214);
            this.backgroundOpacityValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.backgroundOpacityValueLabel.Name = "backgroundOpacityValueLabel";
            this.backgroundOpacityValueLabel.Size = new System.Drawing.Size(46, 22);
            this.backgroundOpacityValueLabel.TabIndex = 10;
            this.backgroundOpacityValueLabel.Text = "100%";
            this.backgroundOpacityValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fontColorsLayout
            // 
            this.fontColorsLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontColorsLayout.ColumnCount = 6;
            this.fontColorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.fontColorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.fontColorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.fontColorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.fontColorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.fontColorsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.fontColorsLayout.Controls.Add(this.cpuFontColorLabel, 0, 0);
            this.fontColorsLayout.Controls.Add(this.cpuFontColor, 1, 0);
            this.fontColorsLayout.Controls.Add(this.gpuFontColorLabel, 2, 0);
            this.fontColorsLayout.Controls.Add(this.gpuFontColor, 3, 0);
            this.fontColorsLayout.Controls.Add(this.fpsFontColorLabel, 4, 0);
            this.fontColorsLayout.Controls.Add(this.fpsFontColor, 5, 0);
            this.fontColorsLayout.Controls.Add(this.ramFontColorLabel, 0, 1);
            this.fontColorsLayout.Controls.Add(this.ramFontColor, 1, 1);
            this.fontColorsLayout.Controls.Add(this.vramFontColorLabel, 2, 1);
            this.fontColorsLayout.Controls.Add(this.vramFontColor, 3, 1);
            this.fontColorsLayout.Controls.Add(this.backgroundColorLabel, 4, 1);
            this.fontColorsLayout.Controls.Add(this.backgroundColor, 5, 1);
            this.fontColorsLayout.Location = new System.Drawing.Point(16, 112);
            this.fontColorsLayout.Margin = new System.Windows.Forms.Padding(2);
            this.fontColorsLayout.Name = "fontColorsLayout";
            this.fontColorsLayout.RowCount = 2;
            this.fontColorsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.fontColorsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.fontColorsLayout.Size = new System.Drawing.Size(603, 86);
            this.fontColorsLayout.TabIndex = 11;
            // 
            // cpuFontColorLabel
            // 
            this.cpuFontColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuFontColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuFontColorLabel.Location = new System.Drawing.Point(2, 2);
            this.cpuFontColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.cpuFontColorLabel.Name = "cpuFontColorLabel";
            this.cpuFontColorLabel.Size = new System.Drawing.Size(70, 39);
            this.cpuFontColorLabel.TabIndex = 5;
            this.cpuFontColorLabel.Text = "CPU color";
            this.cpuFontColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuFontColor
            // 
            this.cpuFontColor.BackColor = System.Drawing.Color.Aqua;
            this.cpuFontColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuFontColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuFontColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuFontColor.Location = new System.Drawing.Point(76, 2);
            this.cpuFontColor.Margin = new System.Windows.Forms.Padding(2);
            this.cpuFontColor.Name = "cpuFontColor";
            this.cpuFontColor.Size = new System.Drawing.Size(122, 39);
            this.cpuFontColor.TabIndex = 3;
            this.cpuFontColor.Text = "🎨";
            this.cpuFontColor.UseVisualStyleBackColor = false;
            this.cpuFontColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // gpuFontColorLabel
            // 
            this.gpuFontColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuFontColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuFontColorLabel.Location = new System.Drawing.Point(202, 2);
            this.gpuFontColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.gpuFontColorLabel.Name = "gpuFontColorLabel";
            this.gpuFontColorLabel.Size = new System.Drawing.Size(70, 39);
            this.gpuFontColorLabel.TabIndex = 6;
            this.gpuFontColorLabel.Text = "GPU color";
            this.gpuFontColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuFontColor
            // 
            this.gpuFontColor.BackColor = System.Drawing.Color.Gold;
            this.gpuFontColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuFontColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuFontColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuFontColor.Location = new System.Drawing.Point(276, 2);
            this.gpuFontColor.Margin = new System.Windows.Forms.Padding(2);
            this.gpuFontColor.Name = "gpuFontColor";
            this.gpuFontColor.Size = new System.Drawing.Size(122, 39);
            this.gpuFontColor.TabIndex = 4;
            this.gpuFontColor.Text = "🎨";
            this.gpuFontColor.UseVisualStyleBackColor = false;
            this.gpuFontColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // fpsFontColorLabel
            // 
            this.fpsFontColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpsFontColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsFontColorLabel.Location = new System.Drawing.Point(402, 2);
            this.fpsFontColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.fpsFontColorLabel.Name = "fpsFontColorLabel";
            this.fpsFontColorLabel.Size = new System.Drawing.Size(70, 39);
            this.fpsFontColorLabel.TabIndex = 11;
            this.fpsFontColorLabel.Text = "FPS color";
            this.fpsFontColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fpsFontColor
            // 
            this.fpsFontColor.BackColor = System.Drawing.Color.WhiteSmoke;
            this.fpsFontColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpsFontColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fpsFontColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsFontColor.Location = new System.Drawing.Point(476, 2);
            this.fpsFontColor.Margin = new System.Windows.Forms.Padding(2);
            this.fpsFontColor.Name = "fpsFontColor";
            this.fpsFontColor.Size = new System.Drawing.Size(125, 39);
            this.fpsFontColor.TabIndex = 12;
            this.fpsFontColor.Text = "🎨";
            this.fpsFontColor.UseVisualStyleBackColor = false;
            this.fpsFontColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // ramFontColorLabel
            // 
            this.ramFontColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ramFontColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramFontColorLabel.Location = new System.Drawing.Point(2, 45);
            this.ramFontColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.ramFontColorLabel.Name = "ramFontColorLabel";
            this.ramFontColorLabel.Size = new System.Drawing.Size(70, 39);
            this.ramFontColorLabel.TabIndex = 7;
            this.ramFontColorLabel.Text = "RAM color";
            this.ramFontColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ramFontColor
            // 
            this.ramFontColor.BackColor = System.Drawing.Color.LightGreen;
            this.ramFontColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ramFontColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ramFontColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramFontColor.Location = new System.Drawing.Point(76, 45);
            this.ramFontColor.Margin = new System.Windows.Forms.Padding(2);
            this.ramFontColor.Name = "ramFontColor";
            this.ramFontColor.Size = new System.Drawing.Size(122, 39);
            this.ramFontColor.TabIndex = 5;
            this.ramFontColor.Text = "🎨";
            this.ramFontColor.UseVisualStyleBackColor = false;
            this.ramFontColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // vramFontColorLabel
            // 
            this.vramFontColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vramFontColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vramFontColorLabel.Location = new System.Drawing.Point(202, 45);
            this.vramFontColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.vramFontColorLabel.Name = "vramFontColorLabel";
            this.vramFontColorLabel.Size = new System.Drawing.Size(70, 39);
            this.vramFontColorLabel.TabIndex = 8;
            this.vramFontColorLabel.Text = "VRAM color";
            this.vramFontColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // vramFontColor
            // 
            this.vramFontColor.BackColor = System.Drawing.Color.Violet;
            this.vramFontColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vramFontColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.vramFontColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vramFontColor.Location = new System.Drawing.Point(276, 45);
            this.vramFontColor.Margin = new System.Windows.Forms.Padding(2);
            this.vramFontColor.Name = "vramFontColor";
            this.vramFontColor.Size = new System.Drawing.Size(122, 39);
            this.vramFontColor.TabIndex = 6;
            this.vramFontColor.Text = "🎨";
            this.vramFontColor.UseVisualStyleBackColor = false;
            this.vramFontColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // backgroundColorLabel
            // 
            this.backgroundColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backgroundColorLabel.Location = new System.Drawing.Point(402, 45);
            this.backgroundColorLabel.Margin = new System.Windows.Forms.Padding(2);
            this.backgroundColorLabel.Name = "backgroundColorLabel";
            this.backgroundColorLabel.Size = new System.Drawing.Size(70, 39);
            this.backgroundColorLabel.TabIndex = 9;
            this.backgroundColorLabel.Text = "BG Color";
            this.backgroundColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // backgroundColor
            // 
            this.backgroundColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.backgroundColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backgroundColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backgroundColor.Location = new System.Drawing.Point(476, 45);
            this.backgroundColor.Margin = new System.Windows.Forms.Padding(2);
            this.backgroundColor.Name = "backgroundColor";
            this.backgroundColor.Size = new System.Drawing.Size(125, 39);
            this.backgroundColor.TabIndex = 7;
            this.backgroundColor.Text = "🎨";
            this.backgroundColor.UseVisualStyleBackColor = false;
            this.backgroundColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // layoutPage
            // 
            this.layoutPage.AutoScroll = true;
            this.layoutPage.AutoScrollMinSize = new System.Drawing.Size(0, 448);
            this.layoutPage.Controls.Add(this.layoutPageLayout);
            this.layoutPage.Location = new System.Drawing.Point(0, 0);
            this.layoutPage.Name = "layoutPage";
            this.layoutPage.Size = new System.Drawing.Size(638, 453);
            this.layoutPage.TabIndex = 2;
            this.layoutPage.Text = "Layout";
            this.layoutPage.UseVisualStyleBackColor = true;
            // 
            // layoutPageLayout
            // 
            this.layoutPageLayout.ColumnCount = 1;
            this.layoutPageLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPageLayout.Controls.Add(this.layoutCard, 0, 0);
            this.layoutPageLayout.Controls.Add(this.spacingCard, 0, 1);
            this.layoutPageLayout.Controls.Add(this.hotkeyCard, 0, 2);
            this.layoutPageLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPageLayout.Location = new System.Drawing.Point(0, 0);
            this.layoutPageLayout.Margin = new System.Windows.Forms.Padding(0);
            this.layoutPageLayout.Name = "layoutPageLayout";
            this.layoutPageLayout.RowCount = 3;
            this.layoutPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutPageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.layoutPageLayout.Size = new System.Drawing.Size(638, 453);
            this.layoutPageLayout.TabIndex = 0;
            // 
            // layoutCard
            // 
            this.layoutCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.layoutCard.Controls.Add(this.orderLabel);
            this.layoutCard.Controls.Add(this.positionLabel);
            this.layoutCard.Controls.Add(this.positionValue);
            this.layoutCard.Controls.Add(this.columnsLabel);
            this.layoutCard.Controls.Add(this.columnsValue);
            this.layoutCard.Controls.Add(this.displayOrderLabel);
            this.layoutCard.Controls.Add(this.itemOrder);
            this.layoutCard.Controls.Add(this.orderUp);
            this.layoutCard.Controls.Add(this.orderDown);
            this.layoutCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutCard.Location = new System.Drawing.Point(0, 0);
            this.layoutCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.layoutCard.MinimumSize = new System.Drawing.Size(0, 230);
            this.layoutCard.Name = "layoutCard";
            this.layoutCard.Size = new System.Drawing.Size(638, 235);
            this.layoutCard.TabIndex = 1;
            // 
            // orderLabel
            // 
            this.orderLabel.AutoSize = true;
            this.orderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.orderLabel.Location = new System.Drawing.Point(16, 16);
            this.orderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(139, 16);
            this.orderLabel.TabIndex = 0;
            this.orderLabel.Text = "Position and layout";
            // 
            // positionLabel
            // 
            this.positionLabel.AutoSize = true;
            this.positionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.positionLabel.Location = new System.Drawing.Point(16, 50);
            this.positionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.Size = new System.Drawing.Size(44, 13);
            this.positionLabel.TabIndex = 1;
            this.positionLabel.Text = "Position";
            // 
            // positionValue
            // 
            this.positionValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.positionValue.FormattingEnabled = true;
            this.positionValue.Location = new System.Drawing.Point(78, 44);
            this.positionValue.Margin = new System.Windows.Forms.Padding(2);
            this.positionValue.Name = "positionValue";
            this.positionValue.Size = new System.Drawing.Size(176, 24);
            this.positionValue.TabIndex = 0;
            this.positionValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // columnsLabel
            // 
            this.columnsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.columnsLabel.AutoSize = true;
            this.columnsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.columnsLabel.Location = new System.Drawing.Point(472, 50);
            this.columnsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.columnsLabel.Name = "columnsLabel";
            this.columnsLabel.Size = new System.Drawing.Size(78, 13);
            this.columnsLabel.TabIndex = 2;
            this.columnsLabel.Text = "No. of columns";
            // 
            // displayOrderLabel
            // 
            this.displayOrderLabel.AutoSize = true;
            this.displayOrderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayOrderLabel.Location = new System.Drawing.Point(16, 77);
            this.displayOrderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.displayOrderLabel.Name = "displayOrderLabel";
            this.displayOrderLabel.Size = new System.Drawing.Size(109, 13);
            this.displayOrderLabel.TabIndex = 3;
            this.displayOrderLabel.Text = "Display order / priority";
            // 
            // itemOrder
            // 
            this.itemOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemOrder.FormattingEnabled = true;
            this.itemOrder.Location = new System.Drawing.Point(18, 100);
            this.itemOrder.Margin = new System.Windows.Forms.Padding(2);
            this.itemOrder.Name = "itemOrder";
            this.itemOrder.Size = new System.Drawing.Size(502, 108);
            this.itemOrder.TabIndex = 2;
            // 
            // orderUp
            // 
            this.orderUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orderUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.orderUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderUp.Location = new System.Drawing.Point(531, 100);
            this.orderUp.Margin = new System.Windows.Forms.Padding(2);
            this.orderUp.Name = "orderUp";
            this.orderUp.Size = new System.Drawing.Size(86, 32);
            this.orderUp.TabIndex = 3;
            this.orderUp.Text = "Up";
            this.orderUp.UseVisualStyleBackColor = true;
            this.orderUp.Click += new System.EventHandler(this.OrderUp_Click);
            // 
            // orderDown
            // 
            this.orderDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orderDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.orderDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderDown.Location = new System.Drawing.Point(531, 138);
            this.orderDown.Margin = new System.Windows.Forms.Padding(2);
            this.orderDown.Name = "orderDown";
            this.orderDown.Size = new System.Drawing.Size(86, 32);
            this.orderDown.TabIndex = 4;
            this.orderDown.Text = "Down";
            this.orderDown.UseVisualStyleBackColor = true;
            this.orderDown.Click += new System.EventHandler(this.OrderDown_Click);
            // 
            // spacingCard
            // 
            this.spacingCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.spacingCard.Controls.Add(this.spacingTitle);
            this.spacingCard.Controls.Add(this.spacingLayout);
            this.spacingCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spacingCard.Location = new System.Drawing.Point(0, 243);
            this.spacingCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.spacingCard.Name = "spacingCard";
            this.spacingCard.Size = new System.Drawing.Size(638, 112);
            this.spacingCard.TabIndex = 1;
            // 
            // spacingTitle
            // 
            this.spacingTitle.AutoSize = true;
            this.spacingTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.spacingTitle.Location = new System.Drawing.Point(16, 14);
            this.spacingTitle.Name = "spacingTitle";
            this.spacingTitle.Size = new System.Drawing.Size(64, 16);
            this.spacingTitle.TabIndex = 0;
            this.spacingTitle.Text = "Spacing";
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
            this.spacingLayout.Controls.Add(this.spacingHeader, 2, 0);
            this.spacingLayout.Controls.Add(this.labelValueSpacing, 3, 0);
            this.spacingLayout.Controls.Add(this.rowsGapHeader, 0, 1);
            this.spacingLayout.Controls.Add(this.rowsSpacing, 1, 1);
            this.spacingLayout.Controls.Add(this.columnsGapHeader, 2, 1);
            this.spacingLayout.Controls.Add(this.columnsSpacing, 3, 1);
            this.spacingLayout.Location = new System.Drawing.Point(16, 35);
            this.spacingLayout.Margin = new System.Windows.Forms.Padding(0);
            this.spacingLayout.Name = "spacingLayout";
            this.spacingLayout.RowCount = 2;
            this.spacingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.spacingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.spacingLayout.Size = new System.Drawing.Size(606, 58);
            this.spacingLayout.TabIndex = 1;
            // 
            // screenMarginLabel
            // 
            this.screenMarginLabel.AutoSize = true;
            this.screenMarginLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenMarginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.screenMarginLabel.Location = new System.Drawing.Point(2, 0);
            this.screenMarginLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.screenMarginLabel.Name = "screenMarginLabel";
            this.screenMarginLabel.Size = new System.Drawing.Size(126, 29);
            this.screenMarginLabel.TabIndex = 10;
            this.screenMarginLabel.Text = "Padding";
            this.screenMarginLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // screenMarginValue
            // 
            this.screenMarginValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenMarginValue.DropDownHeight = 200;
            this.screenMarginValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.screenMarginValue.FormattingEnabled = true;
            this.screenMarginValue.IntegralHeight = false;
            this.screenMarginValue.Location = new System.Drawing.Point(132, 2);
            this.screenMarginValue.Margin = new System.Windows.Forms.Padding(2);
            this.screenMarginValue.Name = "screenMarginValue";
            this.screenMarginValue.Size = new System.Drawing.Size(169, 24);
            this.screenMarginValue.TabIndex = 9;
            this.screenMarginValue.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // rowsGapHeader
            // 
            this.rowsGapHeader.AutoSize = true;
            this.rowsGapHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rowsGapHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rowsGapHeader.Location = new System.Drawing.Point(2, 31);
            this.rowsGapHeader.Margin = new System.Windows.Forms.Padding(2);
            this.rowsGapHeader.Name = "rowsGapHeader";
            this.rowsGapHeader.Size = new System.Drawing.Size(126, 25);
            this.rowsGapHeader.TabIndex = 21;
            this.rowsGapHeader.Text = "Row spacing";
            this.rowsGapHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rowsSpacing
            // 
            this.rowsSpacing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rowsSpacing.DropDownHeight = 200;
            this.rowsSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rowsSpacing.FormattingEnabled = true;
            this.rowsSpacing.IntegralHeight = false;
            this.rowsSpacing.Location = new System.Drawing.Point(132, 31);
            this.rowsSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.rowsSpacing.Name = "rowsSpacing";
            this.rowsSpacing.Size = new System.Drawing.Size(169, 24);
            this.rowsSpacing.TabIndex = 20;
            this.rowsSpacing.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
            // 
            // columnsGapHeader
            // 
            this.columnsGapHeader.AutoSize = true;
            this.columnsGapHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsGapHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.columnsGapHeader.Location = new System.Drawing.Point(305, 31);
            this.columnsGapHeader.Margin = new System.Windows.Forms.Padding(2);
            this.columnsGapHeader.Name = "columnsGapHeader";
            this.columnsGapHeader.Size = new System.Drawing.Size(126, 25);
            this.columnsGapHeader.TabIndex = 22;
            this.columnsGapHeader.Text = "Column spacing";
            this.columnsGapHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // columnsSpacing
            // 
            this.columnsSpacing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsSpacing.DropDownHeight = 200;
            this.columnsSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnsSpacing.FormattingEnabled = true;
            this.columnsSpacing.IntegralHeight = false;
            this.columnsSpacing.Location = new System.Drawing.Point(435, 31);
            this.columnsSpacing.Margin = new System.Windows.Forms.Padding(2);
            this.columnsSpacing.Name = "columnsSpacing";
            this.columnsSpacing.Size = new System.Drawing.Size(169, 24);
            this.columnsSpacing.TabIndex = 23;
            this.columnsSpacing.SelectedIndexChanged += new System.EventHandler(this.VisualSettingChanged);
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
            this.hotkeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.hotkeyLabel.Location = new System.Drawing.Point(16, 14);
            this.hotkeyLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.hotkeyLabel.Name = "hotkeyLabel";
            this.hotkeyLabel.Size = new System.Drawing.Size(319, 16);
            this.hotkeyLabel.TabIndex = 0;
            this.hotkeyLabel.Text = "Global hotkey (press Ctrl, Shift, or Alt + a key)";
            // 
            // hotkeyEnabled
            // 
            this.hotkeyEnabled.AutoSize = true;
            this.hotkeyEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hotkeyEnabled.Location = new System.Drawing.Point(18, 52);
            this.hotkeyEnabled.Margin = new System.Windows.Forms.Padding(2);
            this.hotkeyEnabled.Name = "hotkeyEnabled";
            this.hotkeyEnabled.Size = new System.Drawing.Size(120, 17);
            this.hotkeyEnabled.TabIndex = 0;
            this.hotkeyEnabled.Text = "Enable OSD hotkey";
            this.hotkeyEnabled.UseVisualStyleBackColor = true;
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
            this.saveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.saveBtn.ForeColor = System.Drawing.Color.White;
            this.saveBtn.Location = new System.Drawing.Point(506, 10);
            this.saveBtn.Margin = new System.Windows.Forms.Padding(0);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(132, 30);
            this.saveBtn.TabIndex = 2;
            this.saveBtn.Text = "SAVE";
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
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.exitBtn.ForeColor = System.Drawing.Color.White;
            this.exitBtn.Location = new System.Drawing.Point(618, 0);
            this.exitBtn.Margin = new System.Windows.Forms.Padding(0);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(50, 55);
            this.exitBtn.TabIndex = 3;
            this.exitBtn.TabStop = false;
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
            this.navigationPanel.Controls.Add(this.navigationTitle);
            this.navigationPanel.Controls.Add(this.accentLine);
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
            this.layoutNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.layoutNavButton.Location = new System.Drawing.Point(0, 145);
            this.layoutNavButton.Name = "layoutNavButton";
            this.layoutNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.layoutNavButton.Size = new System.Drawing.Size(130, 45);
            this.layoutNavButton.TabIndex = 3;
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
            this.appearanceNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.appearanceNavButton.Location = new System.Drawing.Point(0, 100);
            this.appearanceNavButton.Name = "appearanceNavButton";
            this.appearanceNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.appearanceNavButton.Size = new System.Drawing.Size(130, 45);
            this.appearanceNavButton.TabIndex = 2;
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
            this.metricsNavButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.metricsNavButton.Location = new System.Drawing.Point(0, 55);
            this.metricsNavButton.Name = "metricsNavButton";
            this.metricsNavButton.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.metricsNavButton.Size = new System.Drawing.Size(130, 45);
            this.metricsNavButton.TabIndex = 1;
            this.metricsNavButton.Text = "Metrics";
            this.metricsNavButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metricsNavButton.UseVisualStyleBackColor = false;
            this.metricsNavButton.Click += new System.EventHandler(this.MetricsNavButton_Click);
            // 
            // navigationTitle
            // 
            this.navigationTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.navigationTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.navigationTitle.ForeColor = System.Drawing.Color.DarkGray;
            this.navigationTitle.Location = new System.Drawing.Point(0, 0);
            this.navigationTitle.Name = "navigationTitle";
            this.navigationTitle.Padding = new System.Windows.Forms.Padding(15, 18, 0, 0);
            this.navigationTitle.Size = new System.Drawing.Size(130, 55);
            this.navigationTitle.TabIndex = 0;
            this.navigationTitle.Text = "OSD Settings";
            this.navigationTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseDown);
            this.navigationTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseMove);
            this.navigationTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragSurface_MouseUp);
            // 
            // accentLine
            // 
            this.accentLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.accentLine.Location = new System.Drawing.Point(0, 60);
            this.accentLine.Name = "accentLine";
            this.accentLine.Size = new System.Drawing.Size(3, 45);
            this.accentLine.TabIndex = 4;
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
            this.outerBorder.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.pageTabs.ResumeLayout(false);
            this.metricsPage.ResumeLayout(false);
            this.metricsPageLayout.ResumeLayout(false);
            this.metricsCard.ResumeLayout(false);
            this.metricsCard.PerformLayout();
            this.labelsCard.ResumeLayout(false);
            this.labelsCard.PerformLayout();
            this.customLabelsLayout.ResumeLayout(false);
            this.customLabelsLayout.PerformLayout();
            this.appearancePage.ResumeLayout(false);
            this.appearanceCard.ResumeLayout(false);
            this.appearanceCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opacityValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundOpacityValue)).EndInit();
            this.fontColorsLayout.ResumeLayout(false);
            this.layoutPage.ResumeLayout(false);
            this.layoutPageLayout.ResumeLayout(false);
            this.layoutCard.ResumeLayout(false);
            this.layoutCard.PerformLayout();
            this.spacingCard.ResumeLayout(false);
            this.spacingCard.PerformLayout();
            this.spacingLayout.ResumeLayout(false);
            this.spacingLayout.PerformLayout();
            this.hotkeyCard.ResumeLayout(false);
            this.hotkeyCard.PerformLayout();
            this.bottomBar.ResumeLayout(false);
            this.titleBar.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
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
        private System.Windows.Forms.Panel labelsCard;
        private System.Windows.Forms.Panel appearanceCard;
        private System.Windows.Forms.TableLayoutPanel fontColorsLayout;
        private System.Windows.Forms.Panel layoutCard;
        private System.Windows.Forms.Panel spacingCard;
        private System.Windows.Forms.Label spacingTitle;
        private System.Windows.Forms.TableLayoutPanel spacingLayout;
        private System.Windows.Forms.Panel hotkeyCard;
        private System.Windows.Forms.Label labelsTitle;
        private System.Windows.Forms.Label displayOrderLabel;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Label positionLabel;
        private System.Windows.Forms.ComboBox positionValue;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.ComboBox fontSizeValue;
        private System.Windows.Forms.Label screenMarginLabel;
        private System.Windows.Forms.ComboBox screenMarginValue;
        private System.Windows.Forms.Label fontFamilyLabel;
        private System.Windows.Forms.ComboBox fontFamilyValue;
        private System.Windows.Forms.Label opacityLabel;
        private System.Windows.Forms.TrackBar opacityValue;
        private System.Windows.Forms.Label opacityValueLabel;
        private System.Windows.Forms.Label backgroundColorLabel;
        private System.Windows.Forms.Button backgroundColor;
        private System.Windows.Forms.TrackBar backgroundOpacityValue;
        private System.Windows.Forms.Label backgroundOpacityValueLabel;
        private System.Windows.Forms.CheckBox showCpuUsage;
        private System.Windows.Forms.CheckBox showGpuUsage;
        private System.Windows.Forms.CheckBox showRamUsage;
        private System.Windows.Forms.CheckBox showVramUsage;
        private System.Windows.Forms.CheckBox showFps;
        private System.Windows.Forms.Label fpsRefreshIntervalLabel;
        private System.Windows.Forms.ComboBox fpsRefreshIntervalValue;
        private System.Windows.Forms.Label fpsLabelValueSpacingLabel;
        private System.Windows.Forms.ComboBox fpsLabelValueSpacing;
        private System.Windows.Forms.CheckBox combineTemperatureAndUsage;
        private System.Windows.Forms.CheckBox customLabelsEnabled;
        private System.Windows.Forms.ComboBox labelValueSpacing;
        private System.Windows.Forms.TableLayoutPanel customLabelsLayout;
        private System.Windows.Forms.Label customCpuLabelCaption;
        private System.Windows.Forms.Label customGpuLabelCaption;
        private System.Windows.Forms.TextBox customCpuLabel;
        private System.Windows.Forms.TextBox customGpuLabel;
        private System.Windows.Forms.Label customCpuUsageLabelCaption;
        private System.Windows.Forms.Label customGpuUsageLabelCaption;
        private System.Windows.Forms.TextBox customCpuUsageLabel;
        private System.Windows.Forms.TextBox customGpuUsageLabel;
        private System.Windows.Forms.Label customRamLabelCaption;
        private System.Windows.Forms.Label customVramLabelCaption;
        private System.Windows.Forms.TextBox customRamLabel;
        private System.Windows.Forms.TextBox customVramLabel;
        private System.Windows.Forms.Label customFpsLabelCaption;
        private System.Windows.Forms.TextBox customFpsLabel;
        private System.Windows.Forms.Label columnsLabel;
        private System.Windows.Forms.ComboBox columnsValue;
        private System.Windows.Forms.Label orderLabel;
        private System.Windows.Forms.ListBox itemOrder;
        private System.Windows.Forms.Button orderUp;
        private System.Windows.Forms.Button orderDown;
        private System.Windows.Forms.Label hotkeyLabel;
        private System.Windows.Forms.CheckBox hotkeyEnabled;
        private System.Windows.Forms.TextBox hotkeyValue;
        private System.Windows.Forms.Label displayedHardwareLabel;
        private System.Windows.Forms.CheckBox showCpu;
        private System.Windows.Forms.CheckBox showGpu;
        private System.Windows.Forms.Label fontColorsLabel;
        private System.Windows.Forms.Label cpuFontColorLabel;
        private System.Windows.Forms.Label gpuFontColorLabel;
        private System.Windows.Forms.Label ramFontColorLabel;
        private System.Windows.Forms.Label vramFontColorLabel;
        private System.Windows.Forms.Label fpsFontColorLabel;
        private System.Windows.Forms.Button cpuFontColor;
        private System.Windows.Forms.Button gpuFontColor;
        private System.Windows.Forms.Button ramFontColor;
        private System.Windows.Forms.Button vramFontColor;
        private System.Windows.Forms.Button fpsFontColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ToolTip settingsToolTip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label spacingHeader;
        private System.Windows.Forms.ComboBox rowsSpacing;
        private System.Windows.Forms.ComboBox columnsSpacing;
        private System.Windows.Forms.Label columnsGapHeader;
        private System.Windows.Forms.Label rowsGapHeader;
        private System.Windows.Forms.Label navigationTitle;
    }
}
