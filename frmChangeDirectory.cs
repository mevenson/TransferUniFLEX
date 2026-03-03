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
    public partial class frmChangeDirectory : Form
    {
        public string directoryName = "";

        public frmChangeDirectory(string directoryName)
        {
            InitializeComponent();
            this.directoryName = directoryName;
        }

        public frmChangeDirectory()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            directoryName = textBoxDirectoryName.Text;
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void frmChangeDirectory_Load(object sender, EventArgs e)
        {
            textBoxDirectoryName.Text = directoryName;
        }
    }
}
