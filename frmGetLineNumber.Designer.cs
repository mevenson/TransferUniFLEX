
namespace TransferUniFLEX
{
    partial class frmGetLineNumber
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
            this.labelEnterLineNumber = new System.Windows.Forms.Label();
            this.textBoxLineNumber = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelEnterLineNumber
            // 
            this.labelEnterLineNumber.AutoSize = true;
            this.labelEnterLineNumber.Location = new System.Drawing.Point(13, 13);
            this.labelEnterLineNumber.Name = "labelEnterLineNumber";
            this.labelEnterLineNumber.Size = new System.Drawing.Size(98, 13);
            this.labelEnterLineNumber.TabIndex = 3;
            this.labelEnterLineNumber.Text = "Enter Line Number:";
            // 
            // textBoxLineNumber
            // 
            this.textBoxLineNumber.Location = new System.Drawing.Point(117, 9);
            this.textBoxLineNumber.Name = "textBoxLineNumber";
            this.textBoxLineNumber.Size = new System.Drawing.Size(66, 20);
            this.textBoxLineNumber.TabIndex = 0;
            this.textBoxLineNumber.TextChanged += new System.EventHandler(this.textBoxLineNumber_TextChanged);
            this.textBoxLineNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLineNumber_KeyPress);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(108, 49);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(13, 49);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // frmGetLineNumber
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(203, 88);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxLineNumber);
            this.Controls.Add(this.labelEnterLineNumber);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGetLineNumber";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Get Line Number";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelEnterLineNumber;
        private System.Windows.Forms.TextBox textBoxLineNumber;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
    }
}