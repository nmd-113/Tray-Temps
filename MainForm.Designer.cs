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
                _tempTimer?.Stop();

                components?.Dispose();

                if (cpuTrayIcon != null) { cpuTrayIcon.Icon?.Dispose(); cpuTrayIcon.Dispose(); }
                if (gpuTrayIcon != null) { gpuTrayIcon.Icon?.Dispose(); gpuTrayIcon.Dispose(); }
                if (NotifyIcon != null) { NotifyIcon.Icon?.Dispose(); NotifyIcon.Dispose(); }

                _trayFont?.Dispose();
                _cpuBrush?.Dispose();
                _gpuBrush?.Dispose();

                _tempTimer?.Dispose();
                _computer?.Close();
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
            this.mainMenu = new System.Windows.Forms.TableLayoutPanel();
            this.AppDataPnl = new System.Windows.Forms.TableLayoutPanel();
            this.appLogo = new System.Windows.Forms.PictureBox();
            this.appTitle = new System.Windows.Forms.Label();
            this.aboutBtn = new System.Windows.Forms.Button();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.homeBtn = new System.Windows.Forms.Button();
            this.exitBtn = new System.Windows.Forms.Button();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.homePage = new System.Windows.Forms.TabPage();
            this.mainComponentsTitle = new System.Windows.Forms.Label();
            this.mainComponentsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.componentType = new System.Windows.Forms.Label();
            this.indexLabel = new System.Windows.Forms.Label();
            this.componentModel = new System.Windows.Forms.Label();
            this.compCpuLabel = new System.Windows.Forms.Label();
            this.cpuIndexSelect = new System.Windows.Forms.ComboBox();
            this.cpuModel = new System.Windows.Forms.Label();
            this.compGpuLabel = new System.Windows.Forms.Label();
            this.gpuIndexSelect = new System.Windows.Forms.ComboBox();
            this.gpuModel = new System.Windows.Forms.Label();
            this.compRamLabel = new System.Windows.Forms.Label();
            this.ramDetails = new System.Windows.Forms.Label();
            this.CompStorageLabel = new System.Windows.Forms.Label();
            this.storageIndexSelect = new System.Windows.Forms.ComboBox();
            this.storageDetails = new System.Windows.Forms.Label();
            this.CompMotherboardLabel = new System.Windows.Forms.Label();
            this.motherboardDetails = new System.Windows.Forms.Label();
            this.placeholderLabel1 = new System.Windows.Forms.Label();
            this.placeholderLabel2 = new System.Windows.Forms.Label();
            this.tempsWrapper = new System.Windows.Forms.TableLayoutPanel();
            this.gpuPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gpuTempLabel = new System.Windows.Forms.Label();
            this.gpuName = new System.Windows.Forms.Label();
            this.gpuTempCurLabel = new System.Windows.Forms.Label();
            this.gpuTempMinLabel = new System.Windows.Forms.Label();
            this.gpuTempMaxLabel = new System.Windows.Forms.Label();
            this.gpuTempCur = new System.Windows.Forms.Label();
            this.gpuTempMin = new System.Windows.Forms.Label();
            this.gpuTempMax = new System.Windows.Forms.Label();
            this.cpuPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuTempLabel = new System.Windows.Forms.Label();
            this.cpuName = new System.Windows.Forms.Label();
            this.cpuTempCurLabel = new System.Windows.Forms.Label();
            this.cpuTempMinLabel = new System.Windows.Forms.Label();
            this.cpuTempMaxLabel = new System.Windows.Forms.Label();
            this.cpuTempCur = new System.Windows.Forms.Label();
            this.cpuTempMin = new System.Windows.Forms.Label();
            this.cpuTempMax = new System.Windows.Forms.Label();
            this.sysmonTitle = new System.Windows.Forms.Label();
            this.divider2 = new System.Windows.Forms.PictureBox();
            this.tempTitle = new System.Windows.Forms.Label();
            this.settingsPage = new System.Windows.Forms.TabPage();
            this.settingsTitle = new System.Windows.Forms.Label();
            this.genSettings = new System.Windows.Forms.Label();
            this.generalSettingsLabel = new System.Windows.Forms.TableLayoutPanel();
            this.autostartInstall = new System.Windows.Forms.CheckBox();
            this.tempsFahrenheit = new System.Windows.Forms.CheckBox();
            this.singleIconTray = new System.Windows.Forms.CheckBox();
            this.refreshPanel = new System.Windows.Forms.TableLayoutPanel();
            this.refreshLabel = new System.Windows.Forms.Label();
            this.refreshValue = new System.Windows.Forms.NumericUpDown();
            this.traySettingsLabel = new System.Windows.Forms.Label();
            this.traySettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.enableCpuTray = new System.Windows.Forms.CheckBox();
            this.enableGpuTray = new System.Windows.Forms.CheckBox();
            this.fontFamilyPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fontFamilyLabel = new System.Windows.Forms.Label();
            this.fontFamilyValue = new System.Windows.Forms.ComboBox();
            this.cpuColorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuColorLabel = new System.Windows.Forms.Label();
            this.cpuColorValue = new System.Windows.Forms.ComboBox();
            this.gpuColorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gpuColorLabel = new System.Windows.Forms.Label();
            this.gpuColorValue = new System.Windows.Forms.ComboBox();
            this.iconsizePanel = new System.Windows.Forms.TableLayoutPanel();
            this.iconsizeLabel = new System.Windows.Forms.Label();
            this.iconsizeValue = new System.Windows.Forms.NumericUpDown();
            this.divider3 = new System.Windows.Forms.PictureBox();
            this.aboutPage = new System.Windows.Forms.TabPage();
            this.aboutTitle = new System.Windows.Forms.Label();
            this.divider1 = new System.Windows.Forms.PictureBox();
            this.appTitleAbout = new System.Windows.Forms.Label();
            this.appAboutExtra = new System.Windows.Forms.Label();
            this.appVersion = new System.Windows.Forms.Label();
            this.githubLink = new System.Windows.Forms.Label();
            this.donatePic = new System.Windows.Forms.PictureBox();
            this.panelWrapper = new System.Windows.Forms.Panel();
            this.cpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowForm = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitForm = new System.Windows.Forms.ToolStripMenuItem();
            this.gpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mainMenu.SuspendLayout();
            this.AppDataPnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).BeginInit();
            this.mainTabControl.SuspendLayout();
            this.homePage.SuspendLayout();
            this.mainComponentsPanel.SuspendLayout();
            this.tempsWrapper.SuspendLayout();
            this.gpuPanel.SuspendLayout();
            this.cpuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.divider2)).BeginInit();
            this.settingsPage.SuspendLayout();
            this.generalSettingsLabel.SuspendLayout();
            this.refreshPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshValue)).BeginInit();
            this.traySettingsPanel.SuspendLayout();
            this.fontFamilyPanel.SuspendLayout();
            this.cpuColorPanel.SuspendLayout();
            this.gpuColorPanel.SuspendLayout();
            this.iconsizePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconsizeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.divider3)).BeginInit();
            this.aboutPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.divider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.donatePic)).BeginInit();
            this.panelWrapper.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mainMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.mainMenu.ColumnCount = 1;
            this.mainMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainMenu.Controls.Add(this.AppDataPnl, 0, 1);
            this.mainMenu.Controls.Add(this.aboutBtn, 0, 6);
            this.mainMenu.Controls.Add(this.settingsBtn, 0, 4);
            this.mainMenu.Controls.Add(this.homeBtn, 0, 3);
            this.mainMenu.Location = new System.Drawing.Point(1, 1);
            this.mainMenu.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.RowCount = 8;
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.mainMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.mainMenu.Size = new System.Drawing.Size(138, 748);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.mainMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.mainMenu.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            // 
            // AppDataPnl
            // 
            this.AppDataPnl.ColumnCount = 1;
            this.AppDataPnl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AppDataPnl.Controls.Add(this.appLogo, 0, 0);
            this.AppDataPnl.Controls.Add(this.appTitle, 0, 1);
            this.AppDataPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppDataPnl.Location = new System.Drawing.Point(4, 60);
            this.AppDataPnl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AppDataPnl.Name = "AppDataPnl";
            this.AppDataPnl.RowCount = 2;
            this.AppDataPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.AppDataPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.AppDataPnl.Size = new System.Drawing.Size(130, 86);
            this.AppDataPnl.TabIndex = 1;
            // 
            // appLogo
            // 
            this.appLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appLogo.Image = global::TrayTemps.Properties.Resources.traytemps;
            this.appLogo.Location = new System.Drawing.Point(4, 4);
            this.appLogo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.appLogo.Name = "appLogo";
            this.appLogo.Size = new System.Drawing.Size(122, 52);
            this.appLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.appLogo.TabIndex = 0;
            this.appLogo.TabStop = false;
            // 
            // appTitle
            // 
            this.appTitle.AutoSize = true;
            this.appTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.appTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appTitle.ForeColor = System.Drawing.Color.White;
            this.appTitle.Location = new System.Drawing.Point(4, 60);
            this.appTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.appTitle.Name = "appTitle";
            this.appTitle.Size = new System.Drawing.Size(122, 26);
            this.appTitle.TabIndex = 4;
            this.appTitle.Text = "TrayTemps";
            this.appTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // aboutBtn
            // 
            this.aboutBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.aboutBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.aboutBtn.FlatAppearance.BorderSize = 0;
            this.aboutBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.aboutBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.aboutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutBtn.ForeColor = System.Drawing.Color.White;
            this.aboutBtn.Location = new System.Drawing.Point(5, 603);
            this.aboutBtn.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(128, 84);
            this.aboutBtn.TabIndex = 2;
            this.aboutBtn.Text = "❓\r\nAbout";
            this.aboutBtn.UseVisualStyleBackColor = false;
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.settingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.settingsBtn.FlatAppearance.BorderSize = 0;
            this.settingsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.settingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsBtn.ForeColor = System.Drawing.Color.White;
            this.settingsBtn.Location = new System.Drawing.Point(5, 379);
            this.settingsBtn.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(128, 84);
            this.settingsBtn.TabIndex = 1;
            this.settingsBtn.Text = "⚙\r\nSettings";
            this.settingsBtn.UseVisualStyleBackColor = false;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // homeBtn
            // 
            this.homeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.homeBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.homeBtn.FlatAppearance.BorderSize = 0;
            this.homeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.homeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.homeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homeBtn.ForeColor = System.Drawing.Color.White;
            this.homeBtn.Location = new System.Drawing.Point(5, 285);
            this.homeBtn.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.homeBtn.Name = "homeBtn";
            this.homeBtn.Size = new System.Drawing.Size(128, 84);
            this.homeBtn.TabIndex = 0;
            this.homeBtn.Text = "🖥️\r\nMain";
            this.homeBtn.UseVisualStyleBackColor = false;
            this.homeBtn.Click += new System.EventHandler(this.homeBtn_Click);
            // 
            // exitBtn
            // 
            this.exitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.ForeColor = System.Drawing.Color.White;
            this.exitBtn.Location = new System.Drawing.Point(930, 1);
            this.exitBtn.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(69, 56);
            this.exitBtn.TabIndex = 1;
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.minimizeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.minimizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeBtn.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeBtn.ForeColor = System.Drawing.Color.White;
            this.minimizeBtn.Location = new System.Drawing.Point(861, 1);
            this.minimizeBtn.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(69, 56);
            this.minimizeBtn.TabIndex = 2;
            this.minimizeBtn.Text = "─";
            this.minimizeBtn.UseVisualStyleBackColor = true;
            this.minimizeBtn.Click += new System.EventHandler(this.minimizeBtn_Click);
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
            this.mainTabControl.Location = new System.Drawing.Point(-6, -6);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(872, 704);
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
            this.homePage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.homePage.Name = "homePage";
            this.homePage.Size = new System.Drawing.Size(864, 695);
            this.homePage.TabIndex = 0;
            this.homePage.Text = "Home";
            // 
            // mainComponentsTitle
            // 
            this.mainComponentsTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainComponentsTitle.Location = new System.Drawing.Point(41, 358);
            this.mainComponentsTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mainComponentsTitle.Name = "mainComponentsTitle";
            this.mainComponentsTitle.Size = new System.Drawing.Size(389, 38);
            this.mainComponentsTitle.TabIndex = 14;
            this.mainComponentsTitle.Text = "🧩 Components";
            // 
            // mainComponentsPanel
            // 
            this.mainComponentsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainComponentsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.mainComponentsPanel.ColumnCount = 3;
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.mainComponentsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainComponentsPanel.Controls.Add(this.componentType, 0, 0);
            this.mainComponentsPanel.Controls.Add(this.indexLabel, 1, 0);
            this.mainComponentsPanel.Controls.Add(this.componentModel, 2, 0);
            this.mainComponentsPanel.Controls.Add(this.compCpuLabel, 0, 1);
            this.mainComponentsPanel.Controls.Add(this.cpuIndexSelect, 1, 1);
            this.mainComponentsPanel.Controls.Add(this.cpuModel, 2, 1);
            this.mainComponentsPanel.Controls.Add(this.compGpuLabel, 0, 2);
            this.mainComponentsPanel.Controls.Add(this.gpuIndexSelect, 1, 2);
            this.mainComponentsPanel.Controls.Add(this.gpuModel, 2, 2);
            this.mainComponentsPanel.Controls.Add(this.compRamLabel, 0, 3);
            this.mainComponentsPanel.Controls.Add(this.ramDetails, 2, 3);
            this.mainComponentsPanel.Controls.Add(this.CompStorageLabel, 0, 4);
            this.mainComponentsPanel.Controls.Add(this.storageIndexSelect, 1, 4);
            this.mainComponentsPanel.Controls.Add(this.storageDetails, 2, 4);
            this.mainComponentsPanel.Controls.Add(this.CompMotherboardLabel, 0, 5);
            this.mainComponentsPanel.Controls.Add(this.motherboardDetails, 2, 5);
            this.mainComponentsPanel.Controls.Add(this.placeholderLabel1, 1, 5);
            this.mainComponentsPanel.Controls.Add(this.placeholderLabel2, 1, 3);
            this.mainComponentsPanel.Location = new System.Drawing.Point(41, 404);
            this.mainComponentsPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mainComponentsPanel.Name = "mainComponentsPanel";
            this.mainComponentsPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.mainComponentsPanel.RowCount = 6;
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainComponentsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainComponentsPanel.Size = new System.Drawing.Size(781, 249);
            this.mainComponentsPanel.TabIndex = 13;
            // 
            // componentType
            // 
            this.componentType.AutoSize = true;
            this.componentType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.componentType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.componentType.ForeColor = System.Drawing.Color.DarkGray;
            this.componentType.Location = new System.Drawing.Point(6, 6);
            this.componentType.Margin = new System.Windows.Forms.Padding(0);
            this.componentType.Name = "componentType";
            this.componentType.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.componentType.Size = new System.Drawing.Size(88, 47);
            this.componentType.TabIndex = 2;
            this.componentType.Text = "Type";
            this.componentType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // indexLabel
            // 
            this.indexLabel.AutoSize = true;
            this.indexLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.indexLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indexLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.indexLabel.Location = new System.Drawing.Point(94, 6);
            this.indexLabel.Margin = new System.Windows.Forms.Padding(0);
            this.indexLabel.Name = "indexLabel";
            this.indexLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.indexLabel.Size = new System.Drawing.Size(62, 47);
            this.indexLabel.TabIndex = 22;
            this.indexLabel.Text = "Index";
            this.indexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // componentModel
            // 
            this.componentModel.AutoSize = true;
            this.componentModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.componentModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentModel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.componentModel.ForeColor = System.Drawing.Color.DarkGray;
            this.componentModel.Location = new System.Drawing.Point(156, 6);
            this.componentModel.Margin = new System.Windows.Forms.Padding(0);
            this.componentModel.Name = "componentModel";
            this.componentModel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.componentModel.Size = new System.Drawing.Size(619, 47);
            this.componentModel.TabIndex = 3;
            this.componentModel.Text = "Model / Info";
            this.componentModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // compCpuLabel
            // 
            this.compCpuLabel.AutoSize = true;
            this.compCpuLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.compCpuLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compCpuLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compCpuLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.compCpuLabel.Location = new System.Drawing.Point(6, 53);
            this.compCpuLabel.Margin = new System.Windows.Forms.Padding(0);
            this.compCpuLabel.Name = "compCpuLabel";
            this.compCpuLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.compCpuLabel.Size = new System.Drawing.Size(88, 37);
            this.compCpuLabel.TabIndex = 10;
            this.compCpuLabel.Text = "CPU:";
            this.compCpuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuIndexSelect
            // 
            this.cpuIndexSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cpuIndexSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuIndexSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cpuIndexSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuIndexSelect.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuIndexSelect.ForeColor = System.Drawing.Color.White;
            this.cpuIndexSelect.FormattingEnabled = true;
            this.cpuIndexSelect.IntegralHeight = false;
            this.cpuIndexSelect.ItemHeight = 19;
            this.cpuIndexSelect.Items.AddRange(new object[] {
            "0"});
            this.cpuIndexSelect.Location = new System.Drawing.Point(95, 58);
            this.cpuIndexSelect.Margin = new System.Windows.Forms.Padding(1);
            this.cpuIndexSelect.Name = "cpuIndexSelect";
            this.cpuIndexSelect.Size = new System.Drawing.Size(60, 27);
            this.cpuIndexSelect.TabIndex = 23;
            this.cpuIndexSelect.SelectedIndexChanged += new System.EventHandler(this.CpuIndexSelect_SelectedIndexChanged);
            // 
            // cpuModel
            // 
            this.cpuModel.AutoSize = true;
            this.cpuModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuModel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuModel.ForeColor = System.Drawing.Color.White;
            this.cpuModel.Location = new System.Drawing.Point(156, 53);
            this.cpuModel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuModel.Name = "cpuModel";
            this.cpuModel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuModel.Size = new System.Drawing.Size(619, 37);
            this.cpuModel.TabIndex = 20;
            this.cpuModel.Text = "N/A";
            this.cpuModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // compGpuLabel
            // 
            this.compGpuLabel.AutoSize = true;
            this.compGpuLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.compGpuLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compGpuLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compGpuLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.compGpuLabel.Location = new System.Drawing.Point(6, 90);
            this.compGpuLabel.Margin = new System.Windows.Forms.Padding(0);
            this.compGpuLabel.Name = "compGpuLabel";
            this.compGpuLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.compGpuLabel.Size = new System.Drawing.Size(88, 37);
            this.compGpuLabel.TabIndex = 11;
            this.compGpuLabel.Text = "GPU:";
            this.compGpuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuIndexSelect
            // 
            this.gpuIndexSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gpuIndexSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuIndexSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gpuIndexSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuIndexSelect.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuIndexSelect.ForeColor = System.Drawing.Color.White;
            this.gpuIndexSelect.FormattingEnabled = true;
            this.gpuIndexSelect.IntegralHeight = false;
            this.gpuIndexSelect.ItemHeight = 19;
            this.gpuIndexSelect.Items.AddRange(new object[] {
            "0"});
            this.gpuIndexSelect.Location = new System.Drawing.Point(95, 95);
            this.gpuIndexSelect.Margin = new System.Windows.Forms.Padding(1);
            this.gpuIndexSelect.Name = "gpuIndexSelect";
            this.gpuIndexSelect.Size = new System.Drawing.Size(60, 27);
            this.gpuIndexSelect.TabIndex = 24;
            this.gpuIndexSelect.SelectedIndexChanged += new System.EventHandler(this.GpuIndexSelect_SelectedIndexChanged);
            // 
            // gpuModel
            // 
            this.gpuModel.AutoSize = true;
            this.gpuModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuModel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuModel.ForeColor = System.Drawing.Color.White;
            this.gpuModel.Location = new System.Drawing.Point(156, 90);
            this.gpuModel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuModel.Name = "gpuModel";
            this.gpuModel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuModel.Size = new System.Drawing.Size(619, 37);
            this.gpuModel.TabIndex = 21;
            this.gpuModel.Text = "N/A";
            this.gpuModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // compRamLabel
            // 
            this.compRamLabel.AutoSize = true;
            this.compRamLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.compRamLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compRamLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compRamLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.compRamLabel.Location = new System.Drawing.Point(6, 127);
            this.compRamLabel.Margin = new System.Windows.Forms.Padding(0);
            this.compRamLabel.Name = "compRamLabel";
            this.compRamLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.compRamLabel.Size = new System.Drawing.Size(88, 37);
            this.compRamLabel.TabIndex = 12;
            this.compRamLabel.Text = "RAM:";
            this.compRamLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ramDetails
            // 
            this.ramDetails.AutoSize = true;
            this.ramDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ramDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ramDetails.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramDetails.ForeColor = System.Drawing.Color.White;
            this.ramDetails.Location = new System.Drawing.Point(156, 127);
            this.ramDetails.Margin = new System.Windows.Forms.Padding(0);
            this.ramDetails.Name = "ramDetails";
            this.ramDetails.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ramDetails.Size = new System.Drawing.Size(619, 37);
            this.ramDetails.TabIndex = 17;
            this.ramDetails.Text = "N/A";
            this.ramDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ramDetails.Resize += new System.EventHandler(this.ComboResize);
            // 
            // CompStorageLabel
            // 
            this.CompStorageLabel.AutoSize = true;
            this.CompStorageLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CompStorageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompStorageLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CompStorageLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.CompStorageLabel.Location = new System.Drawing.Point(6, 164);
            this.CompStorageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.CompStorageLabel.Name = "CompStorageLabel";
            this.CompStorageLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CompStorageLabel.Size = new System.Drawing.Size(88, 37);
            this.CompStorageLabel.TabIndex = 13;
            this.CompStorageLabel.Text = "Storage:";
            this.CompStorageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // storageIndexSelect
            // 
            this.storageIndexSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.storageIndexSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.storageIndexSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.storageIndexSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.storageIndexSelect.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.storageIndexSelect.ForeColor = System.Drawing.Color.White;
            this.storageIndexSelect.FormattingEnabled = true;
            this.storageIndexSelect.IntegralHeight = false;
            this.storageIndexSelect.ItemHeight = 19;
            this.storageIndexSelect.Items.AddRange(new object[] {
            "0"});
            this.storageIndexSelect.Location = new System.Drawing.Point(95, 169);
            this.storageIndexSelect.Margin = new System.Windows.Forms.Padding(1);
            this.storageIndexSelect.Name = "storageIndexSelect";
            this.storageIndexSelect.Size = new System.Drawing.Size(60, 27);
            this.storageIndexSelect.TabIndex = 25;
            this.storageIndexSelect.SelectedIndexChanged += new System.EventHandler(this.storageIndexSelect_SelectedIndexChanged);
            // 
            // storageDetails
            // 
            this.storageDetails.AutoSize = true;
            this.storageDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.storageDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storageDetails.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.storageDetails.ForeColor = System.Drawing.Color.White;
            this.storageDetails.Location = new System.Drawing.Point(156, 164);
            this.storageDetails.Margin = new System.Windows.Forms.Padding(0);
            this.storageDetails.Name = "storageDetails";
            this.storageDetails.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.storageDetails.Size = new System.Drawing.Size(619, 37);
            this.storageDetails.TabIndex = 18;
            this.storageDetails.Text = "N/A";
            this.storageDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CompMotherboardLabel
            // 
            this.CompMotherboardLabel.AutoSize = true;
            this.CompMotherboardLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CompMotherboardLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompMotherboardLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CompMotherboardLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.CompMotherboardLabel.Location = new System.Drawing.Point(6, 201);
            this.CompMotherboardLabel.Margin = new System.Windows.Forms.Padding(0);
            this.CompMotherboardLabel.Name = "CompMotherboardLabel";
            this.CompMotherboardLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CompMotherboardLabel.Size = new System.Drawing.Size(88, 42);
            this.CompMotherboardLabel.TabIndex = 14;
            this.CompMotherboardLabel.Text = "MBO:";
            this.CompMotherboardLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // motherboardDetails
            // 
            this.motherboardDetails.AutoSize = true;
            this.motherboardDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.motherboardDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motherboardDetails.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.motherboardDetails.ForeColor = System.Drawing.Color.White;
            this.motherboardDetails.Location = new System.Drawing.Point(156, 201);
            this.motherboardDetails.Margin = new System.Windows.Forms.Padding(0);
            this.motherboardDetails.Name = "motherboardDetails";
            this.motherboardDetails.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.motherboardDetails.Size = new System.Drawing.Size(619, 42);
            this.motherboardDetails.TabIndex = 19;
            this.motherboardDetails.Text = "N/A";
            this.motherboardDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // placeholderLabel1
            // 
            this.placeholderLabel1.AutoSize = true;
            this.placeholderLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.placeholderLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.placeholderLabel1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.placeholderLabel1.ForeColor = System.Drawing.Color.White;
            this.placeholderLabel1.Location = new System.Drawing.Point(94, 201);
            this.placeholderLabel1.Margin = new System.Windows.Forms.Padding(0);
            this.placeholderLabel1.Name = "placeholderLabel1";
            this.placeholderLabel1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.placeholderLabel1.Size = new System.Drawing.Size(62, 42);
            this.placeholderLabel1.TabIndex = 27;
            this.placeholderLabel1.Text = "-";
            this.placeholderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // placeholderLabel2
            // 
            this.placeholderLabel2.AutoSize = true;
            this.placeholderLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.placeholderLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.placeholderLabel2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.placeholderLabel2.ForeColor = System.Drawing.Color.White;
            this.placeholderLabel2.Location = new System.Drawing.Point(94, 127);
            this.placeholderLabel2.Margin = new System.Windows.Forms.Padding(0);
            this.placeholderLabel2.Name = "placeholderLabel2";
            this.placeholderLabel2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.placeholderLabel2.Size = new System.Drawing.Size(62, 37);
            this.placeholderLabel2.TabIndex = 26;
            this.placeholderLabel2.Text = "-";
            this.placeholderLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tempsWrapper
            // 
            this.tempsWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tempsWrapper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tempsWrapper.ColumnCount = 3;
            this.tempsWrapper.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.5F));
            this.tempsWrapper.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tempsWrapper.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.5F));
            this.tempsWrapper.Controls.Add(this.gpuPanel, 0, 0);
            this.tempsWrapper.Controls.Add(this.cpuPanel, 2, 0);
            this.tempsWrapper.Location = new System.Drawing.Point(38, 156);
            this.tempsWrapper.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tempsWrapper.Name = "tempsWrapper";
            this.tempsWrapper.RowCount = 1;
            this.tempsWrapper.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tempsWrapper.Size = new System.Drawing.Size(789, 188);
            this.tempsWrapper.TabIndex = 12;
            // 
            // gpuPanel
            // 
            this.gpuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuPanel.ColumnCount = 3;
            this.gpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.gpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.gpuPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.gpuPanel.Controls.Add(this.gpuTempLabel, 0, 0);
            this.gpuPanel.Controls.Add(this.gpuName, 0, 1);
            this.gpuPanel.Controls.Add(this.gpuTempCurLabel, 0, 2);
            this.gpuPanel.Controls.Add(this.gpuTempMinLabel, 1, 2);
            this.gpuPanel.Controls.Add(this.gpuTempMaxLabel, 2, 2);
            this.gpuPanel.Controls.Add(this.gpuTempCur, 0, 3);
            this.gpuPanel.Controls.Add(this.gpuTempMin, 1, 3);
            this.gpuPanel.Controls.Add(this.gpuTempMax, 2, 3);
            this.gpuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuPanel.Location = new System.Drawing.Point(4, 4);
            this.gpuPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuPanel.Name = "gpuPanel";
            this.gpuPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gpuPanel.RowCount = 4;
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.5F));
            this.gpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.5F));
            this.gpuPanel.Size = new System.Drawing.Size(374, 180);
            this.gpuPanel.TabIndex = 10;
            // 
            // gpuTempLabel
            // 
            this.gpuTempLabel.AutoSize = true;
            this.gpuTempLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuPanel.SetColumnSpan(this.gpuTempLabel, 3);
            this.gpuTempLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempLabel.Location = new System.Drawing.Point(6, 6);
            this.gpuTempLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempLabel.Name = "gpuTempLabel";
            this.gpuTempLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempLabel.Size = new System.Drawing.Size(362, 33);
            this.gpuTempLabel.TabIndex = 0;
            this.gpuTempLabel.Text = "Graphics card (GPU)";
            this.gpuTempLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuName
            // 
            this.gpuName.AutoSize = true;
            this.gpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuPanel.SetColumnSpan(this.gpuName, 3);
            this.gpuName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuName.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuName.ForeColor = System.Drawing.Color.White;
            this.gpuName.Location = new System.Drawing.Point(6, 39);
            this.gpuName.Margin = new System.Windows.Forms.Padding(0);
            this.gpuName.Name = "gpuName";
            this.gpuName.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gpuName.Size = new System.Drawing.Size(362, 58);
            this.gpuName.TabIndex = 9;
            this.gpuName.Text = "N/A";
            this.gpuName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempCurLabel
            // 
            this.gpuTempCurLabel.AutoSize = true;
            this.gpuTempCurLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempCurLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempCurLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempCurLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempCurLabel.Location = new System.Drawing.Point(6, 97);
            this.gpuTempCurLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempCurLabel.Name = "gpuTempCurLabel";
            this.gpuTempCurLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempCurLabel.Size = new System.Drawing.Size(120, 29);
            this.gpuTempCurLabel.TabIndex = 1;
            this.gpuTempCurLabel.Text = "Current:";
            this.gpuTempCurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMinLabel
            // 
            this.gpuTempMinLabel.AutoSize = true;
            this.gpuTempMinLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMinLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMinLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMinLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempMinLabel.Location = new System.Drawing.Point(126, 97);
            this.gpuTempMinLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMinLabel.Name = "gpuTempMinLabel";
            this.gpuTempMinLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempMinLabel.Size = new System.Drawing.Size(120, 29);
            this.gpuTempMinLabel.TabIndex = 2;
            this.gpuTempMinLabel.Text = "Minimum:";
            this.gpuTempMinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMaxLabel
            // 
            this.gpuTempMaxLabel.AutoSize = true;
            this.gpuTempMaxLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMaxLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMaxLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMaxLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.gpuTempMaxLabel.Location = new System.Drawing.Point(246, 97);
            this.gpuTempMaxLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMaxLabel.Name = "gpuTempMaxLabel";
            this.gpuTempMaxLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempMaxLabel.Size = new System.Drawing.Size(122, 29);
            this.gpuTempMaxLabel.TabIndex = 3;
            this.gpuTempMaxLabel.Text = "Maximum:";
            this.gpuTempMaxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempCur
            // 
            this.gpuTempCur.AutoSize = true;
            this.gpuTempCur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempCur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempCur.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempCur.ForeColor = System.Drawing.Color.White;
            this.gpuTempCur.Location = new System.Drawing.Point(6, 126);
            this.gpuTempCur.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempCur.Name = "gpuTempCur";
            this.gpuTempCur.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempCur.Size = new System.Drawing.Size(120, 48);
            this.gpuTempCur.TabIndex = 4;
            this.gpuTempCur.Text = "N/A";
            this.gpuTempCur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMin
            // 
            this.gpuTempMin.AutoSize = true;
            this.gpuTempMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMin.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMin.ForeColor = System.Drawing.Color.LimeGreen;
            this.gpuTempMin.Location = new System.Drawing.Point(126, 126);
            this.gpuTempMin.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMin.Name = "gpuTempMin";
            this.gpuTempMin.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempMin.Size = new System.Drawing.Size(120, 48);
            this.gpuTempMin.TabIndex = 5;
            this.gpuTempMin.Text = "N/A";
            this.gpuTempMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpuTempMax
            // 
            this.gpuTempMax.AutoSize = true;
            this.gpuTempMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuTempMax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuTempMax.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuTempMax.ForeColor = System.Drawing.Color.Red;
            this.gpuTempMax.Location = new System.Drawing.Point(246, 126);
            this.gpuTempMax.Margin = new System.Windows.Forms.Padding(0);
            this.gpuTempMax.Name = "gpuTempMax";
            this.gpuTempMax.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuTempMax.Size = new System.Drawing.Size(122, 48);
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
            this.cpuPanel.Controls.Add(this.cpuTempLabel, 0, 0);
            this.cpuPanel.Controls.Add(this.cpuName, 0, 1);
            this.cpuPanel.Controls.Add(this.cpuTempCurLabel, 0, 2);
            this.cpuPanel.Controls.Add(this.cpuTempMinLabel, 1, 2);
            this.cpuPanel.Controls.Add(this.cpuTempMaxLabel, 2, 2);
            this.cpuPanel.Controls.Add(this.cpuTempCur, 0, 3);
            this.cpuPanel.Controls.Add(this.cpuTempMin, 1, 3);
            this.cpuPanel.Controls.Add(this.cpuTempMax, 2, 3);
            this.cpuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuPanel.Location = new System.Drawing.Point(409, 4);
            this.cpuPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuPanel.Name = "cpuPanel";
            this.cpuPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cpuPanel.RowCount = 4;
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.5F));
            this.cpuPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.5F));
            this.cpuPanel.Size = new System.Drawing.Size(376, 180);
            this.cpuPanel.TabIndex = 11;
            // 
            // cpuTempLabel
            // 
            this.cpuTempLabel.AutoSize = true;
            this.cpuTempLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuPanel.SetColumnSpan(this.cpuTempLabel, 3);
            this.cpuTempLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempLabel.Location = new System.Drawing.Point(6, 6);
            this.cpuTempLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempLabel.Name = "cpuTempLabel";
            this.cpuTempLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempLabel.Size = new System.Drawing.Size(364, 33);
            this.cpuTempLabel.TabIndex = 1;
            this.cpuTempLabel.Text = "Processor (CPU)";
            this.cpuTempLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuName
            // 
            this.cpuName.AutoSize = true;
            this.cpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuPanel.SetColumnSpan(this.cpuName, 3);
            this.cpuName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuName.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuName.ForeColor = System.Drawing.Color.White;
            this.cpuName.Location = new System.Drawing.Point(6, 39);
            this.cpuName.Margin = new System.Windows.Forms.Padding(0);
            this.cpuName.Name = "cpuName";
            this.cpuName.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cpuName.Size = new System.Drawing.Size(364, 58);
            this.cpuName.TabIndex = 8;
            this.cpuName.Text = "N/A";
            this.cpuName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempCurLabel
            // 
            this.cpuTempCurLabel.AutoSize = true;
            this.cpuTempCurLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempCurLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempCurLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempCurLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempCurLabel.Location = new System.Drawing.Point(6, 97);
            this.cpuTempCurLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempCurLabel.Name = "cpuTempCurLabel";
            this.cpuTempCurLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempCurLabel.Size = new System.Drawing.Size(121, 29);
            this.cpuTempCurLabel.TabIndex = 2;
            this.cpuTempCurLabel.Text = "Current:";
            this.cpuTempCurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMinLabel
            // 
            this.cpuTempMinLabel.AutoSize = true;
            this.cpuTempMinLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMinLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMinLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMinLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempMinLabel.Location = new System.Drawing.Point(127, 97);
            this.cpuTempMinLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMinLabel.Name = "cpuTempMinLabel";
            this.cpuTempMinLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempMinLabel.Size = new System.Drawing.Size(121, 29);
            this.cpuTempMinLabel.TabIndex = 3;
            this.cpuTempMinLabel.Text = "Minimum:";
            this.cpuTempMinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMaxLabel
            // 
            this.cpuTempMaxLabel.AutoSize = true;
            this.cpuTempMaxLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMaxLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMaxLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMaxLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.cpuTempMaxLabel.Location = new System.Drawing.Point(248, 97);
            this.cpuTempMaxLabel.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMaxLabel.Name = "cpuTempMaxLabel";
            this.cpuTempMaxLabel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempMaxLabel.Size = new System.Drawing.Size(122, 29);
            this.cpuTempMaxLabel.TabIndex = 4;
            this.cpuTempMaxLabel.Text = "Maximum:";
            this.cpuTempMaxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempCur
            // 
            this.cpuTempCur.AutoSize = true;
            this.cpuTempCur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempCur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempCur.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempCur.ForeColor = System.Drawing.Color.White;
            this.cpuTempCur.Location = new System.Drawing.Point(6, 126);
            this.cpuTempCur.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempCur.Name = "cpuTempCur";
            this.cpuTempCur.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempCur.Size = new System.Drawing.Size(121, 48);
            this.cpuTempCur.TabIndex = 5;
            this.cpuTempCur.Text = "N/A";
            this.cpuTempCur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMin
            // 
            this.cpuTempMin.AutoSize = true;
            this.cpuTempMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMin.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMin.ForeColor = System.Drawing.Color.LimeGreen;
            this.cpuTempMin.Location = new System.Drawing.Point(127, 126);
            this.cpuTempMin.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMin.Name = "cpuTempMin";
            this.cpuTempMin.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempMin.Size = new System.Drawing.Size(121, 48);
            this.cpuTempMin.TabIndex = 6;
            this.cpuTempMin.Text = "N/A";
            this.cpuTempMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpuTempMax
            // 
            this.cpuTempMax.AutoSize = true;
            this.cpuTempMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuTempMax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuTempMax.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuTempMax.ForeColor = System.Drawing.Color.Red;
            this.cpuTempMax.Location = new System.Drawing.Point(248, 126);
            this.cpuTempMax.Margin = new System.Windows.Forms.Padding(0);
            this.cpuTempMax.Name = "cpuTempMax";
            this.cpuTempMax.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuTempMax.Size = new System.Drawing.Size(122, 48);
            this.cpuTempMax.TabIndex = 7;
            this.cpuTempMax.Text = "N/A";
            this.cpuTempMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sysmonTitle
            // 
            this.sysmonTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sysmonTitle.ForeColor = System.Drawing.Color.White;
            this.sysmonTitle.Location = new System.Drawing.Point(41, 35);
            this.sysmonTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sysmonTitle.Name = "sysmonTitle";
            this.sysmonTitle.Size = new System.Drawing.Size(389, 38);
            this.sysmonTitle.TabIndex = 4;
            this.sysmonTitle.Text = "🖥️ System Monitoring";
            // 
            // divider2
            // 
            this.divider2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.divider2.Location = new System.Drawing.Point(41, 95);
            this.divider2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.divider2.Name = "divider2";
            this.divider2.Size = new System.Drawing.Size(781, 1);
            this.divider2.TabIndex = 5;
            this.divider2.TabStop = false;
            // 
            // tempTitle
            // 
            this.tempTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempTitle.Location = new System.Drawing.Point(41, 110);
            this.tempTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tempTitle.Name = "tempTitle";
            this.tempTitle.Size = new System.Drawing.Size(389, 38);
            this.tempTitle.TabIndex = 9;
            this.tempTitle.Text = "🌡 Temperatures";
            // 
            // settingsPage
            // 
            this.settingsPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.settingsPage.Controls.Add(this.settingsTitle);
            this.settingsPage.Controls.Add(this.genSettings);
            this.settingsPage.Controls.Add(this.generalSettingsLabel);
            this.settingsPage.Controls.Add(this.traySettingsLabel);
            this.settingsPage.Controls.Add(this.traySettingsPanel);
            this.settingsPage.Controls.Add(this.divider3);
            this.settingsPage.ForeColor = System.Drawing.Color.White;
            this.settingsPage.Location = new System.Drawing.Point(4, 5);
            this.settingsPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.settingsPage.Name = "settingsPage";
            this.settingsPage.Size = new System.Drawing.Size(864, 695);
            this.settingsPage.TabIndex = 1;
            this.settingsPage.Text = "Settings";
            // 
            // settingsTitle
            // 
            this.settingsTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsTitle.ForeColor = System.Drawing.Color.White;
            this.settingsTitle.Location = new System.Drawing.Point(41, 35);
            this.settingsTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.settingsTitle.Name = "settingsTitle";
            this.settingsTitle.Size = new System.Drawing.Size(389, 38);
            this.settingsTitle.TabIndex = 4;
            this.settingsTitle.Text = "⚙ Application Settings";
            // 
            // genSettings
            // 
            this.genSettings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.genSettings.Location = new System.Drawing.Point(41, 110);
            this.genSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.genSettings.Name = "genSettings";
            this.genSettings.Size = new System.Drawing.Size(389, 38);
            this.genSettings.TabIndex = 6;
            this.genSettings.Text = "🌐 General Settings";
            // 
            // generalSettingsLabel
            // 
            this.generalSettingsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generalSettingsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.generalSettingsLabel.ColumnCount = 1;
            this.generalSettingsLabel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.generalSettingsLabel.Controls.Add(this.autostartInstall, 0, 0);
            this.generalSettingsLabel.Controls.Add(this.tempsFahrenheit, 0, 1);
            this.generalSettingsLabel.Controls.Add(this.singleIconTray, 0, 2);
            this.generalSettingsLabel.Controls.Add(this.refreshPanel, 0, 3);
            this.generalSettingsLabel.Location = new System.Drawing.Point(41, 161);
            this.generalSettingsLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.generalSettingsLabel.Name = "generalSettingsLabel";
            this.generalSettingsLabel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.generalSettingsLabel.RowCount = 4;
            this.generalSettingsLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.generalSettingsLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.generalSettingsLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.generalSettingsLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.generalSettingsLabel.Size = new System.Drawing.Size(781, 209);
            this.generalSettingsLabel.TabIndex = 7;
            // 
            // autostartInstall
            // 
            this.autostartInstall.AutoSize = true;
            this.autostartInstall.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autostartInstall.Dock = System.Windows.Forms.DockStyle.Left;
            this.autostartInstall.Location = new System.Drawing.Point(10, 10);
            this.autostartInstall.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.autostartInstall.Name = "autostartInstall";
            this.autostartInstall.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.autostartInstall.Size = new System.Drawing.Size(306, 41);
            this.autostartInstall.TabIndex = 0;
            this.autostartInstall.Text = "Autostart at windows boot (Install)";
            this.autostartInstall.UseVisualStyleBackColor = true;
            this.autostartInstall.CheckedChanged += new System.EventHandler(this.AutostartInstall_CheckedChanged);
            // 
            // tempsFahrenheit
            // 
            this.tempsFahrenheit.AutoSize = true;
            this.tempsFahrenheit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tempsFahrenheit.Dock = System.Windows.Forms.DockStyle.Left;
            this.tempsFahrenheit.Location = new System.Drawing.Point(10, 59);
            this.tempsFahrenheit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tempsFahrenheit.Name = "tempsFahrenheit";
            this.tempsFahrenheit.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tempsFahrenheit.Size = new System.Drawing.Size(325, 41);
            this.tempsFahrenheit.TabIndex = 1;
            this.tempsFahrenheit.Text = "Show temperatures in Fahrenheit (°F)";
            this.tempsFahrenheit.UseVisualStyleBackColor = true;
            // 
            // singleIconTray
            // 
            this.singleIconTray.AutoSize = true;
            this.singleIconTray.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singleIconTray.Dock = System.Windows.Forms.DockStyle.Left;
            this.singleIconTray.Location = new System.Drawing.Point(10, 108);
            this.singleIconTray.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.singleIconTray.Name = "singleIconTray";
            this.singleIconTray.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.singleIconTray.Size = new System.Drawing.Size(256, 41);
            this.singleIconTray.TabIndex = 2;
            this.singleIconTray.Text = "Enable tray single-icon style";
            this.singleIconTray.UseVisualStyleBackColor = true;
            this.singleIconTray.CheckedChanged += new System.EventHandler(this.singleIconTray_CheckedChanged);
            // 
            // refreshPanel
            // 
            this.refreshPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshPanel.ColumnCount = 2;
            this.refreshPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 188F));
            this.refreshPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.refreshPanel.Controls.Add(this.refreshLabel, 0, 0);
            this.refreshPanel.Controls.Add(this.refreshValue, 1, 0);
            this.refreshPanel.Location = new System.Drawing.Point(10, 157);
            this.refreshPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.refreshPanel.Name = "refreshPanel";
            this.refreshPanel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.refreshPanel.RowCount = 1;
            this.refreshPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.refreshPanel.Size = new System.Drawing.Size(314, 42);
            this.refreshPanel.TabIndex = 22;
            // 
            // refreshLabel
            // 
            this.refreshLabel.AutoSize = true;
            this.refreshLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshLabel.Location = new System.Drawing.Point(8, 4);
            this.refreshLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.refreshLabel.Name = "refreshLabel";
            this.refreshLabel.Size = new System.Drawing.Size(180, 34);
            this.refreshLabel.TabIndex = 1;
            this.refreshLabel.Text = "Update interval (s):";
            this.refreshLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // refreshValue
            // 
            this.refreshValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.refreshValue.DecimalPlaces = 2;
            this.refreshValue.ForeColor = System.Drawing.Color.White;
            this.refreshValue.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.refreshValue.Location = new System.Drawing.Point(196, 8);
            this.refreshValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.refreshValue.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.refreshValue.Minimum = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.refreshValue.Name = "refreshValue";
            this.refreshValue.Size = new System.Drawing.Size(110, 29);
            this.refreshValue.TabIndex = 3;
            this.refreshValue.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            this.refreshValue.ValueChanged += new System.EventHandler(this.RefreshValue_ValueChanged);
            // 
            // traySettingsLabel
            // 
            this.traySettingsLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traySettingsLabel.Location = new System.Drawing.Point(41, 389);
            this.traySettingsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.traySettingsLabel.Name = "traySettingsLabel";
            this.traySettingsLabel.Size = new System.Drawing.Size(389, 38);
            this.traySettingsLabel.TabIndex = 8;
            this.traySettingsLabel.Text = "🔧 Tray Settings";
            // 
            // traySettingsPanel
            // 
            this.traySettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traySettingsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.traySettingsPanel.ColumnCount = 2;
            this.traySettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.traySettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.traySettingsPanel.Controls.Add(this.enableCpuTray, 0, 0);
            this.traySettingsPanel.Controls.Add(this.enableGpuTray, 0, 1);
            this.traySettingsPanel.Controls.Add(this.fontFamilyPanel, 0, 2);
            this.traySettingsPanel.Controls.Add(this.cpuColorPanel, 1, 0);
            this.traySettingsPanel.Controls.Add(this.gpuColorPanel, 1, 1);
            this.traySettingsPanel.Controls.Add(this.iconsizePanel, 1, 2);
            this.traySettingsPanel.Location = new System.Drawing.Point(41, 435);
            this.traySettingsPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.traySettingsPanel.Name = "traySettingsPanel";
            this.traySettingsPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.traySettingsPanel.RowCount = 3;
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33335F));
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.traySettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.traySettingsPanel.Size = new System.Drawing.Size(781, 219);
            this.traySettingsPanel.TabIndex = 9;
            // 
            // enableCpuTray
            // 
            this.enableCpuTray.AutoSize = true;
            this.enableCpuTray.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.enableCpuTray.Dock = System.Windows.Forms.DockStyle.Left;
            this.enableCpuTray.Location = new System.Drawing.Point(10, 10);
            this.enableCpuTray.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.enableCpuTray.Name = "enableCpuTray";
            this.enableCpuTray.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.enableCpuTray.Size = new System.Drawing.Size(206, 61);
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
            this.enableGpuTray.Location = new System.Drawing.Point(10, 79);
            this.enableGpuTray.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.enableGpuTray.Name = "enableGpuTray";
            this.enableGpuTray.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.enableGpuTray.Size = new System.Drawing.Size(207, 60);
            this.enableGpuTray.TabIndex = 12;
            this.enableGpuTray.Text = "Enable GPU Tray icon";
            this.enableGpuTray.UseVisualStyleBackColor = true;
            this.enableGpuTray.CheckedChanged += new System.EventHandler(this.Setting_CheckedChanged);
            // 
            // fontFamilyPanel
            // 
            this.fontFamilyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fontFamilyPanel.ColumnCount = 2;
            this.fontFamilyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.fontFamilyPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fontFamilyPanel.Controls.Add(this.fontFamilyLabel, 0, 0);
            this.fontFamilyPanel.Controls.Add(this.fontFamilyValue, 1, 0);
            this.fontFamilyPanel.Location = new System.Drawing.Point(10, 147);
            this.fontFamilyPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fontFamilyPanel.Name = "fontFamilyPanel";
            this.fontFamilyPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.fontFamilyPanel.RowCount = 1;
            this.fontFamilyPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fontFamilyPanel.Size = new System.Drawing.Size(314, 62);
            this.fontFamilyPanel.TabIndex = 13;
            // 
            // fontFamilyLabel
            // 
            this.fontFamilyLabel.AutoSize = true;
            this.fontFamilyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fontFamilyLabel.Location = new System.Drawing.Point(10, 6);
            this.fontFamilyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fontFamilyLabel.Name = "fontFamilyLabel";
            this.fontFamilyLabel.Size = new System.Drawing.Size(112, 50);
            this.fontFamilyLabel.TabIndex = 0;
            this.fontFamilyLabel.Text = "Font family:";
            this.fontFamilyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fontFamilyValue
            // 
            this.fontFamilyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.fontFamilyValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.fontFamilyValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontFamilyValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontFamilyValue.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontFamilyValue.ForeColor = System.Drawing.Color.White;
            this.fontFamilyValue.FormattingEnabled = true;
            this.fontFamilyValue.IntegralHeight = false;
            this.fontFamilyValue.ItemHeight = 19;
            this.fontFamilyValue.Items.AddRange(new object[] {
            "Consolas",
            "Cascadia Mono",
            "Lucida Console",
            "Lucida Sans Unicode",
            "Courier New",
            "Segoe UI Variable",
            "Segoe UI Semibold",
            "Segoe UI",
            "Bahnschrift",
            "Tahoma",
            "Verdana",
            "Calibri",
            "Arial",
            "Arial Narrow",
            "Cambria",
            "Candara",
            "Corbel",
            "Constantia",
            "Times New Roman",
            "Georgia",
            "Sitka Text"});
            this.fontFamilyValue.Location = new System.Drawing.Point(127, 17);
            this.fontFamilyValue.Margin = new System.Windows.Forms.Padding(1);
            this.fontFamilyValue.Name = "fontFamilyValue";
            this.fontFamilyValue.Size = new System.Drawing.Size(180, 27);
            this.fontFamilyValue.TabIndex = 24;
            this.fontFamilyValue.SelectedIndexChanged += new System.EventHandler(this.Setting_SelectedIndexChanged);
            // 
            // cpuColorPanel
            // 
            this.cpuColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cpuColorPanel.ColumnCount = 2;
            this.cpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.cpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cpuColorPanel.Controls.Add(this.cpuColorLabel, 0, 0);
            this.cpuColorPanel.Controls.Add(this.cpuColorValue, 1, 0);
            this.cpuColorPanel.Location = new System.Drawing.Point(394, 10);
            this.cpuColorPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cpuColorPanel.Name = "cpuColorPanel";
            this.cpuColorPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cpuColorPanel.RowCount = 1;
            this.cpuColorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cpuColorPanel.Size = new System.Drawing.Size(314, 61);
            this.cpuColorPanel.TabIndex = 16;
            // 
            // cpuColorLabel
            // 
            this.cpuColorLabel.AutoSize = true;
            this.cpuColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpuColorLabel.Location = new System.Drawing.Point(10, 6);
            this.cpuColorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cpuColorLabel.Name = "cpuColorLabel";
            this.cpuColorLabel.Size = new System.Drawing.Size(112, 49);
            this.cpuColorLabel.TabIndex = 0;
            this.cpuColorLabel.Text = "CPU Color:";
            this.cpuColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cpuColorValue
            // 
            this.cpuColorValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cpuColorValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuColorValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cpuColorValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cpuColorValue.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuColorValue.ForeColor = System.Drawing.Color.White;
            this.cpuColorValue.FormattingEnabled = true;
            this.cpuColorValue.IntegralHeight = false;
            this.cpuColorValue.ItemHeight = 19;
            this.cpuColorValue.Items.AddRange(new object[] {
            "DodgerBlue",
            "DeepSkyBlue",
            "Cyan",
            "Turquoise",
            "Aqua",
            "LimeGreen",
            "SpringGreen",
            "MediumSeaGreen",
            "SeaGreen",
            "Yellow",
            "Gold",
            "Khaki",
            "Orange",
            "DarkOrange",
            "OrangeRed",
            "Coral",
            "Red",
            "Tomato",
            "Magenta",
            "HotPink",
            "DeepPink",
            "Violet",
            "Plum",
            "Orchid"});
            this.cpuColorValue.Location = new System.Drawing.Point(127, 17);
            this.cpuColorValue.Margin = new System.Windows.Forms.Padding(1);
            this.cpuColorValue.Name = "cpuColorValue";
            this.cpuColorValue.Size = new System.Drawing.Size(180, 27);
            this.cpuColorValue.TabIndex = 24;
            this.cpuColorValue.SelectedIndexChanged += new System.EventHandler(this.Setting_SelectedIndexChanged);
            // 
            // gpuColorPanel
            // 
            this.gpuColorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gpuColorPanel.ColumnCount = 2;
            this.gpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.gpuColorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gpuColorPanel.Controls.Add(this.gpuColorLabel, 0, 0);
            this.gpuColorPanel.Controls.Add(this.gpuColorValue, 1, 0);
            this.gpuColorPanel.Location = new System.Drawing.Point(394, 79);
            this.gpuColorPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpuColorPanel.Name = "gpuColorPanel";
            this.gpuColorPanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gpuColorPanel.RowCount = 1;
            this.gpuColorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gpuColorPanel.Size = new System.Drawing.Size(314, 60);
            this.gpuColorPanel.TabIndex = 17;
            // 
            // gpuColorLabel
            // 
            this.gpuColorLabel.AutoSize = true;
            this.gpuColorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpuColorLabel.Location = new System.Drawing.Point(10, 6);
            this.gpuColorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.gpuColorLabel.Name = "gpuColorLabel";
            this.gpuColorLabel.Size = new System.Drawing.Size(112, 48);
            this.gpuColorLabel.TabIndex = 0;
            this.gpuColorLabel.Text = "GPU Color:";
            this.gpuColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpuColorValue
            // 
            this.gpuColorValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gpuColorValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuColorValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gpuColorValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gpuColorValue.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuColorValue.ForeColor = System.Drawing.Color.White;
            this.gpuColorValue.FormattingEnabled = true;
            this.gpuColorValue.IntegralHeight = false;
            this.gpuColorValue.ItemHeight = 19;
            this.gpuColorValue.Items.AddRange(new object[] {
            "DodgerBlue",
            "DeepSkyBlue",
            "Cyan",
            "Turquoise",
            "Aqua",
            "LimeGreen",
            "SpringGreen",
            "MediumSeaGreen",
            "SeaGreen",
            "Yellow",
            "Gold",
            "Khaki",
            "Orange",
            "DarkOrange",
            "OrangeRed",
            "Coral",
            "Red",
            "Tomato",
            "Magenta",
            "HotPink",
            "DeepPink",
            "Violet",
            "Plum",
            "Orchid"});
            this.gpuColorValue.Location = new System.Drawing.Point(127, 16);
            this.gpuColorValue.Margin = new System.Windows.Forms.Padding(1);
            this.gpuColorValue.Name = "gpuColorValue";
            this.gpuColorValue.Size = new System.Drawing.Size(180, 27);
            this.gpuColorValue.TabIndex = 24;
            this.gpuColorValue.SelectedIndexChanged += new System.EventHandler(this.Setting_SelectedIndexChanged);
            // 
            // iconsizePanel
            // 
            this.iconsizePanel.ColumnCount = 2;
            this.iconsizePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.iconsizePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.iconsizePanel.Controls.Add(this.iconsizeLabel, 0, 0);
            this.iconsizePanel.Controls.Add(this.iconsizeValue, 1, 0);
            this.iconsizePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.iconsizePanel.Location = new System.Drawing.Point(394, 147);
            this.iconsizePanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.iconsizePanel.Name = "iconsizePanel";
            this.iconsizePanel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.iconsizePanel.RowCount = 1;
            this.iconsizePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.iconsizePanel.Size = new System.Drawing.Size(244, 62);
            this.iconsizePanel.TabIndex = 21;
            // 
            // iconsizeLabel
            // 
            this.iconsizeLabel.AutoSize = true;
            this.iconsizeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconsizeLabel.Location = new System.Drawing.Point(10, 6);
            this.iconsizeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.iconsizeLabel.Name = "iconsizeLabel";
            this.iconsizeLabel.Size = new System.Drawing.Size(112, 50);
            this.iconsizeLabel.TabIndex = 1;
            this.iconsizeLabel.Text = "Icon size (%):";
            this.iconsizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // iconsizeValue
            // 
            this.iconsizeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.iconsizeValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.iconsizeValue.ForeColor = System.Drawing.Color.White;
            this.iconsizeValue.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.iconsizeValue.Location = new System.Drawing.Point(130, 16);
            this.iconsizeValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.iconsizeValue.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.iconsizeValue.Name = "iconsizeValue";
            this.iconsizeValue.Size = new System.Drawing.Size(104, 29);
            this.iconsizeValue.TabIndex = 2;
            this.iconsizeValue.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            this.iconsizeValue.ValueChanged += new System.EventHandler(this.IconsizeValue_ValueChanged);
            // 
            // divider3
            // 
            this.divider3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.divider3.Location = new System.Drawing.Point(41, 95);
            this.divider3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.divider3.Name = "divider3";
            this.divider3.Size = new System.Drawing.Size(781, 1);
            this.divider3.TabIndex = 5;
            this.divider3.TabStop = false;
            // 
            // aboutPage
            // 
            this.aboutPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.aboutPage.Controls.Add(this.aboutTitle);
            this.aboutPage.Controls.Add(this.divider1);
            this.aboutPage.Controls.Add(this.appTitleAbout);
            this.aboutPage.Controls.Add(this.appAboutExtra);
            this.aboutPage.Controls.Add(this.appVersion);
            this.aboutPage.Controls.Add(this.githubLink);
            this.aboutPage.Controls.Add(this.donatePic);
            this.aboutPage.ForeColor = System.Drawing.Color.White;
            this.aboutPage.Location = new System.Drawing.Point(4, 5);
            this.aboutPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.aboutPage.Name = "aboutPage";
            this.aboutPage.Size = new System.Drawing.Size(864, 695);
            this.aboutPage.TabIndex = 2;
            this.aboutPage.Text = "About";
            // 
            // aboutTitle
            // 
            this.aboutTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutTitle.ForeColor = System.Drawing.Color.White;
            this.aboutTitle.Location = new System.Drawing.Point(41, 35);
            this.aboutTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.aboutTitle.Name = "aboutTitle";
            this.aboutTitle.Size = new System.Drawing.Size(389, 38);
            this.aboutTitle.TabIndex = 1;
            this.aboutTitle.Text = "❓ About";
            // 
            // divider1
            // 
            this.divider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.divider1.Location = new System.Drawing.Point(41, 95);
            this.divider1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(781, 1);
            this.divider1.TabIndex = 2;
            this.divider1.TabStop = false;
            // 
            // appTitleAbout
            // 
            this.appTitleAbout.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appTitleAbout.Location = new System.Drawing.Point(41, 110);
            this.appTitleAbout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.appTitleAbout.Name = "appTitleAbout";
            this.appTitleAbout.Size = new System.Drawing.Size(389, 38);
            this.appTitleAbout.TabIndex = 3;
            this.appTitleAbout.Text = "Tray Temps";
            // 
            // appAboutExtra
            // 
            this.appAboutExtra.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appAboutExtra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.appAboutExtra.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appAboutExtra.ForeColor = System.Drawing.Color.DarkGray;
            this.appAboutExtra.Location = new System.Drawing.Point(41, 158);
            this.appAboutExtra.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.appAboutExtra.Name = "appAboutExtra";
            this.appAboutExtra.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.appAboutExtra.Size = new System.Drawing.Size(781, 292);
            this.appAboutExtra.TabIndex = 4;
            this.appAboutExtra.Text = resources.GetString("appAboutExtra.Text");
            this.appAboutExtra.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // appVersion
            // 
            this.appVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.appVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appVersion.ForeColor = System.Drawing.Color.DimGray;
            this.appVersion.Location = new System.Drawing.Point(42, 622);
            this.appVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.appVersion.Name = "appVersion";
            this.appVersion.Size = new System.Drawing.Size(388, 30);
            this.appVersion.TabIndex = 5;
            this.appVersion.Text = "Version: 0.0.0.0";
            // 
            // githubLink
            // 
            this.githubLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.githubLink.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.githubLink.ForeColor = System.Drawing.Color.SeaGreen;
            this.githubLink.Location = new System.Drawing.Point(41, 471);
            this.githubLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.githubLink.Name = "githubLink";
            this.githubLink.Size = new System.Drawing.Size(389, 30);
            this.githubLink.TabIndex = 7;
            this.githubLink.Text = "GitHub: github.com/nmd-113/Tray-Temps";
            this.githubLink.Click += new System.EventHandler(this.GithubLink_Click);
            // 
            // donatePic
            // 
            this.donatePic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.donatePic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.donatePic.Image = global::TrayTemps.Properties.Resources.donate;
            this.donatePic.Location = new System.Drawing.Point(759, 589);
            this.donatePic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.donatePic.Name = "donatePic";
            this.donatePic.Size = new System.Drawing.Size(64, 64);
            this.donatePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.donatePic.TabIndex = 6;
            this.donatePic.TabStop = false;
            this.donatePic.Click += new System.EventHandler(this.donatePic_Click);
            // 
            // panelWrapper
            // 
            this.panelWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWrapper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.panelWrapper.Controls.Add(this.mainTabControl);
            this.panelWrapper.Location = new System.Drawing.Point(139, 58);
            this.panelWrapper.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelWrapper.Name = "panelWrapper";
            this.panelWrapper.Size = new System.Drawing.Size(860, 691);
            this.panelWrapper.TabIndex = 3;
            // 
            // cpuTrayIcon
            // 
            this.cpuTrayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.cpuTrayIcon.Text = "CPU Temp";
            this.cpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cpuTrayIcon_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowForm,
            this.ExitForm});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(115, 52);
            // 
            // ShowForm
            // 
            this.ShowForm.Name = "ShowForm";
            this.ShowForm.Size = new System.Drawing.Size(114, 24);
            this.ShowForm.Text = "Show";
            this.ShowForm.Click += new System.EventHandler(this.ShowForm_Click);
            // 
            // ExitForm
            // 
            this.ExitForm.Name = "ExitForm";
            this.ExitForm.Size = new System.Drawing.Size(114, 24);
            this.ExitForm.Text = "Exit";
            this.ExitForm.Click += new System.EventHandler(this.ExitForm_Click);
            // 
            // gpuTrayIcon
            // 
            this.gpuTrayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.gpuTrayIcon.Text = "GPU Temp";
            this.gpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gpuTrayIcon_MouseDoubleClick);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.BalloonTipText = "Double click to show.";
            this.NotifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "TrayTemps";
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.BackgroundImage = global::TrayTemps.Properties.Resources.border;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 750);
            this.Controls.Add(this.panelWrapper);
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.exitBtn);
            this.Controls.Add(this.mainMenu);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 960);
            this.MinimumSize = new System.Drawing.Size(875, 725);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Test Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.mainMenu.ResumeLayout(false);
            this.AppDataPnl.ResumeLayout(false);
            this.AppDataPnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).EndInit();
            this.mainTabControl.ResumeLayout(false);
            this.homePage.ResumeLayout(false);
            this.mainComponentsPanel.ResumeLayout(false);
            this.mainComponentsPanel.PerformLayout();
            this.tempsWrapper.ResumeLayout(false);
            this.gpuPanel.ResumeLayout(false);
            this.gpuPanel.PerformLayout();
            this.cpuPanel.ResumeLayout(false);
            this.cpuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.divider2)).EndInit();
            this.settingsPage.ResumeLayout(false);
            this.generalSettingsLabel.ResumeLayout(false);
            this.generalSettingsLabel.PerformLayout();
            this.refreshPanel.ResumeLayout(false);
            this.refreshPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshValue)).EndInit();
            this.traySettingsPanel.ResumeLayout(false);
            this.traySettingsPanel.PerformLayout();
            this.fontFamilyPanel.ResumeLayout(false);
            this.fontFamilyPanel.PerformLayout();
            this.cpuColorPanel.ResumeLayout(false);
            this.cpuColorPanel.PerformLayout();
            this.gpuColorPanel.ResumeLayout(false);
            this.gpuColorPanel.PerformLayout();
            this.iconsizePanel.ResumeLayout(false);
            this.iconsizePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconsizeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.divider3)).EndInit();
            this.aboutPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.divider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.donatePic)).EndInit();
            this.panelWrapper.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainMenu;
        private System.Windows.Forms.Button homeBtn;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Button aboutBtn;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Button minimizeBtn;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.Panel panelWrapper;
        private System.Windows.Forms.TabPage homePage;
        private System.Windows.Forms.TabPage settingsPage;
        private System.Windows.Forms.TabPage aboutPage;
        private System.Windows.Forms.Label aboutTitle;
        private System.Windows.Forms.Label appTitle;
        private System.Windows.Forms.TableLayoutPanel AppDataPnl;
        private System.Windows.Forms.PictureBox appLogo;
        private System.Windows.Forms.PictureBox divider1;
        private System.Windows.Forms.Label appAboutExtra;
        private System.Windows.Forms.Label appTitleAbout;
        private System.Windows.Forms.PictureBox donatePic;
        private System.Windows.Forms.Label appVersion;
        private System.Windows.Forms.Label githubLink;
        private System.Windows.Forms.PictureBox divider2;
        private System.Windows.Forms.Label sysmonTitle;
        private System.Windows.Forms.Label genSettings;
        private System.Windows.Forms.PictureBox divider3;
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
        private System.Windows.Forms.Label indexLabel;
        private System.Windows.Forms.ComboBox cpuIndexSelect;
        private System.Windows.Forms.ComboBox gpuIndexSelect;
        private System.Windows.Forms.ComboBox storageIndexSelect;
        private System.Windows.Forms.Label placeholderLabel1;
        private System.Windows.Forms.Label placeholderLabel2;
        private System.Windows.Forms.TableLayoutPanel generalSettingsLabel;
        private System.Windows.Forms.CheckBox autostartInstall;
        private System.Windows.Forms.CheckBox tempsFahrenheit;
        private System.Windows.Forms.CheckBox singleIconTray;
        private System.Windows.Forms.TableLayoutPanel traySettingsPanel;
        private System.Windows.Forms.CheckBox enableCpuTray;
        private System.Windows.Forms.Label traySettingsLabel;
        private System.Windows.Forms.CheckBox enableGpuTray;
        private System.Windows.Forms.TableLayoutPanel fontFamilyPanel;
        private System.Windows.Forms.ComboBox fontFamilyValue;
        private System.Windows.Forms.Label fontFamilyLabel;
        private System.Windows.Forms.TableLayoutPanel gpuColorPanel;
        private System.Windows.Forms.ComboBox gpuColorValue;
        private System.Windows.Forms.Label gpuColorLabel;
        private System.Windows.Forms.TableLayoutPanel cpuColorPanel;
        private System.Windows.Forms.ComboBox cpuColorValue;
        private System.Windows.Forms.Label cpuColorLabel;
        private System.Windows.Forms.TableLayoutPanel iconsizePanel;
        private System.Windows.Forms.Label iconsizeLabel;
        private System.Windows.Forms.NumericUpDown iconsizeValue;
        private System.Windows.Forms.TableLayoutPanel refreshPanel;
        private System.Windows.Forms.NumericUpDown refreshValue;
        private System.Windows.Forms.Label refreshLabel;
        private System.Windows.Forms.NotifyIcon cpuTrayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ShowForm;
        private System.Windows.Forms.ToolStripMenuItem ExitForm;
        private System.Windows.Forms.NotifyIcon gpuTrayIcon;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
    }
}