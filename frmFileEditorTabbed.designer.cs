
namespace TransferUniFLEX
{
    partial class frmFileEditorTabbed
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.gotoLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelSelectedFont = new System.Windows.Forms.Label();
            this.buttonSelectFont = new System.Windows.Forms.Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.panelLineNumbers = new System.Windows.Forms.Panel();
            this.checkBoxLineNumbers = new System.Windows.Forms.CheckBox();
            this.labelSelectedFontSize = new System.Windows.Forms.Label();
            this.richTextBox = new TransferUniFLEX.RichTextBoxWithTab();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(534, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToImageToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Enabled = false;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Visible = false;
            //this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            //this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            //this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // saveToImageToolStripMenuItem
            // 
            this.saveToImageToolStripMenuItem.Name = "saveToImageToolStripMenuItem";
            this.saveToImageToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.saveToImageToolStripMenuItem.Text = "Save To &Image";
            //this.saveToImageToolStripMenuItem.Click += new System.EventHandler(this.saveToImageToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            //this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.findPreviousToolStripMenuItem,
            this.toolStripSeparator3,
            this.gotoLineToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.findToolStripMenuItem.Text = "&Find";
            //this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Enabled = false;
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.findNextToolStripMenuItem.Text = "Find &Next";
            //this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // findPreviousToolStripMenuItem
            // 
            this.findPreviousToolStripMenuItem.Enabled = false;
            this.findPreviousToolStripMenuItem.Name = "findPreviousToolStripMenuItem";
            this.findPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPreviousToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.findPreviousToolStripMenuItem.Text = "Find &Previous";
            //this.findPreviousToolStripMenuItem.Click += new System.EventHandler(this.findPreviousToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
            // 
            // gotoLineToolStripMenuItem
            // 
            this.gotoLineToolStripMenuItem.Name = "gotoLineToolStripMenuItem";
            this.gotoLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoLineToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.gotoLineToolStripMenuItem.Text = "&Goto Line";
            //this.gotoLineToolStripMenuItem.Click += new System.EventHandler(this.gotoLineToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineNumbersToolStripMenuItem,
            this.statusBarToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            //this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // lineNumbersToolStripMenuItem
            // 
            this.lineNumbersToolStripMenuItem.Name = "lineNumbersToolStripMenuItem";
            this.lineNumbersToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.lineNumbersToolStripMenuItem.Text = "&Line Numbers";
            //this.lineNumbersToolStripMenuItem.Click += new System.EventHandler(this.lineNumbersToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.statusBarToolStripMenuItem.Text = "&Status Bar";
            //this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.statusBarToolStripMenuItem_Click);
            // 
            // labelSelectedFont
            // 
            this.labelSelectedFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedFont.AutoSize = true;
            this.labelSelectedFont.Location = new System.Drawing.Point(365, 35);
            this.labelSelectedFont.Name = "labelSelectedFont";
            this.labelSelectedFont.Size = new System.Drawing.Size(16, 13);
            this.labelSelectedFont.TabIndex = 3;
            this.labelSelectedFont.Text = "   ";
            // 
            // buttonSelectFont
            // 
            this.buttonSelectFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectFont.Location = new System.Drawing.Point(259, 30);
            this.buttonSelectFont.Name = "buttonSelectFont";
            this.buttonSelectFont.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectFont.TabIndex = 4;
            this.buttonSelectFont.Text = "Select Font";
            this.buttonSelectFont.UseVisualStyleBackColor = true;
            this.buttonSelectFont.Click += new System.EventHandler(this.buttonSelectFont_Click);
            // 
            // panelLineNumbers
            // 
            this.panelLineNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelLineNumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLineNumbers.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelLineNumbers.Location = new System.Drawing.Point(10, 59);
            this.panelLineNumbers.Name = "panelLineNumbers";
            this.panelLineNumbers.Size = new System.Drawing.Size(40, 462);
            this.panelLineNumbers.TabIndex = 5;
            this.panelLineNumbers.Paint += new System.Windows.Forms.PaintEventHandler(this.panelLineNumbers_Paint);
            // 
            // checkBoxLineNumbers
            // 
            this.checkBoxLineNumbers.AutoSize = true;
            this.checkBoxLineNumbers.Location = new System.Drawing.Point(12, 33);
            this.checkBoxLineNumbers.Name = "checkBoxLineNumbers";
            this.checkBoxLineNumbers.Size = new System.Drawing.Size(91, 17);
            this.checkBoxLineNumbers.TabIndex = 6;
            this.checkBoxLineNumbers.Text = "Line Numbers";
            this.checkBoxLineNumbers.UseVisualStyleBackColor = true;
            this.checkBoxLineNumbers.CheckedChanged += new System.EventHandler(this.checkBoxLineNumbers_CheckedChanged);
            // 
            // labelSelectedFontSize
            // 
            this.labelSelectedFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedFontSize.AutoSize = true;
            this.labelSelectedFontSize.Location = new System.Drawing.Point(454, 35);
            this.labelSelectedFontSize.Name = "labelSelectedFontSize";
            this.labelSelectedFontSize.Size = new System.Drawing.Size(19, 13);
            this.labelSelectedFontSize.TabIndex = 7;
            this.labelSelectedFontSize.Text = "    ";
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox.Location = new System.Drawing.Point(50, 59);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(474, 462);
            this.richTextBox.TabIndex = 1;
            this.richTextBox.TabSize = 4;
            this.richTextBox.Text = "";
            this.richTextBox.WordWrap = false;
            this.richTextBox.SelectionChanged += new System.EventHandler(this.richTextBox_SelectionChanged);
            this.richTextBox.FontChanged += new System.EventHandler(this.richTextBox_FontChanged);
            this.richTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBoxSource_KeyDown);
            this.richTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.richTextBox_KeyUp);
            this.richTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBox_MouseDown);
            this.richTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.richTextBox_MouseMove);
            this.richTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBox_MouseUp);
            // 
            // frmFileEditorTabbed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 533);
            this.Controls.Add(this.labelSelectedFontSize);
            this.Controls.Add(this.checkBoxLineNumbers);
            this.Controls.Add(this.panelLineNumbers);
            this.Controls.Add(this.buttonSelectFont);
            this.Controls.Add(this.labelSelectedFont);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmFileEditorTabbed";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Internal Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFileEditorTabbed_FormClosed);
            this.Load += new System.EventHandler(this.frmFileEditorTabbed_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private RichTextBoxWithTab richTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.Label labelSelectedFont;
        private System.Windows.Forms.Button buttonSelectFont;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Panel panelLineNumbers;
        private System.Windows.Forms.CheckBox checkBoxLineNumbers;
        private System.Windows.Forms.Label labelSelectedFontSize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem gotoLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineNumbersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPreviousToolStripMenuItem;
    }
}