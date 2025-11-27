using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;      // added for registry access

namespace TransferUniFLEX
{
    public partial class frmFindDialog : Form
    {
        // used to set the position of the find dialog

        frmFileEditor parent = null;
        frmFileEditorTabbed parentTabbed = null;

        public string SearchText => txtFind.Text;
        public bool matchCase => checkBoxMatchCase.Checked;
        public bool wholeWord => checkBoxWholeWord.Checked;

        public event EventHandler FindClicked;

        public frmFindDialog(frmFileEditor _parent)
        {
            InitializeComponent();

            parent = _parent;
        }

        public frmFindDialog(frmFileEditorTabbed _parent)
        {
            InitializeComponent();

            parentTabbed = _parent;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void btnFindNext_Click(object sender, EventArgs e)
        {
            FindClicked?.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        bool loading = false;
        private void frmFindDialog_Load(object sender, EventArgs e)
        {
            // Since this dialog refuses to Center on Parent automatically, we will do it manually.

            this.StartPosition = FormStartPosition.Manual;

            // now let's find where the top and left of our form should be

            if (parent != null)
            {
                int centerX = parent.Left + (parent.Width - this.Width) / 2;
                int centerY = parent.Top + (parent.Height - this.Height) / 2;

                this.Left = centerX;
                this.Top = centerY;

                loading = true;
                object findMatchCase = Program.preferencesKey.GetValue("findMatchCase", 0);
                checkBoxMatchCase.Checked = (int)findMatchCase == 1 ? true : false;
                object findWholeWord = Program.preferencesKey.GetValue("findWholeWord", 0);
                checkBoxWholeWord.Checked = (int)findWholeWord == 1 ? true : false;
                loading = false;
            }
            else if (parentTabbed != null)
            {
                int centerX = parentTabbed.Left + (parentTabbed.Width - this.Width) / 2;
                int centerY = parentTabbed.Top + (parentTabbed.Height - this.Height) / 2;

                this.Left = centerX;
                this.Top = centerY;

                loading = true;
                object findMatchCase = Program.preferencesKey.GetValue("findMatchCase", 0);
                checkBoxMatchCase.Checked = (int)findMatchCase == 1 ? true : false;
                object findWholeWord = Program.preferencesKey.GetValue("findWholeWord", 0);
                checkBoxWholeWord.Checked = (int)findWholeWord == 1 ? true : false;
                loading = false;
            }
            else
                MessageBox.Show("OOPS");
        }

        private void checkBoxMatchCase_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
                Program.preferencesKey.SetValue("findMatchCase", checkBoxMatchCase.Checked ? 1 : 0, RegistryValueKind.DWord);
        }

        private void checkBoxWholeWord_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
                Program.preferencesKey.SetValue("findWholeWord", checkBoxWholeWord.Checked ? 1 : 0, RegistryValueKind.DWord);
        }
    }
}
