
namespace TransferUniFLEX
{
    partial class frmFindDialog
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
            this.txtFind = new System.Windows.Forms.TextBox();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.checkBoxWholeWord = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Location = new System.Drawing.Point(14, 15);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(247, 20);
            this.txtFind.TabIndex = 0;
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.AutoSize = true;
            this.checkBoxMatchCase.Location = new System.Drawing.Point(14, 53);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(83, 17);
            this.checkBoxMatchCase.TabIndex = 1;
            this.checkBoxMatchCase.Text = "Match Case";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            this.checkBoxMatchCase.CheckedChanged += new System.EventHandler(this.checkBoxMatchCase_CheckedChanged);
            // 
            // btnFindNext
            // 
            this.btnFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindNext.Location = new System.Drawing.Point(267, 14);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(75, 23);
            this.btnFindNext.TabIndex = 3;
            this.btnFindNext.Text = "Find";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // checkBoxWholeWord
            // 
            this.checkBoxWholeWord.AutoSize = true;
            this.checkBoxWholeWord.Location = new System.Drawing.Point(122, 53);
            this.checkBoxWholeWord.Name = "checkBoxWholeWord";
            this.checkBoxWholeWord.Size = new System.Drawing.Size(86, 17);
            this.checkBoxWholeWord.TabIndex = 2;
            this.checkBoxWholeWord.Text = "Whole Word";
            this.checkBoxWholeWord.UseVisualStyleBackColor = true;
            this.checkBoxWholeWord.CheckedChanged += new System.EventHandler(this.checkBoxWholeWord_CheckedChanged);
            // 
            // frmFindDialog
            // 
            this.AcceptButton = this.btnFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 101);
            this.Controls.Add(this.checkBoxWholeWord);
            this.Controls.Add(this.btnFindNext);
            this.Controls.Add(this.checkBoxMatchCase);
            this.Controls.Add(this.txtFind);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFindDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "find";
            this.Load += new System.EventHandler(this.frmFindDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.CheckBox checkBoxWholeWord;
    }
}