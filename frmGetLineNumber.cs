using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransferUniFLEX
{
    public partial class frmGetLineNumber : Form
    {
        public string lineNumber;

        public frmGetLineNumber()
        {
            InitializeComponent();
        }

        private void textBoxLineNumber_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBoxLineNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control keys (like backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ignore the key
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            lineNumber = textBoxLineNumber.Text;
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
