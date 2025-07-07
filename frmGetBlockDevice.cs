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
    public partial class frmGetBlockDevice : Form
    {
        public string deviceName = "";
        public string saveAs = "";
        public frmGetBlockDevice()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            deviceName = textBoxDeviceName.Text;
            saveAs = textBoxSaveAs.Text;

            DialogResult = DialogResult.OK;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = textBoxSaveAs.Text;

            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                textBoxSaveAs.Text = dlg.FileName;
            }

        }
    }
}
