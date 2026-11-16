namespace TrayTemps
{
    partial class FpsLimiterSettingsDialog
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TableLayoutPanel layout, body;
        private System.Windows.Forms.Panel titleBar, bottomBar, gamesPanel;
        private System.Windows.Forms.TableLayoutPanel editor;
        private System.Windows.Forms.TableLayoutPanel gamesHeader;
        private System.Windows.Forms.Label title, fallbackLabel, gamesLabel, selectedLabel, exclusionsLabel, status;
        private System.Windows.Forms.NumericUpDown fallbackCap, selectedCap;
        private System.Windows.Forms.ListView games;
        private System.Windows.Forms.Label executableHeader, pathHeader, capHeader;
        private System.Windows.Forms.ColumnHeader executableColumn, pathColumn, capColumn;
        private System.Windows.Forms.Button close, addGame, removeGame, save, cancel;
        private System.Windows.Forms.TextBox exclusions, hotkeyValue;
        private System.Windows.Forms.TableLayoutPanel hotkeyEditor;
        private System.Windows.Forms.ToolTip settingsToolTip;
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
            this.settingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.hotkeyValue = new System.Windows.Forms.TextBox();
            this.executablePicker = new System.Windows.Forms.OpenFileDialog();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new System.Windows.Forms.Panel();
            this.title = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.body = new System.Windows.Forms.TableLayoutPanel();
            this.fallbackLabel = new System.Windows.Forms.Label();
            this.fallbackCap = new System.Windows.Forms.NumericUpDown();
            this.gamesLabel = new System.Windows.Forms.Label();
            this.gamesPanel = new System.Windows.Forms.Panel();
            this.games = new System.Windows.Forms.ListView();
            this.executableColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pathColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.capColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gamesHeader = new System.Windows.Forms.TableLayoutPanel();
            this.executableHeader = new System.Windows.Forms.Label();
            this.pathHeader = new System.Windows.Forms.Label();
            this.capHeader = new System.Windows.Forms.Label();
            this.editor = new System.Windows.Forms.TableLayoutPanel();
            this.addGame = new System.Windows.Forms.Button();
            this.removeGame = new System.Windows.Forms.Button();
            this.selectedLabel = new System.Windows.Forms.Label();
            this.selectedCap = new System.Windows.Forms.NumericUpDown();
            this.exclusionsLabel = new System.Windows.Forms.Label();
            this.exclusions = new System.Windows.Forms.TextBox();
            this.hotkeyEditor = new System.Windows.Forms.TableLayoutPanel();
            this.toggleHotkey = new System.Windows.Forms.CheckBox();
            this.status = new System.Windows.Forms.Label();
            this.bottomBar = new System.Windows.Forms.Panel();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.notices = new System.Windows.Forms.LinkLabel();
            this.resizeGrip = new TrayTemps.WindowResizeGripPanel();
            this.layout.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.body.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fallbackCap)).BeginInit();
            this.gamesPanel.SuspendLayout();
            this.gamesHeader.SuspendLayout();
            this.editor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedCap)).BeginInit();
            this.hotkeyEditor.SuspendLayout();
            this.bottomBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // hotkeyValue
            // 
            this.hotkeyValue.AccessibleName = "FPS limiter toggle hotkey";
            this.hotkeyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.hotkeyValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.hotkeyValue.Location = new System.Drawing.Point(152, 10);
            this.hotkeyValue.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.hotkeyValue.Name = "hotkeyValue";
            this.hotkeyValue.ReadOnly = true;
            this.hotkeyValue.ShortcutsEnabled = false;
            this.hotkeyValue.Size = new System.Drawing.Size(491, 21);
            this.hotkeyValue.TabIndex = 1;
            this.hotkeyValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.settingsToolTip.SetToolTip(this.hotkeyValue, "Click here and press Ctrl, Shift, or Alt plus a key to set the limiter toggle sho" +
        "rtcut.");
            this.hotkeyValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyValue_KeyDown);
            // 
            // executablePicker
            // 
            this.executablePicker.Filter = "Windows executables (*.exe)|*.exe|All files (*.*)|*.*";
            this.executablePicker.Multiselect = true;
            this.executablePicker.Title = "Add games to the FPS limiter";
            // 
            // layout
            // 
            this.layout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.titleBar, 0, 0);
            this.layout.Controls.Add(this.body, 0, 1);
            this.layout.Controls.Add(this.bottomBar, 0, 2);
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
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.titleBar.Controls.Add(this.title);
            this.titleBar.Controls.Add(this.close);
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(678, 42);
            this.titleBar.TabIndex = 0;
            // 
            // title
            // 
            this.title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.body.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.body.ColumnCount = 2;
            this.body.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.body.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.body.Controls.Add(this.fallbackLabel, 0, 0);
            this.body.Controls.Add(this.fallbackCap, 1, 0);
            this.body.Controls.Add(this.gamesLabel, 0, 1);
            this.body.Controls.Add(this.gamesPanel, 0, 2);
            this.body.Controls.Add(this.editor, 0, 3);
            this.body.Controls.Add(this.exclusionsLabel, 0, 4);
            this.body.Controls.Add(this.exclusions, 0, 5);
            this.body.Controls.Add(this.hotkeyEditor, 0, 6);
            this.body.Controls.Add(this.status, 0, 7);
            this.body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.body.Location = new System.Drawing.Point(0, 42);
            this.body.Margin = new System.Windows.Forms.Padding(0);
            this.body.Name = "body";
            this.body.Padding = new System.Windows.Forms.Padding(16);
            this.body.RowCount = 8;
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.654037F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.654037F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.19288F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.768465F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.654037F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.654037F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.768465F));
            this.body.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.654037F));
            this.body.Size = new System.Drawing.Size(678, 458);
            this.body.TabIndex = 1;
            // 
            // fallbackLabel
            // 
            this.fallbackLabel.AutoSize = true;
            this.fallbackLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fallbackLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fallbackLabel.ForeColor = System.Drawing.Color.LightGray;
            this.fallbackLabel.Location = new System.Drawing.Point(19, 16);
            this.fallbackLabel.Name = "fallbackLabel";
            this.fallbackLabel.Size = new System.Drawing.Size(446, 32);
            this.fallbackLabel.TabIndex = 0;
            this.fallbackLabel.Text = "Fallback cap (FPS):";
            this.fallbackLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fallbackCap
            // 
            this.fallbackCap.AccessibleName = "Fallback FPS cap";
            this.fallbackCap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.fallbackCap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.fallbackCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fallbackCap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fallbackCap.ForeColor = System.Drawing.Color.LightGray;
            this.fallbackCap.Location = new System.Drawing.Point(471, 21);
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
            this.fallbackCap.Size = new System.Drawing.Size(90, 21);
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
            this.body.SetColumnSpan(this.gamesLabel, 2);
            this.gamesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gamesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gamesLabel.ForeColor = System.Drawing.Color.LightGray;
            this.gamesLabel.Location = new System.Drawing.Point(19, 48);
            this.gamesLabel.Name = "gamesLabel";
            this.gamesLabel.Size = new System.Drawing.Size(640, 32);
            this.gamesLabel.TabIndex = 2;
            this.gamesLabel.Text = "My games — matched by executable path";
            this.gamesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gamesPanel
            // 
            this.gamesPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.body.SetColumnSpan(this.gamesPanel, 2);
            this.gamesPanel.Controls.Add(this.games);
            this.gamesPanel.Controls.Add(this.gamesHeader);
            this.gamesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gamesPanel.Location = new System.Drawing.Point(19, 83);
            this.gamesPanel.Name = "gamesPanel";
            this.gamesPanel.Size = new System.Drawing.Size(640, 173);
            this.gamesPanel.TabIndex = 3;
            // 
            // games
            // 
            this.games.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.games.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.games.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.executableColumn,
            this.pathColumn,
            this.capColumn});
            this.games.Dock = System.Windows.Forms.DockStyle.Fill;
            this.games.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.games.ForeColor = System.Drawing.Color.LightGray;
            this.games.FullRowSelect = true;
            this.games.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.games.HideSelection = false;
            this.games.Location = new System.Drawing.Point(0, 24);
            this.games.MultiSelect = false;
            this.games.Name = "games";
            this.games.ShowItemToolTips = true;
            this.games.Size = new System.Drawing.Size(640, 149);
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
            this.pathColumn.Width = 380;
            // 
            // capColumn
            // 
            this.capColumn.Text = "FPS";
            this.capColumn.Width = 100;
            // 
            // gamesHeader
            // 
            this.gamesHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.gamesHeader.ColumnCount = 3;
            this.gamesHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.gamesHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 380F));
            this.gamesHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.gamesHeader.Controls.Add(this.executableHeader, 0, 0);
            this.gamesHeader.Controls.Add(this.pathHeader, 1, 0);
            this.gamesHeader.Controls.Add(this.capHeader, 2, 0);
            this.gamesHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.gamesHeader.Location = new System.Drawing.Point(0, 0);
            this.gamesHeader.Margin = new System.Windows.Forms.Padding(0);
            this.gamesHeader.Name = "gamesHeader";
            this.gamesHeader.RowCount = 1;
            this.gamesHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gamesHeader.Size = new System.Drawing.Size(640, 24);
            this.gamesHeader.TabIndex = 0;
            // 
            // executableHeader
            // 
            this.executableHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.executableHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.executableHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.executableHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.executableHeader.Location = new System.Drawing.Point(0, 0);
            this.executableHeader.Margin = new System.Windows.Forms.Padding(0);
            this.executableHeader.Name = "executableHeader";
            this.executableHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.executableHeader.Size = new System.Drawing.Size(160, 24);
            this.executableHeader.TabIndex = 0;
            this.executableHeader.Text = "Executable";
            this.executableHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathHeader
            // 
            this.pathHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pathHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.pathHeader.Location = new System.Drawing.Point(160, 0);
            this.pathHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pathHeader.Name = "pathHeader";
            this.pathHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.pathHeader.Size = new System.Drawing.Size(380, 24);
            this.pathHeader.TabIndex = 1;
            this.pathHeader.Text = "Path";
            this.pathHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // capHeader
            // 
            this.capHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.capHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.capHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.capHeader.Location = new System.Drawing.Point(540, 0);
            this.capHeader.Margin = new System.Windows.Forms.Padding(0);
            this.capHeader.Name = "capHeader";
            this.capHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.capHeader.Size = new System.Drawing.Size(100, 24);
            this.capHeader.TabIndex = 2;
            this.capHeader.Text = "FPS";
            this.capHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // editor
            // 
            this.editor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.editor.ColumnCount = 4;
            this.body.SetColumnSpan(this.editor, 2);
            this.editor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.editor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.editor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.editor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.editor.Controls.Add(this.addGame, 0, 0);
            this.editor.Controls.Add(this.removeGame, 1, 0);
            this.editor.Controls.Add(this.selectedLabel, 2, 0);
            this.editor.Controls.Add(this.selectedCap, 3, 0);
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.Location = new System.Drawing.Point(17, 260);
            this.editor.Margin = new System.Windows.Forms.Padding(1);
            this.editor.Name = "editor";
            this.editor.RowCount = 1;
            this.editor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.editor.Size = new System.Drawing.Size(644, 39);
            this.editor.TabIndex = 4;
            // 
            // addGame
            // 
            this.addGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.addGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addGame.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.addGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.addGame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.addGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.addGame.ForeColor = System.Drawing.Color.LightGray;
            this.addGame.Location = new System.Drawing.Point(3, 3);
            this.addGame.Name = "addGame";
            this.addGame.Size = new System.Drawing.Size(104, 33);
            this.addGame.TabIndex = 0;
            this.addGame.Text = "Add game...";
            this.addGame.UseVisualStyleBackColor = false;
            this.addGame.Click += new System.EventHandler(this.AddGame_Click);
            // 
            // removeGame
            // 
            this.removeGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.removeGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.removeGame.Enabled = false;
            this.removeGame.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.removeGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeGame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.removeGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.removeGame.ForeColor = System.Drawing.Color.LightGray;
            this.removeGame.Location = new System.Drawing.Point(113, 3);
            this.removeGame.Name = "removeGame";
            this.removeGame.Size = new System.Drawing.Size(94, 33);
            this.removeGame.TabIndex = 1;
            this.removeGame.Text = "Remove";
            this.removeGame.UseVisualStyleBackColor = false;
            this.removeGame.Click += new System.EventHandler(this.RemoveGame_Click);
            // 
            // selectedLabel
            // 
            this.selectedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedLabel.ForeColor = System.Drawing.Color.LightGray;
            this.selectedLabel.Location = new System.Drawing.Point(222, 3);
            this.selectedLabel.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(324, 33);
            this.selectedLabel.TabIndex = 2;
            this.selectedLabel.Text = "Game cap (FPS):";
            this.selectedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // selectedCap
            // 
            this.selectedCap.AccessibleName = "Selected game FPS cap";
            this.selectedCap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.selectedCap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.selectedCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectedCap.Enabled = false;
            this.selectedCap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCap.ForeColor = System.Drawing.Color.LightGray;
            this.selectedCap.Location = new System.Drawing.Point(552, 10);
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
            this.selectedCap.Size = new System.Drawing.Size(85, 21);
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
            this.body.SetColumnSpan(this.exclusionsLabel, 2);
            this.exclusionsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exclusionsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exclusionsLabel.ForeColor = System.Drawing.Color.LightGray;
            this.exclusionsLabel.Location = new System.Drawing.Point(19, 300);
            this.exclusionsLabel.Name = "exclusionsLabel";
            this.exclusionsLabel.Size = new System.Drawing.Size(640, 32);
            this.exclusionsLabel.TabIndex = 5;
            this.exclusionsLabel.Text = "Exclude process names (commas, semicolons or new lines):";
            this.exclusionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exclusions
            // 
            this.exclusions.AccessibleName = "Excluded process names";
            this.exclusions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.exclusions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.body.SetColumnSpan(this.exclusions, 2);
            this.exclusions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exclusions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exclusions.ForeColor = System.Drawing.Color.LightGray;
            this.exclusions.Location = new System.Drawing.Point(19, 335);
            this.exclusions.MaxLength = 8192;
            this.exclusions.Name = "exclusions";
            this.exclusions.Size = new System.Drawing.Size(640, 21);
            this.exclusions.TabIndex = 6;
            // 
            // hotkeyEditor
            // 
            this.hotkeyEditor.ColumnCount = 2;
            this.body.SetColumnSpan(this.hotkeyEditor, 2);
            this.hotkeyEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.hotkeyEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.hotkeyEditor.Controls.Add(this.toggleHotkey, 0, 0);
            this.hotkeyEditor.Controls.Add(this.hotkeyValue, 1, 0);
            this.hotkeyEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hotkeyEditor.Location = new System.Drawing.Point(16, 364);
            this.hotkeyEditor.Margin = new System.Windows.Forms.Padding(0);
            this.hotkeyEditor.Name = "hotkeyEditor";
            this.hotkeyEditor.RowCount = 1;
            this.hotkeyEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.hotkeyEditor.Size = new System.Drawing.Size(646, 41);
            this.hotkeyEditor.TabIndex = 7;
            // 
            // toggleHotkey
            // 
            this.toggleHotkey.AutoSize = true;
            this.toggleHotkey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toggleHotkey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleHotkey.ForeColor = System.Drawing.Color.LightGray;
            this.toggleHotkey.Location = new System.Drawing.Point(3, 3);
            this.toggleHotkey.Name = "toggleHotkey";
            this.toggleHotkey.Size = new System.Drawing.Size(143, 35);
            this.toggleHotkey.TabIndex = 0;
            this.toggleHotkey.Text = "Enable toggle hotkey:";
            // 
            // status
            // 
            this.status.AutoEllipsis = true;
            this.body.SetColumnSpan(this.status, 2);
            this.status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.ForeColor = System.Drawing.Color.DarkGray;
            this.status.Location = new System.Drawing.Point(19, 405);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(640, 37);
            this.status.TabIndex = 8;
            this.status.Text = "Disabled";
            this.status.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // bottomBar
            // 
            this.bottomBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.bottomBar.Controls.Add(this.save);
            this.bottomBar.Controls.Add(this.cancel);
            this.bottomBar.Controls.Add(this.notices);
            this.bottomBar.Controls.Add(this.resizeGrip);
            this.bottomBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomBar.Location = new System.Drawing.Point(0, 500);
            this.bottomBar.Margin = new System.Windows.Forms.Padding(0);
            this.bottomBar.Name = "bottomBar";
            this.bottomBar.Size = new System.Drawing.Size(678, 48);
            this.bottomBar.TabIndex = 2;
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.save.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(230)))));
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.save.ForeColor = System.Drawing.Color.White;
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
            this.cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cancel.ForeColor = System.Drawing.Color.LightGray;
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
            this.notices.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.notices.LinkColor = System.Drawing.Color.SeaGreen;
            this.notices.Location = new System.Drawing.Point(20, 16);
            this.notices.Name = "notices";
            this.notices.Size = new System.Drawing.Size(118, 16);
            this.notices.TabIndex = 2;
            this.notices.TabStop = true;
            this.notices.Text = "Third-party notices";
            this.notices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Notices_LinkClicked);
            // 
            // resizeGrip
            // 
            this.resizeGrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeGrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.resizeGrip.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeGrip.Location = new System.Drawing.Point(658, 28);
            this.resizeGrip.Name = "resizeGrip";
            this.resizeGrip.Size = new System.Drawing.Size(20, 20);
            this.resizeGrip.TabIndex = 3;
            // 
            // FpsLimiterSettingsDialog
            // 
            this.AcceptButton = this.save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
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
            ((System.ComponentModel.ISupportInitialize)(this.fallbackCap)).EndInit();
            this.gamesPanel.ResumeLayout(false);
            this.gamesHeader.ResumeLayout(false);
            this.editor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectedCap)).EndInit();
            this.hotkeyEditor.ResumeLayout(false);
            this.hotkeyEditor.PerformLayout();
            this.bottomBar.ResumeLayout(false);
            this.bottomBar.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
