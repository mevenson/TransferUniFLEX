using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Diagnostics;

namespace TransferUniFLEX
{
    public partial class frmUniFLEXBrowse : Form
    {
        private frmTransfer _parent;

        public  Dictionary<string, FileInformation> selectedFileInformations           = new Dictionary<string, FileInformation> ();  // this is what is used by the caller to know  what is selected
        private Dictionary<string, FileInformation> previouslySelectedFileInformations = new Dictionary<string, FileInformation>();   // this is what is already selected when called

        public string selectedFile = "";
        public string currentDirectoryNameToBrowse;
        bool allowDirectorySelection = false;
        public string currentWorkingDirectory = "";

        private bool m_nExpandTabs = true;
        private bool m_nAddLinefeed = false;
        private bool m_nCompactBinary = true;
        private bool m_nStripLinefeed = true;
        private bool m_nCompressSpaces = true;
        private bool m_nConvertLfOnly = false;
        private bool m_nConvertLfOnlyToCrLf = false;
        private bool m_nConvertLfOnlyToCr = true;
        private bool disableTextProcessingDuringExport = false;

        private string dialogConfigType = "TransferUniFLEX";
        private string editor = "";
        private bool useExternalEditor = false;
        private bool useInternalEditorTabbedInterface = false;
        private bool logOS9FloppyWrite = false;
        private string os9FloppyWritesFile = "";

        public void SetTitle ()
        {
            if (Program.isMinix)
                this.Text = "Browse Minix directory";
            else
                this.Text = "Browse UniFLEX directory";
        }

        public frmUniFLEXBrowse(frmTransfer parent, Socket _socket, string directoryNameToBrowse, Dictionary<string, FileInformation> _selectedFileInfos, string _ipAddress, string _port, bool _allowDirectorySelection)
        {
            InitializeComponent();
            _parent = parent;

            SetTitle();

            Program.remoteAccess.socket = _socket;
            Program.remoteAccess.ipAddress = _ipAddress;
            Program.remoteAccess.port = _port;
            allowDirectorySelection = _allowDirectorySelection;

            previouslySelectedFileInformations = _selectedFileInfos;                              // These are the ones that were previously selected from the last time
            currentDirectoryNameToBrowse = directoryNameToBrowse;

            Program.remoteAccess.GetRemoteDirectory(directoryNameToBrowse, false);      // handle recursion at the main form level
        }

        public frmUniFLEXBrowse(frmTransfer parent, SerialPort _serialPort, string directoryNameToBrowse, Dictionary<string, FileInformation> _selectedFileInfos, bool _allowDirectorySelection)
        {
            InitializeComponent();
            _parent = parent;

            SetTitle();

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
                    if (Program.isMinix)
                    {
                        FileInformation fileInformation = (FileInformation)lvi.Tag;
                        if ((fileInformation.stat.st_mode & Program.isDirMask) == Program.isDirMask)
                        {
                            if (!allowDirectorySelection)
                                lvi.Selected = false;
                        }
                    }
                    else
                    {
                        FileInformation fileInformation = (FileInformation)lvi.Tag;
                        if ((fileInformation.stat.st_mode & Program.isDirMask) == Program.isDirMask)
                        {
                            if (!allowDirectorySelection)
                                lvi.Selected = false;
                        }
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
            if (Program.isMinix)
            {
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
                    {
                        lvi.Selected = true;
                    }
                }
            }
            else
            {
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
                    {
                        lvi.Selected = true;
                    }
                }
            }

            // on initial entry, currentDirectoryNameToBrowse will be blank, so use currentWorkingDirectory
            string cwd = currentDirectoryNameToBrowse;
            if (cwd.Length == 0)
                cwd = currentWorkingDirectory;
            labelCurrentWorkingDirectory.Text = $"current working directory: {cwd}";
        }

