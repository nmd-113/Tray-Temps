partial class HardwareDetailsDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.outerBorder = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.contentArea = new System.Windows.Forms.Panel();
            this.cardDetails = new System.Windows.Forms.Panel();
            this._detailsBox = new System.Windows.Forms.RichTextBox();
            this.cardSensors = new System.Windows.Forms.Panel();
            this._liveBox = new System.Windows.Forms.RichTextBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.contentHeaderLabel = new System.Windows.Forms.Label();
            this.bottomBar = new System.Windows.Forms.Panel();
            this._closeBtn = new System.Windows.Forms.Button();
            this.copyAllBtn = new System.Windows.Forms.Button();
            this._copyBtn = new System.Windows.Forms.Button();
            this.titleBar = new System.Windows.Forms.Panel();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.closeTopBtn = new System.Windows.Forms.Button();
            this.leftMenu = new System.Windows.Forms.Panel();
            this._sensorsMenuBtn = new System.Windows.Forms.Button();
            this._detailsMenuBtn = new System.Windows.Forms.Button();
            this.menuTitle = new System.Windows.Forms.Label();
            this.iconLabel = new System.Windows.Forms.Label();
            this.accentLine = new System.Windows.Forms.Panel();
            this._liveTimer = new System.Windows.Forms.Timer(this.components);
            this.outerBorder.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.contentArea.SuspendLayout();
            this.cardDetails.SuspendLayout();
            this.cardSensors.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.bottomBar.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.leftMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // outerBorder
            // 
            this.outerBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.outerBorder.Controls.Add(this.mainPanel);
            this.outerBorder.Controls.Add(this.leftMenu);
            this.outerBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerBorder.Font = new System.Drawing.Font("Bunken Tech Sans Pro Book", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outerBorder.Location = new System.Drawing.Point(0, 0);
            this.outerBorder.Name = "outerBorder";
            this.outerBorder.Padding = new System.Windows.Forms.Padding(1);
            this.outerBorder.Size = new System.Drawing.Size(760, 520);
            this.outerBorder.TabIndex = 0;
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.mainPanel.Controls.Add(this.contentArea);
            this.mainPanel.Controls.Add(this.bottomBar);
            this.mainPanel.Controls.Add(this.titleBar);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(121, 1);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(638, 518);
            this.mainPanel.TabIndex = 1;
            // 
            // contentArea
            // 
            this.contentArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.contentArea.Controls.Add(this.cardDetails);
            this.contentArea.Controls.Add(this.cardSensors);
            this.contentArea.Controls.Add(this.headerPanel);
            this.contentArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentArea.Location = new System.Drawing.Point(0, 55);
            this.contentArea.Name = "contentArea";
            this.contentArea.Padding = new System.Windows.Forms.Padding(15, 0, 15, 15);
            this.contentArea.Size = new System.Drawing.Size(638, 413);
            this.contentArea.TabIndex = 2;
            // 
            // cardDetails
            // 
            this.cardDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.cardDetails.Controls.Add(this._detailsBox);
            this.cardDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardDetails.Location = new System.Drawing.Point(15, 35);
            this.cardDetails.Name = "cardDetails";
            this.cardDetails.Padding = new System.Windows.Forms.Padding(15);
            this.cardDetails.Size = new System.Drawing.Size(608, 363);
            this.cardDetails.TabIndex = 2;
            // 
            // _detailsBox
            // 
            this._detailsBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this._detailsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._detailsBox.DetectUrls = false;
            this._detailsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailsBox.Font = new System.Drawing.Font("Consolas", 10F);
            this._detailsBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this._detailsBox.Location = new System.Drawing.Point(15, 15);
            this._detailsBox.Name = "_detailsBox";
            this._detailsBox.ReadOnly = true;
            this._detailsBox.Size = new System.Drawing.Size(578, 333);
            this._detailsBox.TabIndex = 0;
            this._detailsBox.Text = "";
            this._detailsBox.WordWrap = false;
            // 
            // cardSensors
            // 
            this.cardSensors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.cardSensors.Controls.Add(this._liveBox);
            this.cardSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardSensors.Location = new System.Drawing.Point(15, 35);
            this.cardSensors.Name = "cardSensors";
            this.cardSensors.Padding = new System.Windows.Forms.Padding(15);
            this.cardSensors.Size = new System.Drawing.Size(608, 363);
            this.cardSensors.TabIndex = 3;
            this.cardSensors.Visible = false;
            // 
            // _liveBox
            // 
            this._liveBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this._liveBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._liveBox.DetectUrls = false;
            this._liveBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._liveBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._liveBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this._liveBox.Location = new System.Drawing.Point(15, 15);
            this._liveBox.Name = "_liveBox";
            this._liveBox.ReadOnly = true;
            this._liveBox.Size = new System.Drawing.Size(578, 333);
            this._liveBox.TabIndex = 1;
            this._liveBox.Text = "";
            this._liveBox.WordWrap = false;
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.contentHeaderLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(15, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(608, 35);
            this.headerPanel.TabIndex = 4;
            // 
            // contentHeaderLabel
            // 
            this.contentHeaderLabel.AutoSize = true;
            this.contentHeaderLabel.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.contentHeaderLabel.Location = new System.Drawing.Point(0, 9);
            this.contentHeaderLabel.Name = "contentHeaderLabel";
            this.contentHeaderLabel.Size = new System.Drawing.Size(132, 17);
            this.contentHeaderLabel.TabIndex = 0;
            this.contentHeaderLabel.Text = "System information";
            // 
            // bottomBar
            // 
            this.bottomBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.bottomBar.Controls.Add(this._closeBtn);
            this.bottomBar.Controls.Add(this.copyAllBtn);
            this.bottomBar.Controls.Add(this._copyBtn);
            this.bottomBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomBar.Location = new System.Drawing.Point(0, 468);
            this.bottomBar.Name = "bottomBar";
            this.bottomBar.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.bottomBar.Size = new System.Drawing.Size(638, 50);
            this.bottomBar.TabIndex = 1;
            // 
            // _closeBtn
            // 
            this._closeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this._closeBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._closeBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this._closeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this._closeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this._closeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._closeBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Book", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._closeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._closeBtn.Location = new System.Drawing.Point(308, 10);
            this._closeBtn.Name = "_closeBtn";
            this._closeBtn.Size = new System.Drawing.Size(100, 30);
            this._closeBtn.TabIndex = 2;
            this._closeBtn.Text = "Close";
            this._closeBtn.UseVisualStyleBackColor = false;
            // 
            // copyAllBtn
            // 
            this.copyAllBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.copyAllBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.copyAllBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.copyAllBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.copyAllBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.copyAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.copyAllBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Book", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyAllBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.copyAllBtn.Location = new System.Drawing.Point(408, 10);
            this.copyAllBtn.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.copyAllBtn.Name = "copyAllBtn";
            this.copyAllBtn.Size = new System.Drawing.Size(100, 30);
            this.copyAllBtn.TabIndex = 1;
            this.copyAllBtn.Text = "Copy All";
            this.copyAllBtn.UseVisualStyleBackColor = false;
            this.copyAllBtn.Click += new System.EventHandler(this.copyAllBtn_Click);
            // 
            // _copyBtn
            // 
            this._copyBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this._copyBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this._copyBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this._copyBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this._copyBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this._copyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._copyBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Book", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._copyBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._copyBtn.Location = new System.Drawing.Point(508, 10);
            this._copyBtn.Name = "_copyBtn";
            this._copyBtn.Size = new System.Drawing.Size(115, 30);
            this._copyBtn.TabIndex = 0;
            this._copyBtn.Text = "Copy Details";
            this._copyBtn.UseVisualStyleBackColor = false;
            this._copyBtn.Click += new System.EventHandler(this._copyBtn_Click);
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.titleBar.Controls.Add(this._subtitleLabel);
            this.titleBar.Controls.Add(this.titleLabel);
            this.titleBar.Controls.Add(this.minimizeBtn);
            this.titleBar.Controls.Add(this.closeTopBtn);
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(638, 55);
            this.titleBar.TabIndex = 0;
            // 
            // _subtitleLabel
            // 
            this._subtitleLabel.AutoEllipsis = true;
            this._subtitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._subtitleLabel.Font = new System.Drawing.Font("Bunken Tech Sans Pro Book", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._subtitleLabel.ForeColor = System.Drawing.Color.DarkGray;
            this._subtitleLabel.Location = new System.Drawing.Point(0, 32);
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.Padding = new System.Windows.Forms.Padding(15, 2, 0, 0);
            this._subtitleLabel.Size = new System.Drawing.Size(538, 24);
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Static data";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleLabel.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Padding = new System.Windows.Forms.Padding(15, 8, 0, 0);
            this.titleLabel.Size = new System.Drawing.Size(538, 32);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Component Name";
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.minimizeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.minimizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.minimizeBtn.Location = new System.Drawing.Point(538, 0);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(50, 55);
            this.minimizeBtn.TabIndex = 3;
            this.minimizeBtn.TabStop = false;
            this.minimizeBtn.Text = "─";
            this.minimizeBtn.UseVisualStyleBackColor = false;
            this.minimizeBtn.Click += new System.EventHandler(this.minimizeBtn_Click);
            // 
            // closeTopBtn
            // 
            this.closeTopBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.closeTopBtn.FlatAppearance.BorderSize = 0;
            this.closeTopBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.closeTopBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.closeTopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeTopBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeTopBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.closeTopBtn.Location = new System.Drawing.Point(588, 0);
            this.closeTopBtn.Name = "closeTopBtn";
            this.closeTopBtn.Size = new System.Drawing.Size(50, 55);
            this.closeTopBtn.TabIndex = 2;
            this.closeTopBtn.TabStop = false;
            this.closeTopBtn.Text = "✖";
            this.closeTopBtn.UseVisualStyleBackColor = false;
            this.closeTopBtn.Click += new System.EventHandler(this.closeTopBtn_Click);
            // 
            // leftMenu
            // 
            this.leftMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.leftMenu.Controls.Add(this._sensorsMenuBtn);
            this.leftMenu.Controls.Add(this._detailsMenuBtn);
            this.leftMenu.Controls.Add(this.menuTitle);
            this.leftMenu.Controls.Add(this.iconLabel);
            this.leftMenu.Controls.Add(this.accentLine);
            this.leftMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftMenu.Location = new System.Drawing.Point(1, 1);
            this.leftMenu.Name = "leftMenu";
            this.leftMenu.Size = new System.Drawing.Size(120, 518);
            this.leftMenu.TabIndex = 0;
            // 
            // _sensorsMenuBtn
            // 
            this._sensorsMenuBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this._sensorsMenuBtn.FlatAppearance.BorderSize = 0;
            this._sensorsMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this._sensorsMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this._sensorsMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._sensorsMenuBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._sensorsMenuBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._sensorsMenuBtn.Location = new System.Drawing.Point(3, 146);
            this._sensorsMenuBtn.Name = "_sensorsMenuBtn";
            this._sensorsMenuBtn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this._sensorsMenuBtn.Size = new System.Drawing.Size(117, 45);
            this._sensorsMenuBtn.TabIndex = 4;
            this._sensorsMenuBtn.TabStop = false;
            this._sensorsMenuBtn.Text = "Sensors";
            this._sensorsMenuBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._sensorsMenuBtn.UseVisualStyleBackColor = false;
            this._sensorsMenuBtn.Click += new System.EventHandler(this._sensorsMenuBtn_Click);
            // 
            // _detailsMenuBtn
            // 
            this._detailsMenuBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this._detailsMenuBtn.FlatAppearance.BorderSize = 0;
            this._detailsMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this._detailsMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this._detailsMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._detailsMenuBtn.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._detailsMenuBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._detailsMenuBtn.Location = new System.Drawing.Point(3, 101);
            this._detailsMenuBtn.Name = "_detailsMenuBtn";
            this._detailsMenuBtn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this._detailsMenuBtn.Size = new System.Drawing.Size(117, 45);
            this._detailsMenuBtn.TabIndex = 3;
            this._detailsMenuBtn.TabStop = false;
            this._detailsMenuBtn.Text = "Details";
            this._detailsMenuBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._detailsMenuBtn.UseVisualStyleBackColor = false;
            this._detailsMenuBtn.Click += new System.EventHandler(this._detailsMenuBtn_Click);
            // 
            // menuTitle
            // 
            this.menuTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuTitle.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuTitle.ForeColor = System.Drawing.Color.Gray;
            this.menuTitle.Location = new System.Drawing.Point(3, 70);
            this.menuTitle.Name = "menuTitle";
            this.menuTitle.Size = new System.Drawing.Size(117, 31);
            this.menuTitle.TabIndex = 2;
            this.menuTitle.Text = "CATEGORY";
            this.menuTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // iconLabel
            // 
            this.iconLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.iconLabel.Font = new System.Drawing.Font("Bunken Tech Sans Pro Bold", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.iconLabel.Location = new System.Drawing.Point(3, 0);
            this.iconLabel.Name = "iconLabel";
            this.iconLabel.Size = new System.Drawing.Size(117, 70);
            this.iconLabel.TabIndex = 1;
            this.iconLabel.Text = "ℹ️";
            this.iconLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // accentLine
            // 
            this.accentLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.accentLine.Dock = System.Windows.Forms.DockStyle.Left;
            this.accentLine.Location = new System.Drawing.Point(0, 0);
            this.accentLine.Name = "accentLine";
            this.accentLine.Size = new System.Drawing.Size(3, 518);
            this.accentLine.TabIndex = 0;
            // 
            // _liveTimer
            // 
            this._liveTimer.Interval = 1000;
            this._liveTimer.Tick += new System.EventHandler(this._liveTimer_Tick);
            // 
            // HardwareDetailsDialog
            // 
            this.AcceptButton = this._closeBtn;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CancelButton = this._closeBtn;
            this.ClientSize = new System.Drawing.Size(760, 520);
            this.Controls.Add(this.outerBorder);
            this.Font = new System.Drawing.Font("Bunken Tech Sans Pro Book", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(640, 440);
            this.Name = "HardwareDetailsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HardwareDetailsDialog_FormClosed);
            this.Shown += new System.EventHandler(this.HardwareDetailsDialog_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HardwareDetailsDialog_KeyDown);
            this.outerBorder.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.contentArea.ResumeLayout(false);
            this.cardDetails.ResumeLayout(false);
            this.cardSensors.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.bottomBar.ResumeLayout(false);
            this.titleBar.ResumeLayout(false);
            this.leftMenu.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel outerBorder;
    private System.Windows.Forms.Panel mainPanel;
    private System.Windows.Forms.Panel contentArea;
    private System.Windows.Forms.Panel cardDetails;
    private System.Windows.Forms.RichTextBox _detailsBox;
    private System.Windows.Forms.Panel cardSensors;
    private System.Windows.Forms.RichTextBox _liveBox;
    private System.Windows.Forms.Panel headerPanel;
    private System.Windows.Forms.Label contentHeaderLabel;
    private System.Windows.Forms.Panel bottomBar;
    private System.Windows.Forms.Button _closeBtn;
    private System.Windows.Forms.Button copyAllBtn;
    private System.Windows.Forms.Button _copyBtn;
    private System.Windows.Forms.Panel titleBar;
    private System.Windows.Forms.Label _subtitleLabel;
    private System.Windows.Forms.Label titleLabel;
    private System.Windows.Forms.Button minimizeBtn;
    private System.Windows.Forms.Button closeTopBtn;
    private System.Windows.Forms.Panel leftMenu;
    private System.Windows.Forms.Button _sensorsMenuBtn;
    private System.Windows.Forms.Button _detailsMenuBtn;
    private System.Windows.Forms.Label menuTitle;
    private System.Windows.Forms.Label iconLabel;
    private System.Windows.Forms.Panel accentLine;
    private System.Windows.Forms.Timer _liveTimer;
}