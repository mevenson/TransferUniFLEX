using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO.Ports;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace TransferUniFLEX
{
    public partial class frmUniFLEXBrowse : Form
    {

        public  Dictionary<string, FileInformation> selectedFileInformations = new Dictionary<string, FileInformation> ();          // this is what is used by the caller to know  what is selected
        private Dictionary<string, FileInformation> previouslySelectedFileInformations = new Dictionary<string, FileInformation>(); // this is what is already selected when called
        public string selectedFile = "";
        public string currentDirectoryNameToBrowse;
        bool allowDirectorySelection = false;
        public string currentWorkingDirectory = "";

        public frmUniFLEXBrowse(Socket _socket, string directoryNameToBrowse, Dictionary<string, FileInformation> _selectedFileInfos, string _ipAddress, string _port, bool _allowDirectorySelection)
        {
            InitializeComponent();

            Program.remoteAccess.socket = _socket;
            Program.remoteAccess.ipAddress = _ipAddress;
            Program.remoteAccess.port = _port;
            allowDirectorySelection = _allowDirectorySelection;

            previouslySelectedFileInformations = _selectedFileInfos;                              // These are the ones that were previously selected from the last time
            currentDirectoryNameToBrowse = directoryNameToBrowse;

            Program.remoteAccess.GetRemoteDirectory(directoryNameToBrowse, false);      // handle recursion at the main form level
        }

        public frmUniFLEXBrowse(SerialPort _serialPort, string directoryNameToBrowse, Dictionary<string, FileInformation> _selectedFileInfos, bool _allowDirectorySelection)
        {
            InitializeComponent();

            Program.remoteAccess.serialPort = _serialPort;
            allowDirectorySelection = _allowDirectorySelection;

            previouslySelectedFileInformations = _selectedFileInfos;                              // These are the ones that were previously selected from the last time
            currentDirectoryNameToBrowse = directoryNameToBrowse;

            Program.remoteAccess.GetRemoteDirectory(directoryNameToBrowse, false);      // handle recursion at the main form level
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            ListView.SelectedListViewItemCollection collection = listViewFiles.SelectedItems;
            try
            {
                // if the control or Shift Key is held down and the selected index is
                // changing, the list view focused item will be null, so we have to
                // iterrate through the entire set of items and make sure none of the
                // directories are seleced unless the allowDirectorySelection is true.

                foreach (ListViewItem lvi in listView.Items)
                {
                    FileInformation fileInformation = (FileInformation)lvi.Tag;
                    if ((fileInformation.stat.st_mode & Program.isDirMask) == Program.isDirMask)
                    {
                        if (!allowDirectorySelection)
                            lvi.Selected = false;
                    }
                }
            }
            catch 
            {

            }
        }

        private void LoadListView ()
        {
            listViewFiles.Items.Clear();
            foreach (KeyValuePair<string, FileInformation> fileInformation in Program.remoteAccess.sortedInformations)
            {
                ListViewItem item = new ListViewItem
                (
                    new[]
                    {
                        fileInformation.Value.stat.st_dev.ToString(),
                        fileInformation.Value.stat.st_ino.ToString("X4"),
                        fileInformation.Value.stat.st_mode.ToString("X4"),
                        fileInformation.Value.stat.st_nlink.ToString(),
                        fileInformation.Value.stat.st_uid.ToString("X4"),
                        fileInformation.Value.stat.st_size.ToString(),
                        Program.remoteAccess.ConvertDateTime(fileInformation.Value.stat.st_mtime),
                        fileInformation.Key
                    }
                );

                ListViewItem lvi = listViewFiles.Items.Add(item);
                lvi.Tag = fileInformation.Value;                    // we only need the value from the key value pair

                if (previouslySelectedFileInformations.ContainsKey(fileInformation.Key))
                //if (selectedFileInformations.ContainsKey(fileInformation.Key))
                {
                    lvi.Selected = true;
                }
            }
        }

        private void frmUniFLEXBrowse_Load(object sender, EventArgs e)
        {
            // load up the total list of sorted file inforations returned by /bin/pwd (getcwd)
            // the disctionary is sorted first by mode (dir .vs. file) and then alphabetically

            labelNotice.Text = allowDirectorySelection ? "Directories may be selected" : "Directories cannot be selected - only files";
            LoadListView();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selectedIndices = listViewFiles.SelectedIndices;
            if (selectedIndices.Count != 0)
            {
                if (selectedIndices.Count == 1)
                {
                    // if the currentDirectoryNameToBrowse is empty - use the current working directory
                    if (currentDirectoryNameToBrowse.Length == 0)
                        currentDirectoryNameToBrowse = currentWorkingDirectory;

                    if (!currentDirectoryNameToBrowse.EndsWith("/"))
                        selectedFile = currentDirectoryNameToBrowse + "/" + listViewFiles.Items[selectedIndices[0]].SubItems[7].Text.ToString();
                    else
                        selectedFile = currentDirectoryNameToBrowse + listViewFiles.Items[selectedIndices[0]].SubItems[7].Text.ToString();
                }

                // make a copy of the FileInformation list without any directories in it and only the selected items
                // from the listview control
                int index = 0;
                selectedFileInformations.Clear();
                foreach (KeyValuePair<string, FileInformation> fileInfo in Program.remoteAccess.sortedInformations)
                {
                    if (selectedIndices.Contains(index))
                    {
                        if (allowDirectorySelection || (fileInfo.Value.stat.st_mode & Program.isDirMask) != Program.isDirMask)
                            selectedFileInformations.Add(fileInfo.Value.filename, fileInfo.Value);
                    }
                    index++;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            selectedFile = "";
        }

        private void listViewFiles_DoubleClick(object sender, EventArgs e)
        {
            //return; // STILL needs work

            Cursor = Cursors.WaitCursor;

            if (currentDirectoryNameToBrowse.Length == 0)
            {
                currentDirectoryNameToBrowse = currentWorkingDirectory;
            }

            if (currentDirectoryNameToBrowse.StartsWith("/"))
            {
                // when the user double clicks an item that is a directory - do a change directory
                //
                //  if the directory name is .. - just remove the string after the last / in directoryNameToBrowse

                ListView listView = (ListView)sender;
                ListViewItem focusedItem = listView.FocusedItem;

                FileInformation fileInfo = (FileInformation)focusedItem.Tag;
                int mode = fileInfo.stat.st_mode;

                if ((mode & Program.isDirMask) == Program.isDirMask)          // make sure it is a directory
                {
                    string newpath = "";

                    if (fileInfo.filename == "..")      // go up one directory
                    {
                        string[] pathParts = currentDirectoryNameToBrowse.Split('/');
                        if (currentDirectoryNameToBrowse.StartsWith("/"))
                            newpath = "/";

                        for (int i = 0; i < pathParts.Length - 1; i++)
                        {
                            if (pathParts[i].Length > 0)
                            {
                                if (newpath.Length > 0 && newpath != "/")
                                    newpath += "/";

                                newpath += pathParts[i];
                            }
                        }

                        // in case the old path was current working directory (blank)
                        if (newpath.Length == 0)
                            newpath = "..";
                    }
                    else
                    {
                        // this happens when the original directory to browse was blank - CWD.

                        if (currentDirectoryNameToBrowse == "..")
                        {
                            newpath = fileInfo.filename;
                        }
                        else
                        {
                            if (!currentDirectoryNameToBrowse.EndsWith("/"))
                                newpath = currentDirectoryNameToBrowse + "/" + fileInfo.filename;
                            else
                                newpath = currentDirectoryNameToBrowse + fileInfo.filename;
                        }
                    }

                    currentDirectoryNameToBrowse = newpath;
                }

                Program.remoteAccess.GetRemoteDirectory(currentDirectoryNameToBrowse, false);      // handle recursion at the main form level);
                LoadListView();
            }
            else
            {
                // get surrent working diectory from UniFLEX

            }
            Cursor = Cursors.Default;
        }
    }
}
