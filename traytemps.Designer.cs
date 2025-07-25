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
            this.max = new System.Windows.Forms.Label();
            this.current = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.Label();
            this.cpu = new System.Windows.Forms.Label();
            this.CpuTemp = new System.Windows.Forms.Label();
            this.CpuMax = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.Label();
            this.CpuName = new System.Windows.Forms.Label();
            this.GpuName = new System.Windows.Forms.Label();
            this.gpu = new System.Windows.Forms.Label();
            this.GpuTemp = new System.Windows.Forms.Label();
            this.GpuMax = new System.Windows.Forms.Label();
            this.component = new System.Windows.Forms.Label();
            this.temperature = new System.Windows.Forms.Label();
            this.tray_settings = new System.Windows.Forms.Label();
            this.CpuTrayEnable = new System.Windows.Forms.CheckBox();
            this.CpuTrayColor = new System.Windows.Forms.ComboBox();
            this.cpucolor_txt = new System.Windows.Forms.Label();
            this.gpucolor_txt = new System.Windows.Forms.Label();
            this.GpuTrayColor = new System.Windows.Forms.ComboBox();
            this.GpuTrayEnable = new System.Windows.Forms.CheckBox();
            this.cpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowForm = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitForm = new System.Windows.Forms.ToolStripMenuItem();
            this.gpuTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.FontSizeTray = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UpdateInterval = new System.Windows.Forms.ComboBox();
            this.frequency = new System.Windows.Forms.Label();
            this.exit = new System.Windows.Forms.Button();
            this.minimize = new System.Windows.Forms.Button();
            this.AppTitle = new System.Windows.Forms.Label();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.Logo = new System.Windows.Forms.PictureBox();
            this.autostartApp = new System.Windows.Forms.CheckBox();
            this.FontFamilyTray = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // max
            // 
            this.max.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.max.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.max.ForeColor = System.Drawing.Color.White;
            this.max.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.max.Location = new System.Drawing.Point(380, 83);
            this.max.Name = "max";
            this.max.Size = new System.Drawing.Size(55, 18);
            this.max.TabIndex = 1;
            this.max.Text = "Max:";
            this.max.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // current
            // 
            this.current.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.current.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.current.ForeColor = System.Drawing.Color.White;
            this.current.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.current.Location = new System.Drawing.Point(295, 83);
            this.current.Name = "current";
            this.current.Size = new System.Drawing.Size(55, 18);
            this.current.TabIndex = 2;
            this.current.Text = "Current:";
            this.current.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // type
            // 
            this.type.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.type.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.type.ForeColor = System.Drawing.Color.White;
            this.type.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.type.Location = new System.Drawing.Point(15, 83);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(40, 18);
            this.type.TabIndex = 3;
            this.type.Text = "Type:";
            this.type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cpu
            // 
            this.cpu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.cpu.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpu.ForeColor = System.Drawing.Color.White;
            this.cpu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cpu.Location = new System.Drawing.Point(15, 106);
            this.cpu.Name = "cpu";
            this.cpu.Size = new System.Drawing.Size(40, 18);
            this.cpu.TabIndex = 7;
            this.cpu.Text = "CPU";
            this.cpu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuTemp
            // 
            this.CpuTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.CpuTemp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTemp.ForeColor = System.Drawing.Color.White;
            this.CpuTemp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuTemp.Location = new System.Drawing.Point(295, 106);
            this.CpuTemp.Name = "CpuTemp";
            this.CpuTemp.Size = new System.Drawing.Size(55, 18);
            this.CpuTemp.TabIndex = 6;
            this.CpuTemp.Text = "CPU";
            this.CpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpuMax
            // 
            this.CpuMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.CpuMax.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuMax.ForeColor = System.Drawing.Color.White;
            this.CpuMax.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuMax.Location = new System.Drawing.Point(380, 106);
            this.CpuMax.Name = "CpuMax";
            this.CpuMax.Size = new System.Drawing.Size(55, 18);
            this.CpuMax.TabIndex = 5;
            this.CpuMax.Text = "CPU";
            this.CpuMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // name
            // 
            this.name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.name.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.ForeColor = System.Drawing.Color.White;
            this.name.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.name.Location = new System.Drawing.Point(62, 83);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(55, 18);
            this.name.TabIndex = 8;
            this.name.Text = "Name:";
            this.name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuName
            // 
            this.CpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.CpuName.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuName.ForeColor = System.Drawing.Color.White;
            this.CpuName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CpuName.Location = new System.Drawing.Point(62, 106);
            this.CpuName.Name = "CpuName";
            this.CpuName.Size = new System.Drawing.Size(208, 18);
            this.CpuName.TabIndex = 9;
            this.CpuName.Text = "CPU Name";
            this.CpuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuName
            // 
            this.GpuName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.GpuName.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuName.ForeColor = System.Drawing.Color.White;
            this.GpuName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuName.Location = new System.Drawing.Point(62, 124);
            this.GpuName.Name = "GpuName";
            this.GpuName.Size = new System.Drawing.Size(208, 18);
            this.GpuName.TabIndex = 13;
            this.GpuName.Text = "GPU Name";
            this.GpuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpu
            // 
            this.gpu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.gpu.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpu.ForeColor = System.Drawing.Color.White;
            this.gpu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.gpu.Location = new System.Drawing.Point(15, 124);
            this.gpu.Name = "gpu";
            this.gpu.Size = new System.Drawing.Size(40, 18);
            this.gpu.TabIndex = 12;
            this.gpu.Text = "GPU";
            this.gpu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuTemp
            // 
            this.GpuTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.GpuTemp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTemp.ForeColor = System.Drawing.Color.White;
            this.GpuTemp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuTemp.Location = new System.Drawing.Point(295, 124);
            this.GpuTemp.Name = "GpuTemp";
            this.GpuTemp.Size = new System.Drawing.Size(55, 18);
            this.GpuTemp.TabIndex = 11;
            this.GpuTemp.Text = "GPU";
            this.GpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GpuMax
            // 
            this.GpuMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.GpuMax.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuMax.ForeColor = System.Drawing.Color.White;
            this.GpuMax.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GpuMax.Location = new System.Drawing.Point(380, 124);
            this.GpuMax.Name = "GpuMax";
            this.GpuMax.Size = new System.Drawing.Size(55, 18);
            this.GpuMax.TabIndex = 10;
            this.GpuMax.Text = "GPU";
            this.GpuMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // component
            // 
            this.component.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.component.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.component.ForeColor = System.Drawing.Color.White;
            this.component.Location = new System.Drawing.Point(15, 50);
            this.component.MaximumSize = new System.Drawing.Size(255, 20);
            this.component.MinimumSize = new System.Drawing.Size(255, 20);
            this.component.Name = "component";
            this.component.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.component.Size = new System.Drawing.Size(255, 20);
            this.component.TabIndex = 14;
            this.component.Text = "Components";
            this.component.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // temperature
            // 
            this.temperature.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.temperature.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.temperature.ForeColor = System.Drawing.Color.White;
            this.temperature.Location = new System.Drawing.Point(295, 50);
            this.temperature.MaximumSize = new System.Drawing.Size(140, 20);
            this.temperature.MinimumSize = new System.Drawing.Size(140, 20);
            this.temperature.Name = "temperature";
            this.temperature.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.temperature.Size = new System.Drawing.Size(140, 20);
            this.temperature.TabIndex = 15;
            this.temperature.Text = "Temperatures";
            this.temperature.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tray_settings
            // 
            this.tray_settings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.tray_settings.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tray_settings.ForeColor = System.Drawing.Color.White;
            this.tray_settings.Location = new System.Drawing.Point(15, 155);
            this.tray_settings.MaximumSize = new System.Drawing.Size(420, 20);
            this.tray_settings.MinimumSize = new System.Drawing.Size(420, 20);
            this.tray_settings.Name = "tray_settings";
            this.tray_settings.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.tray_settings.Size = new System.Drawing.Size(420, 20);
            this.tray_settings.TabIndex = 16;
            this.tray_settings.Text = "Settings";
            this.tray_settings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpuTrayEnable
            // 
            this.CpuTrayEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.CpuTrayEnable.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuTrayEnable.ForeColor = System.Drawing.Color.White;
            this.CpuTrayEnable.Location = new System.Drawing.Point(15, 189);
            this.CpuTrayEnable.Name = "CpuTrayEnable";
            this.CpuTrayEnable.Size = new System.Drawing.Size(71, 18);
            this.CpuTrayEnable.TabIndex = 17;
            this.CpuTrayEnable.Text = "CPU Tray";
            this.CpuTrayEnable.UseVisualStyleBackColor = false;
            this.CpuTrayEnable.CheckedChanged += new System.EventHandler(this.CpuTrayEnable_CheckedChanged);
            // 
            // CpuTrayColor
            // 
            this.CpuTrayColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
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
            this.CpuTrayColor.Location = new System.Drawing.Point(127, 187);
            this.CpuTrayColor.MaxDropDownItems = 20;
            this.CpuTrayColor.Name = "CpuTrayColor";
            this.CpuTrayColor.Size = new System.Drawing.Size(91, 22);
            this.CpuTrayColor.TabIndex = 18;
            this.CpuTrayColor.SelectedIndexChanged += new System.EventHandler(this.CpuTrayColor_SelectedIndexChanged);
            // 
            // cpucolor_txt
            // 
            this.cpucolor_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.cpucolor_txt.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpucolor_txt.ForeColor = System.Drawing.Color.White;
            this.cpucolor_txt.Location = new System.Drawing.Point(92, 189);
            this.cpucolor_txt.Name = "cpucolor_txt";
            this.cpucolor_txt.Size = new System.Drawing.Size(59, 18);
            this.cpucolor_txt.TabIndex = 19;
            this.cpucolor_txt.Text = "Color:";
            this.cpucolor_txt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpucolor_txt
            // 
            this.gpucolor_txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.gpucolor_txt.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpucolor_txt.ForeColor = System.Drawing.Color.White;
            this.gpucolor_txt.Location = new System.Drawing.Point(92, 216);
            this.gpucolor_txt.Name = "gpucolor_txt";
            this.gpucolor_txt.Size = new System.Drawing.Size(59, 18);
            this.gpucolor_txt.TabIndex = 24;
            this.gpucolor_txt.Text = "Color:";
            this.gpucolor_txt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GpuTrayColor
            // 
            this.GpuTrayColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
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
            this.GpuTrayColor.Location = new System.Drawing.Point(127, 214);
            this.GpuTrayColor.MaxDropDownItems = 20;
            this.GpuTrayColor.Name = "GpuTrayColor";
            this.GpuTrayColor.Size = new System.Drawing.Size(91, 22);
            this.GpuTrayColor.TabIndex = 23;
            this.GpuTrayColor.SelectedIndexChanged += new System.EventHandler(this.GpuTrayColor_SelectedIndexChanged);
            // 
            // GpuTrayEnable
            // 
            this.GpuTrayEnable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.GpuTrayEnable.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GpuTrayEnable.ForeColor = System.Drawing.Color.White;
            this.GpuTrayEnable.Location = new System.Drawing.Point(15, 216);
            this.GpuTrayEnable.Name = "GpuTrayEnable";
            this.GpuTrayEnable.Size = new System.Drawing.Size(71, 18);
            this.GpuTrayEnable.TabIndex = 22;
            this.GpuTrayEnable.Text = "GPU Tray";
            this.GpuTrayEnable.UseVisualStyleBackColor = false;
            this.GpuTrayEnable.CheckedChanged += new System.EventHandler(this.GpuTrayEnable_CheckedChanged);
            // 
            // cpuTrayIcon
            // 
            this.cpuTrayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.cpuTrayIcon.Text = "CPU Temp";
            this.cpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cpuTrayIcon_MouseDoubleClick);
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
            this.gpuTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gpuTrayIcon_MouseDoubleClick);
            // 
            // FontSizeTray
            // 
            this.FontSizeTray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.FontSizeTray.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FontSizeTray.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontSizeTray.ForeColor = System.Drawing.Color.White;
            this.FontSizeTray.FormattingEnabled = true;
            this.FontSizeTray.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.FontSizeTray.Location = new System.Drawing.Point(280, 216);
            this.FontSizeTray.MaxDropDownItems = 20;
            this.FontSizeTray.Name = "FontSizeTray";
            this.FontSizeTray.Size = new System.Drawing.Size(40, 22);
            this.FontSizeTray.TabIndex = 25;
            this.FontSizeTray.SelectedIndexChanged += new System.EventHandler(this.FontSizeTray_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(225, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 18);
            this.label1.TabIndex = 26;
            this.label1.Text = "Font Style:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UpdateInterval
            // 
            this.UpdateInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.UpdateInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UpdateInterval.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateInterval.ForeColor = System.Drawing.Color.White;
            this.UpdateInterval.FormattingEnabled = true;
            this.UpdateInterval.Items.AddRange(new object[] {
            "0.10",
            "0.25",
            "0.50",
            "0.75",
            "1.00",
            "1.50",
            "2.00"});
            this.UpdateInterval.Location = new System.Drawing.Point(382, 216);
            this.UpdateInterval.MaxDropDownItems = 10;
            this.UpdateInterval.Name = "UpdateInterval";
            this.UpdateInterval.Size = new System.Drawing.Size(53, 22);
            this.UpdateInterval.TabIndex = 29;
            this.UpdateInterval.SelectedIndexChanged += new System.EventHandler(this.UpdateInterval_SelectedIndexChanged);
            // 
            // frequency
            // 
            this.frequency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.frequency.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frequency.ForeColor = System.Drawing.Color.White;
            this.frequency.Location = new System.Drawing.Point(328, 218);
            this.frequency.Name = "frequency";
            this.frequency.Size = new System.Drawing.Size(51, 18);
            this.frequency.TabIndex = 30;
            this.frequency.Text = "Update(s):";
            this.frequency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exit
            // 
            this.exit.BackColor = System.Drawing.Color.Chocolate;
            this.exit.FlatAppearance.BorderSize = 0;
            this.exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exit.ForeColor = System.Drawing.Color.White;
            this.exit.Location = new System.Drawing.Point(400, 15);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(35, 20);
            this.exit.TabIndex = 31;
            this.exit.Text = "✖";
            this.exit.UseVisualStyleBackColor = false;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // minimize
            // 
            this.minimize.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.minimize.FlatAppearance.BorderSize = 0;
            this.minimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimize.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimize.ForeColor = System.Drawing.Color.White;
            this.minimize.Location = new System.Drawing.Point(355, 15);
            this.minimize.Name = "minimize";
            this.minimize.Size = new System.Drawing.Size(35, 20);
            this.minimize.TabIndex = 32;
            this.minimize.Text = "➖";
            this.minimize.UseVisualStyleBackColor = false;
            this.minimize.Click += new System.EventHandler(this.minimize_Click);
            // 
            // AppTitle
            // 
            this.AppTitle.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppTitle.ForeColor = System.Drawing.Color.White;
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
            this.autostartApp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autostartApp.ForeColor = System.Drawing.Color.White;
            this.autostartApp.Location = new System.Drawing.Point(355, 156);
            this.autostartApp.Name = "autostartApp";
            this.autostartApp.Size = new System.Drawing.Size(75, 18);
            this.autostartApp.TabIndex = 35;
            this.autostartApp.Text = "Autostart";
            this.autostartApp.UseVisualStyleBackColor = false;
            this.autostartApp.CheckedChanged += new System.EventHandler(this.autostartApp_CheckedChanged);
            // 
            // FontFamilyTray
            // 
            this.FontFamilyTray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
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
            this.FontFamilyTray.Location = new System.Drawing.Point(280, 187);
            this.FontFamilyTray.MaxDropDownItems = 20;
            this.FontFamilyTray.Name = "FontFamilyTray";
            this.FontFamilyTray.Size = new System.Drawing.Size(155, 22);
            this.FontFamilyTray.TabIndex = 36;
            this.FontFamilyTray.SelectedIndexChanged += new System.EventHandler(this.FontFamilyTray_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(225, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 18);
            this.label2.TabIndex = 37;
            this.label2.Text = "Font Size:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TrayTemps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(450, 250);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FontFamilyTray);
            this.Controls.Add(this.autostartApp);
            this.Controls.Add(this.Logo);
            this.Controls.Add(this.AppTitle);
            this.Controls.Add(this.minimize);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.UpdateInterval);
            this.Controls.Add(this.frequency);
            this.Controls.Add(this.FontSizeTray);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GpuTrayColor);
            this.Controls.Add(this.CpuTrayColor);
            this.Controls.Add(this.gpucolor_txt);
            this.Controls.Add(this.GpuTrayEnable);
            this.Controls.Add(this.cpucolor_txt);
            this.Controls.Add(this.CpuTrayEnable);
            this.Controls.Add(this.tray_settings);
            this.Controls.Add(this.temperature);
            this.Controls.Add(this.component);
            this.Controls.Add(this.GpuName);
            this.Controls.Add(this.gpu);
            this.Controls.Add(this.GpuTemp);
            this.Controls.Add(this.GpuMax);
            this.Controls.Add(this.CpuName);
            this.Controls.Add(this.name);
            this.Controls.Add(this.cpu);
            this.Controls.Add(this.CpuTemp);
            this.Controls.Add(this.CpuMax);
            this.Controls.Add(this.type);
            this.Controls.Add(this.current);
            this.Controls.Add(this.max);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(450, 250);
            this.MinimumSize = new System.Drawing.Size(450, 250);
            this.Name = "TrayTemps";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tray Temps";
            this.Load += new System.EventHandler(this.TrayTemps_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrayTemps_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label max;
        private System.Windows.Forms.Label current;
        private System.Windows.Forms.Label type;
        private System.Windows.Forms.Label cpu;
        private System.Windows.Forms.Label CpuTemp;
        private System.Windows.Forms.Label CpuMax;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label CpuName;
        private System.Windows.Forms.Label GpuName;
        private System.Windows.Forms.Label gpu;
        private System.Windows.Forms.Label GpuTemp;
        private System.Windows.Forms.Label GpuMax;
        private System.Windows.Forms.Label component;
        private System.Windows.Forms.Label temperature;
        private System.Windows.Forms.Label tray_settings;
        private System.Windows.Forms.CheckBox CpuTrayEnable;
        private System.Windows.Forms.ComboBox CpuTrayColor;
        private System.Windows.Forms.Label cpucolor_txt;
        private System.Windows.Forms.Label gpucolor_txt;
        private System.Windows.Forms.ComboBox GpuTrayColor;
        private System.Windows.Forms.CheckBox GpuTrayEnable;
        private System.Windows.Forms.NotifyIcon cpuTrayIcon;
        private System.Windows.Forms.NotifyIcon gpuTrayIcon;
        private System.Windows.Forms.ComboBox FontSizeTray;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox UpdateInterval;
        private System.Windows.Forms.Label frequency;
        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.Button minimize;
        private System.Windows.Forms.Label AppTitle;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.CheckBox autostartApp;
        private System.Windows.Forms.ComboBox FontFamilyTray;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ShowForm;
        private System.Windows.Forms.ToolStripMenuItem ExitForm;
    }
}