namespace TrayTemps
{
    partial class PerformanceHistoryDialog
    {
        private System.ComponentModel.IContainer components;
        protected override void Dispose(bool disposing) { if (disposing) { UnsubscribeTheme(); components?.Dispose(); } base.Dispose(disposing); }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.root = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new System.Windows.Forms.Panel();
            this.title = new System.Windows.Forms.Label();
            this.exitBtn = new System.Windows.Forms.Button();
            this.header = new System.Windows.Forms.TableLayoutPanel();
            this.recording = new System.Windows.Forms.CheckBox();
            this.rangeLabel = new System.Windows.Forms.Label();
            this.range = new System.Windows.Forms.ComboBox();
            this.clear = new System.Windows.Forms.Button();
            this.content = new System.Windows.Forms.Panel();
            this.plots = new System.Windows.Forms.TableLayoutPanel();
            this.temperatureGraph = new TrayTemps.HistoryGraphControl();
            this.usageGraph = new TrayTemps.HistoryGraphControl();
            this.fpsGraph = new TrayTemps.HistoryGraphControl();
            this.memoryGraph = new TrayTemps.HistoryGraphControl();
            this.frameGraph = new TrayTemps.HistoryGraphControl();
            this.footer = new System.Windows.Forms.Panel();
            this.resizeGrip = new TrayTemps.WindowResizeGripPanel();
            this.viewTimer = new System.Windows.Forms.Timer(this.components);
            this.help = new System.Windows.Forms.ToolTip(this.components);
            this.root.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.header.SuspendLayout();
            this.content.SuspendLayout();
            this.plots.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            //
            // root
            //
            this.root.ColumnCount = 1;
            this.root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.Controls.Add(this.titleBar, 0, 0);
            this.root.Controls.Add(this.header, 0, 1);
            this.root.Controls.Add(this.content, 0, 2);
            this.root.Controls.Add(this.footer, 0, 3);
            this.root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.root.Location = new System.Drawing.Point(1, 1);
            this.root.Margin = new System.Windows.Forms.Padding(0);
            this.root.Name = "root";
            this.root.RowCount = 4;
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.root.Size = new System.Drawing.Size(898, 698);
            this.root.TabIndex = 0;
            //
            // titleBar
            //
            this.titleBar.Controls.Add(this.title);
            this.titleBar.Controls.Add(this.exitBtn);
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(898, 42);
            this.titleBar.TabIndex = 0;
            this.titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Title_MouseDown);
            //
            // title
            //
            this.title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.title.Location = new System.Drawing.Point(0, 0);
            this.title.Name = "title";
            this.title.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.title.Size = new System.Drawing.Size(856, 42);
            this.title.TabIndex = 0;
            this.title.Tag = "TrayTemps.Font.ExtraBold";
            this.title.Text = "Performance history";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.title.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Title_MouseDown);
            //
            // exitBtn
            //
            this.exitBtn.AccessibleName = "Close performance history";
            this.exitBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.exitBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.exitBtn.Location = new System.Drawing.Point(856, 0);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(42, 42);
            this.exitBtn.TabIndex = 1;
            this.exitBtn.TabStop = false;
            this.exitBtn.Tag = "TrayTemps.Font.ExtraBold";
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.Exit_Click);
            //
            // header
            //
            this.header.ColumnCount = 4;
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.header.Controls.Add(this.recording, 0, 0);
            this.header.Controls.Add(this.rangeLabel, 1, 0);
            this.header.Controls.Add(this.range, 2, 0);
            this.header.Controls.Add(this.clear, 3, 0);
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Location = new System.Drawing.Point(0, 42);
            this.header.Margin = new System.Windows.Forms.Padding(0);
            this.header.Name = "header";
            this.header.Padding = new System.Windows.Forms.Padding(16, 4, 16, 4);
            this.header.RowCount = 1;
            this.header.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.header.Size = new System.Drawing.Size(898, 50);
            this.header.TabIndex = 1;
            //
            // recording
            //
            this.recording.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recording.Location = new System.Drawing.Point(19, 7);
            this.recording.Name = "recording";
            this.recording.Size = new System.Drawing.Size(585, 36);
            this.recording.TabIndex = 0;
            this.recording.Text = "Record history";
            this.help.SetToolTip(this.recording, "Collect while TrayTemps runs, even with this window closed. Off stops collection;" +
        " recorded samples stay in memory.");
            this.recording.CheckedChanged += new System.EventHandler(this.Recording_CheckedChanged);
            //
            // rangeLabel
            //
            this.rangeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rangeLabel.Location = new System.Drawing.Point(610, 4);
            this.rangeLabel.Name = "rangeLabel";
            this.rangeLabel.Size = new System.Drawing.Size(84, 42);
            this.rangeLabel.TabIndex = 1;
            this.rangeLabel.Text = "Show last:";
            this.rangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // range
            //
            this.range.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.range.AutoSize = true;
            this.range.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.range.Items.AddRange(new object[] {
            "1 minute",
            "5 minutes",
            "10 minutes",
            "30 minutes"});
            this.range.Location = new System.Drawing.Point(700, 14);
            this.range.Name = "range";
            this.range.Size = new System.Drawing.Size(104, 21);
            this.range.TabIndex = 1;
            this.help.SetToolTip(this.range, "Display range only. Up to 30 minutes are retained; samples are cleared when TrayT" +
        "emps exits.");
            this.range.SelectedIndexChanged += new System.EventHandler(this.Range_SelectedIndexChanged);
            //
            // clear
            //
            this.clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clear.Location = new System.Drawing.Point(810, 8);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(69, 33);
            this.clear.TabIndex = 2;
            this.clear.Text = "Clear";
            this.clear.Click += new System.EventHandler(this.Clear_Click);
            //
            // content
            //
            this.content.AutoScroll = true;
            this.content.Controls.Add(this.plots);
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Location = new System.Drawing.Point(0, 92);
            this.content.Margin = new System.Windows.Forms.Padding(0);
            this.content.Name = "content";
            this.content.Padding = new System.Windows.Forms.Padding(16);
            this.content.Size = new System.Drawing.Size(898, 576);
            this.content.TabIndex = 2;
            //
            // plots
            //
            this.plots.AutoSize = true;
            this.plots.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.plots.ColumnCount = 1;
            this.plots.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.plots.Controls.Add(this.temperatureGraph, 0, 0);
            this.plots.Controls.Add(this.usageGraph, 0, 1);
            this.plots.Controls.Add(this.fpsGraph, 0, 2);
            this.plots.Controls.Add(this.memoryGraph, 0, 3);
            this.plots.Controls.Add(this.frameGraph, 0, 4);
            this.plots.Dock = System.Windows.Forms.DockStyle.Top;
            this.plots.Location = new System.Drawing.Point(16, 16);
            this.plots.Name = "plots";
            this.plots.RowCount = 5;
            this.plots.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.plots.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.plots.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.plots.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.plots.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.plots.Size = new System.Drawing.Size(849, 655);
            this.plots.TabIndex = 0;
            //
            // temperatureGraph
            //
            this.temperatureGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.temperatureGraph.Location = new System.Drawing.Point(3, 3);
            this.temperatureGraph.MinimumSize = new System.Drawing.Size(0, 110);
            this.temperatureGraph.Name = "temperatureGraph";
            this.temperatureGraph.Size = new System.Drawing.Size(843, 125);
            this.temperatureGraph.TabIndex = 0;
            this.temperatureGraph.Text = "Temperatures";
            //
            // usageGraph
            //
            this.usageGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.usageGraph.Location = new System.Drawing.Point(3, 134);
            this.usageGraph.MinimumSize = new System.Drawing.Size(0, 110);
            this.usageGraph.Name = "usageGraph";
            this.usageGraph.Size = new System.Drawing.Size(843, 125);
            this.usageGraph.TabIndex = 1;
            this.usageGraph.Text = "Usage";
            //
            // fpsGraph
            //
            this.fpsGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.fpsGraph.Location = new System.Drawing.Point(3, 265);
            this.fpsGraph.MinimumSize = new System.Drawing.Size(0, 110);
            this.fpsGraph.Name = "fpsGraph";
            this.fpsGraph.Size = new System.Drawing.Size(843, 125);
            this.fpsGraph.TabIndex = 2;
            this.fpsGraph.Text = "FPS";
            //
            // memoryGraph
            //
            this.memoryGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.memoryGraph.Location = new System.Drawing.Point(3, 396);
            this.memoryGraph.MinimumSize = new System.Drawing.Size(0, 110);
            this.memoryGraph.Name = "memoryGraph";
            this.memoryGraph.Size = new System.Drawing.Size(843, 125);
            this.memoryGraph.TabIndex = 3;
            this.memoryGraph.Text = "Memory";
            //
            // frameGraph
            //
            this.frameGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.frameGraph.Location = new System.Drawing.Point(3, 527);
            this.frameGraph.MinimumSize = new System.Drawing.Size(0, 110);
            this.frameGraph.Name = "frameGraph";
            this.frameGraph.Size = new System.Drawing.Size(843, 125);
            this.frameGraph.TabIndex = 4;
            this.frameGraph.Text = "Frametime";
            //
            // footer
            //
            this.footer.Controls.Add(this.resizeGrip);
            this.footer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footer.Location = new System.Drawing.Point(0, 668);
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(898, 30);
            this.footer.TabIndex = 3;
            //
            // resizeGrip
            //
            this.resizeGrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeGrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.resizeGrip.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeGrip.Location = new System.Drawing.Point(698, -70);
            this.resizeGrip.Name = "resizeGrip";
            this.resizeGrip.Size = new System.Drawing.Size(24, 24);
            this.resizeGrip.TabIndex = 0;
            //
            // viewTimer
            //
            this.viewTimer.Interval = 100;
            this.viewTimer.Tick += new System.EventHandler(this.ViewTimer_Tick);
            //
            // PerformanceHistoryDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(this.root);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "PerformanceHistoryDialog";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Performance history";
            this.root.ResumeLayout(false);
            this.titleBar.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.content.ResumeLayout(false);
            this.content.PerformLayout();
            this.plots.ResumeLayout(false);
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TableLayoutPanel root, header, plots;
        private System.Windows.Forms.Panel titleBar, footer;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Panel content;
        private System.Windows.Forms.CheckBox recording;
        private System.Windows.Forms.Label rangeLabel;
        private System.Windows.Forms.ComboBox range;
        private System.Windows.Forms.Button clear;
        private System.Windows.Forms.Timer viewTimer;
        private System.Windows.Forms.ToolTip help;
        private TrayTemps.HistoryGraphControl temperatureGraph, usageGraph, fpsGraph, memoryGraph, frameGraph;
        private TrayTemps.WindowResizeGripPanel resizeGrip;
    }
}
