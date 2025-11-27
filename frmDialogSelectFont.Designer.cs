
namespace TransferUniFLEX
{
    partial class frmDialogSelectFont
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
            this.listViewFonts = new System.Windows.Forms.ListView();
            this.FontName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelSize = new System.Windows.Forms.Label();
            this.textBoxSize = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listViewFonts
            // 
            this.listViewFonts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFonts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FontName});
            this.listViewFonts.HideSelection = false;
            this.listViewFonts.Location = new System.Drawing.Point(12, 12);
            this.listViewFonts.Name = "listViewFonts";
            this.listViewFonts.Size = new System.Drawing.Size(175, 320);
            this.listViewFonts.TabIndex = 0;
            this.listViewFonts.UseCompatibleStateImageBehavior = false;
            this.listViewFonts.View = System.Windows.Forms.View.Details;
            this.listViewFonts.SelectedIndexChanged += new System.EventHandler(this.listViewFonts_SelectedIndexChanged);
            this.listViewFonts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewFonts_MouseDoubleClick);
            // 
            // FontName
            // 
            this.FontName.Text = "Font Name";
            this.FontName.Width = 148;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(13, 371);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(109, 371);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelSize
            // 
            this.labelSize.AutoSize = true;
            this.labelSize.Location = new System.Drawing.Point(9, 345);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(27, 13);
            this.labelSize.TabIndex = 4;
            this.labelSize.Text = "Size";
            // 
            // textBoxSize
            // 
            this.textBoxSize.Location = new System.Drawing.Point(40, 341);
            this.textBoxSize.Name = "textBoxSize";
            this.textBoxSize.Size = new System.Drawing.Size(44, 20);
            this.textBoxSize.TabIndex = 1;
            // 
            // frmDialogSelectFont
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(199, 409);
            this.Controls.Add(this.textBoxSize);
            this.Controls.Add(this.labelSize);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewFonts);
            this.Name = "frmDialogSelectFont";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Font";
            this.Load += new System.EventHandler(this.frmDialogSelectFont_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewFonts;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ColumnHeader FontName;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.TextBox textBoxSize;
    }
}