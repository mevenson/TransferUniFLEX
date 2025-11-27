using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

namespace TransferUniFLEX
{
    public partial class frmDialogSelectFont : Form
    {
        public System.Drawing.FontFamily selectedFontFamily = null;

        public string selectedFontFamilyName = Program.defaultFontFamilyName;
        public float selectedFontSize = Program.defaultFontSize;

        public frmDialogSelectFont(string _selectedFontFamilyName, float _selectedFontSize)
        {
            InitializeComponent();

            selectedFontFamilyName = _selectedFontFamilyName;
            selectedFontSize = _selectedFontSize;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (listViewFonts.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection selectedItems = listViewFonts.SelectedItems;

                selectedFontFamily = (FontFamily)selectedItems[0].Tag;
                selectedFontFamilyName = selectedItems[0].Text;

                if (textBoxSize.Text.Length > 0)
                {
                    float size = Program.defaultFontSize;     // this is the default
                    bool success = float.TryParse(textBoxSize.Text, out size);
                    if (success)
                    {
                        Program.preferencesKey.SetValue("Source Font Size", size.ToString(), RegistryValueKind.String);   // save this for font dialog
                        Program.selectedFontSize = size;
                    }
                }
                else
                {
                    Program.preferencesKey.SetValue("Source Font Size", "", RegistryValueKind.String);
                    Program.selectedFontSize = 10.0F;
                }


                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("you must select one and only one font");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SelectListViewItemByText(ListView listView, string text)
        {
            // Find the item with the specified text
            ListViewItem selectedItem = listView.FindItemWithText(text);

            // Check if the item was found
            if (selectedItem != null)
            {
                // Select the item
                selectedItem.Selected = true;

                // Optionally, ensure the item is visible
                selectedItem.EnsureVisible();
            }
        }

        private void frmDialogSelectFont_Load(object sender, EventArgs e)
        {
            //  System.Drawing.FontFamily fontFamily in 
            foreach (KeyValuePair<string, System.Drawing.FontFamily> kvp in Program.fontFamilies)
            {
                ListViewItem lvi = listViewFonts.Items.Add(kvp.Key);
                lvi.Tag = kvp.Value;
            }

            // select the one passed in

            SelectListViewItemByText(listViewFonts, selectedFontFamilyName);
            textBoxSize.Text = selectedFontSize.ToString();
        }

        private void listViewFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listViewFonts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonOK_Click(sender, e);
        }
    }
}
