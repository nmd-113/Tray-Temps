using System;

namespace TrayTemps
{
    partial class TrayTemps
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
                components?.Dispose();
                cpuTrayIcon?.Dispose();
                gpuTrayIcon?.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayTemps));
            this.maxLbl = new System.Windows.Forms.Label();
            this.currentLbl = new System.Windows.Forms.Label();
            this.typeLbl = new System.Windows.Forms.Label();
            this.cpuLbl = new System.Windows.Forms.Label();
            this.CpuTemp = new System.Windows.Forms.Label();
            this.CpuMax = new System.Windows.Forms.Label();
            this.nameamountLbl = new System.Windows.Forms.Label();
            this.CpuName = new System.Windows.Forms.Label();
            this.GpuName = new System.Windows.Forms.Label();
            this.gpuLbl = new System.Windows.Forms.Label();
            this.GpuTemp = new System.Windows.Forms.Label();
            this.GpuMax = new System.Windows.Forms.Label();
            this.componentsLbl = new System.Windows.Forms.Label();
            this.temperaturesLbl = new System.Windows.Forms.Label();
            this.traysettingsLbl = new System.Windows.Forms.Label();
            this.CpuTrayEnable = new System.Windows.Forms.CheckBox();
            this.CpuTrayColor = new System.Windows.Forms.ComboBox();
            this.cpucolorLbl = new System.Windows.Forms.Label();
            this.gpucolorLbl = new System.Windows.Forms.Label();
            this.GpuTrayColor = new System.Windows.Forms.ComboBox();
            this.GpuTrayEnable = new System.Windows.Forms.CheckBox();
            this.cpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowForm = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitForm = new System.Windows.Forms.ToolStripMenuItem();
            this.gpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.FontSizeTray = new System.Windows.Forms.NumericUpDown();
            this.fontStyleLbl = new System.Windows.Forms.Label();
            this.UpdateInterval = new System.Windows.Forms.NumericUpDown();
            this.frequencyLbl = new System.Windows.Forms.Label();
            this.exitBtn = new System.Windows.Forms.Button();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.AppTitle = new System.Windows.Forms.Label();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.Logo = new System.Windows.Forms.PictureBox();
            this.autostartApp = new System.Windows.Forms.CheckBox();
            this.FontFamilyTray = new System.Windows.Forms.ComboBox();
            this.fontSizeLbl = new System.Windows.Forms.Label();
            this.MboName = new System.Windows.Forms.Label();
            this.mboLbl = new System.Windows.Forms.Label();
            this.RamAmount = new System.Windows.Forms.Label();
            this.ramLbl = new System.Windows.Forms.Label();
            this.GpuMin = new System.Windows.Forms.Label();
            this.CpuMin = new System.Windows.Forms.Label();
            this.minLbl = new System.Windows.Forms.Label();
            this.cpugpuBack = new System.Windows.Forms.PictureBox();
            this.mboramBack = new System.Windows.Forms.PictureBox();
            this.comNoLbl = new System.Windows.Forms.Label();
            this.GpuSelector = new System.Windows.Forms.ComboBox();
            this.CpuSelector = new System.Windows.Forms.ComboBox();
            this.nameamount2Lbl = new System.Windows.Forms.Label();
            this.type2Lbl = new System.Windows.Forms.Label();
            this.tempsBack = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FontSizeTray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpugpuBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mboramBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tempsBack)).BeginInit();
            this.SuspendLayout();
            // 
            // maxLbl
            // 
            this.maxLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.maxLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxLbl.ForeColor = System.Drawing.Color.White;
            this.maxLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.maxLbl.Location = new System.Drawing.Point(443, 93);
            this.maxLbl.Name = "maxLbl";
            this.maxLbl.Size = new System.Drawing.Size(33, 18);
            this.maxLbl.TabIndex = 1;
            this.maxLbl.Text = "Max";
            this.maxLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currentLbl
            // 
            this.currentLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.currentLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentLbl.ForeColor = System.Drawing.Color.White;
            this.currentLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.currentLbl.Location = new System.Drawing.Point(364, 93);
            this.currentLbl.Name = "currentLbl";
            this.currentLbl.Size = new System.Drawing.Size(33, 18);
            this.currentLbl.TabIndex = 2;
            this.currentLbl.Text = "Now";
            this.currentLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // typeLbl
            // 
            this.typeLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.typeLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.typeLbl.ForeColor = System.Drawing.Color.White;
            this.typeLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.typeLbl.Location = new System.Drawing.Point(20, 93);
            this.typeLbl.Name = "typeLbl";
            this.typeLbl.Size = new System.Drawing.Size(35, 18);
            this.typeLbl.TabIndex = 3;
            this.typeLbl.Text = "Type";
            this.typeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cpuLbl
            // 
            this.cpuLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cpuLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuLbl.ForeColor = System.Drawing.Color.White;
            this.cpuLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cpuLbl.Location = new System.Drawing.Point(20, 119);
            this.cpuLbl.Name = "cpuLbl";
            this.cpuLbl.Size = new System.Drawing.Size(35, 18);
            this.cpuLbl.TabIndex = 7;
            this.cpuLbl.Text = "CPU:";
            this.cpuLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuTemp
            // 
            this.CpuTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuTemp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTemp.ForeColor = System.Drawing.Color.White;
            this.CpuTemp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuTemp.Location = new System.Drawing.Point(364, 119);
            this.CpuTemp.Name = "CpuTemp";
            this.CpuTemp.Size = new System.Drawing.Size(33, 18);
            this.CpuTemp.TabIndex = 6;
            this.CpuTemp.Text = "N/A";
            this.CpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpuMax
            // 
            this.CpuMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuMax.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuMax.ForeColor = System.Drawing.Color.White;
            this.CpuMax.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuMax.Location = new System.Drawing.Point(443, 119);
            this.CpuMax.Name = "CpuMax";
            this.CpuMax.Size = new System.Drawing.Size(33, 18);
            this.CpuMax.TabIndex = 5;
            this.CpuMax.Text = "N/A";
            this.CpuMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nameamountLbl
            // 
            this.nameamountLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.nameamountLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameamountLbl.ForeColor = System.Drawing.Color.White;
            this.nameamountLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nameamountLbl.Location = new System.Drawing.Point(112, 93);
            this.nameamountLbl.Name = "nameamountLbl";
            this.nameamountLbl.Size = new System.Drawing.Size(45, 18);
            this.nameamountLbl.TabIndex = 8;
            this.nameamountLbl.Text = "Name";
            this.nameamountLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuName
            // 
            this.CpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuName.ForeColor = System.Drawing.Color.White;
            this.CpuName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuName.Location = new System.Drawing.Point(112, 119);
            this.CpuName.Name = "CpuName";
            this.CpuName.Size = new System.Drawing.Size(233, 18);
            this.CpuName.TabIndex = 9;
            this.CpuName.Text = "N/A";
            this.CpuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuName
            // 
            this.GpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuName.ForeColor = System.Drawing.Color.White;
            this.GpuName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuName.Location = new System.Drawing.Point(112, 146);
            this.GpuName.Name = "GpuName";
            this.GpuName.Size = new System.Drawing.Size(233, 18);
            this.GpuName.TabIndex = 13;
            this.GpuName.Text = "N/A";
            this.GpuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpuLbl
            // 
            this.gpuLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.gpuLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuLbl.ForeColor = System.Drawing.Color.White;
            this.gpuLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.gpuLbl.Location = new System.Drawing.Point(20, 146);
            this.gpuLbl.Name = "gpuLbl";
            this.gpuLbl.Size = new System.Drawing.Size(35, 18);
            this.gpuLbl.TabIndex = 12;
            this.gpuLbl.Text = "GPU:";
            this.gpuLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuTemp
            // 
            this.GpuTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuTemp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTemp.ForeColor = System.Drawing.Color.White;
            this.GpuTemp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuTemp.Location = new System.Drawing.Point(364, 146);
            this.GpuTemp.Name = "GpuTemp";
            this.GpuTemp.Size = new System.Drawing.Size(33, 18);
            this.GpuTemp.TabIndex = 11;
            this.GpuTemp.Text = "N/A";
            this.GpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GpuMax
            // 
            this.GpuMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuMax.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuMax.ForeColor = System.Drawing.Color.White;
            this.GpuMax.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuMax.Location = new System.Drawing.Point(443, 146);
            this.GpuMax.Name = "GpuMax";
            this.GpuMax.Size = new System.Drawing.Size(33, 18);
            this.GpuMax.TabIndex = 10;
            this.GpuMax.Text = "N/A";
            this.GpuMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // componentsLbl
            // 
            this.componentsLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.componentsLbl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.componentsLbl.ForeColor = System.Drawing.Color.White;
            this.componentsLbl.Location = new System.Drawing.Point(15, 50);
            this.componentsLbl.MinimumSize = new System.Drawing.Size(285, 25);
            this.componentsLbl.Name = "componentsLbl";
            this.componentsLbl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.componentsLbl.Size = new System.Drawing.Size(335, 25);
            this.componentsLbl.TabIndex = 14;
            this.componentsLbl.Text = "Components";
            this.componentsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // temperaturesLbl
            // 
            this.temperaturesLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.temperaturesLbl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.temperaturesLbl.ForeColor = System.Drawing.Color.White;
            this.temperaturesLbl.Location = new System.Drawing.Point(360, 50);
            this.temperaturesLbl.MaximumSize = new System.Drawing.Size(125, 25);
            this.temperaturesLbl.MinimumSize = new System.Drawing.Size(125, 25);
            this.temperaturesLbl.Name = "temperaturesLbl";
            this.temperaturesLbl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.temperaturesLbl.Size = new System.Drawing.Size(125, 25);
            this.temperaturesLbl.TabIndex = 15;
            this.temperaturesLbl.Text = "Temperatures";
            this.temperaturesLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // traysettingsLbl
            // 
            this.traysettingsLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.traysettingsLbl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traysettingsLbl.ForeColor = System.Drawing.Color.White;
            this.traysettingsLbl.Location = new System.Drawing.Point(15, 278);
            this.traysettingsLbl.MinimumSize = new System.Drawing.Size(420, 25);
            this.traysettingsLbl.Name = "traysettingsLbl";
            this.traysettingsLbl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.traysettingsLbl.Size = new System.Drawing.Size(470, 25);
            this.traysettingsLbl.TabIndex = 16;
            this.traysettingsLbl.Text = "Tray Settings";
            this.traysettingsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuTrayEnable
            // 
            this.CpuTrayEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.CpuTrayEnable.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTrayEnable.ForeColor = System.Drawing.Color.White;
            this.CpuTrayEnable.Location = new System.Drawing.Point(15, 317);
            this.CpuTrayEnable.Name = "CpuTrayEnable";
            this.CpuTrayEnable.Size = new System.Drawing.Size(50, 18);
            this.CpuTrayEnable.TabIndex = 17;
            this.CpuTrayEnable.Text = "CPU";
            this.CpuTrayEnable.UseVisualStyleBackColor = false;
            this.CpuTrayEnable.CheckedChanged += new System.EventHandler(this.CpuTrayEnable_CheckedChanged);
            // 
            // CpuTrayColor
            // 
            this.CpuTrayColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuTrayColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CpuTrayColor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTrayColor.ForeColor = System.Drawing.Color.White;
            this.CpuTrayColor.FormattingEnabled = true;
            this.CpuTrayColor.Items.AddRange(new object[] {
            "White",
            "Black",
            "Silver",
            "Gray",
            "Red",
            "OrangeRed",
            "DarkOrange",
            "Gold",
            "LawnGreen",
            "MediumSeaGreen",
            "Teal",
            "Aqua",
            "DeepSkyBlue",
            "DodgerBlue",
            "RoyalBlue",
            "HotPink",
            "Fuchsia",
            "Orchid",
            "DarkViolet",
            "Indigo"});
            this.CpuTrayColor.Location = new System.Drawing.Point(108, 315);
            this.CpuTrayColor.MaxDropDownItems = 20;
            this.CpuTrayColor.Name = "CpuTrayColor";
            this.CpuTrayColor.Size = new System.Drawing.Size(155, 22);
            this.CpuTrayColor.TabIndex = 18;
            this.CpuTrayColor.SelectedIndexChanged += new System.EventHandler(this.CpuTrayColor_SelectedIndexChanged);
            // 
            // cpucolorLbl
            // 
            this.cpucolorLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.cpucolorLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpucolorLbl.ForeColor = System.Drawing.Color.White;
            this.cpucolorLbl.Location = new System.Drawing.Point(69, 317);
            this.cpucolorLbl.Name = "cpucolorLbl";
            this.cpucolorLbl.Size = new System.Drawing.Size(35, 18);
            this.cpucolorLbl.TabIndex = 19;
            this.cpucolorLbl.Text = "Color:";
            this.cpucolorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpucolorLbl
            // 
            this.gpucolorLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.gpucolorLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpucolorLbl.ForeColor = System.Drawing.Color.White;
            this.gpucolorLbl.Location = new System.Drawing.Point(69, 345);
            this.gpucolorLbl.Name = "gpucolorLbl";
            this.gpucolorLbl.Size = new System.Drawing.Size(35, 18);
            this.gpucolorLbl.TabIndex = 24;
            this.gpucolorLbl.Text = "Color:";
            this.gpucolorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuTrayColor
            // 
            this.GpuTrayColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuTrayColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GpuTrayColor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTrayColor.ForeColor = System.Drawing.Color.White;
            this.GpuTrayColor.FormattingEnabled = true;
            this.GpuTrayColor.Items.AddRange(new object[] {
            "White",
            "Black",
            "Silver",
            "Gray",
            "Red",
            "OrangeRed",
            "DarkOrange",
            "Gold",
            "LawnGreen",
            "MediumSeaGreen",
            "Teal",
            "Aqua",
            "DeepSkyBlue",
            "DodgerBlue",
            "RoyalBlue",
            "HotPink",
            "Fuchsia",
            "Orchid",
            "DarkViolet",
            "Indigo"});
            this.GpuTrayColor.Location = new System.Drawing.Point(108, 343);
            this.GpuTrayColor.MaxDropDownItems = 20;
            this.GpuTrayColor.Name = "GpuTrayColor";
            this.GpuTrayColor.Size = new System.Drawing.Size(155, 22);
            this.GpuTrayColor.TabIndex = 23;
            this.GpuTrayColor.SelectedIndexChanged += new System.EventHandler(this.GpuTrayColor_SelectedIndexChanged);
            // 
            // GpuTrayEnable
            // 
            this.GpuTrayEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.GpuTrayEnable.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTrayEnable.ForeColor = System.Drawing.Color.White;
            this.GpuTrayEnable.Location = new System.Drawing.Point(15, 345);
            this.GpuTrayEnable.Name = "GpuTrayEnable";
            this.GpuTrayEnable.Size = new System.Drawing.Size(50, 18);
            this.GpuTrayEnable.TabIndex = 22;
            this.GpuTrayEnable.Text = "GPU";
            this.GpuTrayEnable.UseVisualStyleBackColor = false;
            this.GpuTrayEnable.CheckedChanged += new System.EventHandler(this.GpuTrayEnable_CheckedChanged);
            // 
            // cpuTrayIcon
            // 
            this.cpuTrayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.cpuTrayIcon.Text = "CPU Temp";
            this.cpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CpuTrayIcon_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowForm,
            this.ExitForm});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(104, 48);
            // 
            // ShowForm
            // 
            this.ShowForm.Name = "ShowForm";
            this.ShowForm.Size = new System.Drawing.Size(103, 22);
            this.ShowForm.Text = "Show";
            this.ShowForm.Click += new System.EventHandler(this.ShowForm_Click);
            // 
            // ExitForm
            // 
            this.ExitForm.Name = "ExitForm";
            this.ExitForm.Size = new System.Drawing.Size(103, 22);
            this.ExitForm.Text = "Exit";
            this.ExitForm.Click += new System.EventHandler(this.ExitForm_Click);
            // 
            // gpuTrayIcon
            // 
            this.gpuTrayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.gpuTrayIcon.Text = "GPU Temp";
            this.gpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GpuTrayIcon_MouseDoubleClick);
            // 
            // FontSizeTray
            // 
            this.FontSizeTray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.FontSizeTray.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontSizeTray.ForeColor = System.Drawing.Color.White;
            this.FontSizeTray.Location = new System.Drawing.Point(330, 344);
            this.FontSizeTray.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.FontSizeTray.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.FontSizeTray.Name = "FontSizeTray";
            this.FontSizeTray.Size = new System.Drawing.Size(42, 21);
            this.FontSizeTray.TabIndex = 25;
            this.FontSizeTray.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.FontSizeTray.ValueChanged += new System.EventHandler(this.FontSizeTray_ValueChanged);
            // 
            // fontStyleLbl
            // 
            this.fontStyleLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.fontStyleLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontStyleLbl.ForeColor = System.Drawing.Color.White;
            this.fontStyleLbl.Location = new System.Drawing.Point(270, 317);
            this.fontStyleLbl.Name = "fontStyleLbl";
            this.fontStyleLbl.Size = new System.Drawing.Size(58, 18);
            this.fontStyleLbl.TabIndex = 26;
            this.fontStyleLbl.Text = "Font Style:";
            this.fontStyleLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UpdateInterval
            // 
            this.UpdateInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.UpdateInterval.DecimalPlaces = 2;
            this.UpdateInterval.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateInterval.ForeColor = System.Drawing.Color.White;
            this.UpdateInterval.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.UpdateInterval.Location = new System.Drawing.Point(436, 344);
            this.UpdateInterval.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.UpdateInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.UpdateInterval.Name = "UpdateInterval";
            this.UpdateInterval.Size = new System.Drawing.Size(49, 21);
            this.UpdateInterval.TabIndex = 29;
            this.UpdateInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.UpdateInterval.ValueChanged += new System.EventHandler(this.UpdateInterval_ValueChanged);
            // 
            // frequencyLbl
            // 
            this.frequencyLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.frequencyLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frequencyLbl.ForeColor = System.Drawing.Color.White;
            this.frequencyLbl.Location = new System.Drawing.Point(376, 345);
            this.frequencyLbl.Name = "frequencyLbl";
            this.frequencyLbl.Size = new System.Drawing.Size(58, 18);
            this.frequencyLbl.TabIndex = 30;
            this.frequencyLbl.Text = "Update(s):";
            this.frequencyLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exitBtn
            // 
            this.exitBtn.BackColor = System.Drawing.Color.Chocolate;
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.ForeColor = System.Drawing.Color.White;
            this.exitBtn.Location = new System.Drawing.Point(450, 15);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(35, 20);
            this.exitBtn.TabIndex = 31;
            this.exitBtn.Text = "✖";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.Exit_Click);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeBtn.ForeColor = System.Drawing.Color.White;
            this.minimizeBtn.Location = new System.Drawing.Point(405, 15);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(35, 20);
            this.minimizeBtn.TabIndex = 32;
            this.minimizeBtn.Text = "➖";
            this.minimizeBtn.UseVisualStyleBackColor = false;
            this.minimizeBtn.Click += new System.EventHandler(this.Minimize_Click);
            // 
            // AppTitle
            // 
            this.AppTitle.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppTitle.ForeColor = System.Drawing.Color.AliceBlue;
            this.AppTitle.Location = new System.Drawing.Point(48, 11);
            this.AppTitle.Name = "AppTitle";
            this.AppTitle.Size = new System.Drawing.Size(123, 29);
            this.AppTitle.TabIndex = 33;
            this.AppTitle.Text = "TrayTemps";
            this.AppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.BalloonTipText = "Double click to show.";
            this.NotifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "TrayTemps";
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // Logo
            // 
            this.Logo.BackgroundImage = global::TrayTemps.Properties.Resources.traytemps;
            this.Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Logo.Location = new System.Drawing.Point(15, 11);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(31, 29);
            this.Logo.TabIndex = 34;
            this.Logo.TabStop = false;
            // 
            // autostartApp
            // 
            this.autostartApp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.autostartApp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autostartApp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartApp.ForeColor = System.Drawing.Color.White;
            this.autostartApp.Location = new System.Drawing.Point(405, 281);
            this.autostartApp.Name = "autostartApp";
            this.autostartApp.Size = new System.Drawing.Size(75, 20);
            this.autostartApp.TabIndex = 35;
            this.autostartApp.Text = "Autostart";
            this.autostartApp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autostartApp.UseVisualStyleBackColor = false;
            this.autostartApp.CheckedChanged += new System.EventHandler(this.AutostartApp_CheckedChanged);
            // 
            // FontFamilyTray
            // 
            this.FontFamilyTray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.FontFamilyTray.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FontFamilyTray.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontFamilyTray.ForeColor = System.Drawing.Color.White;
            this.FontFamilyTray.FormattingEnabled = true;
            this.FontFamilyTray.Items.AddRange(new object[] {
            "Arial",
            "Arial Black",
            "Calibri",
            "Consolas",
            "Courier New",
            "Franklin Gothic Medium",
            "Lucida Console",
            "Microsoft Sans Serif",
            "Segoe UI",
            "Tahoma",
            "Trebuchet MS",
            "Verdana"});
            this.FontFamilyTray.Location = new System.Drawing.Point(330, 315);
            this.FontFamilyTray.MaxDropDownItems = 20;
            this.FontFamilyTray.Name = "FontFamilyTray";
            this.FontFamilyTray.Size = new System.Drawing.Size(155, 22);
            this.FontFamilyTray.TabIndex = 36;
            this.FontFamilyTray.SelectedIndexChanged += new System.EventHandler(this.FontFamilyTray_SelectedIndexChanged);
            // 
            // fontSizeLbl
            // 
            this.fontSizeLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.fontSizeLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontSizeLbl.ForeColor = System.Drawing.Color.White;
            this.fontSizeLbl.Location = new System.Drawing.Point(270, 345);
            this.fontSizeLbl.Name = "fontSizeLbl";
            this.fontSizeLbl.Size = new System.Drawing.Size(58, 18);
            this.fontSizeLbl.TabIndex = 37;
            this.fontSizeLbl.Text = "Font Size:";
            this.fontSizeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MboName
            // 
            this.MboName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.MboName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MboName.ForeColor = System.Drawing.Color.White;
            this.MboName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MboName.Location = new System.Drawing.Point(68, 215);
            this.MboName.Name = "MboName";
            this.MboName.Size = new System.Drawing.Size(412, 18);
            this.MboName.TabIndex = 39;
            this.MboName.Text = "N/A";
            this.MboName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mboLbl
            // 
            this.mboLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.mboLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mboLbl.ForeColor = System.Drawing.Color.White;
            this.mboLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mboLbl.Location = new System.Drawing.Point(21, 215);
            this.mboLbl.Name = "mboLbl";
            this.mboLbl.Size = new System.Drawing.Size(40, 18);
            this.mboLbl.TabIndex = 38;
            this.mboLbl.Text = "MBO:";
            this.mboLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RamAmount
            // 
            this.RamAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.RamAmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RamAmount.ForeColor = System.Drawing.Color.White;
            this.RamAmount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.RamAmount.Location = new System.Drawing.Point(68, 242);
            this.RamAmount.Name = "RamAmount";
            this.RamAmount.Size = new System.Drawing.Size(412, 18);
            this.RamAmount.TabIndex = 41;
            this.RamAmount.Text = "N/A";
            this.RamAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ramLbl
            // 
            this.ramLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ramLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramLbl.ForeColor = System.Drawing.Color.White;
            this.ramLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ramLbl.Location = new System.Drawing.Point(21, 242);
            this.ramLbl.Name = "ramLbl";
            this.ramLbl.Size = new System.Drawing.Size(40, 18);
            this.ramLbl.TabIndex = 40;
            this.ramLbl.Text = "RAM:";
            this.ramLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuMin
            // 
            this.GpuMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuMin.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuMin.ForeColor = System.Drawing.Color.White;
            this.GpuMin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuMin.Location = new System.Drawing.Point(404, 146);
            this.GpuMin.Name = "GpuMin";
            this.GpuMin.Size = new System.Drawing.Size(33, 18);
            this.GpuMin.TabIndex = 44;
            this.GpuMin.Text = "N/A";
            this.GpuMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpuMin
            // 
            this.CpuMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuMin.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuMin.ForeColor = System.Drawing.Color.White;
            this.CpuMin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuMin.Location = new System.Drawing.Point(404, 119);
            this.CpuMin.Name = "CpuMin";
            this.CpuMin.Size = new System.Drawing.Size(33, 18);
            this.CpuMin.TabIndex = 43;
            this.CpuMin.Text = "N/A";
            this.CpuMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // minLbl
            // 
            this.minLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.minLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLbl.ForeColor = System.Drawing.Color.White;
            this.minLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.minLbl.Location = new System.Drawing.Point(404, 93);
            this.minLbl.Name = "minLbl";
            this.minLbl.Size = new System.Drawing.Size(33, 18);
            this.minLbl.TabIndex = 42;
            this.minLbl.Text = "Min";
            this.minLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cpugpuBack
            // 
            this.cpugpuBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cpugpuBack.Location = new System.Drawing.Point(15, 87);
            this.cpugpuBack.Name = "cpugpuBack";
            this.cpugpuBack.Size = new System.Drawing.Size(335, 84);
            this.cpugpuBack.TabIndex = 45;
            this.cpugpuBack.TabStop = false;
            // 
            // mboramBack
            // 
            this.mboramBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.mboramBack.Location = new System.Drawing.Point(15, 183);
            this.mboramBack.Name = "mboramBack";
            this.mboramBack.Size = new System.Drawing.Size(470, 84);
            this.mboramBack.TabIndex = 46;
            this.mboramBack.TabStop = false;
            // 
            // comNoLbl
            // 
            this.comNoLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.comNoLbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comNoLbl.ForeColor = System.Drawing.Color.White;
            this.comNoLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.comNoLbl.Location = new System.Drawing.Point(63, 93);
            this.comNoLbl.Name = "comNoLbl";
            this.comNoLbl.Size = new System.Drawing.Size(35, 18);
            this.comNoLbl.TabIndex = 47;
            this.comNoLbl.Text = "No.";
            this.comNoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuSelector
            // 
            this.GpuSelector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GpuSelector.ForeColor = System.Drawing.Color.White;
            this.GpuSelector.FormattingEnabled = true;
            this.GpuSelector.Location = new System.Drawing.Point(63, 145);
            this.GpuSelector.Name = "GpuSelector";
            this.GpuSelector.Size = new System.Drawing.Size(35, 21);
            this.GpuSelector.TabIndex = 50;
            this.GpuSelector.SelectedIndexChanged += new System.EventHandler(this.GpuSelector_SelectedIndexChanged);
            // 
            // CpuSelector
            // 
            this.CpuSelector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuSelector.Cursor = System.Windows.Forms.Cursors.Default;
            this.CpuSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CpuSelector.ForeColor = System.Drawing.Color.White;
            this.CpuSelector.FormattingEnabled = true;
            this.CpuSelector.Location = new System.Drawing.Point(63, 118);
            this.CpuSelector.Name = "CpuSelector";
            this.CpuSelector.Size = new System.Drawing.Size(35, 21);
            this.CpuSelector.TabIndex = 51;
            this.CpuSelector.SelectedIndexChanged += new System.EventHandler(this.CpuSelector_SelectedIndexChanged);
            // 
            // nameamount2Lbl
            // 
            this.nameamount2Lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.nameamount2Lbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameamount2Lbl.ForeColor = System.Drawing.Color.White;
            this.nameamount2Lbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nameamount2Lbl.Location = new System.Drawing.Point(68, 190);
            this.nameamount2Lbl.Name = "nameamount2Lbl";
            this.nameamount2Lbl.Size = new System.Drawing.Size(45, 18);
            this.nameamount2Lbl.TabIndex = 53;
            this.nameamount2Lbl.Text = "Name";
            this.nameamount2Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // type2Lbl
            // 
            this.type2Lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.type2Lbl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.type2Lbl.ForeColor = System.Drawing.Color.White;
            this.type2Lbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.type2Lbl.Location = new System.Drawing.Point(21, 190);
            this.type2Lbl.Name = "type2Lbl";
            this.type2Lbl.Size = new System.Drawing.Size(40, 18);
            this.type2Lbl.TabIndex = 52;
            this.type2Lbl.Text = "Type";
            this.type2Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tempsBack
            // 
            this.tempsBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tempsBack.Location = new System.Drawing.Point(360, 87);
            this.tempsBack.Name = "tempsBack";
            this.tempsBack.Size = new System.Drawing.Size(125, 84);
            this.tempsBack.TabIndex = 54;
            this.tempsBack.TabStop = false;
            // 
            // TrayTemps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(500, 380);
            this.Controls.Add(this.nameamount2Lbl);
            this.Controls.Add(this.type2Lbl);
            this.Controls.Add(this.CpuSelector);
            this.Controls.Add(this.GpuSelector);
            this.Controls.Add(this.comNoLbl);
            this.Controls.Add(this.GpuMin);
            this.Controls.Add(this.CpuMin);
            this.Controls.Add(this.minLbl);
            this.Controls.Add(this.RamAmount);
            this.Controls.Add(this.ramLbl);
            this.Controls.Add(this.MboName);
            this.Controls.Add(this.mboLbl);
            this.Controls.Add(this.GpuTrayColor);
            this.Controls.Add(this.CpuTrayColor);
            this.Controls.Add(this.UpdateInterval);
            this.Controls.Add(this.FontFamilyTray);
            this.Controls.Add(this.FontSizeTray);
            this.Controls.Add(this.fontSizeLbl);
            this.Controls.Add(this.autostartApp);
            this.Controls.Add(this.Logo);
            this.Controls.Add(this.AppTitle);
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.exitBtn);
            this.Controls.Add(this.frequencyLbl);
            this.Controls.Add(this.fontStyleLbl);
            this.Controls.Add(this.gpucolorLbl);
            this.Controls.Add(this.GpuTrayEnable);
            this.Controls.Add(this.cpucolorLbl);
            this.Controls.Add(this.CpuTrayEnable);
            this.Controls.Add(this.traysettingsLbl);
            this.Controls.Add(this.temperaturesLbl);
            this.Controls.Add(this.componentsLbl);
            this.Controls.Add(this.GpuName);
            this.Controls.Add(this.gpuLbl);
            this.Controls.Add(this.GpuTemp);
            this.Controls.Add(this.GpuMax);
            this.Controls.Add(this.CpuName);
            this.Controls.Add(this.nameamountLbl);
            this.Controls.Add(this.cpuLbl);
            this.Controls.Add(this.CpuTemp);
            this.Controls.Add(this.CpuMax);
            this.Controls.Add(this.typeLbl);
            this.Controls.Add(this.currentLbl);
            this.Controls.Add(this.maxLbl);
            this.Controls.Add(this.cpugpuBack);
            this.Controls.Add(this.mboramBack);
            this.Controls.Add(this.tempsBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 380);
            this.MinimumSize = new System.Drawing.Size(500, 380);
            this.Name = "TrayTemps";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tray Temps";
            this.Load += new System.EventHandler(this.TrayTemps_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrayTemps_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FontSizeTray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpugpuBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mboramBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tempsBack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label maxLbl;
        private System.Windows.Forms.Label currentLbl;
        private System.Windows.Forms.Label typeLbl;
        private System.Windows.Forms.Label cpuLbl;
        private System.Windows.Forms.Label CpuTemp;
        private System.Windows.Forms.Label CpuMax;
        private System.Windows.Forms.Label nameamountLbl;
        private System.Windows.Forms.Label CpuName;
        private System.Windows.Forms.Label GpuName;
        private System.Windows.Forms.Label gpuLbl;
        private System.Windows.Forms.Label GpuTemp;
        private System.Windows.Forms.Label GpuMax;
        private System.Windows.Forms.Label componentsLbl;
        private System.Windows.Forms.Label temperaturesLbl;
        private System.Windows.Forms.Label traysettingsLbl;
        private System.Windows.Forms.CheckBox CpuTrayEnable;
        private System.Windows.Forms.ComboBox CpuTrayColor;
        private System.Windows.Forms.Label cpucolorLbl;
        private System.Windows.Forms.Label gpucolorLbl;
        private System.Windows.Forms.ComboBox GpuTrayColor;
        private System.Windows.Forms.CheckBox GpuTrayEnable;
        private System.Windows.Forms.NotifyIcon cpuTrayIcon;
        private System.Windows.Forms.NotifyIcon gpuTrayIcon;
        private System.Windows.Forms.NumericUpDown FontSizeTray;
        private System.Windows.Forms.Label fontStyleLbl;
        private System.Windows.Forms.NumericUpDown UpdateInterval;
        private System.Windows.Forms.Label frequencyLbl;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Button minimizeBtn;
        private System.Windows.Forms.Label AppTitle;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.CheckBox autostartApp;
        private System.Windows.Forms.ComboBox FontFamilyTray;
        private System.Windows.Forms.Label fontSizeLbl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ShowForm;
        private System.Windows.Forms.ToolStripMenuItem ExitForm;
        private System.Windows.Forms.Label MboName;
        private System.Windows.Forms.Label mboLbl;
        private System.Windows.Forms.Label RamAmount;
        private System.Windows.Forms.Label ramLbl;
        private System.Windows.Forms.Label GpuMin;
        private System.Windows.Forms.Label CpuMin;
        private System.Windows.Forms.Label minLbl;
        private System.Windows.Forms.PictureBox cpugpuBack;
        private System.Windows.Forms.PictureBox mboramBack;
        private System.Windows.Forms.Label comNoLbl;
        private System.Windows.Forms.ComboBox GpuSelector;
        private System.Windows.Forms.ComboBox CpuSelector;
        private System.Windows.Forms.Label nameamount2Lbl;
        private System.Windows.Forms.Label type2Lbl;
        private System.Windows.Forms.PictureBox tempsBack;
    }
}