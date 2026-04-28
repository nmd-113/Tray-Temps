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
            this.comNoLbl = new System.Windows.Forms.Label();
            this.GpuSelector = new System.Windows.Forms.ComboBox();
            this.CpuSelector = new System.Windows.Forms.ComboBox();
            this.nameamount2Lbl = new System.Windows.Forms.Label();
            this.type2Lbl = new System.Windows.Forms.Label();
            this.tempsBack = new System.Windows.Forms.PictureBox();
            this.Logo = new System.Windows.Forms.PictureBox();
            this.cpugpuBack = new System.Windows.Forms.PictureBox();
            this.mboramBack = new System.Windows.Forms.PictureBox();
            this.appVersionLbl = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FontSizeTray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tempsBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpugpuBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mboramBack)).BeginInit();
            this.SuspendLayout();
            // 
            // maxLbl
            // 
            this.maxLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.maxLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.maxLbl.Location = new System.Drawing.Point(460, 101);
            this.maxLbl.Name = "maxLbl";
            this.maxLbl.Size = new System.Drawing.Size(35, 15);
            this.maxLbl.TabIndex = 1;
            this.maxLbl.Text = "MAX";
            this.maxLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currentLbl
            // 
            this.currentLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.currentLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.currentLbl.Location = new System.Drawing.Point(360, 101);
            this.currentLbl.Name = "currentLbl";
            this.currentLbl.Size = new System.Drawing.Size(40, 15);
            this.currentLbl.TabIndex = 2;
            this.currentLbl.Text = "NOW";
            this.currentLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // typeLbl
            // 
            this.typeLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.typeLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.typeLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.typeLbl.Location = new System.Drawing.Point(25, 101);
            this.typeLbl.Name = "typeLbl";
            this.typeLbl.Size = new System.Drawing.Size(40, 15);
            this.typeLbl.TabIndex = 3;
            this.typeLbl.Text = "TYPE";
            this.typeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cpuLbl
            // 
            this.cpuLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpuLbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuLbl.ForeColor = System.Drawing.Color.White;
            this.cpuLbl.Location = new System.Drawing.Point(25, 126);
            this.cpuLbl.Name = "cpuLbl";
            this.cpuLbl.Size = new System.Drawing.Size(40, 18);
            this.cpuLbl.TabIndex = 7;
            this.cpuLbl.Text = "CPU";
            this.cpuLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuTemp
            // 
            this.CpuTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CpuTemp.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTemp.ForeColor = System.Drawing.Color.White;
            this.CpuTemp.Location = new System.Drawing.Point(360, 126);
            this.CpuTemp.Name = "CpuTemp";
            this.CpuTemp.Size = new System.Drawing.Size(40, 18);
            this.CpuTemp.TabIndex = 6;
            this.CpuTemp.Text = "N/A";
            this.CpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpuMax
            // 
            this.CpuMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CpuMax.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.CpuMax.Location = new System.Drawing.Point(460, 126);
            this.CpuMax.Name = "CpuMax";
            this.CpuMax.Size = new System.Drawing.Size(35, 18);
            this.CpuMax.TabIndex = 5;
            this.CpuMax.Text = "N/A";
            this.CpuMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nameamountLbl
            // 
            this.nameamountLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.nameamountLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameamountLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nameamountLbl.Location = new System.Drawing.Point(125, 101);
            this.nameamountLbl.Name = "nameamountLbl";
            this.nameamountLbl.Size = new System.Drawing.Size(100, 15);
            this.nameamountLbl.TabIndex = 8;
            this.nameamountLbl.Text = "DEVICE NAME";
            this.nameamountLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuName
            // 
            this.CpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CpuName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuName.ForeColor = System.Drawing.Color.White;
            this.CpuName.Location = new System.Drawing.Point(125, 126);
            this.CpuName.Name = "CpuName";
            this.CpuName.Size = new System.Drawing.Size(210, 18);
            this.CpuName.TabIndex = 9;
            this.CpuName.Text = "N/A";
            this.CpuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuName
            // 
            this.GpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.GpuName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuName.ForeColor = System.Drawing.Color.White;
            this.GpuName.Location = new System.Drawing.Point(125, 156);
            this.GpuName.Name = "GpuName";
            this.GpuName.Size = new System.Drawing.Size(210, 18);
            this.GpuName.TabIndex = 13;
            this.GpuName.Text = "N/A";
            this.GpuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpuLbl
            // 
            this.gpuLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.gpuLbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpuLbl.ForeColor = System.Drawing.Color.White;
            this.gpuLbl.Location = new System.Drawing.Point(25, 156);
            this.gpuLbl.Name = "gpuLbl";
            this.gpuLbl.Size = new System.Drawing.Size(40, 18);
            this.gpuLbl.TabIndex = 12;
            this.gpuLbl.Text = "GPU";
            this.gpuLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuTemp
            // 
            this.GpuTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.GpuTemp.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTemp.ForeColor = System.Drawing.Color.White;
            this.GpuTemp.Location = new System.Drawing.Point(360, 156);
            this.GpuTemp.Name = "GpuTemp";
            this.GpuTemp.Size = new System.Drawing.Size(40, 18);
            this.GpuTemp.TabIndex = 11;
            this.GpuTemp.Text = "N/A";
            this.GpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GpuMax
            // 
            this.GpuMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.GpuMax.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.GpuMax.Location = new System.Drawing.Point(460, 156);
            this.GpuMax.Name = "GpuMax";
            this.GpuMax.Size = new System.Drawing.Size(35, 18);
            this.GpuMax.TabIndex = 10;
            this.GpuMax.Text = "N/A";
            this.GpuMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // componentsLbl
            // 
            this.componentsLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.componentsLbl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.componentsLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.componentsLbl.Location = new System.Drawing.Point(15, 61);
            this.componentsLbl.Name = "componentsLbl";
            this.componentsLbl.Size = new System.Drawing.Size(200, 20);
            this.componentsLbl.TabIndex = 14;
            this.componentsLbl.Text = "HARDWARE COMPONENTS";
            this.componentsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // temperaturesLbl
            // 
            this.temperaturesLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.temperaturesLbl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.temperaturesLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.temperaturesLbl.Location = new System.Drawing.Point(363, 61);
            this.temperaturesLbl.Name = "temperaturesLbl";
            this.temperaturesLbl.Size = new System.Drawing.Size(142, 20);
            this.temperaturesLbl.TabIndex = 15;
            this.temperaturesLbl.Text = "TEMPERATURES";
            this.temperaturesLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // traysettingsLbl
            // 
            this.traysettingsLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.traysettingsLbl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traysettingsLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.traysettingsLbl.Location = new System.Drawing.Point(15, 306);
            this.traysettingsLbl.Name = "traysettingsLbl";
            this.traysettingsLbl.Size = new System.Drawing.Size(200, 20);
            this.traysettingsLbl.TabIndex = 16;
            this.traysettingsLbl.Text = "TRAY SETTINGS";
            this.traysettingsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuTrayEnable
            // 
            this.CpuTrayEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuTrayEnable.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTrayEnable.ForeColor = System.Drawing.Color.White;
            this.CpuTrayEnable.Location = new System.Drawing.Point(15, 337);
            this.CpuTrayEnable.Name = "CpuTrayEnable";
            this.CpuTrayEnable.Size = new System.Drawing.Size(50, 18);
            this.CpuTrayEnable.TabIndex = 17;
            this.CpuTrayEnable.Text = "CPU";
            this.CpuTrayEnable.UseVisualStyleBackColor = false;
            this.CpuTrayEnable.CheckedChanged += new System.EventHandler(this.CpuTrayEnable_CheckedChanged);
            // 
            // CpuTrayColor
            // 
            this.CpuTrayColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CpuTrayColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CpuTrayColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CpuTrayColor.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.CpuTrayColor.Location = new System.Drawing.Point(115, 334);
            this.CpuTrayColor.MaxDropDownItems = 10;
            this.CpuTrayColor.Name = "CpuTrayColor";
            this.CpuTrayColor.Size = new System.Drawing.Size(100, 21);
            this.CpuTrayColor.TabIndex = 18;
            this.CpuTrayColor.SelectedIndexChanged += new System.EventHandler(this.CpuTrayColor_SelectedIndexChanged);
            // 
            // cpucolorLbl
            // 
            this.cpucolorLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cpucolorLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpucolorLbl.ForeColor = System.Drawing.Color.White;
            this.cpucolorLbl.Location = new System.Drawing.Point(75, 335);
            this.cpucolorLbl.Name = "cpucolorLbl";
            this.cpucolorLbl.Size = new System.Drawing.Size(40, 18);
            this.cpucolorLbl.TabIndex = 19;
            this.cpucolorLbl.Text = "Color:";
            this.cpucolorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpucolorLbl
            // 
            this.gpucolorLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.gpucolorLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpucolorLbl.ForeColor = System.Drawing.Color.White;
            this.gpucolorLbl.Location = new System.Drawing.Point(75, 365);
            this.gpucolorLbl.Name = "gpucolorLbl";
            this.gpucolorLbl.Size = new System.Drawing.Size(40, 18);
            this.gpucolorLbl.TabIndex = 24;
            this.gpucolorLbl.Text = "Color:";
            this.gpucolorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuTrayColor
            // 
            this.GpuTrayColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuTrayColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GpuTrayColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GpuTrayColor.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.GpuTrayColor.Location = new System.Drawing.Point(115, 364);
            this.GpuTrayColor.MaxDropDownItems = 10;
            this.GpuTrayColor.Name = "GpuTrayColor";
            this.GpuTrayColor.Size = new System.Drawing.Size(100, 21);
            this.GpuTrayColor.TabIndex = 23;
            this.GpuTrayColor.SelectedIndexChanged += new System.EventHandler(this.GpuTrayColor_SelectedIndexChanged);
            // 
            // GpuTrayEnable
            // 
            this.GpuTrayEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GpuTrayEnable.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTrayEnable.ForeColor = System.Drawing.Color.White;
            this.GpuTrayEnable.Location = new System.Drawing.Point(15, 367);
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
            this.FontSizeTray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.FontSizeTray.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FontSizeTray.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontSizeTray.ForeColor = System.Drawing.Color.White;
            this.FontSizeTray.Location = new System.Drawing.Point(295, 363);
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
            this.FontSizeTray.Size = new System.Drawing.Size(50, 22);
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
            this.fontStyleLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.fontStyleLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontStyleLbl.ForeColor = System.Drawing.Color.White;
            this.fontStyleLbl.Location = new System.Drawing.Point(230, 335);
            this.fontStyleLbl.Name = "fontStyleLbl";
            this.fontStyleLbl.Size = new System.Drawing.Size(65, 18);
            this.fontStyleLbl.TabIndex = 26;
            this.fontStyleLbl.Text = "Font Style:";
            this.fontStyleLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UpdateInterval
            // 
            this.UpdateInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.UpdateInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UpdateInterval.DecimalPlaces = 2;
            this.UpdateInterval.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateInterval.ForeColor = System.Drawing.Color.White;
            this.UpdateInterval.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.UpdateInterval.Location = new System.Drawing.Point(455, 363);
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
            this.UpdateInterval.Size = new System.Drawing.Size(50, 22);
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
            this.frequencyLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.frequencyLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frequencyLbl.ForeColor = System.Drawing.Color.White;
            this.frequencyLbl.Location = new System.Drawing.Point(364, 365);
            this.frequencyLbl.Name = "frequencyLbl";
            this.frequencyLbl.Size = new System.Drawing.Size(90, 18);
            this.frequencyLbl.TabIndex = 30;
            this.frequencyLbl.Text = "Update Interval:";
            this.frequencyLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exitBtn
            // 
            this.exitBtn.BackColor = System.Drawing.Color.Transparent;
            this.exitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.exitBtn.Location = new System.Drawing.Point(468, 2);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(50, 35);
            this.exitBtn.TabIndex = 31;
            this.exitBtn.Text = "✕";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.Exit_Click);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.minimizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.minimizeBtn.Location = new System.Drawing.Point(418, 2);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(50, 35);
            this.minimizeBtn.TabIndex = 32;
            this.minimizeBtn.Text = "—";
            this.minimizeBtn.UseVisualStyleBackColor = false;
            this.minimizeBtn.Click += new System.EventHandler(this.Minimize_Click);
            // 
            // AppTitle
            // 
            this.AppTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppTitle.ForeColor = System.Drawing.Color.White;
            this.AppTitle.Location = new System.Drawing.Point(50, 21);
            this.AppTitle.Name = "AppTitle";
            this.AppTitle.Size = new System.Drawing.Size(86, 20);
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
            // autostartApp
            // 
            this.autostartApp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.autostartApp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autostartApp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartApp.ForeColor = System.Drawing.Color.White;
            this.autostartApp.Location = new System.Drawing.Point(431, 334);
            this.autostartApp.Name = "autostartApp";
            this.autostartApp.Size = new System.Drawing.Size(74, 20);
            this.autostartApp.TabIndex = 35;
            this.autostartApp.Text = "Autostart";
            this.autostartApp.UseVisualStyleBackColor = false;
            this.autostartApp.CheckedChanged += new System.EventHandler(this.AutostartApp_CheckedChanged);
            // 
            // FontFamilyTray
            // 
            this.FontFamilyTray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.FontFamilyTray.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FontFamilyTray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FontFamilyTray.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.FontFamilyTray.Location = new System.Drawing.Point(295, 334);
            this.FontFamilyTray.MaxDropDownItems = 10;
            this.FontFamilyTray.Name = "FontFamilyTray";
            this.FontFamilyTray.Size = new System.Drawing.Size(125, 21);
            this.FontFamilyTray.TabIndex = 36;
            this.FontFamilyTray.SelectedIndexChanged += new System.EventHandler(this.FontFamilyTray_SelectedIndexChanged);
            // 
            // fontSizeLbl
            // 
            this.fontSizeLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.fontSizeLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontSizeLbl.ForeColor = System.Drawing.Color.White;
            this.fontSizeLbl.Location = new System.Drawing.Point(230, 365);
            this.fontSizeLbl.Name = "fontSizeLbl";
            this.fontSizeLbl.Size = new System.Drawing.Size(65, 18);
            this.fontSizeLbl.TabIndex = 37;
            this.fontSizeLbl.Text = "Font Size:";
            this.fontSizeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MboName
            // 
            this.MboName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.MboName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MboName.ForeColor = System.Drawing.Color.White;
            this.MboName.Location = new System.Drawing.Point(125, 241);
            this.MboName.Name = "MboName";
            this.MboName.Size = new System.Drawing.Size(350, 18);
            this.MboName.TabIndex = 39;
            this.MboName.Text = "N/A";
            this.MboName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mboLbl
            // 
            this.mboLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.mboLbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mboLbl.ForeColor = System.Drawing.Color.White;
            this.mboLbl.Location = new System.Drawing.Point(25, 241);
            this.mboLbl.Name = "mboLbl";
            this.mboLbl.Size = new System.Drawing.Size(40, 18);
            this.mboLbl.TabIndex = 38;
            this.mboLbl.Text = "MBO";
            this.mboLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RamAmount
            // 
            this.RamAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.RamAmount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RamAmount.ForeColor = System.Drawing.Color.White;
            this.RamAmount.Location = new System.Drawing.Point(125, 266);
            this.RamAmount.Name = "RamAmount";
            this.RamAmount.Size = new System.Drawing.Size(350, 18);
            this.RamAmount.TabIndex = 41;
            this.RamAmount.Text = "N/A";
            this.RamAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ramLbl
            // 
            this.ramLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ramLbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramLbl.ForeColor = System.Drawing.Color.White;
            this.ramLbl.Location = new System.Drawing.Point(25, 266);
            this.ramLbl.Name = "ramLbl";
            this.ramLbl.Size = new System.Drawing.Size(40, 18);
            this.ramLbl.TabIndex = 40;
            this.ramLbl.Text = "RAM";
            this.ramLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuMin
            // 
            this.GpuMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.GpuMin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.GpuMin.Location = new System.Drawing.Point(410, 156);
            this.GpuMin.Name = "GpuMin";
            this.GpuMin.Size = new System.Drawing.Size(35, 18);
            this.GpuMin.TabIndex = 44;
            this.GpuMin.Text = "N/A";
            this.GpuMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpuMin
            // 
            this.CpuMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CpuMin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.CpuMin.Location = new System.Drawing.Point(410, 126);
            this.CpuMin.Name = "CpuMin";
            this.CpuMin.Size = new System.Drawing.Size(35, 18);
            this.CpuMin.TabIndex = 43;
            this.CpuMin.Text = "N/A";
            this.CpuMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // minLbl
            // 
            this.minLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.minLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.minLbl.Location = new System.Drawing.Point(410, 101);
            this.minLbl.Name = "minLbl";
            this.minLbl.Size = new System.Drawing.Size(35, 15);
            this.minLbl.TabIndex = 42;
            this.minLbl.Text = "MIN";
            this.minLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comNoLbl
            // 
            this.comNoLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.comNoLbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comNoLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.comNoLbl.Location = new System.Drawing.Point(75, 101);
            this.comNoLbl.Name = "comNoLbl";
            this.comNoLbl.Size = new System.Drawing.Size(40, 15);
            this.comNoLbl.TabIndex = 47;
            this.comNoLbl.Text = "NO.";
            this.comNoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuSelector
            // 
            this.GpuSelector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.GpuSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GpuSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GpuSelector.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuSelector.ForeColor = System.Drawing.Color.White;
            this.GpuSelector.FormattingEnabled = true;
            this.GpuSelector.Location = new System.Drawing.Point(75, 155);
            this.GpuSelector.Name = "GpuSelector";
            this.GpuSelector.Size = new System.Drawing.Size(40, 21);
            this.GpuSelector.TabIndex = 50;
            this.GpuSelector.SelectedIndexChanged += new System.EventHandler(this.GpuSelector_SelectedIndexChanged);
            // 
            // CpuSelector
            // 
            this.CpuSelector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.CpuSelector.Cursor = System.Windows.Forms.Cursors.Default;
            this.CpuSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CpuSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CpuSelector.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuSelector.ForeColor = System.Drawing.Color.White;
            this.CpuSelector.FormattingEnabled = true;
            this.CpuSelector.Location = new System.Drawing.Point(75, 125);
            this.CpuSelector.Name = "CpuSelector";
            this.CpuSelector.Size = new System.Drawing.Size(40, 21);
            this.CpuSelector.TabIndex = 51;
            this.CpuSelector.SelectedIndexChanged += new System.EventHandler(this.CpuSelector_SelectedIndexChanged);
            // 
            // nameamount2Lbl
            // 
            this.nameamount2Lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.nameamount2Lbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameamount2Lbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nameamount2Lbl.Location = new System.Drawing.Point(125, 221);
            this.nameamount2Lbl.Name = "nameamount2Lbl";
            this.nameamount2Lbl.Size = new System.Drawing.Size(100, 15);
            this.nameamount2Lbl.TabIndex = 53;
            this.nameamount2Lbl.Text = "DEVICE NAME";
            this.nameamount2Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // type2Lbl
            // 
            this.type2Lbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.type2Lbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.type2Lbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.type2Lbl.Location = new System.Drawing.Point(25, 221);
            this.type2Lbl.Name = "type2Lbl";
            this.type2Lbl.Size = new System.Drawing.Size(40, 15);
            this.type2Lbl.TabIndex = 52;
            this.type2Lbl.Text = "TYPE";
            this.type2Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tempsBack
            // 
            this.tempsBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tempsBack.Location = new System.Drawing.Point(340, 101);
            this.tempsBack.Name = "tempsBack";
            this.tempsBack.Size = new System.Drawing.Size(1, 75);
            this.tempsBack.TabIndex = 54;
            this.tempsBack.TabStop = false;
            // 
            // Logo
            // 
            this.Logo.BackgroundImage = global::TrayTemps.Properties.Resources.traytemps;
            this.Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Logo.Location = new System.Drawing.Point(15, 16);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(30, 30);
            this.Logo.TabIndex = 34;
            this.Logo.TabStop = false;
            // 
            // cpugpuBack
            // 
            this.cpugpuBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cpugpuBack.Location = new System.Drawing.Point(15, 91);
            this.cpugpuBack.Name = "cpugpuBack";
            this.cpugpuBack.Size = new System.Drawing.Size(490, 95);
            this.cpugpuBack.TabIndex = 45;
            this.cpugpuBack.TabStop = false;
            // 
            // mboramBack
            // 
            this.mboramBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.mboramBack.Location = new System.Drawing.Point(15, 211);
            this.mboramBack.Name = "mboramBack";
            this.mboramBack.Size = new System.Drawing.Size(490, 80);
            this.mboramBack.TabIndex = 46;
            this.mboramBack.TabStop = false;
            // 
            // appVersionLbl
            // 
            this.appVersionLbl.AutoSize = true;
            this.appVersionLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.appVersionLbl.Location = new System.Drawing.Point(142, 25);
            this.appVersionLbl.Name = "appVersionLbl";
            this.appVersionLbl.Size = new System.Drawing.Size(0, 13);
            this.appVersionLbl.TabIndex = 55;
            this.appVersionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TrayTemps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.BackgroundImage = global::TrayTemps.Properties.Resources.border;
            this.ClientSize = new System.Drawing.Size(520, 400);
            this.Controls.Add(this.appVersionLbl);
            this.Controls.Add(this.tempsBack);
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
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(520, 400);
            this.MinimumSize = new System.Drawing.Size(520, 400);
            this.Name = "TrayTemps";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tray Temps";
            this.Load += new System.EventHandler(this.TrayTemps_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrayTemps_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FontSizeTray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tempsBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpugpuBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mboramBack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Label appVersionLbl;
    }
}