        private void frmUniFLEXBrowse_Load(object sender, EventArgs e)
        {
            // load up the total list of sorted file inforations returned by /bin/pwd (getcwd)
            // the disctionary is sorted first by mode (dir .vs. file) and then alphabetically

            labelNotice.Text = allowDirectorySelection ?
                            "Directories may be selected - double click on a directory to change directory - double click a file to open it": 
                            "Directories cannot be selected - double click on a directory to change directory - double click a file to open it";
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

        private void ReloadOptions()
        {
            // reload options from config file in case they were saved (with Apply) in the Option Dialog and them the user pressed cancel.

            m_nExpandTabs = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileExport/ExpandTabs", "enabled", "0") == "1" ? true : false;
            m_nAddLinefeed = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileExport/AddLinefeed", "enabled", "0") == "1" ? true : false;
            m_nCompactBinary = Program.GetConfigurationAttribute("Global/TransferUniFLEX/BinaryFile/CompactBinary", "enabled", "0") == "1" ? true : false;
            m_nStripLinefeed = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileImport/StripLinefeed", "enabled", "0") == "1" ? true : false;
            m_nCompressSpaces = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileImport/CompressSpaces", "enabled", "0") == "1" ? true : false;

            m_nConvertLfOnly = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileImport/ConvertLfOnly", "enabled", "0") == "1" ? true : false;
            if (m_nConvertLfOnly)
            {
                m_nConvertLfOnlyToCrLf = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileImport/ConvertLfOnlyToCrLf", "enabled", "0") == "1" ? true : false;
                m_nConvertLfOnlyToCr = Program.GetConfigurationAttribute("Global/TransferUniFLEX/FileImport/ConvertLfOnlyToCr", "enabled", "0") == "1" ? true : false;
            }
            else
            {
                m_nConvertLfOnlyToCrLf = false;
                m_nConvertLfOnlyToCr = false;
            }

            editor = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "EditorPath", "");
            useExternalEditor = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "UseExternalEditor", "N") == "Y" ? true : false;
            useInternalEditorTabbedInterface = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "useInternalEditorTabbedInterface", "N") == "Y" ? true : false;
            logOS9FloppyWrite = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "LogOS9FloppyWrites", "N") == "Y" ? true : false;
            os9FloppyWritesFile = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "os9FloppyWritesFile", "");
        }

        private void listViewFiles_DoubleClick(object sender, EventArgs e)
        {
            //return; // STILL needs work

            ReloadOptions();
            Cursor = Cursors.WaitCursor;

            if (currentDirectoryNameToBrowse.Length == 0)
            {
                currentDirectoryNameToBrowse = currentWorkingDirectory;
            }

            if (currentDirectoryNameToBrowse.StartsWith("/"))
            {
                // when the user double clicks an item that is a directory - do a change directory
                // when the user double clicks an item that is a not a dir - get the file to the temp directory and display it.
                //
                //  if the directory name is .. - just remove the string after the last / in directoryNameToBrowse

                ListView listView = (ListView)sender;
                ListViewItem focusedItem = listView.FocusedItem;

                if (Program.isMinix)
                {
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
                    else
                    {
                        // the user clicked on an item in the list view that is NOT a directory. build the nameof
                        // the file to request from the remote.

                        bool error = false;
                        string filename = Path.Combine(currentDirectoryNameToBrowse, fileInfo.filename).Replace(@"\", "/");

                        // use the SendFileNameAndRecieveFile code in the main form to do the transfer:
                        //
                        //      error = SendFileNameAndRecieveFile(Program.remoteAccess.serialPort, fileToGet.Replace(@"\", "/"), textBoxUniFLEXFileName.Text.Replace(@"\", "/"), 0);
                        //
                        string localDirectory;
                        if (Program.isMinix)
                            localDirectory = "D:/FilesFromMinix";
                        else
                            localDirectory = "D:/FilesFromUniFLEX";
                        string localFilename = localDirectory + currentDirectoryNameToBrowse + "/" + fileInfo.filename;
                        error = _parent.SendFileNameAndRecieveFile(Program.remoteAccess.serialPort, localFilename, filename, 0);

                        // if there is no error when retreiving the file from the remote - present the file to the user in their favorite editor.
                        // for now we will use EmEditor.

                        if (useExternalEditor)
                        {
                            Process rc;

                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = editor;
                            startInfo.Arguments = localFilename;

                            try
                            {
                                rc = Process.Start(startInfo);
                            }
                            catch
                            {
                                MsgBox.Show("could not load requested editor");
                            }
                        }
                        else
                        {
                            frmFileEditor pDlg = new frmFileEditor(dialogConfigType, localFilename, fileInfo.filename);
                            pDlg.pDlgInvoker = this;
                            pDlg.Show(this.Parent);
                        }
                    }
                }
                else
                {
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
                    else
                    {
                        // the user clicked on an item in the list view that is NOT a directory. build the nameof
                        // the file to request from the remote.

                        bool error = false;
                        string filename = Path.Combine(currentDirectoryNameToBrowse, fileInfo.filename).Replace(@"\", "/");

                        // use the SendFileNameAndRecieveFile code in the main form to do the transfer:
                        //
                        //      error = SendFileNameAndRecieveFile(Program.remoteAccess.serialPort, fileToGet.Replace(@"\", "/"), textBoxUniFLEXFileName.Text.Replace(@"\", "/"), 0);
                        //
                        string localDirectory;
                        if (Program.isMinix)
                            localDirectory = "D:/FilesFromMinix";
                        else
                            localDirectory = "D:/FilesFromUniFLEX";

                        string localFilename = localDirectory + currentDirectoryNameToBrowse + "/" + fileInfo.filename;
                        error = _parent.SendFileNameAndRecieveFile(Program.remoteAccess.serialPort, localFilename, filename, 0);

                        // if there is no error when retreiving the file from the remote - prsent the file to the user in their favorite editor.
                        // for now we will use EmEditor.

                        if (useExternalEditor)
                        {
                            Process rc;

                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = editor;
                            startInfo.Arguments = localFilename;

                            try
                            {
                                rc = Process.Start(startInfo);
                            }
                            catch
                            {
                                MsgBox.Show("could not load requested editor");
                            }
                        }
                        else
                        {
                            frmFileEditor pDlg = new frmFileEditor(dialogConfigType, localFilename, fileInfo.filename);
                            pDlg.pDlgInvoker = this;
                            pDlg.Show(this.Parent);
                        }
                    }
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
