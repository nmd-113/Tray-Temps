namespace TrayTemps
{
    partial class FpsLimiterNoticesDialog
    {
        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.TextBox noticeText;
        private System.Windows.Forms.Button close;

        private void InitializeComponent()
        {
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.noticeText = new System.Windows.Forms.TextBox();
            this.close = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.noticeText, 0, 0);
            this.layout.Controls.Add(this.close, 0, 1);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(12);
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.layout.Size = new System.Drawing.Size(624, 458);
            this.layout.TabIndex = 0;
            // 
            // noticeText
            // 
            this.noticeText.AccessibleName = "Third-party license notices";
            this.noticeText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.noticeText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.noticeText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noticeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noticeText.ForeColor = System.Drawing.Color.LightGray;
            this.noticeText.Location = new System.Drawing.Point(15, 15);
            this.noticeText.Multiline = true;
            this.noticeText.Name = "noticeText";
            this.noticeText.ReadOnly = true;
            this.noticeText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.noticeText.Size = new System.Drawing.Size(594, 384);
            this.noticeText.TabIndex = 0;
            // 
            // close
            // 
            this.close.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.close.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.close.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.close.ForeColor = System.Drawing.Color.LightGray;
            this.close.Location = new System.Drawing.Point(517, 408);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(92, 32);
            this.close.TabIndex = 1;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = false;
            // 
            // FpsLimiterNoticesDialog
            // 
            this.AcceptButton = this.close;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(624, 458);
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 360);
            this.Name = "FpsLimiterNoticesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FPS limiter third-party notices";
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
