
namespace TransferUniFLEX
{
    partial class frmUniFLEXBrowse
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
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.columnHeaderDev = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIno = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLinks = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderUID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelNotice = new System.Windows.Forms.Label();
            this.labelCurrentWorkingDirectory = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewFiles
            // 
            this.listViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderDev,
            this.columnHeaderIno,
            this.columnHeaderMode,
            this.columnHeaderLinks,
            this.columnHeaderUID,
            this.columnHeaderSize,
            this.columnHeaderDateTime,
            this.columnHeaderFilename});
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new System.Drawing.Point(12, 55);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(549, 314);
            this.listViewFiles.TabIndex = 0;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewFiles_ColumnClick);
            this.listViewFiles.SelectedIndexChanged += new System.EventHandler(this.listViewFiles_SelectedIndexChanged);
            this.listViewFiles.DoubleClick += new System.EventHandler(this.listViewFiles_DoubleClick);
            // 
            // columnHeaderDev
            // 
            this.columnHeaderDev.Text = "dev";
            this.columnHeaderDev.Width = 31;
            // 
            // columnHeaderIno
            // 
            this.columnHeaderIno.Text = "FDN";
            this.columnHeaderIno.Width = 44;
            // 
            // columnHeaderMode
            // 
            this.columnHeaderMode.Text = "mode";
            this.columnHeaderMode.Width = 38;
            // 
            // columnHeaderLinks
            // 
            this.columnHeaderLinks.Text = "Links";
            this.columnHeaderLinks.Width = 41;
            // 
            // columnHeaderUID
            // 
            this.columnHeaderUID.Text = "UID";
            this.columnHeaderUID.Width = 38;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeaderDateTime
            // 
            this.columnHeaderDateTime.Text = "Datetime";
            this.columnHeaderDateTime.Width = 117;
            // 
            // columnHeaderFilename
            // 
            this.columnHeaderFilename.Text = "Filename";
            this.columnHeaderFilename.Width = 153;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(373, 384);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(486, 384);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelNotice
            // 
            this.labelNotice.AutoSize = true;
            this.labelNotice.Location = new System.Drawing.Point(12, 23);
            this.labelNotice.Name = "labelNotice";
            this.labelNotice.Size = new System.Drawing.Size(200, 13);
            this.labelNotice.TabIndex = 3;
            this.labelNotice.Text = "Directories cannot be selected - only files";
            // 
            // labelCurrentWorkingDirectory
            // 
            this.labelCurrentWorkingDirectory.AutoSize = true;
            this.labelCurrentWorkingDirectory.Location = new System.Drawing.Point(12, 384);
            this.labelCurrentWorkingDirectory.Name = "labelCurrentWorkingDirectory";
            this.labelCurrentWorkingDirectory.Size = new System.Drawing.Size(129, 13);
            this.labelCurrentWorkingDirectory.TabIndex = 4;
            this.labelCurrentWorkingDirectory.Text = "current working directory: ";
            // 
            // frmUniFLEXBrowse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(575, 419);
            this.Controls.Add(this.labelCurrentWorkingDirectory);
            this.Controls.Add(this.labelNotice);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewFiles);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUniFLEXBrowse";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browse UniFLEX directory";
            this.Load += new System.EventHandler(this.frmUniFLEXBrowse_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.ColumnHeader columnHeaderDev;
        private System.Windows.Forms.ColumnHeader columnHeaderIno;
        private System.Windows.Forms.ColumnHeader columnHeaderMode;
        private System.Windows.Forms.ColumnHeader columnHeaderLinks;
        private System.Windows.Forms.ColumnHeader columnHeaderUID;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderDateTime;
        private System.Windows.Forms.ColumnHeader columnHeaderFilename;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelNotice;
        private System.Windows.Forms.Label labelCurrentWorkingDirectory;
    }
}