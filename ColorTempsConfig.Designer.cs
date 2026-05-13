namespace TrayTemps
{
    partial class ColorTempsConfig
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorTempsConfig));
            this.exitBtn = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.lineLabel = new System.Windows.Forms.Label();
            this.warmTempMax = new System.Windows.Forms.NumericUpDown();
            this.warmTempMin = new System.Windows.Forms.NumericUpDown();
            this.tempsIntervalLabel = new System.Windows.Forms.Label();
            this.saveBtn = new System.Windows.Forms.Button();
            this.colorsetLabel = new System.Windows.Forms.Label();
            this.hotTempColor = new System.Windows.Forms.Button();
            this.warmTempColor = new System.Windows.Forms.Button();
            this.normalTempColor = new System.Windows.Forms.Button();
            this.formTitle = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warmTempMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warmTempMin)).BeginInit();
            this.SuspendLayout();
            // 
            // exitBtn
            // 
            this.exitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.ForeColor = System.Drawing.Color.White;
            this.exitBtn.Location = new System.Drawing.Point(244, 1);
            this.exitBtn.Margin = new System.Windows.Forms.Padding(4);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(55, 44);
            this.exitBtn.TabIndex = 3;
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.mainPanel.Controls.Add(this.lineLabel);
            this.mainPanel.Controls.Add(this.warmTempMax);
            this.mainPanel.Controls.Add(this.warmTempMin);
            this.mainPanel.Controls.Add(this.tempsIntervalLabel);
            this.mainPanel.Controls.Add(this.saveBtn);
            this.mainPanel.Controls.Add(this.colorsetLabel);
            this.mainPanel.Controls.Add(this.hotTempColor);
            this.mainPanel.Controls.Add(this.warmTempColor);
            this.mainPanel.Controls.Add(this.normalTempColor);
            this.mainPanel.Location = new System.Drawing.Point(2, 46);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(296, 202);
            this.mainPanel.TabIndex = 5;
            // 
            // lineLabel
            // 
            this.lineLabel.AutoSize = true;
            this.lineLabel.Location = new System.Drawing.Point(144, 102);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(13, 17);
            this.lineLabel.TabIndex = 8;
            this.lineLabel.Text = "-";
            // 
            // warmTempMax
            // 
            this.warmTempMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.warmTempMax.ForeColor = System.Drawing.Color.White;
            this.warmTempMax.Location = new System.Drawing.Point(174, 98);
            this.warmTempMax.Maximum = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.warmTempMax.Name = "warmTempMax";
            this.warmTempMax.Size = new System.Drawing.Size(74, 25);
            this.warmTempMax.TabIndex = 7;
            this.warmTempMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.warmTempMax.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.warmTempMax.ValueChanged += new System.EventHandler(this.NumericRange_ValueChanged);
            // 
            // warmTempMin
            // 
            this.warmTempMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.warmTempMin.ForeColor = System.Drawing.Color.White;
            this.warmTempMin.Location = new System.Drawing.Point(52, 98);
            this.warmTempMin.Maximum = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.warmTempMin.Name = "warmTempMin";
            this.warmTempMin.Size = new System.Drawing.Size(74, 25);
            this.warmTempMin.TabIndex = 6;
            this.warmTempMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.warmTempMin.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.warmTempMin.ValueChanged += new System.EventHandler(this.NumericRange_ValueChanged);
            // 
            // tempsIntervalLabel
            // 
            this.tempsIntervalLabel.AutoSize = true;
            this.tempsIntervalLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempsIntervalLabel.ForeColor = System.Drawing.Color.LightGray;
            this.tempsIntervalLabel.Location = new System.Drawing.Point(43, 72);
            this.tempsIntervalLabel.Name = "tempsIntervalLabel";
            this.tempsIntervalLabel.Size = new System.Drawing.Size(174, 17);
            this.tempsIntervalLabel.TabIndex = 5;
            this.tempsIntervalLabel.Text = "Temperature interval (Warm)";
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.saveBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBtn.ForeColor = System.Drawing.Color.White;
            this.saveBtn.Location = new System.Drawing.Point(22, 148);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(256, 34);
            this.saveBtn.TabIndex = 4;
            this.saveBtn.Text = "💾 SAVE";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // colorsetLabel
            // 
            this.colorsetLabel.AutoSize = true;
            this.colorsetLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorsetLabel.ForeColor = System.Drawing.Color.LightGray;
            this.colorsetLabel.Location = new System.Drawing.Point(38, 8);
            this.colorsetLabel.Name = "colorsetLabel";
            this.colorsetLabel.Size = new System.Drawing.Size(183, 17);
            this.colorsetLabel.TabIndex = 3;
            this.colorsetLabel.Text = "Set temperature-based colors";
            // 
            // hotTempColor
            // 
            this.hotTempColor.BackColor = System.Drawing.Color.Red;
            this.hotTempColor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.hotTempColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hotTempColor.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hotTempColor.ForeColor = System.Drawing.Color.DimGray;
            this.hotTempColor.Location = new System.Drawing.Point(204, 34);
            this.hotTempColor.Name = "hotTempColor";
            this.hotTempColor.Size = new System.Drawing.Size(74, 25);
            this.hotTempColor.TabIndex = 2;
            this.hotTempColor.Text = "Hot";
            this.hotTempColor.UseVisualStyleBackColor = false;
            this.hotTempColor.Click += new System.EventHandler(this.HotTempColor_Click);
            // 
            // warmTempColor
            // 
            this.warmTempColor.BackColor = System.Drawing.Color.Yellow;
            this.warmTempColor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.warmTempColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.warmTempColor.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warmTempColor.ForeColor = System.Drawing.Color.DimGray;
            this.warmTempColor.Location = new System.Drawing.Point(113, 34);
            this.warmTempColor.Name = "warmTempColor";
            this.warmTempColor.Size = new System.Drawing.Size(74, 25);
            this.warmTempColor.TabIndex = 1;
            this.warmTempColor.Text = "Warm";
            this.warmTempColor.UseVisualStyleBackColor = false;
            this.warmTempColor.Click += new System.EventHandler(this.WarmTempColor_Click);
            // 
            // normalTempColor
            // 
            this.normalTempColor.BackColor = System.Drawing.Color.White;
            this.normalTempColor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.normalTempColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.normalTempColor.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.normalTempColor.ForeColor = System.Drawing.Color.DimGray;
            this.normalTempColor.Location = new System.Drawing.Point(22, 34);
            this.normalTempColor.Name = "normalTempColor";
            this.normalTempColor.Size = new System.Drawing.Size(74, 25);
            this.normalTempColor.TabIndex = 0;
            this.normalTempColor.Text = "Normal";
            this.normalTempColor.UseVisualStyleBackColor = false;
            this.normalTempColor.Click += new System.EventHandler(this.MinTempColor_Click);
            // 
            // formTitle
            // 
            this.formTitle.AutoSize = true;
            this.formTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.formTitle.Location = new System.Drawing.Point(22, 15);
            this.formTitle.Name = "formTitle";
            this.formTitle.Size = new System.Drawing.Size(69, 17);
            this.formTitle.TabIndex = 6;
            this.formTitle.Text = "Configure";
            // 
            // ColorTempsConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.BackgroundImage = global::TrayTemps.Properties.Resources.border;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.formTitle);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.exitBtn);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorTempsConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ColorTempsConfig";
            this.Load += new System.EventHandler(this.ColorTempsConfig_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorTempsConfig_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorTempsConfig_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorTempsConfig_MouseUp);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warmTempMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warmTempMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label formTitle;
        private System.Windows.Forms.Button hotTempColor;
        private System.Windows.Forms.Button warmTempColor;
        private System.Windows.Forms.Button normalTempColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label colorsetLabel;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.NumericUpDown warmTempMax;
        private System.Windows.Forms.NumericUpDown warmTempMin;
        private System.Windows.Forms.Label tempsIntervalLabel;
        private System.Windows.Forms.Label lineLabel;
    }
}