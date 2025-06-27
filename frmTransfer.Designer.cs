
namespace TransferUniFLEX
{
    partial class frmTransfer
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
            this.groupBoxDirection = new System.Windows.Forms.GroupBox();
            this.radioButtonReceive = new System.Windows.Forms.RadioButton();
            this.radioButtonSend = new System.Windows.Forms.RadioButton();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelUniFLEXFileName = new System.Windows.Forms.Label();
            this.labelLocalFileName = new System.Windows.Forms.Label();
            this.textBoxUniFLEXFileName = new System.Windows.Forms.TextBox();
            this.textBoxLocalFileName = new System.Windows.Forms.TextBox();
            this.buttonBrowseLocalFileName = new System.Windows.Forms.Button();
            this.labelCOMPort = new System.Windows.Forms.Label();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.buttonBrowseLocalDirectory = new System.Windows.Forms.Button();
            this.textBoxLocalDirName = new System.Windows.Forms.TextBox();
            this.labelLocalDirName = new System.Windows.Forms.Label();
            this.checkBoxFixLineFeeds = new System.Windows.Forms.CheckBox();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.currentFileProgress = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.buttonClearUniFLEXFilename = new System.Windows.Forms.Button();
            this.labelBaudRate = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.checkBoxRecursive = new System.Windows.Forms.CheckBox();
            this.textBoxDirectoryReplaceString = new System.Windows.Forms.TextBox();
            this.groupBoxOperatorEntertainment = new System.Windows.Forms.GroupBox();
            this.buttonClearResponseWindow = new System.Windows.Forms.Button();
            this.labelResponses = new System.Windows.Forms.Label();
            this.textBoxResponses = new System.Windows.Forms.TextBox();
            this.groupBoxTCPIP = new System.Windows.Forms.GroupBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.labelIPAddress = new System.Windows.Forms.Label();
            this.labelIPPort = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.getABlockDevieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxCOMPort = new System.Windows.Forms.GroupBox();
            this.groupBoxMethod = new System.Windows.Forms.GroupBox();
            this.radioButtonTCPIP = new System.Windows.Forms.RadioButton();
            this.radioButtonCOMPort = new System.Windows.Forms.RadioButton();
            this.checkBoxWarningsOff = new System.Windows.Forms.CheckBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxAllowDirectorySelection = new System.Windows.Forms.CheckBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.forceRemoteExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonForceRemoteExit = new System.Windows.Forms.Button();
            this.groupBoxDirection.SuspendLayout();
            this.currentFileProgress.SuspendLayout();
            this.groupBoxOperatorEntertainment.SuspendLayout();
            this.groupBoxTCPIP.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.groupBoxCOMPort.SuspendLayout();
            this.groupBoxMethod.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxDirection
            // 
            this.groupBoxDirection.Controls.Add(this.radioButtonReceive);
            this.groupBoxDirection.Controls.Add(this.radioButtonSend);
            this.groupBoxDirection.Location = new System.Drawing.Point(12, 105);
            this.groupBoxDirection.Name = "groupBoxDirection";
            this.groupBoxDirection.Size = new System.Drawing.Size(201, 72);
            this.groupBoxDirection.TabIndex = 2;
            this.groupBoxDirection.TabStop = false;
            this.groupBoxDirection.Text = "Direction";
            // 
            // radioButtonReceive
            // 
            this.radioButtonReceive.AutoSize = true;
            this.radioButtonReceive.Location = new System.Drawing.Point(22, 44);
            this.radioButtonReceive.Name = "radioButtonReceive";
            this.radioButtonReceive.Size = new System.Drawing.Size(136, 17);
            this.radioButtonReceive.TabIndex = 1;
            this.radioButtonReceive.TabStop = true;
            this.radioButtonReceive.Text = "Receive From UniFLEX";
            this.radioButtonReceive.UseVisualStyleBackColor = true;
            this.radioButtonReceive.CheckedChanged += new System.EventHandler(this.radioButtonReceive_CheckedChanged);
            // 
            // radioButtonSend
            // 
            this.radioButtonSend.AutoSize = true;
            this.radioButtonSend.Checked = true;
            this.radioButtonSend.Location = new System.Drawing.Point(22, 20);
            this.radioButtonSend.Name = "radioButtonSend";
            this.radioButtonSend.Size = new System.Drawing.Size(111, 17);
            this.radioButtonSend.TabIndex = 0;
            this.radioButtonSend.TabStop = true;
            this.radioButtonSend.Text = "Send To UniFLEX";
            this.radioButtonSend.UseVisualStyleBackColor = true;
            this.radioButtonSend.CheckedChanged += new System.EventHandler(this.radioButtonSend_CheckedChanged);
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(425, 25);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 17;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelUniFLEXFileName
            // 
            this.labelUniFLEXFileName.AutoSize = true;
            this.labelUniFLEXFileName.Location = new System.Drawing.Point(12, 211);
            this.labelUniFLEXFileName.Name = "labelUniFLEXFileName";
            this.labelUniFLEXFileName.Size = new System.Drawing.Size(99, 13);
            this.labelUniFLEXFileName.TabIndex = 6;
            this.labelUniFLEXFileName.Text = "UniFLEX File Name";
            // 
            // labelLocalFileName
            // 
            this.labelLocalFileName.AutoSize = true;
            this.labelLocalFileName.Location = new System.Drawing.Point(12, 235);
            this.labelLocalFileName.Name = "labelLocalFileName";
            this.labelLocalFileName.Size = new System.Drawing.Size(83, 13);
            this.labelLocalFileName.TabIndex = 9;
            this.labelLocalFileName.Text = "Local File Name";
            // 
            // textBoxUniFLEXFileName
            // 
            this.textBoxUniFLEXFileName.Location = new System.Drawing.Point(128, 207);
            this.textBoxUniFLEXFileName.Name = "textBoxUniFLEXFileName";
            this.textBoxUniFLEXFileName.Size = new System.Drawing.Size(291, 20);
            this.textBoxUniFLEXFileName.TabIndex = 7;
            this.textBoxUniFLEXFileName.TextChanged += new System.EventHandler(this.textBoxUniFLEXFileName_TextChanged);
            // 
            // textBoxLocalFileName
            // 
            this.textBoxLocalFileName.Location = new System.Drawing.Point(128, 231);
            this.textBoxLocalFileName.Name = "textBoxLocalFileName";
            this.textBoxLocalFileName.Size = new System.Drawing.Size(291, 20);
            this.textBoxLocalFileName.TabIndex = 10;
            this.textBoxLocalFileName.TextChanged += new System.EventHandler(this.textBoxLocalFileName_TextChanged);
            // 
            // buttonBrowseLocalFileName
            // 
            this.buttonBrowseLocalFileName.Location = new System.Drawing.Point(425, 230);
            this.buttonBrowseLocalFileName.Name = "buttonBrowseLocalFileName";
            this.buttonBrowseLocalFileName.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseLocalFileName.TabIndex = 11;
            this.buttonBrowseLocalFileName.Text = "Browse";
            this.buttonBrowseLocalFileName.UseVisualStyleBackColor = true;
            this.buttonBrowseLocalFileName.Click += new System.EventHandler(this.buttonBrowseLocalFileName_Click);
            // 
            // labelCOMPort
            // 
            this.labelCOMPort.AutoSize = true;
            this.labelCOMPort.Location = new System.Drawing.Point(50, 23);
            this.labelCOMPort.Name = "labelCOMPort";
            this.labelCOMPort.Size = new System.Drawing.Size(53, 13);
            this.labelCOMPort.TabIndex = 0;
            this.labelCOMPort.Text = "COM Port";
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.DropDownHeight = 68;
            this.comboBoxCOMPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.IntegralHeight = false;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(109, 19);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(59, 21);
            this.comboBoxCOMPorts.Sorted = true;
            this.comboBoxCOMPorts.TabIndex = 1;
            this.comboBoxCOMPorts.DropDown += new System.EventHandler(this.comboBoxCOMPorts_DropDown);
            this.comboBoxCOMPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOMPorts_SelectedIndexChanged);
            // 
            // buttonBrowseLocalDirectory
            // 
            this.buttonBrowseLocalDirectory.Location = new System.Drawing.Point(425, 254);
            this.buttonBrowseLocalDirectory.Name = "buttonBrowseLocalDirectory";
            this.buttonBrowseLocalDirectory.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseLocalDirectory.TabIndex = 14;
            this.buttonBrowseLocalDirectory.Text = "Browse";
            this.buttonBrowseLocalDirectory.UseVisualStyleBackColor = true;
            this.buttonBrowseLocalDirectory.Click += new System.EventHandler(this.buttonBrowseLocalDirectory_Click);
            // 
            // textBoxLocalDirName
            // 
            this.textBoxLocalDirName.Location = new System.Drawing.Point(128, 255);
            this.textBoxLocalDirName.Name = "textBoxLocalDirName";
            this.textBoxLocalDirName.Size = new System.Drawing.Size(291, 20);
            this.textBoxLocalDirName.TabIndex = 13;
            this.textBoxLocalDirName.TextChanged += new System.EventHandler(this.textBoxLocalDirName_TextChanged);
            // 
            // labelLocalDirName
            // 
            this.labelLocalDirName.AutoSize = true;
            this.labelLocalDirName.Location = new System.Drawing.Point(12, 259);
            this.labelLocalDirName.Name = "labelLocalDirName";
            this.labelLocalDirName.Size = new System.Drawing.Size(109, 13);
            this.labelLocalDirName.TabIndex = 12;
            this.labelLocalDirName.Text = "Local Directory Name";
            // 
            // checkBoxFixLineFeeds
            // 
            this.checkBoxFixLineFeeds.AutoSize = true;
            this.checkBoxFixLineFeeds.Location = new System.Drawing.Point(7, 46);
            this.checkBoxFixLineFeeds.Name = "checkBoxFixLineFeeds";
            this.checkBoxFixLineFeeds.Size = new System.Drawing.Size(263, 17);
            this.checkBoxFixLineFeeds.TabIndex = 1;
            this.checkBoxFixLineFeeds.Text = "Replace <CR><LF> and <LF> with <CR> on Send";
            this.checkBoxFixLineFeeds.UseVisualStyleBackColor = true;
            this.checkBoxFixLineFeeds.CheckedChanged += new System.EventHandler(this.checkBoxFixLineFeeds_CheckedChanged);
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(425, 51);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 18;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(425, 79);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 19;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // currentFileProgress
            // 
            this.currentFileProgress.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.currentFileProgress.Location = new System.Drawing.Point(0, 435);
            this.currentFileProgress.Name = "currentFileProgress";
            this.currentFileProgress.Size = new System.Drawing.Size(509, 22);
            this.currentFileProgress.TabIndex = 15;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel.Text = "Status Bar";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            this.progressBar.Step = 1;
            // 
            // buttonClearUniFLEXFilename
            // 
            this.buttonClearUniFLEXFilename.Location = new System.Drawing.Point(425, 206);
            this.buttonClearUniFLEXFilename.Name = "buttonClearUniFLEXFilename";
            this.buttonClearUniFLEXFilename.Size = new System.Drawing.Size(75, 23);
            this.buttonClearUniFLEXFilename.TabIndex = 8;
            this.buttonClearUniFLEXFilename.Text = "Clear";
            this.buttonClearUniFLEXFilename.UseVisualStyleBackColor = true;
            this.buttonClearUniFLEXFilename.Click += new System.EventHandler(this.buttonClearUniFLEXFilename_Click);
            // 
            // labelBaudRate
            // 
            this.labelBaudRate.AutoSize = true;
            this.labelBaudRate.Location = new System.Drawing.Point(34, 47);
            this.labelBaudRate.Name = "labelBaudRate";
            this.labelBaudRate.Size = new System.Drawing.Size(58, 13);
            this.labelBaudRate.TabIndex = 2;
            this.labelBaudRate.Text = "Baud Rate";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(109, 43);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(59, 21);
            this.comboBoxBaudRate.TabIndex = 3;
            this.comboBoxBaudRate.SelectedIndexChanged += new System.EventHandler(this.comboBoxBaudRate_SelectedIndexChanged);
            // 
            // checkBoxRecursive
            // 
            this.checkBoxRecursive.AutoSize = true;
            this.checkBoxRecursive.Location = new System.Drawing.Point(12, 185);
            this.checkBoxRecursive.Name = "checkBoxRecursive";
            this.checkBoxRecursive.Size = new System.Drawing.Size(74, 17);
            this.checkBoxRecursive.TabIndex = 4;
            this.checkBoxRecursive.Text = "Recursive";
            this.checkBoxRecursive.UseVisualStyleBackColor = true;
            this.checkBoxRecursive.CheckedChanged += new System.EventHandler(this.checkBoxTopLevelOnly_CheckedChanged);
            // 
            // textBoxDirectoryReplaceString
            // 
            this.textBoxDirectoryReplaceString.Location = new System.Drawing.Point(128, 183);
            this.textBoxDirectoryReplaceString.Name = "textBoxDirectoryReplaceString";
            this.textBoxDirectoryReplaceString.Size = new System.Drawing.Size(291, 20);
            this.textBoxDirectoryReplaceString.TabIndex = 5;
            // 
            // groupBoxOperatorEntertainment
            // 
            this.groupBoxOperatorEntertainment.Controls.Add(this.buttonClearResponseWindow);
            this.groupBoxOperatorEntertainment.Controls.Add(this.labelResponses);
            this.groupBoxOperatorEntertainment.Controls.Add(this.textBoxResponses);
            this.groupBoxOperatorEntertainment.Location = new System.Drawing.Point(12, 279);
            this.groupBoxOperatorEntertainment.Name = "groupBoxOperatorEntertainment";
            this.groupBoxOperatorEntertainment.Size = new System.Drawing.Size(488, 149);
            this.groupBoxOperatorEntertainment.TabIndex = 15;
            this.groupBoxOperatorEntertainment.TabStop = false;
            // 
            // buttonClearResponseWindow
            // 
            this.buttonClearResponseWindow.Location = new System.Drawing.Point(11, 54);
            this.buttonClearResponseWindow.Name = "buttonClearResponseWindow";
            this.buttonClearResponseWindow.Size = new System.Drawing.Size(75, 23);
            this.buttonClearResponseWindow.TabIndex = 1;
            this.buttonClearResponseWindow.Text = "Clear";
            this.buttonClearResponseWindow.UseVisualStyleBackColor = true;
            this.buttonClearResponseWindow.Click += new System.EventHandler(this.buttonClearResponseWindow_Click);
            // 
            // labelResponses
            // 
            this.labelResponses.AutoSize = true;
            this.labelResponses.Location = new System.Drawing.Point(19, 25);
            this.labelResponses.Name = "labelResponses";
            this.labelResponses.Size = new System.Drawing.Size(57, 13);
            this.labelResponses.TabIndex = 0;
            this.labelResponses.Text = "Resonses:";
            // 
            // textBoxResponses
            // 
            this.textBoxResponses.AcceptsReturn = true;
            this.textBoxResponses.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxResponses.Location = new System.Drawing.Point(101, 21);
            this.textBoxResponses.Multiline = true;
            this.textBoxResponses.Name = "textBoxResponses";
            this.textBoxResponses.ReadOnly = true;
            this.textBoxResponses.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResponses.Size = new System.Drawing.Size(376, 112);
            this.textBoxResponses.TabIndex = 2;
            // 
            // groupBoxTCPIP
            // 
            this.groupBoxTCPIP.Controls.Add(this.textBoxPort);
            this.groupBoxTCPIP.Controls.Add(this.textBoxIPAddress);
            this.groupBoxTCPIP.Controls.Add(this.labelIPAddress);
            this.groupBoxTCPIP.Controls.Add(this.labelIPPort);
            this.groupBoxTCPIP.Location = new System.Drawing.Point(203, 279);
            this.groupBoxTCPIP.Name = "groupBoxTCPIP";
            this.groupBoxTCPIP.Size = new System.Drawing.Size(200, 72);
            this.groupBoxTCPIP.TabIndex = 16;
            this.groupBoxTCPIP.TabStop = false;
            this.groupBoxTCPIP.Text = "TCPIP Setup";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(84, 44);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxPort.TabIndex = 19;
            this.textBoxPort.TextChanged += new System.EventHandler(this.textBoxPort_TextChanged);
            // 
            // textBoxIPAddress
            // 
            this.textBoxIPAddress.Location = new System.Drawing.Point(84, 19);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(100, 20);
            this.textBoxIPAddress.TabIndex = 18;
            this.textBoxIPAddress.TextChanged += new System.EventHandler(this.textBoxIPAddress_TextChanged);
            // 
            // labelIPAddress
            // 
            this.labelIPAddress.AutoSize = true;
            this.labelIPAddress.Location = new System.Drawing.Point(20, 23);
            this.labelIPAddress.Name = "labelIPAddress";
            this.labelIPAddress.Size = new System.Drawing.Size(58, 13);
            this.labelIPAddress.TabIndex = 7;
            this.labelIPAddress.Text = "IP Address";
            // 
            // labelIPPort
            // 
            this.labelIPPort.AutoSize = true;
            this.labelIPPort.Location = new System.Drawing.Point(20, 47);
            this.labelIPPort.Name = "labelIPPort";
            this.labelIPPort.Size = new System.Drawing.Size(26, 13);
            this.labelIPPort.TabIndex = 17;
            this.labelIPPort.Text = "Port";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(425, 180);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(75, 23);
            this.buttonHelp.TabIndex = 20;
            this.buttonHelp.Text = "&Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(509, 24);
            this.menuStripMain.TabIndex = 22;
            this.menuStripMain.Text = "Menu Strip Main";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripSeparator3,
            this.getABlockDevieToolStripMenuItem,
            this.toolStripSeparator1,
            this.forceRemoteExitToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startToolStripMenuItem.Text = "&Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pauseToolStripMenuItem.Text = "&Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stopToolStripMenuItem.Text = "S&top";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // getABlockDevieToolStripMenuItem
            // 
            this.getABlockDevieToolStripMenuItem.Name = "getABlockDevieToolStripMenuItem";
            this.getABlockDevieToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.getABlockDevieToolStripMenuItem.Text = "Get a Block Device";
            this.getABlockDevieToolStripMenuItem.Click += new System.EventHandler(this.getABlockDevieToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // groupBoxCOMPort
            // 
            this.groupBoxCOMPort.Controls.Add(this.labelCOMPort);
            this.groupBoxCOMPort.Controls.Add(this.comboBoxCOMPorts);
            this.groupBoxCOMPort.Controls.Add(this.labelBaudRate);
            this.groupBoxCOMPort.Controls.Add(this.comboBoxBaudRate);
            this.groupBoxCOMPort.Location = new System.Drawing.Point(219, 28);
            this.groupBoxCOMPort.Name = "groupBoxCOMPort";
            this.groupBoxCOMPort.Size = new System.Drawing.Size(200, 72);
            this.groupBoxCOMPort.TabIndex = 1;
            this.groupBoxCOMPort.TabStop = false;
            this.groupBoxCOMPort.Text = "COM Port Setup";
            // 
            // groupBoxMethod
            // 
            this.groupBoxMethod.Controls.Add(this.radioButtonTCPIP);
            this.groupBoxMethod.Controls.Add(this.radioButtonCOMPort);
            this.groupBoxMethod.Location = new System.Drawing.Point(12, 28);
            this.groupBoxMethod.Name = "groupBoxMethod";
            this.groupBoxMethod.Size = new System.Drawing.Size(201, 70);
            this.groupBoxMethod.TabIndex = 0;
            this.groupBoxMethod.TabStop = false;
            this.groupBoxMethod.Text = "Transmission Method";
            // 
            // radioButtonTCPIP
            // 
            this.radioButtonTCPIP.AutoSize = true;
            this.radioButtonTCPIP.Location = new System.Drawing.Point(14, 46);
            this.radioButtonTCPIP.Name = "radioButtonTCPIP";
            this.radioButtonTCPIP.Size = new System.Drawing.Size(185, 17);
            this.radioButtonTCPIP.TabIndex = 1;
            this.radioButtonTCPIP.TabStop = true;
            this.radioButtonTCPIP.Text = "TCPIP - uses CPU09GPP/09NET";
            this.radioButtonTCPIP.UseVisualStyleBackColor = true;
            this.radioButtonTCPIP.CheckedChanged += new System.EventHandler(this.radioButtonTCPIP_CheckedChanged);
            // 
            // radioButtonCOMPort
            // 
            this.radioButtonCOMPort.AutoSize = true;
            this.radioButtonCOMPort.Location = new System.Drawing.Point(14, 20);
            this.radioButtonCOMPort.Name = "radioButtonCOMPort";
            this.radioButtonCOMPort.Size = new System.Drawing.Size(160, 17);
            this.radioButtonCOMPort.TabIndex = 0;
            this.radioButtonCOMPort.TabStop = true;
            this.radioButtonCOMPort.Text = "COM Port - uses CPU09SR4";
            this.radioButtonCOMPort.UseVisualStyleBackColor = true;
            this.radioButtonCOMPort.CheckedChanged += new System.EventHandler(this.radioButtonCOMPort_CheckedChanged);
            // 
            // checkBoxWarningsOff
            // 
            this.checkBoxWarningsOff.AutoSize = true;
            this.checkBoxWarningsOff.Location = new System.Drawing.Point(7, 20);
            this.checkBoxWarningsOff.Name = "checkBoxWarningsOff";
            this.checkBoxWarningsOff.Size = new System.Drawing.Size(113, 17);
            this.checkBoxWarningsOff.TabIndex = 0;
            this.checkBoxWarningsOff.Text = "Turn Warnings Off";
            this.checkBoxWarningsOff.UseVisualStyleBackColor = true;
            this.checkBoxWarningsOff.CheckedChanged += new System.EventHandler(this.checkBoxWarningsOff_CheckedChanged);
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.buttonForceRemoteExit);
            this.groupBoxOptions.Controls.Add(this.checkBoxWarningsOff);
            this.groupBoxOptions.Controls.Add(this.checkBoxFixLineFeeds);
            this.groupBoxOptions.Location = new System.Drawing.Point(219, 105);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(279, 70);
            this.groupBoxOptions.TabIndex = 3;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Options";
            // 
            // checkBoxAllowDirectorySelection
            // 
            this.checkBoxAllowDirectorySelection.AutoSize = true;
            this.checkBoxAllowDirectorySelection.Location = new System.Drawing.Point(92, 185);
            this.checkBoxAllowDirectorySelection.Name = "checkBoxAllowDirectorySelection";
            this.checkBoxAllowDirectorySelection.Size = new System.Drawing.Size(143, 17);
            this.checkBoxAllowDirectorySelection.TabIndex = 23;
            this.checkBoxAllowDirectorySelection.Text = "Allow Directory Selection";
            this.checkBoxAllowDirectorySelection.UseVisualStyleBackColor = true;
            this.checkBoxAllowDirectorySelection.Visible = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // forceRemoteExitToolStripMenuItem
            // 
            this.forceRemoteExitToolStripMenuItem.Name = "forceRemoteExitToolStripMenuItem";
            this.forceRemoteExitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.forceRemoteExitToolStripMenuItem.Text = "&Force Remote Exit";
            this.forceRemoteExitToolStripMenuItem.Click += new System.EventHandler(this.forceRemoteExitToolStripMenuItem_Click);
            // 
            // buttonForceRemoteExit
            // 
            this.buttonForceRemoteExit.Location = new System.Drawing.Point(206, 14);
            this.buttonForceRemoteExit.Name = "buttonForceRemoteExit";
            this.buttonForceRemoteExit.Size = new System.Drawing.Size(75, 23);
            this.buttonForceRemoteExit.TabIndex = 2;
            this.buttonForceRemoteExit.Text = "Kill Remote";
            this.buttonForceRemoteExit.UseVisualStyleBackColor = true;
            this.buttonForceRemoteExit.Click += new System.EventHandler(this.buttonForceRemoteExit_Click);
            // 
            // frmTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 457);
            this.Controls.Add(this.checkBoxAllowDirectorySelection);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBoxTCPIP);
            this.Controls.Add(this.groupBoxMethod);
            this.Controls.Add(this.groupBoxCOMPort);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.groupBoxOperatorEntertainment);
            this.Controls.Add(this.textBoxDirectoryReplaceString);
            this.Controls.Add(this.checkBoxRecursive);
            this.Controls.Add(this.buttonClearUniFLEXFilename);
            this.Controls.Add(this.currentFileProgress);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.textBoxLocalDirName);
            this.Controls.Add(this.labelLocalDirName);
            this.Controls.Add(this.buttonBrowseLocalDirectory);
            this.Controls.Add(this.buttonBrowseLocalFileName);
            this.Controls.Add(this.textBoxLocalFileName);
            this.Controls.Add(this.textBoxUniFLEXFileName);
            this.Controls.Add(this.labelLocalFileName);
            this.Controls.Add(this.labelUniFLEXFileName);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.groupBoxDirection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "frmTransfer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer To/From UniFLEX";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTransfer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTransfer_FormClosed);
            this.Load += new System.EventHandler(this.frmTransfer_Load);
            this.groupBoxDirection.ResumeLayout(false);
            this.groupBoxDirection.PerformLayout();
            this.currentFileProgress.ResumeLayout(false);
            this.currentFileProgress.PerformLayout();
            this.groupBoxOperatorEntertainment.ResumeLayout(false);
            this.groupBoxOperatorEntertainment.PerformLayout();
            this.groupBoxTCPIP.ResumeLayout(false);
            this.groupBoxTCPIP.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.groupBoxCOMPort.ResumeLayout(false);
            this.groupBoxCOMPort.PerformLayout();
            this.groupBoxMethod.ResumeLayout(false);
            this.groupBoxMethod.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxDirection;
        private System.Windows.Forms.RadioButton radioButtonReceive;
        private System.Windows.Forms.RadioButton radioButtonSend;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelUniFLEXFileName;
        private System.Windows.Forms.Label labelLocalFileName;
        private System.Windows.Forms.TextBox textBoxUniFLEXFileName;
        private System.Windows.Forms.TextBox textBoxLocalFileName;
        private System.Windows.Forms.Button buttonBrowseLocalFileName;
        private System.Windows.Forms.Label labelCOMPort;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Button buttonBrowseLocalDirectory;
        private System.Windows.Forms.TextBox textBoxLocalDirName;
        private System.Windows.Forms.Label labelLocalDirName;
        private System.Windows.Forms.CheckBox checkBoxFixLineFeeds;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.StatusStrip currentFileProgress;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button buttonClearUniFLEXFilename;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.CheckBox checkBoxRecursive;
        private System.Windows.Forms.TextBox textBoxDirectoryReplaceString;
        private System.Windows.Forms.GroupBox groupBoxOperatorEntertainment;
        private System.Windows.Forms.Label labelResponses;
        private System.Windows.Forms.TextBox textBoxResponses;
        private System.Windows.Forms.Button buttonClearResponseWindow;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxCOMPort;
        private System.Windows.Forms.GroupBox groupBoxMethod;
        private System.Windows.Forms.RadioButton radioButtonTCPIP;
        private System.Windows.Forms.RadioButton radioButtonCOMPort;
        private System.Windows.Forms.GroupBox groupBoxTCPIP;
        private System.Windows.Forms.TextBox textBoxIPAddress;
        private System.Windows.Forms.Label labelIPAddress;
        private System.Windows.Forms.Label labelIPPort;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.CheckBox checkBoxWarningsOff;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem getABlockDevieToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxAllowDirectorySelection;
        private System.Windows.Forms.ToolStripMenuItem forceRemoteExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button buttonForceRemoteExit;
    }
}

