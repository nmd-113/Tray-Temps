namespace TrayTemps
{
    partial class FpsLimiterSettingsDialog
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TableLayoutPanel layout, body;
        private System.Windows.Forms.Panel titleBar, bottomBar, gamesPanel;
        private System.Windows.Forms.FlowLayoutPanel editor;
        private System.Windows.Forms.TableLayoutPanel gamesHeader;
        private System.Windows.Forms.Label title, fallbackLabel, gamesLabel, selectedLabel, exclusionsLabel, status;
        private System.Windows.Forms.NumericUpDown fallbackCap, selectedCap;
        private System.Windows.Forms.ListView games;
        private System.Windows.Forms.Label executableHeader, pathHeader, capHeader;
        private System.Windows.Forms.ColumnHeader executableColumn, pathColumn, capColumn;
        private System.Windows.Forms.Button close, addGame, removeGame, save, cancel;
        private System.Windows.Forms.TextBox exclusions;
        private System.Windows.Forms.CheckBox toggleHotkey;
        private System.Windows.Forms.LinkLabel notices;
        private System.Windows.Forms.OpenFileDialog executablePicker;
        private WindowResizeGripPanel resizeGrip;

        protected override void Dispose(bool disposing)
        {
            if (disposing) { Unsubscribe(); components?.Dispose(); }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new System.Windows.Forms.Panel();
            this.title = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.body = new System.Windows.Forms.TableLayoutPanel();
            this.fallbackLabel = new System.Windows.Forms.Label();
            this.fallbackCap = new System.Windows.Forms.NumericUpDown();
            this.gamesLabel = new System.Windows.Forms.Label();
            this.gamesPanel = new System.Windows.Forms.Panel();
            this.gamesHeader = new System.Windows.Forms.TableLayoutPanel();
            this.executableHeader = new System.Windows.Forms.Label();
            this.pathHeader = new System.Windows.Forms.Label();
            this.capHeader = new System.Windows.Forms.Label();
            this.games = new System.Windows.Forms.ListView();
            this.executableColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pathColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.capColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.editor = new System.Windows.Forms.FlowLayoutPanel();
            this.addGame = new System.Windows.Forms.Button();
            this.removeGame = new System.Windows.Forms.Button();
            this.selectedLabel = new System.Windows.Forms.Label();
            this.selectedCap = new System.Windows.Forms.NumericUpDown();
            this.exclusionsLabel = new System.Windows.Forms.Label();
            this.exclusions = new System.Windows.Forms.TextBox();
            this.toggleHotkey = new System.Windows.Forms.CheckBox();
            this.status = new System.Windows.Forms.Label();
            this.bottomBar = new System.Windows.Forms.Panel();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.notices = new System.Windows.Forms.LinkLabel();
            this.resizeGrip = new TrayTemps.WindowResizeGripPanel();
            this.executablePicker = new System.Windows.Forms.OpenFileDialog();
            this.components.Add(this.executablePicker);
            this.layout.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.body.SuspendLayout();
            this.gamesPanel.SuspendLayout();
            this.gamesHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fallbackCap)).BeginInit();
            this.editor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedCap)).BeginInit();
            this.bottomBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.titleBar, 0, 0);
            this.layout.Controls.Add(this.body, 0, 1);
            this.layout.Controls.Add(this.bottomBar, 0, 2);
            this.layout.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(1, 1);
            this.layout.Margin = new System.Windows.Forms.Padding(0);
            this.layout.Name = "layout";
            this.layout.RowCount = 3;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.layout.Size = new System.Drawing.Size(678, 548);
            this.layout.TabIndex = 0;
            // 
            // titleBar
            // 
            this.titleBar.Controls.Add(this.title);
            this.titleBar.Controls.Add(this.close);
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(678, 42);
            this.titleBar.TabIndex = 0;
            // 
            // title
            // 
            this.title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular);
            this.title.Location = new System.Drawing.Point(0, 0);
            this.title.Name = "title";
            this.title.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.title.Size = new System.Drawing.Size(636, 42);
            this.title.TabIndex = 0;
            this.title.Tag = "TrayTemps.Font.ExtraBold";
            this.title.Text = "FPS limiter settings";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.title.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Title_MouseDown);
            // 
            // close
            // 
            this.close.AccessibleName = "Cancel and close settings";
            this.close.Dock = System.Windows.Forms.DockStyle.Right;
            this.close.FlatAppearance.BorderSize = 0;
            this.close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Font = new System.Drawing.Font("TrayTemps Mono ExtraBold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.close.Location = new System.Drawing.Point(636, 0);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(42, 42);
            this.close.TabIndex = 1;
            this.close.Tag = "TrayTemps.Font.ExtraBold";
            this.close.Text = "✖";
            this.close.UseVisualStyleBackColor = false;
            this.close.Click += new System.EventHandler(this.Close_Click);
            // 
            // body
            // 
            this.body.ColumnCount = 2;
            this.body.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.body.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.body.Controls.Add(this.fallbackLabel, 0, 0);
            this.body.Controls.Add(this.fallbackCap, 1, 0);
            this.body.Controls.Add(this.gamesLabel, 0, 1);
            this.body.Controls.Add(this.gamesPanel, 0, 2);
            this.body.SetColumnSpan(this.gamesPanel, 2);
            this.body.Controls.Add(this.editor, 0, 3);
            this.body.Controls.Add(this.exclusionsLabel, 0, 4);
            this.body.Controls.Add(this.exclusions, 0, 5);
            this.body.Controls.Add(this.toggleHotkey, 0, 6);
            this.body.Controls.Add(this.status, 0, 7);
            this.body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.body.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.body.Location = new System.Drawing.Point(0, 42);
            this.body.Margin = new System.Windows.Forms.Padding(0);
            this.body.Name = "body";
            this.body.Padding = new System.Windows.Forms.Padding(16);
            this.body.RowCount = 8;
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.body.Size = new System.Drawing.Size(678, 458);
            this.body.TabIndex = 1;
            // 
            // fallbackLabel
            // 
            this.fallbackLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.fallbackLabel.AutoSize = true;
            this.fallbackLabel.ForeColor = System.Drawing.Color.LightGray;
            this.fallbackLabel.Location = new System.Drawing.Point(19, 24);
            this.fallbackLabel.Name = "fallbackLabel";
            this.fallbackLabel.Size = new System.Drawing.Size(102, 15);
            this.fallbackLabel.TabIndex = 0;
            this.fallbackLabel.Text = "Fallback cap (FPS)";
            // 
            // fallbackCap
            // 
            this.fallbackCap.AccessibleName = "Fallback FPS cap";
            this.fallbackCap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.fallbackCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fallbackCap.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.fallbackCap.ForeColor = System.Drawing.Color.LightGray;
            this.fallbackCap.Location = new System.Drawing.Point(471, 20);
            this.fallbackCap.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.fallbackCap.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.fallbackCap.Name = "fallbackCap";
            this.fallbackCap.Size = new System.Drawing.Size(90, 23);
            this.fallbackCap.TabIndex = 1;
            this.fallbackCap.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // gamesLabel
            // 
            this.gamesLabel.AutoSize = true;
            this.gamesLabel.ForeColor = System.Drawing.Color.LightGray;
            this.body.SetColumnSpan(this.gamesLabel, 2);
            this.gamesLabel.Location = new System.Drawing.Point(19, 48);
            this.gamesLabel.Name = "gamesLabel";
            this.gamesLabel.Size = new System.Drawing.Size(230, 15);
            this.gamesLabel.TabIndex = 2;
            this.gamesLabel.Text = "My games — matched by executable path";
            //
            // gamesPanel
            //
            this.gamesPanel.Controls.Add(this.games);
            this.gamesPanel.Controls.Add(this.gamesHeader);
            this.gamesPanel.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.gamesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gamesPanel.Margin = new System.Windows.Forms.Padding(3);
            this.gamesPanel.Name = "gamesPanel";
            this.gamesPanel.TabIndex = 3;
            //
            // gamesHeader
            //
            this.gamesHeader.ColumnCount = 3;
            this.gamesHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.gamesHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 285F));
            this.gamesHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gamesHeader.Controls.Add(this.executableHeader, 0, 0);
            this.gamesHeader.Controls.Add(this.pathHeader, 1, 0);
            this.gamesHeader.Controls.Add(this.capHeader, 2, 0);
            this.gamesHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.gamesHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.gamesHeader.Height = 24;
            this.gamesHeader.Margin = new System.Windows.Forms.Padding(0);
            this.gamesHeader.Name = "gamesHeader";
            this.gamesHeader.RowCount = 1;
            this.gamesHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gamesHeader.TabIndex = 0;
            //
            // games header labels
            //
            this.executableHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.executableHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.executableHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.executableHeader.Margin = new System.Windows.Forms.Padding(0);
            this.executableHeader.Name = "executableHeader";
            this.executableHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.executableHeader.Text = "Executable";
            this.executableHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.executableHeader.TabIndex = 0;
            this.pathHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.pathHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.pathHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pathHeader.Name = "pathHeader";
            this.pathHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.pathHeader.Text = "Path";
            this.pathHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pathHeader.TabIndex = 1;
            this.capHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.capHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.capHeader.Margin = new System.Windows.Forms.Padding(0);
            this.capHeader.Name = "capHeader";
            this.capHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.capHeader.Text = "FPS";
            this.capHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.capHeader.TabIndex = 2;
            // 
            // games
            // 
            this.games.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.games.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.games.ForeColor = System.Drawing.Color.LightGray;
            this.games.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.executableColumn,
            this.pathColumn,
            this.capColumn});
            this.games.Dock = System.Windows.Forms.DockStyle.Fill;
            this.games.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.games.FullRowSelect = true;
            this.games.HideSelection = false;
            this.games.Location = new System.Drawing.Point(19, 75);
            this.games.MultiSelect = false;
            this.games.Name = "games";
            this.games.ShowItemToolTips = true;
            this.games.Size = new System.Drawing.Size(640, 254);
            this.games.TabIndex = 3;
            this.games.UseCompatibleStateImageBehavior = false;
            this.games.View = System.Windows.Forms.View.Details;
            this.games.SelectedIndexChanged += new System.EventHandler(this.Games_SelectedIndexChanged);
            // 
            // executableColumn
            // 
            this.executableColumn.Text = "Executable";
            this.executableColumn.Width = 160;
            // 
            // pathColumn
            // 
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 285;
            // 
            // capColumn
            // 
            this.capColumn.Text = "FPS";
            this.capColumn.Width = 100;
            // 
            // editor
            // 
            this.body.SetColumnSpan(this.editor, 2);
            this.editor.Controls.Add(this.addGame);
            this.editor.Controls.Add(this.removeGame);
            this.editor.Controls.Add(this.selectedLabel);
            this.editor.Controls.Add(this.selectedCap);
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.editor.Location = new System.Drawing.Point(19, 335);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(640, 34);
            this.editor.TabIndex = 4;
            this.editor.WrapContents = false;
            // 
            // addGame
            // 
            this.addGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addGame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(55, 55, 55);
            this.addGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(65, 65, 65);
            this.addGame.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.addGame.ForeColor = System.Drawing.Color.LightGray;
            this.addGame.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.addGame.UseVisualStyleBackColor = false;
            this.addGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.addGame.Location = new System.Drawing.Point(3, 3);
            this.addGame.Name = "addGame";
            this.addGame.Size = new System.Drawing.Size(110, 30);
            this.addGame.TabIndex = 0;
            this.addGame.Text = "Add game...";
            this.addGame.Click += new System.EventHandler(this.AddGame_Click);
            // 
            // removeGame
            // 
            this.removeGame.Enabled = false;
            this.removeGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeGame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(55, 55, 55);
            this.removeGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(65, 65, 65);
            this.removeGame.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.removeGame.ForeColor = System.Drawing.Color.LightGray;
            this.removeGame.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.removeGame.UseVisualStyleBackColor = false;
            this.removeGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.removeGame.Location = new System.Drawing.Point(119, 3);
            this.removeGame.Name = "removeGame";
            this.removeGame.Size = new System.Drawing.Size(100, 30);
            this.removeGame.TabIndex = 1;
            this.removeGame.Text = "Remove";
            this.removeGame.Click += new System.EventHandler(this.RemoveGame_Click);
            // 
            // selectedLabel
            // 
            this.selectedLabel.AutoSize = true;
            this.selectedLabel.ForeColor = System.Drawing.Color.LightGray;
            this.selectedLabel.Location = new System.Drawing.Point(234, 8);
            this.selectedLabel.Margin = new System.Windows.Forms.Padding(12, 8, 3, 0);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(60, 15);
            this.selectedLabel.TabIndex = 2;
            this.selectedLabel.Text = "Game cap";
            // 
            // selectedCap
            // 
            this.selectedCap.AccessibleName = "Selected game FPS cap";
            this.selectedCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectedCap.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.selectedCap.ForeColor = System.Drawing.Color.LightGray;
            this.selectedCap.Enabled = false;
            this.selectedCap.Location = new System.Drawing.Point(300, 5);
            this.selectedCap.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.selectedCap.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.selectedCap.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.selectedCap.Name = "selectedCap";
            this.selectedCap.Size = new System.Drawing.Size(85, 23);
            this.selectedCap.TabIndex = 3;
            this.selectedCap.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.selectedCap.ValueChanged += new System.EventHandler(this.SelectedCap_ValueChanged);
            // 
            // exclusionsLabel
            // 
            this.exclusionsLabel.AutoSize = true;
            this.exclusionsLabel.ForeColor = System.Drawing.Color.LightGray;
            this.body.SetColumnSpan(this.exclusionsLabel, 2);
            this.exclusionsLabel.Location = new System.Drawing.Point(19, 372);
            this.exclusionsLabel.Name = "exclusionsLabel";
            this.exclusionsLabel.Size = new System.Drawing.Size(318, 15);
            this.exclusionsLabel.TabIndex = 5;
            this.exclusionsLabel.Text = "Exclude process names (commas, semicolons or new lines)";
            // 
            // exclusions
            // 
            this.exclusions.AccessibleName = "Excluded process names";
            this.exclusions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exclusions.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.exclusions.ForeColor = System.Drawing.Color.LightGray;
            this.body.SetColumnSpan(this.exclusions, 2);
            this.exclusions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exclusions.Location = new System.Drawing.Point(19, 399);
            this.exclusions.MaxLength = 8192;
            this.exclusions.Name = "exclusions";
            this.exclusions.Size = new System.Drawing.Size(640, 23);
            this.exclusions.TabIndex = 6;
            // 
            // toggleHotkey
            // 
            this.toggleHotkey.AutoSize = true;
            this.toggleHotkey.ForeColor = System.Drawing.Color.LightGray;
            this.body.SetColumnSpan(this.toggleHotkey, 2);
            this.toggleHotkey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toggleHotkey.Location = new System.Drawing.Point(19, 427);
            this.toggleHotkey.Name = "toggleHotkey";
            this.toggleHotkey.Size = new System.Drawing.Size(640, 26);
            this.toggleHotkey.TabIndex = 7;
            this.toggleHotkey.Text = "Toggle limiter with Ctrl+Shift+F12";
            // 
            // status
            // 
            this.status.AutoEllipsis = true;
            this.status.ForeColor = System.Drawing.Color.DarkGray;
            this.body.SetColumnSpan(this.status, 2);
            this.status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.status.Location = new System.Drawing.Point(19, 456);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(640, 36);
            this.status.TabIndex = 8;
            this.status.Text = "Disabled";
            // 
            // bottomBar
            // 
            this.bottomBar.Controls.Add(this.save);
            this.bottomBar.Controls.Add(this.cancel);
            this.bottomBar.Controls.Add(this.notices);
            this.bottomBar.Controls.Add(this.resizeGrip);
            this.bottomBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomBar.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.bottomBar.Location = new System.Drawing.Point(0, 500);
            this.bottomBar.Margin = new System.Windows.Forms.Padding(0);
            this.bottomBar.Name = "bottomBar";
            this.bottomBar.Size = new System.Drawing.Size(678, 48);
            this.bottomBar.TabIndex = 2;
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
            this.save.ForeColor = System.Drawing.Color.White;
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.save.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 120, 212);
            this.save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 130, 230);
            this.save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 100, 180);
            this.save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.save.Location = new System.Drawing.Point(542, 8);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(100, 32);
            this.save.TabIndex = 0;
            this.save.Tag = "TrayTemps.Font.ExtraBold";
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = false;
            this.save.Click += new System.EventHandler(this.Save_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(55, 55, 55);
            this.cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(65, 65, 65);
            this.cancel.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.cancel.ForeColor = System.Drawing.Color.LightGray;
            this.cancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cancel.Location = new System.Drawing.Point(432, 8);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(100, 32);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = false;
            // 
            // notices
            // 
            this.notices.AutoSize = true;
            this.notices.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.notices.LinkColor = System.Drawing.Color.SeaGreen;
            this.notices.Location = new System.Drawing.Point(20, 16);
            this.notices.Name = "notices";
            this.notices.Size = new System.Drawing.Size(107, 15);
            this.notices.TabIndex = 2;
            this.notices.TabStop = true;
            this.notices.Text = "Third-party notices";
            this.notices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Notices_LinkClicked);
            // 
            // resizeGrip
            // 
            this.resizeGrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeGrip.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.resizeGrip.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeGrip.Location = new System.Drawing.Point(658, 28);
            this.resizeGrip.Name = "resizeGrip";
            this.resizeGrip.Size = new System.Drawing.Size(20, 20);
            this.resizeGrip.TabIndex = 3;
            // 
            // executablePicker
            // 
            this.executablePicker.Filter = "Windows executables (*.exe)|*.exe|All files (*.*)|*.*";
            this.executablePicker.Multiselect = true;
            this.executablePicker.Title = "Add games to the FPS limiter";
            // 
            // FpsLimiterSettingsDialog
            // 
            this.AcceptButton = this.save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(21, 21, 21);
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(680, 550);
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(650, 500);
            this.Name = "FpsLimiterSettingsDialog";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FPS limiter settings";
            this.layout.ResumeLayout(false);
            this.titleBar.ResumeLayout(false);
            this.body.ResumeLayout(false);
            this.body.PerformLayout();
            this.gamesHeader.ResumeLayout(false);
            this.gamesPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fallbackCap)).EndInit();
            this.editor.ResumeLayout(false);
            this.editor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedCap)).EndInit();
            this.bottomBar.ResumeLayout(false);
            this.bottomBar.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
