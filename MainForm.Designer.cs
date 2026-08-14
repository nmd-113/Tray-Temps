namespace TrayTemps
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ExecuteShutdownSequence();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cpuTempSensorSelect = new System.Windows.Forms.ComboBox();
            this.cpuIndexSelect = new System.Windows.Forms.ComboBox();
            this.gpuTempSensorSelect = new System.Windows.Forms.ComboBox();
            this.gpuIndexSelect = new System.Windows.Forms.ComboBox();
            this.mainMenu = new System.Windows.Forms.TableLayoutPanel();
            this.aboutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.aboutBtn = new System.Windows.Forms.Button();
            this.sidepanelAbout = new System.Windows.Forms.Panel();
            this.settingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.sidepanelSettings = new System.Windows.Forms.Panel();
            this.AppDataPnl = new System.Windows.Forms.TableLayoutPanel();
            this.appLogo = new System.Windows.Forms.PictureBox();
            this.appTitle = new System.Windows.Forms.Label();
            this.homePanel = new System.Windows.Forms.TableLayoutPanel();
            this.homeBtn = new System.Windows.Forms.Button();
            this.sidepanelHome = new System.Windows.Forms.Panel();
            this.exitBtn = new System.Windows.Forms.Button();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.homePage = new System.Windows.Forms.TabPage();
            this.mainComponentsTitle = new System.Windows.Forms.Label();
            this.mainComponentsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuIcon = new System.Windows.Forms.PictureBox();
            this.gpuIcon = new System.Windows.Forms.PictureBox();
            this.ramIcon = new System.Windows.Forms.PictureBox();
            this.ssdIcon = new System.Windows.Forms.PictureBox();
            this.mboIcon = new System.Windows.Forms.PictureBox();
            this.componentType = new System.Windows.Forms.Label();
            this.indexLabel = new System.Windows.Forms.Label();
            this.componentModel = new System.Windows.Forms.Label();
            this.compCpuLabel = new System.Windows.Forms.Label();
            this.cpuConfigButton = new System.Windows.Forms.Button();
            this.cpuModel = new System.Windows.Forms.Label();
            this.compGpuLabel = new System.Windows.Forms.Label();
            this.gpuModel = new System.Windows.Forms.Label();
            this.compRamLabel = new System.Windows.Forms.Label();
            this.ramDetails = new System.Windows.Forms.Label();
            this.CompStorageLabel = new System.Windows.Forms.Label();
            this.storageDetails = new System.Windows.Forms.Label();
            this.CompMotherboardLabel = new System.Windows.Forms.Label();
            this.motherboardDetails = new System.Windows.Forms.Label();
            this.gpuConfigButton = new System.Windows.Forms.Button();
            this.tempsWrapper = new System.Windows.Forms.TableLayoutPanel();
            this.gpuPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gpuBrandPic = new System.Windows.Forms.PictureBox();
            this.gpuTempLabel = new System.Windows.Forms.Label();
            this.gpuName = new System.Windows.Forms.Label();
            this.gpuTempCurLabel = new System.Windows.Forms.Label();
            this.gpuTempMinLabel = new System.Windows.Forms.Label();
            this.gpuTempMaxLabel = new System.Windows.Forms.Label();
            this.gpuTempCur = new System.Windows.Forms.Label();
            this.gpuTempMin = new System.Windows.Forms.Label();
            this.gpuTempMax = new System.Windows.Forms.Label();
            this.cpuPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuBrandPic = new System.Windows.Forms.PictureBox();
            this.cpuTempLabel = new System.Windows.Forms.Label();
            this.cpuName = new System.Windows.Forms.Label();
            this.cpuTempCurLabel = new System.Windows.Forms.Label();
            this.cpuTempMinLabel = new System.Windows.Forms.Label();
            this.cpuTempMaxLabel = new System.Windows.Forms.Label();
            this.cpuTempCur = new System.Windows.Forms.Label();
            this.cpuTempMin = new System.Windows.Forms.Label();
            this.cpuTempMax = new System.Windows.Forms.Label();
            this.sysmonTitle = new System.Windows.Forms.Label();
            this.divider2 = new System.Windows.Forms.Panel();
            this.tempTitle = new System.Windows.Forms.Label();
            this.settingsPage = new System.Windows.Forms.TabPage();
            this.settingsTitle = new System.Windows.Forms.Label();
            this.genSettings = new System.Windows.Forms.Label();
            this.generalSettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.minimizeOnStart = new System.Windows.Forms.CheckBox();
            this.clearSettings = new System.Windows.Forms.Button();
            this.osdPanel = new System.Windows.Forms.TableLayoutPanel();
            this.osdEnable = new System.Windows.Forms.CheckBox();
            this.osdSettings = new System.Windows.Forms.Button();
            this.refreshPanel = new System.Windows.Forms.TableLayoutPanel();
            this.refreshLabel = new System.Windows.Forms.Label();
            this.refreshValue = new System.Windows.Forms.ComboBox();
            this.autostartInstall = new System.Windows.Forms.CheckBox();
            this.tempsFahrenheit = new System.Windows.Forms.CheckBox();
            this.lightModeSwitch = new System.Windows.Forms.CheckBox();
            this.traySettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.colortempsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.colortempsEnable = new System.Windows.Forms.CheckBox();
            this.colortempsConfig = new System.Windows.Forms.Button();
            this.fontFamilyPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fontFamilyLabel = new System.Windows.Forms.Label();
            this.fontFamilyValue = new System.Windows.Forms.ComboBox();
            this.singleIconTray = new System.Windows.Forms.CheckBox();
            this.enableCpuTray = new System.Windows.Forms.CheckBox();
            this.enableGpuTray = new System.Windows.Forms.CheckBox();
            this.cpuColorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuColorValue = new System.Windows.Forms.Button();
            this.cpuColorLabel = new System.Windows.Forms.Label();
            this.gpuColorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gpuColorValue = new System.Windows.Forms.Button();
            this.gpuColorLabel = new System.Windows.Forms.Label();
            this.iconsizePanel = new System.Windows.Forms.TableLayoutPanel();
            this.iconsizeLabel = new System.Windows.Forms.Label();
            this.iconsizeValue = new System.Windows.Forms.ComboBox();
            this.divider3 = new System.Windows.Forms.Panel();
            this.traySettingsLabel = new System.Windows.Forms.Label();
            this.aboutPage = new System.Windows.Forms.TabPage();
            this.checkUpdates = new System.Windows.Forms.Button();
            this.aboutTitle = new System.Windows.Forms.Label();
            this.divider1 = new System.Windows.Forms.Panel();
            this.appTitleAbout = new System.Windows.Forms.Label();
            this.appAboutExtra = new System.Windows.Forms.Label();
            this.appVersion = new System.Windows.Forms.Label();
            this.githubLink = new System.Windows.Forms.Label();
            this.donatePic = new System.Windows.Forms.PictureBox();
            this.panelWrapper = new System.Windows.Forms.Panel();
            this.resizeGrip = new TrayTemps.MainForm.ResizeGripPanel();
            this.cpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowForm = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuSeparatorTop = new System.Windows.Forms.ToolStripSeparator();
            this.trayDisplayMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.trayCpuEnabledMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.trayGpuEnabledMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.trayCombinedMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuSeparatorDisplay = new System.Windows.Forms.ToolStripSeparator();
            this.trayFahrenheitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.trayTemperatureColorsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.trayConfigureColorsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openSettingsTray = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuSeparatorBottom = new System.Windows.Forms.ToolStripSeparator();
            this.SettingsTray = new System.Windows.Forms.ToolStripMenuItem();
            this.gpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.mainMenu.SuspendLayout();
            this.aboutPanel.SuspendLayout();
            this.settingsPanel.SuspendLayout();
            this.AppDataPnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).BeginInit();
            this.homePanel.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.homePage.SuspendLayout();
            this.mainComponentsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpuIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ramIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ssdIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mboIcon)).BeginInit();
            this.tempsWrapper.SuspendLayout();
            this.gpuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpuBrandPic)).BeginInit();
            this.cpuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuBrandPic)).BeginInit();
            this.settingsPage.SuspendLayout();
            this.generalSettingsPanel.SuspendLayout();
            this.osdPanel.SuspendLayout();
            this.refreshPanel.SuspendLayout();
            this.traySettingsPanel.SuspendLayout();
            this.colortempsPanel.SuspendLayout();
            this.fontFamilyPanel.SuspendLayout();
            this.cpuColorPanel.SuspendLayout();
            this.gpuColorPanel.SuspendLayout();
            this.iconsizePanel.SuspendLayout();
            this.aboutPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.donatePic)).BeginInit();
            this.panelWrapper.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cpuTempSensorSelect
            // 
            this.cpuTempSensorSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cpuTempSensorSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempSensorSelect.DropDownHeight = 110;
            this.cpuTempSensorSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cpuTempSensorSelect.DropDownWidth = 110;
            this.cpuTempSensorSelect.Enabled = false;
            this.cpuTempSensorSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuTempSensorSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempSensorSelect.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempSensorSelect.FormattingEnabled = true;
            this.cpuTempSensorSelect.IntegralHeight = false;
            this.cpuTempSensorSelect.ItemHeight = 13;
            this.cpuTempSensorSelect.Location = new System.Drawing.Point(445, 44);
            this.cpuTempSensorSelect.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cpuTempSensorSelect.Name = "cpuTempSensorSelect";
            this.cpuTempSensorSelect.Size = new System.Drawing.Size(110, 21);
            this.cpuTempSensorSelect.TabIndex = 35;
            this.cpuTempSensorSelect.SelectedIndexChanged += new System.EventHandler(this.CpuTempSensorSelect_SelectedIndexChanged);
            // 
            // cpuIndexSelect
            // 
            this.cpuIndexSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cpuIndexSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuIndexSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cpuIndexSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuIndexSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuIndexSelect.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuIndexSelect.FormattingEnabled = true;
            this.cpuIndexSelect.IntegralHeight = false;
            this.cpuIndexSelect.ItemHeight = 13;
            this.cpuIndexSelect.Items.AddRange(new object[] {
            "0"});
            this.cpuIndexSelect.Location = new System.Drawing.Point(565, 44);
            this.cpuIndexSelect.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cpuIndexSelect.Name = "cpuIndexSelect";
            this.cpuIndexSelect.Size = new System.Drawing.Size(50, 21);
            this.cpuIndexSelect.TabIndex = 23;
            this.cpuIndexSelect.SelectedIndexChanged += new System.EventHandler(this.CpuIndexSelect_SelectedIndexChanged);
            // 
            // gpuTempSensorSelect
            // 
            this.gpuTempSensorSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gpuTempSensorSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempSensorSelect.DropDownHeight = 110;
            this.gpuTempSensorSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gpuTempSensorSelect.DropDownWidth = 110;
            this.gpuTempSensorSelect.Enabled = false;
            this.gpuTempSensorSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuTempSensorSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempSensorSelect.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempSensorSelect.FormattingEnabled = true;
            this.gpuTempSensorSelect.IntegralHeight = false;
            this.gpuTempSensorSelect.ItemHeight = 13;
            this.gpuTempSensorSelect.Location = new System.Drawing.Point(445, 72);
            this.gpuTempSensorSelect.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.gpuTempSensorSelect.Name = "gpuTempSensorSelect";
            this.gpuTempSensorSelect.Size = new System.Drawing.Size(110, 21);
            this.gpuTempSensorSelect.TabIndex = 36;
            this.gpuTempSensorSelect.SelectedIndexChanged += new System.EventHandler(this.GpuTempSensorSelect_SelectedIndexChanged);
            // 
            // gpuIndexSelect
            // 
            this.gpuIndexSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gpuIndexSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuIndexSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gpuIndexSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuIndexSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuIndexSelect.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuIndexSelect.FormattingEnabled = true;
            this.gpuIndexSelect.IntegralHeight = false;
            this.gpuIndexSelect.ItemHeight = 13;
            this.gpuIndexSelect.Items.AddRange(new object[] {
            "0"});
            this.gpuIndexSelect.Location = new System.Drawing.Point(565, 72);
            this.gpuIndexSelect.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.gpuIndexSelect.Name = "gpuIndexSelect";
            this.gpuIndexSelect.Size = new System.Drawing.Size(50, 21);
            this.gpuIndexSelect.TabIndex = 24;
            this.gpuIndexSelect.SelectedIndexChanged += new System.EventHandler(this.GpuIndexSelect_SelectedIndexChanged);
            // 
            // mainMenu
            // 
            this.mainMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mainMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.mainMenu.ColumnCount = 1;
            this.mainMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainMenu.Controls.Add(this.aboutPanel, 0, 6);
            this.mainMenu.Controls.Add(this.settingsPanel, 0, 4);
            this.mainMenu.Controls.Add(this.AppDataPnl, 0, 1);
            this.mainMenu.Controls.Add(this.homePanel, 0, 3);
            this.mainMenu.Location = new System.Drawing.Point(1, 1);
            this.mainMenu.Margin = new System.Windows.Forms.Padding(4);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.RowCount = 8;
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.mainMenu.Size = new System.Drawing.Size(110, 598);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            // 
            // aboutPanel
            // 
            this.aboutPanel.ColumnCount = 2;
            this.aboutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
            this.aboutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aboutPanel.Controls.Add(this.aboutBtn, 1, 0);
            this.aboutPanel.Controls.Add(this.sidepanelAbout, 0, 0);
            this.aboutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutPanel.Location = new System.Drawing.Point(5, 503);
            this.aboutPanel.Margin = new System.Windows.Forms.Padding(5);
            this.aboutPanel.Name = "aboutPanel";
            this.aboutPanel.RowCount = 1;
            this.aboutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aboutPanel.Size = new System.Drawing.Size(100, 65);
            this.aboutPanel.TabIndex = 5;
            // 
            // aboutBtn
            // 
            this.aboutBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.aboutBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.aboutBtn.FlatAppearance.BorderSize = 0;
            this.aboutBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.aboutBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.aboutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.aboutBtn.Location = new System.Drawing.Point(3, 0);
            this.aboutBtn.Margin = new System.Windows.Forms.Padding(0);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(97, 65);
            this.aboutBtn.TabIndex = 3;
            this.aboutBtn.Text = "❓\r\nAbout";
            this.aboutBtn.UseVisualStyleBackColor = false;
            this.aboutBtn.Click += new System.EventHandler(this.AboutBtn_Click);
            // 
            // sidepanelAbout
            // 
            this.sidepanelAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.sidepanelAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidepanelAbout.Location = new System.Drawing.Point(0, 0);
            this.sidepanelAbout.Margin = new System.Windows.Forms.Padding(0);
            this.sidepanelAbout.Name = "sidepanelAbout";
            this.sidepanelAbout.Size = new System.Drawing.Size(3, 65);
            this.sidepanelAbout.TabIndex = 0;
            // 
            // settingsPanel
            // 
            this.settingsPanel.ColumnCount = 2;
            this.settingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
            this.settingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.settingsPanel.Controls.Add(this.settingsBtn, 1, 0);
            this.settingsPanel.Controls.Add(this.sidepanelSettings, 0, 0);
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPanel.Location = new System.Drawing.Point(5, 304);
            this.settingsPanel.Margin = new System.Windows.Forms.Padding(5);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.RowCount = 1;
            this.settingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.settingsPanel.Size = new System.Drawing.Size(100, 65);
            this.settingsPanel.TabIndex = 4;
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.settingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.settingsBtn.FlatAppearance.BorderSize = 0;
            this.settingsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.settingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.settingsBtn.Location = new System.Drawing.Point(3, 0);
            this.settingsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(97, 65);
            this.settingsBtn.TabIndex = 2;
            this.settingsBtn.Text = "⚙\r\nSettings";
            this.settingsBtn.UseVisualStyleBackColor = false;
            this.settingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            // 
            // sidepanelSettings
            // 
            this.sidepanelSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.sidepanelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidepanelSettings.Location = new System.Drawing.Point(0, 0);
            this.sidepanelSettings.Margin = new System.Windows.Forms.Padding(0);
            this.sidepanelSettings.Name = "sidepanelSettings";
            this.sidepanelSettings.Size = new System.Drawing.Size(3, 65);
            this.sidepanelSettings.TabIndex = 0;
            // 
            // AppDataPnl
            // 
            this.AppDataPnl.ColumnCount = 1;
            this.AppDataPnl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AppDataPnl.Controls.Add(this.appLogo, 0, 0);
            this.AppDataPnl.Controls.Add(this.appTitle, 0, 1);
            this.AppDataPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppDataPnl.Location = new System.Drawing.Point(3, 28);
            this.AppDataPnl.Name = "AppDataPnl";
            this.AppDataPnl.RowCount = 2;
            this.AppDataPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.AppDataPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.AppDataPnl.Size = new System.Drawing.Size(104, 69);
            this.AppDataPnl.TabIndex = 1;
            // 
            // appLogo
            // 
            this.appLogo.BackgroundImage = global::TrayTemps.Properties.Resources.traytemps;
            this.appLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.appLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appLogo.Location = new System.Drawing.Point(3, 3);
            this.appLogo.Name = "appLogo";
            this.appLogo.Size = new System.Drawing.Size(98, 42);
            this.appLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.appLogo.TabIndex = 0;
            this.appLogo.TabStop = false;
            // 
            // appTitle
            // 
            this.appTitle.AutoSize = true;
            this.appTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.appTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.appTitle.Location = new System.Drawing.Point(3, 48);
            this.appTitle.Name = "appTitle";
            this.appTitle.Size = new System.Drawing.Size(98, 16);
            this.appTitle.TabIndex = 4;
            this.appTitle.Text = "TrayTemps";
            this.appTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // homePanel
            // 
            this.homePanel.ColumnCount = 2;
            this.homePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
            this.homePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.homePanel.Controls.Add(this.homeBtn, 1, 0);
            this.homePanel.Controls.Add(this.sidepanelHome, 0, 0);
            this.homePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homePanel.Location = new System.Drawing.Point(5, 229);
            this.homePanel.Margin = new System.Windows.Forms.Padding(5);
            this.homePanel.Name = "homePanel";
            this.homePanel.RowCount = 1;
            this.homePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.homePanel.Size = new System.Drawing.Size(100, 65);
            this.homePanel.TabIndex = 3;
            this.homePanel.Click += new System.EventHandler(this.HomeBtn_Click);
            // 
            // homeBtn
            // 
            this.homeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.homeBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.homeBtn.FlatAppearance.BorderSize = 0;
            this.homeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.homeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.homeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.homeBtn.Location = new System.Drawing.Point(3, 0);
            this.homeBtn.Margin = new System.Windows.Forms.Padding(0);
            this.homeBtn.Name = "homeBtn";
            this.homeBtn.Size = new System.Drawing.Size(97, 65);
            this.homeBtn.TabIndex = 1;
            this.homeBtn.Text = "🖥️\r\nMain";
            this.homeBtn.UseVisualStyleBackColor = false;
            this.homeBtn.Click += new System.EventHandler(this.HomeBtn_Click);
            // 
            // sidepanelHome
            // 
            this.sidepanelHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.sidepanelHome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidepanelHome.Location = new System.Drawing.Point(0, 0);
            this.sidepanelHome.Margin = new System.Windows.Forms.Padding(0);
            this.sidepanelHome.Name = "sidepanelHome";
            this.sidepanelHome.Size = new System.Drawing.Size(3, 65);
            this.sidepanelHome.TabIndex = 0;
            // 
            // exitBtn
            // 
            this.exitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.exitBtn.Location = new System.Drawing.Point(744, 1);
            this.exitBtn.Margin = new System.Windows.Forms.Padding(4);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(55, 45);
            this.exitBtn.TabIndex = 1;
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.minimizeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.minimizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.minimizeBtn.Location = new System.Drawing.Point(689, 1);
            this.minimizeBtn.Margin = new System.Windows.Forms.Padding(4);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(55, 45);
            this.minimizeBtn.TabIndex = 2;
            this.minimizeBtn.Text = "─";
            this.minimizeBtn.UseVisualStyleBackColor = true;
            this.minimizeBtn.Click += new System.EventHandler(this.MinimizeBtn_Click);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.mainTabControl.Controls.Add(this.homePage);
            this.mainTabControl.Controls.Add(this.settingsPage);
            this.mainTabControl.Controls.Add(this.aboutPage);
            this.mainTabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.mainTabControl.Location = new System.Drawing.Point(-5, -5);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(698, 563);
            this.mainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainTabControl.TabIndex = 0;
            // 
            // homePage
            // 
            this.homePage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.homePage.Controls.Add(this.mainComponentsTitle);
            this.homePage.Controls.Add(this.mainComponentsPanel);
            this.homePage.Controls.Add(this.tempsWrapper);
            this.homePage.Controls.Add(this.sysmonTitle);
            this.homePage.Controls.Add(this.divider2);
            this.homePage.Controls.Add(this.tempTitle);
            this.homePage.ForeColor = System.Drawing.Color.White;
            this.homePage.Location = new System.Drawing.Point(4, 5);
            this.homePage.Name = "homePage";
            this.homePage.Size = new System.Drawing.Size(690, 554);
            this.homePage.TabIndex = 0;
            this.homePage.Text = "Home";
            // 
            // mainComponentsTitle
            // 
            this.mainComponentsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainComponentsTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.mainComponentsTitle.Location = new System.Drawing.Point(33, 286);
            this.mainComponentsTitle.Name = "mainComponentsTitle";
            this.mainComponentsTitle.Size = new System.Drawing.Size(311, 30);
            this.mainComponentsTitle.TabIndex = 14;
            this.mainComponentsTitle.Text = "🧩 Components";
            this.mainComponentsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainComponentsPanel
            // 
            this.mainComponentsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainComponentsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.mainComponentsPanel.ColumnCount = 4;
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.mainComponentsPanel.Controls.Add(this.cpuIcon, 0, 1);
            this.mainComponentsPanel.Controls.Add(this.gpuIcon, 0, 2);
            this.mainComponentsPanel.Controls.Add(this.ramIcon, 0, 3);
            this.mainComponentsPanel.Controls.Add(this.ssdIcon, 0, 4);
            this.mainComponentsPanel.Controls.Add(this.mboIcon, 0, 5);
            this.mainComponentsPanel.Controls.Add(this.componentType, 0, 0);
            this.mainComponentsPanel.Controls.Add(this.indexLabel, 3, 0);
            this.mainComponentsPanel.Controls.Add(this.componentModel, 2, 0);
            this.mainComponentsPanel.Controls.Add(this.compCpuLabel, 1, 1);
            this.mainComponentsPanel.Controls.Add(this.cpuConfigButton, 3, 1);
            this.mainComponentsPanel.Controls.Add(this.cpuModel, 2, 1);
            this.mainComponentsPanel.Controls.Add(this.compGpuLabel, 1, 2);
            this.mainComponentsPanel.Controls.Add(this.gpuModel, 2, 2);
            this.mainComponentsPanel.Controls.Add(this.compRamLabel, 1, 3);
            this.mainComponentsPanel.Controls.Add(this.ramDetails, 2, 3);
            this.mainComponentsPanel.Controls.Add(this.CompStorageLabel, 1, 4);
            this.mainComponentsPanel.Controls.Add(this.storageDetails, 2, 4);
            this.mainComponentsPanel.Controls.Add(this.CompMotherboardLabel, 1, 5);
            this.mainComponentsPanel.Controls.Add(this.motherboardDetails, 2, 5);
            this.mainComponentsPanel.Controls.Add(this.gpuConfigButton, 3, 2);
            this.mainComponentsPanel.Location = new System.Drawing.Point(33, 323);
            this.mainComponentsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainComponentsPanel.Name = "mainComponentsPanel";
            this.mainComponentsPanel.Padding = new System.Windows.Forms.Padding(5);
            this.mainComponentsPanel.RowCount = 6;
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.00008F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.00007F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.00007F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.00007F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.00007F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.99966F));
            this.mainComponentsPanel.Size = new System.Drawing.Size(625, 199);
            this.mainComponentsPanel.TabIndex = 13;
            // 
            // cpuIcon
            // 
            this.cpuIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuIcon.Image = global::TrayTemps.Properties.Resources.cpu;
            this.cpuIcon.Location = new System.Drawing.Point(5, 42);
            this.cpuIcon.Margin = new System.Windows.Forms.Padding(0);
            this.cpuIcon.Name = "cpuIcon";
            this.cpuIcon.Size = new System.Drawing.Size(30, 30);
            this.cpuIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cpuIcon.TabIndex = 28;
            this.cpuIcon.TabStop = false;
            // 
            // gpuIcon
            // 
            this.gpuIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuIcon.Image = global::TrayTemps.Properties.Resources.gpu;
            this.gpuIcon.Location = new System.Drawing.Point(5, 72);
            this.gpuIcon.Margin = new System.Windows.Forms.Padding(0);
            this.gpuIcon.Name = "gpuIcon";
            this.gpuIcon.Size = new System.Drawing.Size(30, 30);
            this.gpuIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.gpuIcon.TabIndex = 30;
            this.gpuIcon.TabStop = false;
            // 
            // ramIcon
            // 
            this.ramIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ramIcon.Image = global::TrayTemps.Properties.Resources.ram;
            this.ramIcon.Location = new System.Drawing.Point(5, 102);
            this.ramIcon.Margin = new System.Windows.Forms.Padding(0);
            this.ramIcon.Name = "ramIcon";
            this.ramIcon.Size = new System.Drawing.Size(30, 30);
            this.ramIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ramIcon.TabIndex = 31;
            this.ramIcon.TabStop = false;
            // 
            // ssdIcon
            // 
            this.ssdIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdIcon.Image = global::TrayTemps.Properties.Resources.ssd;
            this.ssdIcon.Location = new System.Drawing.Point(5, 132);
            this.ssdIcon.Margin = new System.Windows.Forms.Padding(0);
            this.ssdIcon.Name = "ssdIcon";
            this.ssdIcon.Size = new System.Drawing.Size(30, 30);
            this.ssdIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ssdIcon.TabIndex = 32;
            this.ssdIcon.TabStop = false;
            // 
            // mboIcon
            // 
            this.mboIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mboIcon.Image = global::TrayTemps.Properties.Resources.motherboard;
            this.mboIcon.Location = new System.Drawing.Point(5, 162);
            this.mboIcon.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.mboIcon.Name = "mboIcon";
            this.mboIcon.Size = new System.Drawing.Size(30, 30);
            this.mboIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mboIcon.TabIndex = 33;
            this.mboIcon.TabStop = false;
            // 
            // componentType
            // 
            this.componentType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.mainComponentsPanel.SetColumnSpan(this.componentType, 2);
            this.componentType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentType.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.componentType.ForeColor = System.Drawing.Color.Gray;
            this.componentType.Location = new System.Drawing.Point(5, 5);
            this.componentType.Margin = new System.Windows.Forms.Padding(0);
            this.componentType.Name = "componentType";
            this.componentType.Padding = new System.Windows.Forms.Padding(3);
            this.componentType.Size = new System.Drawing.Size(90, 37);
            this.componentType.TabIndex = 2;
            this.componentType.Text = "Type";
            this.componentType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // indexLabel
            // 
            this.indexLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.indexLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.indexLabel.ForeColor = System.Drawing.Color.Gray;
            this.indexLabel.Location = new System.Drawing.Point(590, 5);
            this.indexLabel.Margin = new System.Windows.Forms.Padding(0);
            this.indexLabel.Name = "indexLabel";
            this.indexLabel.Padding = new System.Windows.Forms.Padding(3);
            this.indexLabel.Size = new System.Drawing.Size(30, 37);
            this.indexLabel.TabIndex = 22;
            this.indexLabel.Text = " ";
            this.indexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // componentModel
            // 
            this.componentModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.componentModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.componentModel.ForeColor = System.Drawing.Color.Gray;
            this.componentModel.Location = new System.Drawing.Point(95, 5);
            this.componentModel.Margin = new System.Windows.Forms.Padding(0);
            this.componentModel.Name = "componentModel";
            this.componentModel.Padding = new System.Windows.Forms.Padding(3);
            this.componentModel.Size = new System.Drawing.Size(495, 37);
            this.componentModel.TabIndex = 3;
            this.componentModel.Text = "Click the label 🠟 for details";
            this.componentModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // compCpuLabel
            // 
            this.compCpuLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.compCpuLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compCpuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.compCpuLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.compCpuLabel.Location = new System.Drawing.Point(35, 42);
            this.compCpuLabel.Margin = new System.Windows.Forms.Padding(0);
            this.compCpuLabel.Name = "compCpuLabel";
            this.compCpuLabel.Padding = new System.Windows.Forms.Padding(3);
            this.compCpuLabel.Size = new System.Drawing.Size(60, 30);
            this.compCpuLabel.TabIndex = 10;
            this.compCpuLabel.Text = "CPU:";
            this.compCpuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cpuConfigButton
            // 
            this.cpuConfigButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cpuConfigButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuConfigButton.Enabled = false;
            this.cpuConfigButton.FlatAppearance.BorderSize = 0;
            this.cpuConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuConfigButton.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuConfigButton.Location = new System.Drawing.Point(590, 42);
            this.cpuConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.cpuConfigButton.Name = "cpuConfigButton";
            this.cpuConfigButton.Size = new System.Drawing.Size(30, 30);
            this.cpuConfigButton.TabIndex = 35;
            this.cpuConfigButton.Text = "⛭";
            this.cpuConfigButton.UseVisualStyleBackColor = true;
            this.cpuConfigButton.Click += new System.EventHandler(this.CpuConfigButton_Click);
            // 
            // cpuModel
            // 
            this.cpuModel.AutoEllipsis = true;
            this.cpuModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuModel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cpuModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuModel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuModel.Location = new System.Drawing.Point(95, 42);
            this.cpuModel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuModel.Name = "cpuModel";
            this.cpuModel.Padding = new System.Windows.Forms.Padding(3);
            this.cpuModel.Size = new System.Drawing.Size(495, 30);
            this.cpuModel.TabIndex = 20;
            this.cpuModel.Text = "Loading hardware information...";
            this.cpuModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cpuModel.Click += new System.EventHandler(this.CpuModel_Click);
            // 
            // compGpuLabel
            // 
            this.compGpuLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.compGpuLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compGpuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.compGpuLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.compGpuLabel.Location = new System.Drawing.Point(35, 72);
            this.compGpuLabel.Margin = new System.Windows.Forms.Padding(0);
            this.compGpuLabel.Name = "compGpuLabel";
            this.compGpuLabel.Padding = new System.Windows.Forms.Padding(3);
            this.compGpuLabel.Size = new System.Drawing.Size(60, 30);
            this.compGpuLabel.TabIndex = 11;
            this.compGpuLabel.Text = "GPU:";
            this.compGpuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpuModel
            // 
            this.gpuModel.AutoEllipsis = true;
            this.gpuModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuModel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gpuModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuModel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuModel.Location = new System.Drawing.Point(95, 72);
            this.gpuModel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuModel.Name = "gpuModel";
            this.gpuModel.Padding = new System.Windows.Forms.Padding(3);
            this.gpuModel.Size = new System.Drawing.Size(495, 30);
            this.gpuModel.TabIndex = 21;
            this.gpuModel.Text = "Loading hardware information...";
            this.gpuModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.gpuModel.Click += new System.EventHandler(this.GpuModel_Click);
            // 
            // compRamLabel
            // 
            this.compRamLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.compRamLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compRamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.compRamLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.compRamLabel.Location = new System.Drawing.Point(35, 102);
            this.compRamLabel.Margin = new System.Windows.Forms.Padding(0);
            this.compRamLabel.Name = "compRamLabel";
            this.compRamLabel.Padding = new System.Windows.Forms.Padding(3);
            this.compRamLabel.Size = new System.Drawing.Size(60, 30);
            this.compRamLabel.TabIndex = 12;
            this.compRamLabel.Text = "RAM:";
            this.compRamLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ramDetails
            // 
            this.ramDetails.AutoEllipsis = true;
            this.ramDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ramDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ramDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ramDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramDetails.ForeColor = System.Drawing.Color.DarkGray;
            this.ramDetails.Location = new System.Drawing.Point(95, 102);
            this.ramDetails.Margin = new System.Windows.Forms.Padding(0);
            this.ramDetails.Name = "ramDetails";
            this.ramDetails.Padding = new System.Windows.Forms.Padding(3);
            this.ramDetails.Size = new System.Drawing.Size(495, 30);
            this.ramDetails.TabIndex = 17;
            this.ramDetails.Text = "Loading hardware information...";
            this.ramDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ramDetails.Click += new System.EventHandler(this.RamDetails_Click);
            // 
            // CompStorageLabel
            // 
            this.CompStorageLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CompStorageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompStorageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.CompStorageLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.CompStorageLabel.Location = new System.Drawing.Point(35, 132);
            this.CompStorageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.CompStorageLabel.Name = "CompStorageLabel";
            this.CompStorageLabel.Padding = new System.Windows.Forms.Padding(3);
            this.CompStorageLabel.Size = new System.Drawing.Size(60, 30);
            this.CompStorageLabel.TabIndex = 13;
            this.CompStorageLabel.Text = "Storage:";
            this.CompStorageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // storageDetails
            // 
            this.storageDetails.AutoEllipsis = true;
            this.storageDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.storageDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.storageDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storageDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.storageDetails.ForeColor = System.Drawing.Color.DarkGray;
            this.storageDetails.Location = new System.Drawing.Point(95, 132);
            this.storageDetails.Margin = new System.Windows.Forms.Padding(0);
            this.storageDetails.Name = "storageDetails";
            this.storageDetails.Padding = new System.Windows.Forms.Padding(3);
            this.storageDetails.Size = new System.Drawing.Size(495, 30);
            this.storageDetails.TabIndex = 18;
            this.storageDetails.Text = "Loading hardware information...";
            this.storageDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.storageDetails.Click += new System.EventHandler(this.StorageDetails_Click);
            // 
            // CompMotherboardLabel
            // 
            this.CompMotherboardLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CompMotherboardLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompMotherboardLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.CompMotherboardLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.CompMotherboardLabel.Location = new System.Drawing.Point(35, 162);
            this.CompMotherboardLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.CompMotherboardLabel.Name = "CompMotherboardLabel";
            this.CompMotherboardLabel.Padding = new System.Windows.Forms.Padding(3);
            this.CompMotherboardLabel.Size = new System.Drawing.Size(60, 30);
            this.CompMotherboardLabel.TabIndex = 14;
            this.CompMotherboardLabel.Text = "MB:";
            this.CompMotherboardLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // motherboardDetails
            // 
            this.motherboardDetails.AutoEllipsis = true;
            this.motherboardDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.motherboardDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.motherboardDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motherboardDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.motherboardDetails.ForeColor = System.Drawing.Color.DarkGray;
            this.motherboardDetails.Location = new System.Drawing.Point(95, 162);
            this.motherboardDetails.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.motherboardDetails.Name = "motherboardDetails";
            this.motherboardDetails.Padding = new System.Windows.Forms.Padding(3);
            this.motherboardDetails.Size = new System.Drawing.Size(495, 30);
            this.motherboardDetails.TabIndex = 19;
            this.motherboardDetails.Text = "Loading hardware information...";
            this.motherboardDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.motherboardDetails.Click += new System.EventHandler(this.MotherboardDetails_Click);
            // 
            // gpuConfigButton
            // 
            this.gpuConfigButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gpuConfigButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuConfigButton.Enabled = false;
            this.gpuConfigButton.FlatAppearance.BorderSize = 0;
            this.gpuConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuConfigButton.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuConfigButton.Location = new System.Drawing.Point(590, 72);
            this.gpuConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.gpuConfigButton.Name = "gpuConfigButton";
            this.gpuConfigButton.Size = new System.Drawing.Size(30, 30);
            this.gpuConfigButton.TabIndex = 36;
            this.gpuConfigButton.Text = "⛭";
            this.gpuConfigButton.UseVisualStyleBackColor = true;
            this.gpuConfigButton.Click += new System.EventHandler(this.GpuConfigButton_Click);
            // 
            // tempsWrapper
            // 
            this.tempsWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tempsWrapper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tempsWrapper.ColumnCount = 3;
            this.tempsWrapper.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tempsWrapper.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tempsWrapper.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tempsWrapper.Controls.Add(this.gpuPanel, 0, 0);
            this.tempsWrapper.Controls.Add(this.cpuPanel, 2, 0);
            this.tempsWrapper.Location = new System.Drawing.Point(30, 125);
            this.tempsWrapper.Margin = new System.Windows.Forms.Padding(0);
            this.tempsWrapper.Name = "tempsWrapper";
            this.tempsWrapper.RowCount = 1;
            this.tempsWrapper.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tempsWrapper.Size = new System.Drawing.Size(631, 150);
            this.tempsWrapper.TabIndex = 12;
            // 
            // gpuPanel
            // 
            this.gpuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuPanel.ColumnCount = 3;
            this.gpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.gpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.gpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.gpuPanel.Controls.Add(this.gpuBrandPic, 2, 0);
            this.gpuPanel.Controls.Add(this.gpuTempLabel, 0, 0);
            this.gpuPanel.Controls.Add(this.gpuName, 0, 1);
            this.gpuPanel.Controls.Add(this.gpuTempCurLabel, 0, 2);
            this.gpuPanel.Controls.Add(this.gpuTempMinLabel, 1, 2);
            this.gpuPanel.Controls.Add(this.gpuTempMaxLabel, 2, 2);
            this.gpuPanel.Controls.Add(this.gpuTempCur, 0, 3);
            this.gpuPanel.Controls.Add(this.gpuTempMin, 1, 3);
            this.gpuPanel.Controls.Add(this.gpuTempMax, 2, 3);
            this.gpuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuPanel.Location = new System.Drawing.Point(3, 3);
            this.gpuPanel.Name = "gpuPanel";
            this.gpuPanel.Padding = new System.Windows.Forms.Padding(12);
            this.gpuPanel.RowCount = 4;
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.5F));
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.gpuPanel.Size = new System.Drawing.Size(299, 144);
            this.gpuPanel.TabIndex = 10;
            // 
            // gpuBrandPic
            // 
            this.gpuBrandPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuBrandPic.Location = new System.Drawing.Point(197, 15);
            this.gpuBrandPic.Name = "gpuBrandPic";
            this.gpuBrandPic.Size = new System.Drawing.Size(87, 18);
            this.gpuBrandPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.gpuBrandPic.TabIndex = 10;
            this.gpuBrandPic.TabStop = false;
            // 
            // gpuTempLabel
            // 
            this.gpuTempLabel.AutoSize = true;
            this.gpuTempLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuPanel.SetColumnSpan(this.gpuTempLabel, 2);
            this.gpuTempLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempLabel.Location = new System.Drawing.Point(12, 12);
            this.gpuTempLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempLabel.Name = "gpuTempLabel";
            this.gpuTempLabel.Size = new System.Drawing.Size(182, 24);
            this.gpuTempLabel.TabIndex = 0;
            this.gpuTempLabel.Text = "GPU (Graphics card)";
            this.gpuTempLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpuName
            // 
            this.gpuName.AutoSize = true;
            this.gpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuPanel.SetColumnSpan(this.gpuName, 3);
            this.gpuName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuName.ForeColor = System.Drawing.Color.Silver;
            this.gpuName.Location = new System.Drawing.Point(12, 36);
            this.gpuName.Margin = new System.Windows.Forms.Padding(0);
            this.gpuName.Name = "gpuName";
            this.gpuName.Size = new System.Drawing.Size(275, 48);
            this.gpuName.TabIndex = 9;
            this.gpuName.Text = "N/A";
            this.gpuName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempCurLabel
            // 
            this.gpuTempCurLabel.AutoSize = true;
            this.gpuTempCurLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempCurLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempCurLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempCurLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempCurLabel.Location = new System.Drawing.Point(12, 84);
            this.gpuTempCurLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempCurLabel.Name = "gpuTempCurLabel";
            this.gpuTempCurLabel.Size = new System.Drawing.Size(91, 21);
            this.gpuTempCurLabel.TabIndex = 1;
            this.gpuTempCurLabel.Text = "Current:";
            this.gpuTempCurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMinLabel
            // 
            this.gpuTempMinLabel.AutoSize = true;
            this.gpuTempMinLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMinLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMinLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMinLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempMinLabel.Location = new System.Drawing.Point(103, 84);
            this.gpuTempMinLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMinLabel.Name = "gpuTempMinLabel";
            this.gpuTempMinLabel.Size = new System.Drawing.Size(91, 21);
            this.gpuTempMinLabel.TabIndex = 2;
            this.gpuTempMinLabel.Text = "Minimum:";
            this.gpuTempMinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMaxLabel
            // 
            this.gpuTempMaxLabel.AutoSize = true;
            this.gpuTempMaxLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMaxLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMaxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMaxLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempMaxLabel.Location = new System.Drawing.Point(194, 84);
            this.gpuTempMaxLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMaxLabel.Name = "gpuTempMaxLabel";
            this.gpuTempMaxLabel.Size = new System.Drawing.Size(93, 21);
            this.gpuTempMaxLabel.TabIndex = 3;
            this.gpuTempMaxLabel.Text = "Maximum:";
            this.gpuTempMaxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempCur
            // 
            this.gpuTempCur.AutoSize = true;
            this.gpuTempCur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempCur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempCur.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempCur.ForeColor = System.Drawing.Color.LightGray;
            this.gpuTempCur.Location = new System.Drawing.Point(12, 105);
            this.gpuTempCur.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempCur.Name = "gpuTempCur";
            this.gpuTempCur.Size = new System.Drawing.Size(91, 27);
            this.gpuTempCur.TabIndex = 4;
            this.gpuTempCur.Text = "N/A";
            this.gpuTempCur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMin
            // 
            this.gpuTempMin.AutoSize = true;
            this.gpuTempMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMin.ForeColor = System.Drawing.Color.LimeGreen;
            this.gpuTempMin.Location = new System.Drawing.Point(103, 105);
            this.gpuTempMin.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMin.Name = "gpuTempMin";
            this.gpuTempMin.Size = new System.Drawing.Size(91, 27);
            this.gpuTempMin.TabIndex = 5;
            this.gpuTempMin.Text = "N/A";
            this.gpuTempMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMax
            // 
            this.gpuTempMax.AutoSize = true;
            this.gpuTempMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMax.ForeColor = System.Drawing.Color.Red;
            this.gpuTempMax.Location = new System.Drawing.Point(194, 105);
            this.gpuTempMax.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMax.Name = "gpuTempMax";
            this.gpuTempMax.Size = new System.Drawing.Size(93, 27);
            this.gpuTempMax.TabIndex = 6;
            this.gpuTempMax.Text = "N/A";
            this.gpuTempMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuPanel
            // 
            this.cpuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuPanel.ColumnCount = 3;
            this.cpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.cpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.cpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.cpuPanel.Controls.Add(this.cpuBrandPic, 2, 0);
            this.cpuPanel.Controls.Add(this.cpuTempLabel, 0, 0);
            this.cpuPanel.Controls.Add(this.cpuName, 0, 1);
            this.cpuPanel.Controls.Add(this.cpuTempCurLabel, 0, 2);
            this.cpuPanel.Controls.Add(this.cpuTempMinLabel, 1, 2);
            this.cpuPanel.Controls.Add(this.cpuTempMaxLabel, 2, 2);
            this.cpuPanel.Controls.Add(this.cpuTempCur, 0, 3);
            this.cpuPanel.Controls.Add(this.cpuTempMin, 1, 3);
            this.cpuPanel.Controls.Add(this.cpuTempMax, 2, 3);
            this.cpuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuPanel.Location = new System.Drawing.Point(328, 3);
            this.cpuPanel.Name = "cpuPanel";
            this.cpuPanel.Padding = new System.Windows.Forms.Padding(12);
            this.cpuPanel.RowCount = 4;
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.5F));
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.cpuPanel.Size = new System.Drawing.Size(300, 144);
            this.cpuPanel.TabIndex = 11;
            // 
            // cpuBrandPic
            // 
            this.cpuBrandPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuBrandPic.Location = new System.Drawing.Point(198, 15);
            this.cpuBrandPic.Name = "cpuBrandPic";
            this.cpuBrandPic.Size = new System.Drawing.Size(87, 18);
            this.cpuBrandPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cpuBrandPic.TabIndex = 11;
            this.cpuBrandPic.TabStop = false;
            // 
            // cpuTempLabel
            // 
            this.cpuTempLabel.AutoSize = true;
            this.cpuTempLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuPanel.SetColumnSpan(this.cpuTempLabel, 2);
            this.cpuTempLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempLabel.Location = new System.Drawing.Point(12, 12);
            this.cpuTempLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempLabel.Name = "cpuTempLabel";
            this.cpuTempLabel.Size = new System.Drawing.Size(183, 24);
            this.cpuTempLabel.TabIndex = 1;
            this.cpuTempLabel.Text = "CPU (Processor)";
            this.cpuTempLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cpuName
            // 
            this.cpuName.AutoSize = true;
            this.cpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuPanel.SetColumnSpan(this.cpuName, 3);
            this.cpuName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuName.ForeColor = System.Drawing.Color.Silver;
            this.cpuName.Location = new System.Drawing.Point(12, 36);
            this.cpuName.Margin = new System.Windows.Forms.Padding(0);
            this.cpuName.Name = "cpuName";
            this.cpuName.Size = new System.Drawing.Size(276, 48);
            this.cpuName.TabIndex = 8;
            this.cpuName.Text = "N/A";
            this.cpuName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempCurLabel
            // 
            this.cpuTempCurLabel.AutoSize = true;
            this.cpuTempCurLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempCurLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempCurLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempCurLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempCurLabel.Location = new System.Drawing.Point(12, 84);
            this.cpuTempCurLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempCurLabel.Name = "cpuTempCurLabel";
            this.cpuTempCurLabel.Size = new System.Drawing.Size(91, 21);
            this.cpuTempCurLabel.TabIndex = 2;
            this.cpuTempCurLabel.Text = "Current:";
            this.cpuTempCurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMinLabel
            // 
            this.cpuTempMinLabel.AutoSize = true;
            this.cpuTempMinLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMinLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMinLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMinLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempMinLabel.Location = new System.Drawing.Point(103, 84);
            this.cpuTempMinLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMinLabel.Name = "cpuTempMinLabel";
            this.cpuTempMinLabel.Size = new System.Drawing.Size(92, 21);
            this.cpuTempMinLabel.TabIndex = 3;
            this.cpuTempMinLabel.Text = "Minimum:";
            this.cpuTempMinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMaxLabel
            // 
            this.cpuTempMaxLabel.AutoSize = true;
            this.cpuTempMaxLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMaxLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMaxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMaxLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempMaxLabel.Location = new System.Drawing.Point(195, 84);
            this.cpuTempMaxLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMaxLabel.Name = "cpuTempMaxLabel";
            this.cpuTempMaxLabel.Size = new System.Drawing.Size(93, 21);
            this.cpuTempMaxLabel.TabIndex = 4;
            this.cpuTempMaxLabel.Text = "Maximum:";
            this.cpuTempMaxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempCur
            // 
            this.cpuTempCur.AutoSize = true;
            this.cpuTempCur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempCur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempCur.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempCur.ForeColor = System.Drawing.Color.LightGray;
            this.cpuTempCur.Location = new System.Drawing.Point(12, 105);
            this.cpuTempCur.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempCur.Name = "cpuTempCur";
            this.cpuTempCur.Size = new System.Drawing.Size(91, 27);
            this.cpuTempCur.TabIndex = 5;
            this.cpuTempCur.Text = "N/A";
            this.cpuTempCur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMin
            // 
            this.cpuTempMin.AutoSize = true;
            this.cpuTempMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMin.ForeColor = System.Drawing.Color.LimeGreen;
            this.cpuTempMin.Location = new System.Drawing.Point(103, 105);
            this.cpuTempMin.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMin.Name = "cpuTempMin";
            this.cpuTempMin.Size = new System.Drawing.Size(92, 27);
            this.cpuTempMin.TabIndex = 6;
            this.cpuTempMin.Text = "N/A";
            this.cpuTempMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMax
            // 
            this.cpuTempMax.AutoSize = true;
            this.cpuTempMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMax.ForeColor = System.Drawing.Color.Red;
            this.cpuTempMax.Location = new System.Drawing.Point(195, 105);
            this.cpuTempMax.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMax.Name = "cpuTempMax";
            this.cpuTempMax.Size = new System.Drawing.Size(93, 27);
            this.cpuTempMax.TabIndex = 7;
            this.cpuTempMax.Text = "N/A";
            this.cpuTempMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sysmonTitle
            // 
            this.sysmonTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sysmonTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.sysmonTitle.Location = new System.Drawing.Point(33, 28);
            this.sysmonTitle.Name = "sysmonTitle";
            this.sysmonTitle.Size = new System.Drawing.Size(311, 30);
            this.sysmonTitle.TabIndex = 4;
            this.sysmonTitle.Text = "🖥️ System Monitoring";
            this.sysmonTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // divider2
            // 
            this.divider2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.divider2.Location = new System.Drawing.Point(33, 76);
            this.divider2.Name = "divider2";
            this.divider2.Size = new System.Drawing.Size(625, 1);
            this.divider2.TabIndex = 5;
            // 
            // tempTitle
            // 
            this.tempTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.tempTitle.Location = new System.Drawing.Point(33, 89);
            this.tempTitle.Name = "tempTitle";
            this.tempTitle.Size = new System.Drawing.Size(311, 30);
            this.tempTitle.TabIndex = 9;
            this.tempTitle.Text = "🌡 Temperatures";
            this.tempTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // settingsPage
            // 
            this.settingsPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.settingsPage.Controls.Add(this.settingsTitle);
            this.settingsPage.Controls.Add(this.genSettings);
            this.settingsPage.Controls.Add(this.generalSettingsPanel);
            this.settingsPage.Controls.Add(this.traySettingsPanel);
            this.settingsPage.Controls.Add(this.divider3);
            this.settingsPage.Controls.Add(this.traySettingsLabel);
            this.settingsPage.ForeColor = System.Drawing.Color.White;
            this.settingsPage.Location = new System.Drawing.Point(4, 5);
            this.settingsPage.Name = "settingsPage";
            this.settingsPage.Size = new System.Drawing.Size(690, 554);
            this.settingsPage.TabIndex = 1;
            this.settingsPage.Text = "Settings";
            // 
            // settingsTitle
            // 
            this.settingsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.settingsTitle.Location = new System.Drawing.Point(33, 28);
            this.settingsTitle.Name = "settingsTitle";
            this.settingsTitle.Size = new System.Drawing.Size(311, 30);
            this.settingsTitle.TabIndex = 4;
            this.settingsTitle.Text = "⚙ Application Settings";
            this.settingsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // genSettings
            // 
            this.genSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.genSettings.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.genSettings.Location = new System.Drawing.Point(33, 89);
            this.genSettings.Name = "genSettings";
            this.genSettings.Size = new System.Drawing.Size(311, 30);
            this.genSettings.TabIndex = 6;
            this.genSettings.Text = "🌐 General Settings";
            this.genSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // generalSettingsPanel
            // 
            this.generalSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generalSettingsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.generalSettingsPanel.ColumnCount = 2;
            this.generalSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51F));
            this.generalSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49F));
            this.generalSettingsPanel.Controls.Add(this.minimizeOnStart, 1, 0);
            this.generalSettingsPanel.Controls.Add(this.clearSettings, 1, 2);
            this.generalSettingsPanel.Controls.Add(this.osdPanel, 0, 3);
            this.generalSettingsPanel.Controls.Add(this.refreshPanel, 1, 1);
            this.generalSettingsPanel.Controls.Add(this.autostartInstall, 0, 0);
            this.generalSettingsPanel.Controls.Add(this.tempsFahrenheit, 0, 1);
            this.generalSettingsPanel.Controls.Add(this.lightModeSwitch, 0, 2);
            this.generalSettingsPanel.Location = new System.Drawing.Point(33, 129);
            this.generalSettingsPanel.Name = "generalSettingsPanel";
            this.generalSettingsPanel.Padding = new System.Windows.Forms.Padding(5);
            this.generalSettingsPanel.RowCount = 4;
            this.generalSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.generalSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.generalSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.generalSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.generalSettingsPanel.Size = new System.Drawing.Size(625, 152);
            this.generalSettingsPanel.TabIndex = 7;
            // 
            // minimizeOnStart
            // 
            this.minimizeOnStart.AutoSize = true;
            this.minimizeOnStart.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.minimizeOnStart.Dock = System.Windows.Forms.DockStyle.Left;
            this.minimizeOnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeOnStart.ForeColor = System.Drawing.Color.LightGray;
            this.minimizeOnStart.Location = new System.Drawing.Point(321, 8);
            this.minimizeOnStart.Name = "minimizeOnStart";
            this.minimizeOnStart.Padding = new System.Windows.Forms.Padding(3);
            this.minimizeOnStart.Size = new System.Drawing.Size(153, 29);
            this.minimizeOnStart.TabIndex = 25;
            this.minimizeOnStart.Text = "Start minimized to tray";
            this.minimizeOnStart.UseVisualStyleBackColor = true;
            this.minimizeOnStart.CheckedChanged += new System.EventHandler(this.MinimizeOnStart_CheckedChanged);
            // 
            // clearSettings
            // 
            this.clearSettings.BackColor = System.Drawing.Color.Crimson;
            this.clearSettings.Dock = System.Windows.Forms.DockStyle.Left;
            this.clearSettings.FlatAppearance.BorderSize = 0;
            this.clearSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearSettings.ForeColor = System.Drawing.Color.White;
            this.clearSettings.Location = new System.Drawing.Point(327, 78);
            this.clearSettings.Margin = new System.Windows.Forms.Padding(9, 3, 3, 3);
            this.clearSettings.Name = "clearSettings";
            this.clearSettings.Size = new System.Drawing.Size(179, 29);
            this.clearSettings.TabIndex = 24;
            this.clearSettings.Text = "🔁 Reset Settings";
            this.clearSettings.UseVisualStyleBackColor = false;
            this.clearSettings.Click += new System.EventHandler(this.ClearSettings_Click);
            // 
            // osdPanel
            // 
            this.osdPanel.ColumnCount = 2;
            this.osdPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            this.osdPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.osdPanel.Controls.Add(this.osdEnable, 0, 0);
            this.osdPanel.Controls.Add(this.osdSettings, 1, 0);
            this.osdPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.osdPanel.Location = new System.Drawing.Point(8, 113);
            this.osdPanel.Name = "osdPanel";
            this.osdPanel.Padding = new System.Windows.Forms.Padding(1);
            this.osdPanel.RowCount = 1;
            this.osdPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.osdPanel.Size = new System.Drawing.Size(301, 31);
            this.osdPanel.TabIndex = 26;
            // 
            // osdEnable
            // 
            this.osdEnable.AutoSize = true;
            this.osdEnable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.osdEnable.Dock = System.Windows.Forms.DockStyle.Left;
            this.osdEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.osdEnable.ForeColor = System.Drawing.Color.LightGray;
            this.osdEnable.Location = new System.Drawing.Point(4, 4);
            this.osdEnable.Name = "osdEnable";
            this.osdEnable.Size = new System.Drawing.Size(161, 23);
            this.osdEnable.TabIndex = 13;
            this.osdEnable.Text = "On-screen display temps";
            this.osdEnable.UseVisualStyleBackColor = true;
            this.osdEnable.CheckedChanged += new System.EventHandler(this.OsdEnable_CheckedChanged);
            // 
            // osdSettings
            // 
            this.osdSettings.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.osdSettings.BackColor = System.Drawing.Color.SeaGreen;
            this.osdSettings.Enabled = false;
            this.osdSettings.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.osdSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.osdSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.osdSettings.ForeColor = System.Drawing.Color.White;
            this.osdSettings.Location = new System.Drawing.Point(186, 1);
            this.osdSettings.Margin = new System.Windows.Forms.Padding(0);
            this.osdSettings.Name = "osdSettings";
            this.osdSettings.Size = new System.Drawing.Size(51, 29);
            this.osdSettings.TabIndex = 14;
            this.osdSettings.Text = "⛭";
            this.osdSettings.UseVisualStyleBackColor = false;
            this.osdSettings.Click += new System.EventHandler(this.OsdSettings_Click);
            // 
            // refreshPanel
            // 
            this.refreshPanel.ColumnCount = 2;
            this.refreshPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.refreshPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.refreshPanel.Controls.Add(this.refreshLabel, 0, 0);
            this.refreshPanel.Controls.Add(this.refreshValue, 1, 0);
            this.refreshPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.refreshPanel.Location = new System.Drawing.Point(321, 43);
            this.refreshPanel.Name = "refreshPanel";
            this.refreshPanel.Padding = new System.Windows.Forms.Padding(3);
            this.refreshPanel.RowCount = 1;
            this.refreshPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.refreshPanel.Size = new System.Drawing.Size(195, 29);
            this.refreshPanel.TabIndex = 23;
            // 
            // refreshLabel
            // 
            this.refreshLabel.AutoSize = true;
            this.refreshLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshLabel.ForeColor = System.Drawing.Color.LightGray;
            this.refreshLabel.Location = new System.Drawing.Point(3, 3);
            this.refreshLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.refreshLabel.Name = "refreshLabel";
            this.refreshLabel.Padding = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.refreshLabel.Size = new System.Drawing.Size(117, 23);
            this.refreshLabel.TabIndex = 1;
            this.refreshLabel.Text = "Update interval (s):";
            this.refreshLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // refreshValue
            // 
            this.refreshValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.refreshValue.DropDownHeight = 200;
            this.refreshValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.refreshValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshValue.ForeColor = System.Drawing.Color.LightGray;
            this.refreshValue.FormattingEnabled = true;
            this.refreshValue.IntegralHeight = false;
            this.refreshValue.Items.AddRange(new object[] {
            "0.25",
            "0.5",
            "0.75",
            "1",
            "1.25",
            "1.5",
            "1.75",
            "2",
            "2.25",
            "2.5",
            "2.75",
            "3",
            "3.25",
            "3.5",
            "3.75",
            "4",
            "4.25",
            "4.5",
            "4.75",
            "5",
            "5.25",
            "5.5",
            "5.75",
            "6",
            "6.25",
            "6.5",
            "6.75",
            "7",
            "7.25",
            "7.5",
            "7.75",
            "8",
            "8.25",
            "8.5",
            "8.75",
            "9",
            "9.25",
            "9.5",
            "9.75",
            "10"});
            this.refreshValue.Location = new System.Drawing.Point(125, 4);
            this.refreshValue.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.refreshValue.Name = "refreshValue";
            this.refreshValue.Size = new System.Drawing.Size(67, 21);
            this.refreshValue.TabIndex = 3;
            this.refreshValue.SelectedIndexChanged += new System.EventHandler(this.RefreshValue_ValueChanged);
            // 
            // autostartInstall
            // 
            this.autostartInstall.AutoSize = true;
            this.autostartInstall.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autostartInstall.Dock = System.Windows.Forms.DockStyle.Left;
            this.autostartInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartInstall.ForeColor = System.Drawing.Color.LightGray;
            this.autostartInstall.Location = new System.Drawing.Point(8, 8);
            this.autostartInstall.Name = "autostartInstall";
            this.autostartInstall.Padding = new System.Windows.Forms.Padding(3);
            this.autostartInstall.Size = new System.Drawing.Size(162, 29);
            this.autostartInstall.TabIndex = 0;
            this.autostartInstall.Text = "Autostart at boot (Install)";
            this.autostartInstall.UseVisualStyleBackColor = true;
            this.autostartInstall.CheckedChanged += new System.EventHandler(this.AutostartInstall_CheckedChanged);
            // 
            // tempsFahrenheit
            // 
            this.tempsFahrenheit.AutoSize = true;
            this.tempsFahrenheit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tempsFahrenheit.Dock = System.Windows.Forms.DockStyle.Left;
            this.tempsFahrenheit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempsFahrenheit.ForeColor = System.Drawing.Color.LightGray;
            this.tempsFahrenheit.Location = new System.Drawing.Point(8, 43);
            this.tempsFahrenheit.Name = "tempsFahrenheit";
            this.tempsFahrenheit.Padding = new System.Windows.Forms.Padding(3);
            this.tempsFahrenheit.Size = new System.Drawing.Size(207, 29);
            this.tempsFahrenheit.TabIndex = 1;
            this.tempsFahrenheit.Text = "Temperatures in Fahrenheit (°F)";
            this.tempsFahrenheit.UseVisualStyleBackColor = true;
            this.tempsFahrenheit.CheckedChanged += new System.EventHandler(this.Setting_CheckedChanged);
            // 
            // lightModeSwitch
            // 
            this.lightModeSwitch.AutoSize = true;
            this.lightModeSwitch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lightModeSwitch.Dock = System.Windows.Forms.DockStyle.Left;
            this.lightModeSwitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lightModeSwitch.ForeColor = System.Drawing.Color.LightGray;
            this.lightModeSwitch.Location = new System.Drawing.Point(8, 78);
            this.lightModeSwitch.Name = "lightModeSwitch";
            this.lightModeSwitch.Padding = new System.Windows.Forms.Padding(3);
            this.lightModeSwitch.Size = new System.Drawing.Size(136, 29);
            this.lightModeSwitch.TabIndex = 2;
            this.lightModeSwitch.Text = "Enable Light mode";
            this.lightModeSwitch.UseVisualStyleBackColor = true;
            this.lightModeSwitch.CheckedChanged += new System.EventHandler(this.LightModeSwitch_CheckedChanged);
            // 
            // traySettingsPanel
            // 
            this.traySettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traySettingsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.traySettingsPanel.ColumnCount = 2;
            this.traySettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51F));
            this.traySettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49F));
            this.traySettingsPanel.Controls.Add(this.colortempsPanel, 0, 3);
            this.traySettingsPanel.Controls.Add(this.fontFamilyPanel, 2, 2);
            this.traySettingsPanel.Controls.Add(this.singleIconTray, 0, 2);
            this.traySettingsPanel.Controls.Add(this.enableCpuTray, 0, 0);
            this.traySettingsPanel.Controls.Add(this.enableGpuTray, 0, 1);
            this.traySettingsPanel.Controls.Add(this.cpuColorPanel, 1, 0);
            this.traySettingsPanel.Controls.Add(this.gpuColorPanel, 1, 1);
            this.traySettingsPanel.Controls.Add(this.iconsizePanel, 1, 3);
            this.traySettingsPanel.Location = new System.Drawing.Point(33, 333);
            this.traySettingsPanel.Name = "traySettingsPanel";
            this.traySettingsPanel.Padding = new System.Windows.Forms.Padding(5);
            this.traySettingsPanel.RowCount = 4;
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00064F));
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00061F));
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.traySettingsPanel.Size = new System.Drawing.Size(625, 190);
            this.traySettingsPanel.TabIndex = 9;
            // 
            // colortempsPanel
            // 
            this.colortempsPanel.ColumnCount = 2;
            this.colortempsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            this.colortempsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.colortempsPanel.Controls.Add(this.colortempsEnable, 0, 0);
            this.colortempsPanel.Controls.Add(this.colortempsConfig, 1, 0);
            this.colortempsPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.colortempsPanel.Location = new System.Drawing.Point(8, 143);
            this.colortempsPanel.Name = "colortempsPanel";
            this.colortempsPanel.Padding = new System.Windows.Forms.Padding(1);
            this.colortempsPanel.RowCount = 1;
            this.colortempsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.colortempsPanel.Size = new System.Drawing.Size(301, 39);
            this.colortempsPanel.TabIndex = 25;
            // 
            // colortempsEnable
            // 
            this.colortempsEnable.AutoSize = true;
            this.colortempsEnable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.colortempsEnable.Dock = System.Windows.Forms.DockStyle.Left;
            this.colortempsEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colortempsEnable.ForeColor = System.Drawing.Color.LightGray;
            this.colortempsEnable.Location = new System.Drawing.Point(4, 4);
            this.colortempsEnable.Name = "colortempsEnable";
            this.colortempsEnable.Size = new System.Drawing.Size(171, 31);
            this.colortempsEnable.TabIndex = 13;
            this.colortempsEnable.Text = "Temperature-based colors";
            this.colortempsEnable.UseVisualStyleBackColor = true;
            this.colortempsEnable.CheckedChanged += new System.EventHandler(this.ColortempsEnable_CheckedChanged);
            // 
            // colortempsConfig
            // 
            this.colortempsConfig.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.colortempsConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.colortempsConfig.Enabled = false;
            this.colortempsConfig.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.colortempsConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colortempsConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colortempsConfig.ForeColor = System.Drawing.Color.White;
            this.colortempsConfig.Location = new System.Drawing.Point(186, 5);
            this.colortempsConfig.Margin = new System.Windows.Forms.Padding(0);
            this.colortempsConfig.Name = "colortempsConfig";
            this.colortempsConfig.Size = new System.Drawing.Size(51, 29);
            this.colortempsConfig.TabIndex = 14;
            this.colortempsConfig.Text = "⛭";
            this.colortempsConfig.UseVisualStyleBackColor = false;
            this.colortempsConfig.Click += new System.EventHandler(this.ColortempsConfig_Click);
            // 
            // fontFamilyPanel
            // 
            this.fontFamilyPanel.ColumnCount = 2;
            this.fontFamilyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.fontFamilyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fontFamilyPanel.Controls.Add(this.fontFamilyLabel, 0, 0);
            this.fontFamilyPanel.Controls.Add(this.fontFamilyValue, 1, 0);
            this.fontFamilyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fontFamilyPanel.Location = new System.Drawing.Point(321, 98);
            this.fontFamilyPanel.Name = "fontFamilyPanel";
            this.fontFamilyPanel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.fontFamilyPanel.RowCount = 1;
            this.fontFamilyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fontFamilyPanel.Size = new System.Drawing.Size(296, 39);
            this.fontFamilyPanel.TabIndex = 23;
            // 
            // fontFamilyLabel
            // 
            this.fontFamilyLabel.AutoSize = true;
            this.fontFamilyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fontFamilyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontFamilyLabel.ForeColor = System.Drawing.Color.LightGray;
            this.fontFamilyLabel.Location = new System.Drawing.Point(8, 0);
            this.fontFamilyLabel.Name = "fontFamilyLabel";
            this.fontFamilyLabel.Size = new System.Drawing.Size(96, 39);
            this.fontFamilyLabel.TabIndex = 0;
            this.fontFamilyLabel.Text = "Font family:";
            this.fontFamilyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fontFamilyValue
            // 
            this.fontFamilyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.fontFamilyValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.fontFamilyValue.DropDownHeight = 200;
            this.fontFamilyValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontFamilyValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontFamilyValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontFamilyValue.ForeColor = System.Drawing.Color.LightGray;
            this.fontFamilyValue.FormattingEnabled = true;
            this.fontFamilyValue.IntegralHeight = false;
            this.fontFamilyValue.ItemHeight = 13;
            this.fontFamilyValue.Location = new System.Drawing.Point(108, 9);
            this.fontFamilyValue.Margin = new System.Windows.Forms.Padding(1);
            this.fontFamilyValue.Name = "fontFamilyValue";
            this.fontFamilyValue.Size = new System.Drawing.Size(182, 21);
            this.fontFamilyValue.TabIndex = 24;
            this.fontFamilyValue.SelectedIndexChanged += new System.EventHandler(this.Setting_SelectedIndexChanged);
            // 
            // singleIconTray
            // 
            this.singleIconTray.AutoSize = true;
            this.singleIconTray.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singleIconTray.Dock = System.Windows.Forms.DockStyle.Left;
            this.singleIconTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.singleIconTray.ForeColor = System.Drawing.Color.LightGray;
            this.singleIconTray.Location = new System.Drawing.Point(8, 98);
            this.singleIconTray.Name = "singleIconTray";
            this.singleIconTray.Padding = new System.Windows.Forms.Padding(5);
            this.singleIconTray.Size = new System.Drawing.Size(146, 39);
            this.singleIconTray.TabIndex = 22;
            this.singleIconTray.Text = "Single tray icon style";
            this.singleIconTray.UseVisualStyleBackColor = true;
            this.singleIconTray.CheckedChanged += new System.EventHandler(this.SingleIconTray_CheckedChanged);
            // 
            // enableCpuTray
            // 
            this.enableCpuTray.AutoSize = true;
            this.enableCpuTray.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.enableCpuTray.Dock = System.Windows.Forms.DockStyle.Left;
            this.enableCpuTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enableCpuTray.ForeColor = System.Drawing.Color.LightGray;
            this.enableCpuTray.Location = new System.Drawing.Point(8, 8);
            this.enableCpuTray.Name = "enableCpuTray";
            this.enableCpuTray.Padding = new System.Windows.Forms.Padding(5);
            this.enableCpuTray.Size = new System.Drawing.Size(155, 39);
            this.enableCpuTray.TabIndex = 0;
            this.enableCpuTray.Text = "Enable CPU Tray icon";
            this.enableCpuTray.UseVisualStyleBackColor = true;
            this.enableCpuTray.CheckedChanged += new System.EventHandler(this.Setting_CheckedChanged);
            // 
            // enableGpuTray
            // 
            this.enableGpuTray.AutoSize = true;
            this.enableGpuTray.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.enableGpuTray.Dock = System.Windows.Forms.DockStyle.Left;
            this.enableGpuTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enableGpuTray.ForeColor = System.Drawing.Color.LightGray;
            this.enableGpuTray.Location = new System.Drawing.Point(8, 53);
            this.enableGpuTray.Name = "enableGpuTray";
            this.enableGpuTray.Padding = new System.Windows.Forms.Padding(5);
            this.enableGpuTray.Size = new System.Drawing.Size(156, 39);
            this.enableGpuTray.TabIndex = 12;
            this.enableGpuTray.Text = "Enable GPU Tray icon";
            this.enableGpuTray.UseVisualStyleBackColor = true;
            this.enableGpuTray.CheckedChanged += new System.EventHandler(this.Setting_CheckedChanged);
            // 
            // cpuColorPanel
            // 
            this.cpuColorPanel.ColumnCount = 2;
            this.cpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.cpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cpuColorPanel.Controls.Add(this.cpuColorValue, 1, 0);
            this.cpuColorPanel.Controls.Add(this.cpuColorLabel, 0, 0);
            this.cpuColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuColorPanel.Location = new System.Drawing.Point(321, 8);
            this.cpuColorPanel.Name = "cpuColorPanel";
            this.cpuColorPanel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cpuColorPanel.RowCount = 1;
            this.cpuColorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cpuColorPanel.Size = new System.Drawing.Size(296, 39);
            this.cpuColorPanel.TabIndex = 16;
            // 
            // cpuColorValue
            // 
            this.cpuColorValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cpuColorValue.BackColor = System.Drawing.Color.Aqua;
            this.cpuColorValue.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.cpuColorValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuColorValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuColorValue.ForeColor = System.Drawing.Color.Black;
            this.cpuColorValue.Location = new System.Drawing.Point(107, 3);
            this.cpuColorValue.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cpuColorValue.Name = "cpuColorValue";
            this.cpuColorValue.Size = new System.Drawing.Size(184, 33);
            this.cpuColorValue.TabIndex = 10;
            this.cpuColorValue.Text = "🎨";
            this.cpuColorValue.UseVisualStyleBackColor = false;
            this.cpuColorValue.Click += new System.EventHandler(this.CpuColorValue_Click);
            // 
            // cpuColorLabel
            // 
            this.cpuColorLabel.AutoSize = true;
            this.cpuColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuColorLabel.ForeColor = System.Drawing.Color.LightGray;
            this.cpuColorLabel.Location = new System.Drawing.Point(8, 0);
            this.cpuColorLabel.Name = "cpuColorLabel";
            this.cpuColorLabel.Size = new System.Drawing.Size(96, 39);
            this.cpuColorLabel.TabIndex = 0;
            this.cpuColorLabel.Text = "CPU Color:";
            this.cpuColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpuColorPanel
            // 
            this.gpuColorPanel.ColumnCount = 2;
            this.gpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.gpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gpuColorPanel.Controls.Add(this.gpuColorValue, 1, 0);
            this.gpuColorPanel.Controls.Add(this.gpuColorLabel, 0, 0);
            this.gpuColorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuColorPanel.Location = new System.Drawing.Point(321, 53);
            this.gpuColorPanel.Name = "gpuColorPanel";
            this.gpuColorPanel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.gpuColorPanel.RowCount = 1;
            this.gpuColorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gpuColorPanel.Size = new System.Drawing.Size(296, 39);
            this.gpuColorPanel.TabIndex = 17;
            // 
            // gpuColorValue
            // 
            this.gpuColorValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gpuColorValue.BackColor = System.Drawing.Color.Gold;
            this.gpuColorValue.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.gpuColorValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuColorValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuColorValue.ForeColor = System.Drawing.Color.Black;
            this.gpuColorValue.Location = new System.Drawing.Point(107, 3);
            this.gpuColorValue.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.gpuColorValue.Name = "gpuColorValue";
            this.gpuColorValue.Size = new System.Drawing.Size(184, 33);
            this.gpuColorValue.TabIndex = 11;
            this.gpuColorValue.Text = "🎨";
            this.gpuColorValue.UseVisualStyleBackColor = false;
            this.gpuColorValue.Click += new System.EventHandler(this.GpuColorValue_Click);
            // 
            // gpuColorLabel
            // 
            this.gpuColorLabel.AutoSize = true;
            this.gpuColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuColorLabel.ForeColor = System.Drawing.Color.LightGray;
            this.gpuColorLabel.Location = new System.Drawing.Point(8, 0);
            this.gpuColorLabel.Name = "gpuColorLabel";
            this.gpuColorLabel.Size = new System.Drawing.Size(96, 39);
            this.gpuColorLabel.TabIndex = 0;
            this.gpuColorLabel.Text = "GPU Color:";
            this.gpuColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // iconsizePanel
            // 
            this.iconsizePanel.ColumnCount = 2;
            this.iconsizePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.iconsizePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.iconsizePanel.Controls.Add(this.iconsizeLabel, 0, 0);
            this.iconsizePanel.Controls.Add(this.iconsizeValue, 1, 0);
            this.iconsizePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.iconsizePanel.Location = new System.Drawing.Point(321, 143);
            this.iconsizePanel.Name = "iconsizePanel";
            this.iconsizePanel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.iconsizePanel.RowCount = 1;
            this.iconsizePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.iconsizePanel.Size = new System.Drawing.Size(180, 39);
            this.iconsizePanel.TabIndex = 21;
            // 
            // iconsizeLabel
            // 
            this.iconsizeLabel.AutoSize = true;
            this.iconsizeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconsizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconsizeLabel.ForeColor = System.Drawing.Color.LightGray;
            this.iconsizeLabel.Location = new System.Drawing.Point(8, 0);
            this.iconsizeLabel.Name = "iconsizeLabel";
            this.iconsizeLabel.Size = new System.Drawing.Size(94, 39);
            this.iconsizeLabel.TabIndex = 1;
            this.iconsizeLabel.Text = "Icon size (%):";
            this.iconsizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // iconsizeValue
            // 
            this.iconsizeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.iconsizeValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.iconsizeValue.DropDownHeight = 200;
            this.iconsizeValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.iconsizeValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconsizeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconsizeValue.ForeColor = System.Drawing.Color.LightGray;
            this.iconsizeValue.FormattingEnabled = true;
            this.iconsizeValue.IntegralHeight = false;
            this.iconsizeValue.Items.AddRange(new object[] {
            "30",
            "35",
            "40",
            "45",
            "50",
            "55",
            "60",
            "65",
            "70",
            "75",
            "80",
            "85",
            "90",
            "95",
            "100"});
            this.iconsizeValue.Location = new System.Drawing.Point(108, 9);
            this.iconsizeValue.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.iconsizeValue.Name = "iconsizeValue";
            this.iconsizeValue.Size = new System.Drawing.Size(67, 21);
            this.iconsizeValue.TabIndex = 2;
            this.iconsizeValue.SelectedIndexChanged += new System.EventHandler(this.IconsizeValue_ValueChanged);
            // 
            // divider3
            // 
            this.divider3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.divider3.Location = new System.Drawing.Point(33, 76);
            this.divider3.Name = "divider3";
            this.divider3.Size = new System.Drawing.Size(625, 1);
            this.divider3.TabIndex = 5;
            // 
            // traySettingsLabel
            // 
            this.traySettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traySettingsLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.traySettingsLabel.Location = new System.Drawing.Point(33, 293);
            this.traySettingsLabel.Name = "traySettingsLabel";
            this.traySettingsLabel.Size = new System.Drawing.Size(311, 33);
            this.traySettingsLabel.TabIndex = 8;
            this.traySettingsLabel.Text = "🔧 Tray Settings";
            this.traySettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // aboutPage
            // 
            this.aboutPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.aboutPage.Controls.Add(this.checkUpdates);
            this.aboutPage.Controls.Add(this.aboutTitle);
            this.aboutPage.Controls.Add(this.divider1);
            this.aboutPage.Controls.Add(this.appTitleAbout);
            this.aboutPage.Controls.Add(this.appAboutExtra);
            this.aboutPage.Controls.Add(this.appVersion);
            this.aboutPage.Controls.Add(this.githubLink);
            this.aboutPage.Controls.Add(this.donatePic);
            this.aboutPage.ForeColor = System.Drawing.Color.White;
            this.aboutPage.Location = new System.Drawing.Point(4, 5);
            this.aboutPage.Name = "aboutPage";
            this.aboutPage.Size = new System.Drawing.Size(690, 554);
            this.aboutPage.TabIndex = 2;
            this.aboutPage.Text = "About";
            // 
            // checkUpdates
            // 
            this.checkUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkUpdates.BackColor = System.Drawing.Color.Gray;
            this.checkUpdates.FlatAppearance.BorderSize = 0;
            this.checkUpdates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkUpdates.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkUpdates.Location = new System.Drawing.Point(182, 498);
            this.checkUpdates.Name = "checkUpdates";
            this.checkUpdates.Size = new System.Drawing.Size(135, 27);
            this.checkUpdates.TabIndex = 8;
            this.checkUpdates.Text = "Check updates ⏳";
            this.checkUpdates.UseVisualStyleBackColor = false;
            this.checkUpdates.Click += new System.EventHandler(this.CheckUpdates_Click);
            // 
            // aboutTitle
            // 
            this.aboutTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.aboutTitle.Location = new System.Drawing.Point(33, 28);
            this.aboutTitle.Name = "aboutTitle";
            this.aboutTitle.Size = new System.Drawing.Size(311, 30);
            this.aboutTitle.TabIndex = 1;
            this.aboutTitle.Text = "❓ About";
            this.aboutTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // divider1
            // 
            this.divider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.divider1.Location = new System.Drawing.Point(33, 76);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(626, 1);
            this.divider1.TabIndex = 2;
            // 
            // appTitleAbout
            // 
            this.appTitleAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appTitleAbout.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.appTitleAbout.Location = new System.Drawing.Point(33, 89);
            this.appTitleAbout.Name = "appTitleAbout";
            this.appTitleAbout.Size = new System.Drawing.Size(311, 30);
            this.appTitleAbout.TabIndex = 3;
            this.appTitleAbout.Text = "🌡️ Tray Temps";
            this.appTitleAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // appAboutExtra
            // 
            this.appAboutExtra.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appAboutExtra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.appAboutExtra.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appAboutExtra.ForeColor = System.Drawing.Color.DarkGray;
            this.appAboutExtra.Location = new System.Drawing.Point(33, 126);
            this.appAboutExtra.Name = "appAboutExtra";
            this.appAboutExtra.Padding = new System.Windows.Forms.Padding(10);
            this.appAboutExtra.Size = new System.Drawing.Size(626, 234);
            this.appAboutExtra.TabIndex = 4;
            this.appAboutExtra.Text = resources.GetString("appAboutExtra.Text");
            this.appAboutExtra.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // appVersion
            // 
            this.appVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.appVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appVersion.ForeColor = System.Drawing.Color.DimGray;
            this.appVersion.Location = new System.Drawing.Point(33, 499);
            this.appVersion.Name = "appVersion";
            this.appVersion.Size = new System.Drawing.Size(143, 24);
            this.appVersion.TabIndex = 5;
            this.appVersion.Text = "Version: 0.0.0.0";
            this.appVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // githubLink
            // 
            this.githubLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.githubLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.githubLink.ForeColor = System.Drawing.Color.SeaGreen;
            this.githubLink.Location = new System.Drawing.Point(33, 377);
            this.githubLink.Name = "githubLink";
            this.githubLink.Size = new System.Drawing.Size(311, 24);
            this.githubLink.TabIndex = 7;
            this.githubLink.Text = "GitHub: github.com/nmd-113/Tray-Temps";
            this.githubLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.githubLink.Click += new System.EventHandler(this.GithubLink_Click);
            // 
            // donatePic
            // 
            this.donatePic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.donatePic.BackgroundImage = global::TrayTemps.Properties.Resources.donate;
            this.donatePic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.donatePic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.donatePic.Location = new System.Drawing.Point(608, 472);
            this.donatePic.Name = "donatePic";
            this.donatePic.Size = new System.Drawing.Size(51, 51);
            this.donatePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.donatePic.TabIndex = 6;
            this.donatePic.TabStop = false;
            this.donatePic.Click += new System.EventHandler(this.DonatePic_Click);
            // 
            // panelWrapper
            // 
            this.panelWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWrapper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panelWrapper.Controls.Add(this.resizeGrip);
            this.panelWrapper.Controls.Add(this.mainTabControl);
            this.panelWrapper.Location = new System.Drawing.Point(111, 46);
            this.panelWrapper.Name = "panelWrapper";
            this.panelWrapper.Size = new System.Drawing.Size(688, 553);
            this.panelWrapper.TabIndex = 3;
            // 
            // resizeGrip
            // 
            this.resizeGrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeGrip.BackColor = System.Drawing.Color.Transparent;
            this.resizeGrip.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeGrip.Location = new System.Drawing.Point(668, 533);
            this.resizeGrip.Margin = new System.Windows.Forms.Padding(0);
            this.resizeGrip.Name = "resizeGrip";
            this.resizeGrip.Size = new System.Drawing.Size(20, 20);
            this.resizeGrip.TabIndex = 1;
            this.resizeGrip.Paint += new System.Windows.Forms.PaintEventHandler(this.ResizeGrip_Paint);
            this.resizeGrip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeGrip_MouseDown);
            // 
            // cpuTrayIcon
            // 
            this.cpuTrayIcon.ContextMenuStrip = this.contextMenuStrip;
            this.cpuTrayIcon.Text = "CPU Temp";
            this.cpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CpuTrayIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.contextMenuStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.contextMenuStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowForm,
            this.trayMenuSeparatorTop,
            this.trayDisplayMenu,
            this.openSettingsTray,
            this.trayMenuSeparatorBottom,
            this.SettingsTray});
            this.contextMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.Size = new System.Drawing.Size(149, 136);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_Opening);
            // 
            // ShowForm
            // 
            this.ShowForm.AutoSize = false;
            this.ShowForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowForm.ForeColor = System.Drawing.Color.White;
            this.ShowForm.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ShowForm.Name = "ShowForm";
            this.ShowForm.Size = new System.Drawing.Size(145, 30);
            this.ShowForm.Text = "🖥️ Show Window";
            this.ShowForm.Click += new System.EventHandler(this.ShowForm_Click);
            // 
            // trayMenuSeparatorTop
            // 
            this.trayMenuSeparatorTop.Name = "trayMenuSeparatorTop";
            this.trayMenuSeparatorTop.Size = new System.Drawing.Size(145, 6);
            // 
            // trayDisplayMenu
            // 
            this.trayDisplayMenu.AutoSize = false;
            this.trayDisplayMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayCpuEnabledMenu,
            this.trayGpuEnabledMenu,
            this.trayCombinedMenu,
            this.trayMenuSeparatorDisplay,
            this.trayFahrenheitMenu,
            this.trayTemperatureColorsMenu,
            this.trayConfigureColorsMenu});
            this.trayDisplayMenu.Name = "trayDisplayMenu";
            this.trayDisplayMenu.Size = new System.Drawing.Size(148, 30);
            this.trayDisplayMenu.Text = "Display";
            // 
            // trayCpuEnabledMenu
            // 
            this.trayCpuEnabledMenu.AutoSize = false;
            this.trayCpuEnabledMenu.Name = "trayCpuEnabledMenu";
            this.trayCpuEnabledMenu.Size = new System.Drawing.Size(255, 28);
            this.trayCpuEnabledMenu.Text = "Show CPU temperature";
            this.trayCpuEnabledMenu.Click += new System.EventHandler(this.TrayCpuEnabledMenu_Click);
            // 
            // trayGpuEnabledMenu
            // 
            this.trayGpuEnabledMenu.AutoSize = false;
            this.trayGpuEnabledMenu.Name = "trayGpuEnabledMenu";
            this.trayGpuEnabledMenu.Size = new System.Drawing.Size(255, 28);
            this.trayGpuEnabledMenu.Text = "Show GPU temperature";
            this.trayGpuEnabledMenu.Click += new System.EventHandler(this.TrayGpuEnabledMenu_Click);
            // 
            // trayCombinedMenu
            // 
            this.trayCombinedMenu.AutoSize = false;
            this.trayCombinedMenu.Name = "trayCombinedMenu";
            this.trayCombinedMenu.Size = new System.Drawing.Size(255, 28);
            this.trayCombinedMenu.Text = "Combine CPU and GPU";
            this.trayCombinedMenu.Click += new System.EventHandler(this.TrayCombinedMenu_Click);
            // 
            // trayMenuSeparatorDisplay
            // 
            this.trayMenuSeparatorDisplay.Name = "trayMenuSeparatorDisplay";
            this.trayMenuSeparatorDisplay.Size = new System.Drawing.Size(252, 6);
            // 
            // trayFahrenheitMenu
            // 
            this.trayFahrenheitMenu.AutoSize = false;
            this.trayFahrenheitMenu.Name = "trayFahrenheitMenu";
            this.trayFahrenheitMenu.Size = new System.Drawing.Size(255, 28);
            this.trayFahrenheitMenu.Text = "Use Fahrenheit";
            this.trayFahrenheitMenu.Click += new System.EventHandler(this.TrayFahrenheitMenu_Click);
            // 
            // trayTemperatureColorsMenu
            // 
            this.trayTemperatureColorsMenu.AutoSize = false;
            this.trayTemperatureColorsMenu.Name = "trayTemperatureColorsMenu";
            this.trayTemperatureColorsMenu.Size = new System.Drawing.Size(255, 28);
            this.trayTemperatureColorsMenu.Text = "Temperature-based colors";
            this.trayTemperatureColorsMenu.Click += new System.EventHandler(this.TrayTemperatureColorsMenu_Click);
            // 
            // trayConfigureColorsMenu
            // 
            this.trayConfigureColorsMenu.AutoSize = false;
            this.trayConfigureColorsMenu.Name = "trayConfigureColorsMenu";
            this.trayConfigureColorsMenu.Size = new System.Drawing.Size(255, 28);
            this.trayConfigureColorsMenu.Text = "Configure temperature colors...";
            this.trayConfigureColorsMenu.Click += new System.EventHandler(this.TrayConfigureColorsMenu_Click);
            // 
            // openSettingsTray
            // 
            this.openSettingsTray.AutoSize = false;
            this.openSettingsTray.Name = "openSettingsTray";
            this.openSettingsTray.Size = new System.Drawing.Size(145, 30);
            this.openSettingsTray.Text = "Open Settings";
            this.openSettingsTray.Click += new System.EventHandler(this.OpenSettingsTray_Click);
            // 
            // trayMenuSeparatorBottom
            // 
            this.trayMenuSeparatorBottom.Name = "trayMenuSeparatorBottom";
            this.trayMenuSeparatorBottom.Size = new System.Drawing.Size(145, 6);
            // 
            // SettingsTray
            // 
            this.SettingsTray.AutoSize = false;
            this.SettingsTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsTray.ForeColor = System.Drawing.Color.Red;
            this.SettingsTray.Name = "SettingsTray";
            this.SettingsTray.Size = new System.Drawing.Size(145, 30);
            this.SettingsTray.Text = "❌ Exit";
            this.SettingsTray.Click += new System.EventHandler(this.ExitForm_Click);
            // 
            // gpuTrayIcon
            // 
            this.gpuTrayIcon.ContextMenuStrip = this.contextMenuStrip;
            this.gpuTrayIcon.Text = "GPU Temp";
            this.gpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GpuTrayIcon_MouseDoubleClick);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.BalloonTipText = "Double click to show.";
            this.NotifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "TrayTemps";
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panelWrapper);
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.exitBtn);
            this.Controls.Add(this.mainMenu);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1200, 800);
            this.MinimumSize = new System.Drawing.Size(720, 580);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TrayTemps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.mainMenu.ResumeLayout(false);
            this.aboutPanel.ResumeLayout(false);
            this.settingsPanel.ResumeLayout(false);
            this.AppDataPnl.ResumeLayout(false);
            this.AppDataPnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).EndInit();
            this.homePanel.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.homePage.ResumeLayout(false);
            this.mainComponentsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cpuIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpuIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ramIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ssdIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mboIcon)).EndInit();
            this.tempsWrapper.ResumeLayout(false);
            this.gpuPanel.ResumeLayout(false);
            this.gpuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpuBrandPic)).EndInit();
            this.cpuPanel.ResumeLayout(false);
            this.cpuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuBrandPic)).EndInit();
            this.settingsPage.ResumeLayout(false);
            this.generalSettingsPanel.ResumeLayout(false);
            this.generalSettingsPanel.PerformLayout();
            this.osdPanel.ResumeLayout(false);
            this.osdPanel.PerformLayout();
            this.refreshPanel.ResumeLayout(false);
            this.refreshPanel.PerformLayout();
            this.traySettingsPanel.ResumeLayout(false);
            this.traySettingsPanel.PerformLayout();
            this.colortempsPanel.ResumeLayout(false);
            this.colortempsPanel.PerformLayout();
            this.fontFamilyPanel.ResumeLayout(false);
            this.fontFamilyPanel.PerformLayout();
            this.cpuColorPanel.ResumeLayout(false);
            this.cpuColorPanel.PerformLayout();
            this.gpuColorPanel.ResumeLayout(false);
            this.gpuColorPanel.PerformLayout();
            this.iconsizePanel.ResumeLayout(false);
            this.iconsizePanel.PerformLayout();
            this.aboutPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.donatePic)).EndInit();
            this.panelWrapper.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainMenu;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Button minimizeBtn;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.Panel panelWrapper;
        private ResizeGripPanel resizeGrip;
        private System.Windows.Forms.TabPage homePage;
        private System.Windows.Forms.TabPage settingsPage;
        private System.Windows.Forms.TabPage aboutPage;
        private System.Windows.Forms.Label aboutTitle;
        private System.Windows.Forms.Label appTitle;
        private System.Windows.Forms.TableLayoutPanel AppDataPnl;
        private System.Windows.Forms.PictureBox appLogo;
        private System.Windows.Forms.Panel divider1;
        private System.Windows.Forms.Label appAboutExtra;
        private System.Windows.Forms.Label appTitleAbout;
        private System.Windows.Forms.PictureBox donatePic;
        private System.Windows.Forms.Label appVersion;
        private System.Windows.Forms.Label githubLink;
        private System.Windows.Forms.Panel divider2;
        private System.Windows.Forms.Label sysmonTitle;
        private System.Windows.Forms.Label genSettings;
        private System.Windows.Forms.Panel divider3;
        private System.Windows.Forms.Label settingsTitle;
        private System.Windows.Forms.TableLayoutPanel cpuPanel;
        private System.Windows.Forms.Label cpuTempLabel;
        private System.Windows.Forms.TableLayoutPanel gpuPanel;
        private System.Windows.Forms.Label gpuTempLabel;
        private System.Windows.Forms.Label tempTitle;
        private System.Windows.Forms.Label gpuTempMaxLabel;
        private System.Windows.Forms.Label gpuTempMinLabel;
        private System.Windows.Forms.Label gpuTempCurLabel;
        private System.Windows.Forms.Label cpuTempMaxLabel;
        private System.Windows.Forms.Label cpuTempMinLabel;
        private System.Windows.Forms.Label cpuTempCurLabel;
        private System.Windows.Forms.Label cpuTempMax;
        private System.Windows.Forms.Label cpuTempMin;
        private System.Windows.Forms.Label cpuTempCur;
        private System.Windows.Forms.Label gpuTempMax;
        private System.Windows.Forms.Label gpuTempMin;
        private System.Windows.Forms.Label gpuTempCur;
        private System.Windows.Forms.Label cpuName;
        private System.Windows.Forms.Label gpuName;
        private System.Windows.Forms.TableLayoutPanel tempsWrapper;
        private System.Windows.Forms.TableLayoutPanel mainComponentsPanel;
        private System.Windows.Forms.Label mainComponentsTitle;
        private System.Windows.Forms.Label componentModel;
        private System.Windows.Forms.Label componentType;
        private System.Windows.Forms.Label CompMotherboardLabel;
        private System.Windows.Forms.Label CompStorageLabel;
        private System.Windows.Forms.Label compRamLabel;
        private System.Windows.Forms.Label compGpuLabel;
        private System.Windows.Forms.Label compCpuLabel;
        private System.Windows.Forms.Label motherboardDetails;
        private System.Windows.Forms.Label storageDetails;
        private System.Windows.Forms.Label ramDetails;
        private System.Windows.Forms.Label gpuModel;
        private System.Windows.Forms.Label cpuModel;
        private System.Windows.Forms.ComboBox cpuTempSensorSelect;
        private System.Windows.Forms.ComboBox cpuIndexSelect;
        private System.Windows.Forms.ComboBox gpuTempSensorSelect;
        private System.Windows.Forms.ComboBox gpuIndexSelect;
        private System.Windows.Forms.Button cpuConfigButton;
        private System.Windows.Forms.Button gpuConfigButton;
        private System.Windows.Forms.TableLayoutPanel generalSettingsPanel;
        private System.Windows.Forms.CheckBox autostartInstall;
        private System.Windows.Forms.CheckBox tempsFahrenheit;
        private System.Windows.Forms.TableLayoutPanel traySettingsPanel;
        private System.Windows.Forms.CheckBox enableCpuTray;
        private System.Windows.Forms.Label traySettingsLabel;
        private System.Windows.Forms.CheckBox enableGpuTray;
        private System.Windows.Forms.TableLayoutPanel gpuColorPanel;
        private System.Windows.Forms.Label gpuColorLabel;
        private System.Windows.Forms.TableLayoutPanel cpuColorPanel;
        private System.Windows.Forms.Label cpuColorLabel;
        private System.Windows.Forms.TableLayoutPanel iconsizePanel;
        private System.Windows.Forms.Label iconsizeLabel;
        private System.Windows.Forms.ComboBox iconsizeValue;
        private System.Windows.Forms.NotifyIcon cpuTrayIcon;
        private System.Windows.Forms.NotifyIcon gpuTrayIcon;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.TableLayoutPanel fontFamilyPanel;
        private System.Windows.Forms.Label fontFamilyLabel;
        private System.Windows.Forms.ComboBox fontFamilyValue;
        private System.Windows.Forms.CheckBox singleIconTray;
        private System.Windows.Forms.TableLayoutPanel colortempsPanel;
        private System.Windows.Forms.CheckBox colortempsEnable;
        private System.Windows.Forms.Button colortempsConfig;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ShowForm;
        private System.Windows.Forms.ToolStripMenuItem SettingsTray;
        private System.Windows.Forms.ToolStripSeparator trayMenuSeparatorTop;
        private System.Windows.Forms.ToolStripMenuItem trayDisplayMenu;
        private System.Windows.Forms.ToolStripMenuItem trayCpuEnabledMenu;
        private System.Windows.Forms.ToolStripMenuItem trayGpuEnabledMenu;
        private System.Windows.Forms.ToolStripMenuItem trayCombinedMenu;
        private System.Windows.Forms.ToolStripSeparator trayMenuSeparatorDisplay;
        private System.Windows.Forms.ToolStripMenuItem trayFahrenheitMenu;
        private System.Windows.Forms.ToolStripMenuItem trayTemperatureColorsMenu;
        private System.Windows.Forms.ToolStripMenuItem trayConfigureColorsMenu;
        private System.Windows.Forms.ToolStripSeparator trayMenuSeparatorBottom;
        private System.Windows.Forms.ToolStripMenuItem openSettingsTray;
        private System.Windows.Forms.Button cpuColorValue;
        private System.Windows.Forms.Button gpuColorValue;
        private System.Windows.Forms.TableLayoutPanel homePanel;
        private System.Windows.Forms.Panel sidepanelHome;
        private System.Windows.Forms.Button homeBtn;
        private System.Windows.Forms.TableLayoutPanel settingsPanel;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Panel sidepanelSettings;
        private System.Windows.Forms.TableLayoutPanel aboutPanel;
        private System.Windows.Forms.Button aboutBtn;
        private System.Windows.Forms.Panel sidepanelAbout;
        private System.Windows.Forms.PictureBox gpuBrandPic;
        private System.Windows.Forms.PictureBox cpuBrandPic;
        private System.Windows.Forms.PictureBox cpuIcon;
        private System.Windows.Forms.PictureBox mboIcon;
        private System.Windows.Forms.PictureBox ssdIcon;
        private System.Windows.Forms.PictureBox ramIcon;
        private System.Windows.Forms.PictureBox gpuIcon;
        private System.Windows.Forms.Button clearSettings;
        private System.Windows.Forms.TableLayoutPanel refreshPanel;
        private System.Windows.Forms.Label refreshLabel;
        private System.Windows.Forms.ComboBox refreshValue;
        private System.Windows.Forms.CheckBox lightModeSwitch;
        private System.Windows.Forms.CheckBox minimizeOnStart;
        private System.Windows.Forms.Button checkUpdates;
        private System.Windows.Forms.Label indexLabel;
        private System.Windows.Forms.TableLayoutPanel osdPanel;
        private System.Windows.Forms.CheckBox osdEnable;
        private System.Windows.Forms.Button osdSettings;
    }
}
