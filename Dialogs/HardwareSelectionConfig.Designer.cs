namespace TrayTemps
{
    partial class HardwareSelectionConfig
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HardwareSelectionConfig));
            this.exitBtn = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.saveBtn = new System.Windows.Forms.Button();
            this.sensorSelect = new System.Windows.Forms.ComboBox();
            this.sensorLabel = new System.Windows.Forms.Label();
            this.hardwareSelect = new System.Windows.Forms.ComboBox();
            this.hardwareLabel = new System.Windows.Forms.Label();
            this.formTitle = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // exitBtn
            // 
            this.exitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.exitBtn.Location = new System.Drawing.Point(284, 1);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(46, 44);
            this.exitBtn.TabIndex = 0;
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.Controls.Add(this.saveBtn);
            this.mainPanel.Controls.Add(this.sensorSelect);
            this.mainPanel.Controls.Add(this.sensorLabel);
            this.mainPanel.Controls.Add(this.hardwareSelect);
            this.mainPanel.Controls.Add(this.hardwareLabel);
            this.mainPanel.Location = new System.Drawing.Point(2, 46);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(326, 158);
            this.mainPanel.TabIndex = 1;
            // 
            // saveBtn
            // 
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.saveBtn.Location = new System.Drawing.Point(22, 109);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(282, 34);
            this.saveBtn.TabIndex = 4;
            this.saveBtn.Text = "💾 SAVE";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // sensorSelect
            // 
            this.sensorSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sensorSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sensorSelect.FormattingEnabled = true;
            this.sensorSelect.Location = new System.Drawing.Point(22, 75);
            this.sensorSelect.Name = "sensorSelect";
            this.sensorSelect.Size = new System.Drawing.Size(282, 24);
            this.sensorSelect.TabIndex = 3;
            // 
            // sensorLabel
            // 
            this.sensorLabel.AutoSize = true;
            this.sensorLabel.Location = new System.Drawing.Point(22, 54);
            this.sensorLabel.Name = "sensorLabel";
            this.sensorLabel.Size = new System.Drawing.Size(129, 16);
            this.sensorLabel.TabIndex = 2;
            this.sensorLabel.Text = "Temperature sensor";
            // 
            // hardwareSelect
            // 
            this.hardwareSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hardwareSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hardwareSelect.FormattingEnabled = true;
            this.hardwareSelect.Location = new System.Drawing.Point(22, 26);
            this.hardwareSelect.Name = "hardwareSelect";
            this.hardwareSelect.Size = new System.Drawing.Size(282, 24);
            this.hardwareSelect.TabIndex = 1;
            this.hardwareSelect.SelectedIndexChanged += new System.EventHandler(this.HardwareSelect_SelectedIndexChanged);
            // 
            // hardwareLabel
            // 
            this.hardwareLabel.AutoSize = true;
            this.hardwareLabel.Location = new System.Drawing.Point(22, 5);
            this.hardwareLabel.Name = "hardwareLabel";
            this.hardwareLabel.Size = new System.Drawing.Size(113, 16);
            this.hardwareLabel.TabIndex = 0;
            this.hardwareLabel.Text = "Detected devices";
            // 
            // formTitle
            // 
            this.formTitle.AutoSize = true;
            this.formTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.formTitle.Location = new System.Drawing.Point(22, 15);
            this.formTitle.Name = "formTitle";
            this.formTitle.Size = new System.Drawing.Size(73, 16);
            this.formTitle.TabIndex = 2;
            this.formTitle.Text = "Configure";
            // 
            // HardwareSelectionConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(330, 206);
            this.Controls.Add(this.formTitle);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.exitBtn);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HardwareSelectionConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hardware Selection Config";
            this.Load += new System.EventHandler(this.HardwareSelectionConfig_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HardwareSelectionConfig_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HardwareSelectionConfig_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HardwareSelectionConfig_MouseUp);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.ComboBox sensorSelect;
        private System.Windows.Forms.Label sensorLabel;
        private System.Windows.Forms.ComboBox hardwareSelect;
        private System.Windows.Forms.Label hardwareLabel;
        private System.Windows.Forms.Label formTitle;
    }
}
