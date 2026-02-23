using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;
using Microsoft.Win32;

using System.Net;
using System.Net.Sockets;

namespace TransferUniFLEX
{
    public partial class frmTransfer : Form
    {
        string _version = Program.version.ToString();
        RegistryKey registryKey = null;

        private string dialogConfigType = "TransferUbiFLEX";
        private string editor = "";
        private bool useExternalEditor = false;

        public frmTransfer()
        {
            InitializeComponent();
        }

        volatile bool pause = false;
        volatile bool stop = false;

        byte acceptFileName              = 0x02;    // tells the remote to accept a file name to receive and receive it
        byte sendFileCommand             = 0x04;    // tell remote to accept a filename to send and send it
        byte sendCurrentDirectoryCommand = 0x05;    // tell the remote to send us a string containing the current working directory
        byte getBlockDeviceCommand       = 0x07;    // tell remote to send the complete contents of a block device

        ToolStripStatusLabel statusLabel;

        List<string> tempbuffer = new List<string>();

        static ushort[] crc_table = new ushort[]
        {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef, 
            0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6, 0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de, 
            0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485, 0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d, 
            0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4, 0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc, 
            0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823, 0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b, 
            0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12, 0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a, 
            0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41, 0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49, 
            0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70, 0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78, 
            0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f, 0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067, 
            0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e, 0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256, 
            0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d, 0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405, 
            0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c, 0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634, 
            0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab, 0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3, 
            0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a, 0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92, 
            0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9, 0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1, 
            0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8, 0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
        };

        private void ShowVersionInTitle(string _cDriveFileTitle)
        {
            this.Text = string.Format("Transfer to/from UniFLEX version {0}", _version);
        }

        static ushort CRCCCITT(byte[] data, int startIndex, int length, ushort seed, ushort final)
        {

            int count;
            ushort crc = seed;
            ushort temp;
            int dataindex = startIndex;

            for (count = 0; count < length; ++count)
            {
                temp = (ushort)((data[dataindex++] ^ (crc >> 8)) & 0xff);
                crc = (ushort)(crc_table[temp] ^ (crc << 8));
            }

            return (ushort)(crc ^ final);
        }

        static ushort CRCCCITT_file(string strFname)
        {
            ushort ccitt = 0xffff;

            BinaryReader br = new BinaryReader(File.Open(strFname, FileMode.Open, FileAccess.Read));
            if (br != null)
            {
                long sizeOfFile = br.BaseStream.Length;

                byte[] fileContent = br.ReadBytes((int)sizeOfFile);
                ccitt = CRCCCITT(fileContent, 0, (int)sizeOfFile, 0xffff, 0);

                br.Close();
            }
            return ccitt;
        }

        //private SerialPort OpenComPort()
        //{
        //    string portName = comboBoxCOMPorts.Text;

        //    // if there is an open serial port - close it in case the selected port has changed
        //    if (Program.remoteAccess.serialPort != null && Program.remoteAccess.serialPort.IsOpen)
        //    {
        //        Program.remoteAccess.serialPort.Close();
        //    }

        //    // now open the one specified in the comboBoxCOMPorts.Text combo box
        //    try
        //    {
        //        Program.remoteAccess.serialPort = new SerialPort(portName);

        //        // Optional: Configure other SerialPort settings such as BaudRate, Parity, DataBits, StopBits, etc.

        //        string stringBaudRate = comboBoxBaudRate.Text;
        //        int baudRate = 2400;

        //        bool success = Int32.TryParse(stringBaudRate, out baudRate);

        //        //serialPort.BaudRate = baudRate;
        //        Program.remoteAccess.serialPort.BaudRate = baudRate;
        //        Program.remoteAccess.serialPort.Parity = Parity.None;
        //        Program.remoteAccess.serialPort.DataBits = 8;
        //        Program.remoteAccess.serialPort.StopBits = StopBits.One;
        //        Program.remoteAccess.serialPort.ReadTimeout = 5000;      // set timeout to 5 seconds

        //        Program.remoteAccess.serialPort.Open();

        //        // MsgBox.Show($"COM Port {portName} opened successfully.");
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        // MsgBox.Show($"Access to COM Port {portName} is unauthorized: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        // MsgBox.Show($"An error occurred: {ex.Message}");
        //    }
        //    return Program.remoteAccess.serialPort;
        //}

        private bool OpenTCPPort(bool forceReOpen = false)
        {
            bool success = true;
            bool created = false;

            Program.remoteAccess.ipAddress  = textBoxIPAddress.Text;
            Program.remoteAccess.port       = textBoxPort.Text;

            // if the socket has not yet been created and we are going to need one - create it
            if (Program.remoteAccess.socket == null)         // socket has not yet been opened
            {
                Program.remoteAccess.socket = Program.OpenSocket(textBoxIPAddress.Text, textBoxPort.Text);
                
                created = true;
                success = true;
            }

            // if we were already open or just opened - make sure we are connected
            if ((!Program.remoteAccess.socket.Connected || forceReOpen) && !created)
            {
                // if not connected - first try to close the socket so we can re-open it
                try
                {
                    Program.remoteAccess.socket.Close();
                }
                catch
                {
                    // if we fail to close - set return status to false in case the re-open doe not work
                    success = false;
                }

                // now try to re-establish a connectin to the remote by Openning the socket
                try
                {
                    Program.remoteAccess.socket = Program.OpenSocket(textBoxIPAddress.Text, textBoxPort.Text);
                    if (Program.remoteAccess.socket.Connected)
                        success = true;     // we were successful - return a good status
                    else
                        success = false;
                }
                catch
                {
                    success = false;        // the remote must not be running the tuff program or the network is down
                }
            }

            return success;
        }

        // returns -1 if timeout, otherwise it will return whatever the remote sends as a response to the bytes sent to it.
        // this will be always be 0x06 unless the last byte sent to the remote was the low byte of the CCITT value. If this
        // is the case the response will be 0x06 if the transmitted CCITT matches the remote calculated CCIT or 0x15 if the
        // transmitted CCITT does not match the remote calculated VVITT.

        private int SendByte (SerialPort serialPort, byte b)
        {
            int response = -1;
            byte[] byteToSend = new byte[1];
            byteToSend[0] = b;

            try
            {
                serialPort.Write(byteToSend, 0, 1);
                response = serialPort.ReadByte();
            }
            catch (TimeoutException ex)
            {
                MsgBox.Show($"Read operation timed out waiting for ack: {ex.Message}");
            }
            catch (Exception ex)
            {
                MsgBox.Show($"Read operation failed waiting for ack: {ex.Message}");
            }

            return response;
        }

        // This is used to send both strings and byte buffers. When sending strings it is called with
        // sendNull set to true so that a null byte gets sent to the remote at the end of the string.
        //
        // If using this to send a buffer of bytes as a block - call with sendNull set to false so
        // that a null character is NOT sent after the last byte of the buffer.
        //
        // In either case, for every character sent - an ACK byte is returned to make sure the remote
        // is going to be ready to accept another character after the one just sent. This keeps us from
        // over running the transmit data register on our end and the receive data register on the
        // remote end.

        private int SendBytes(SerialPort serialPort, byte [] b, int startIndex, int length, bool sendNull)
        {
            int response = -1;

            int i;

            for (i = startIndex; i < length; i++)
            {
                response = SendByte(serialPort, b[i]);

                if (response == -1)
                    break;
            }
            if (sendNull)
            {
                response = SendByte(serialPort, 0x00);
            }

            return response;
        }

        private bool FileContainsOnlyText (string filename)
        {
            bool onlyText = true;

            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                FileInfo fi = new FileInfo(filename);

                byte[] buffer = reader.ReadBytes((int)fi.Length);   // we can get away with this since UniFLEX does not allow files larger than the max value of an int
                for(int i = 0; i < buffer.Length; i++)
                {
                    // if the file contains anything other than asci 0x20 through 0x7F or <cr> or <lf> <tab> or <null> or <ff> - flag as not a text file

                    if (!((buffer[i] >= 0x20 && buffer[i] <= 0x7F) || buffer[i] == 0x0d || buffer[i] == 0x0a || buffer[i] == 0x09 || buffer[i] == 0x00 || buffer[i] == 0x0c))
                    {
                        onlyText = false;
                        break;
                    }
                }
            }

            return onlyText;
        }

        private int GetLastByteFromRemote (SerialPort serialPort)
        {
            int response = -1;

            // flush the incoming stream except for the last character received.

            serialPort.ReadTimeout = 1;                 // set quick timeout
            while (true)
            {
                try
                {
                    int tmpResponse = serialPort.ReadByte();    // if we timeout - go to the catch
                    response = tmpResponse;                     // did not timeout - save the last one read before we timed out;
                }
                catch
                {
                    // no more characters waiting to be flushed - the last character received was the
                    // last character sent by the remote - return response of -1

                    break;
                }
            }

            // always reset the timeout to 5 seconds

            serialPort.ReadTimeout = 5000;

            return response;
        }

        // we are going to make this function public so that the Browse dialog can use it to view a file
        public bool SendFileNameAndRecieveFile (SerialPort serialPort, string localFilename, string filename, int mode)
        {
            // mode is currently not used - always 0. I am not sure why it is here, but it isn't hurting
            // anything so - just leave it alone for now. Maybe we can use it know that we are being
            // called from the browse dialog and we should run silent!

            bool error = false;
            int response = 0;

            byte[] ackBuffer = new byte[] { 0x06 };
            byte[] nakBuffer = new byte[] { 0x15 };

            // create the directory if it does not exist
            bool directoryOK = true;
            if (Path.GetDirectoryName(localFilename).Length > 0)
            {
                if (!Directory.Exists(Path.GetDirectoryName(localFilename)))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(localFilename));
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show(ex.Message);
                        directoryOK = false;
                        error = true;
                    }
                }
            }

            // the directory either already existed or we were able to creat it - so proceed
            if (directoryOK)
            {
                // entertain the operator
                currentFileProgress.Text = Path.GetFileName(localFilename);       // show the local filename as the control's text

                if (statusLabel != null)
                {
                    // show the activity as the item.
                    currentFileProgress.Items.Clear();                      // make sure there is only ever one item in the status line
                    statusLabel.Text = $" {filename} ->  {localFilename}";  // format the string
                    currentFileProgress.Items.Add(statusLabel);             // add the item
                    currentFileProgress.Refresh();                          // Force redraw
                }
                Application.DoEvents();                                 // let the system update the status bar.

                DateTime remoteAccessedDateTime = DateTime.Now;
                DateTime remoteModifiedDateTime = DateTime.Now;
                DateTime remoteCreatedDateTime  = DateTime.Now;

                using (BinaryWriter writer = new BinaryWriter(File.Open(localFilename, FileMode.Create, FileAccess.Write)))
                {
                    if (radioButtonCOMPort.Checked)
                    {
                        tempbuffer.Add($"Retrieving: {filename}");
                        textBoxResponses.Lines = tempbuffer.ToArray();
                        //textBoxResponses.Text += $"Retrieving: {filename}";

                        // make sure the line we just added is visible
                        textBoxResponses.SelectionStart = textBoxResponses.Text.Length;     
                        textBoxResponses.ScrollToCaret();

                        // start by sending a command to remote to accept a filename to send the file
                        response = SendByte(serialPort, sendFileCommand);

                        if (response == 0x06)
                        {
                            //// remote is ready to receive the filename. make sure it is not a directory name
                            //if ((mode & Program.isDirMask) != Program.isDirMask)
                            //{
                            // send the filename one byte at a time - remote will ACK each byte sent

                            for (int i = 0; i < filename.Length; i++)
                            {
                                response = SendByte(serialPort, (byte)filename[i]);
                            }
                            response = SendByte(serialPort, 0x00);                             // signal end of filename

                            // now start receiving bytes for the file -
                            //      each block will start with a byte count then 
                            //      then the data
                            //      then the CCITT
                            //
                            //  if CCITT macthes calculated CCITT - save block to locl file and send ACK
                            //  otherwise - do not save the block and send NAK.
                            //
                            //  When the remote is done sending the file, it will send a byte count of zero
                            //  and no data or CCITT. move on to the next file.

                            while (true)
                            {
                                Application.DoEvents();     // let the system update the status bar.

                                int blockSize = 0;
                                blockSize = (byte)serialPort.ReadByte() * 256;
                                blockSize += (byte)serialPort.ReadByte();

                                if (blockSize > 0)
                                {
                                    byte[] blockBuffer = new byte[blockSize];

                                    // now get the data bytes
                                    for (int i = 0; i < blockSize; i++)
                                    {
                                        blockBuffer[i] = (byte)serialPort.ReadByte();
                                    }

                                    // now get the CCITT
                                    ushort ccitt = 0;
                                    ccitt = (ushort)((byte)serialPort.ReadByte() * 256);
                                    ccitt += (ushort)((byte)serialPort.ReadByte());

                                    // calculate the local CCITT
                                    ushort calculatedCCITT = CRCCCITT(blockBuffer, 0, blockSize, 0xffff, 0);

                                    if (calculatedCCITT == ccitt)
                                    {
                                        writer.Write(blockBuffer, 0, blockSize);
                                        serialPort.Write(ackBuffer, 0, 1);
                                    }
                                    else
                                    {
                                        Application.DoEvents();
                                        if (stop)
                                        {
                                            error = true;
                                            break;
                                        }
                                        serialPort.Write(nakBuffer, 0, 1);
                                    }
                                }
                                else
                                    break;
                            }
                            //}
                            tempbuffer.Add(" - done"); 
                            textBoxResponses.Lines = tempbuffer.ToArray();
                            //textBoxResponses.Text += " - done\r\n";
                            textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                            textBoxResponses.ScrollToCaret();
                        }
                    }
                    else
                    {
                        // build a command packet with the command as the first byte and the filename to
                        // retrieve as the rest of the command packet

                        byte[] commandBuffer = new byte[filename.Length + 2];   // buffer size if the length of the filename plus 1 for the command byte and 1 for the null terminater
                        commandBuffer[0] = 0x04;                                // retreive file command
                        for (int i = 0; i < filename.Length; i++)
                        {
                            commandBuffer[i + 1] = (byte)filename[i];
                        }
                        commandBuffer[filename.Length + 1] = 0x00;

                        try
                        {
                            Application.DoEvents();

                            Program.remoteAccess.socket.Send(commandBuffer, 0, commandBuffer.Length, SocketFlags.None);

                            byte[] responseBuffer = new byte[512];
                            for (; ; )
                            {
                                Application.DoEvents();

                                Program.remoteAccess.socket.ReceiveTimeout = 1000;   // set receive timeout to 1 seconds - that should be suffcient

                                int bytesReceived = 0;
                                bool timeout = true;
                                int retryCount = 5;         // allow 5 retry attempts on timeout

                                while (timeout && retryCount > 0)
                                {
                                    try
                                    {
                                        bytesReceived = Program.remoteAccess.socket.Receive(responseBuffer);
                                        timeout = false;
                                    }
                                    catch (SocketException ex)
                                    {
                                        if (ex.SocketErrorCode == SocketError.TimedOut)
                                        {
                                            timeout = true;
                                            retryCount--;
                                        }
                                    }
                                }

                                if (bytesReceived > 4)
                                {
                                    int size = responseBuffer[0] * 256 + responseBuffer[1];

                                    ushort ccitt = 0;
                                    ccitt = (ushort)(responseBuffer[size + 2] * 256);
                                    ccitt += (ushort)((byte)responseBuffer[size + 3]);

                                    ushort calculatedCCITT = CRCCCITT(responseBuffer, 2, size, 0xffff, 0);
                                    if (calculatedCCITT == ccitt)
                                    {
                                        // calculated and received CCITT values match - good block - save to file
                                        writer.Write(responseBuffer, 2, size);

                                        // send ACK for next packet
                                        Program.remoteAccess.socket.Send(ackBuffer, 0, 1, SocketFlags.None);    // request another block of file data
                                    }
                                    else
                                    {
                                        Program.remoteAccess.socket.Send(nakBuffer, 0, 1, SocketFlags.None);    // request another block of file data
                                    }
                                }
                                else if (bytesReceived == 2)
                                {
                                    // only 2 bytes means no more packets remaining - we are done
                                    int size = responseBuffer[0] * 256 + responseBuffer[1];
                                    if (size == 0)
                                    {
                                        // now get the original file time stamp from the selectedFileInfos for this file
                                        // using the filename as the key to the selectedFileInfos dictionary

                                        // if there are no entries in selectedFileInfo, this means we were call from the Browse dialog
                                        // because it does not populate the selectedFileInfo dictionary. But this is Ok becasue we are
                                        // getting the file to temparoray space so we can display it, so there is no need to set the 
                                        // date and time of the file to what it is on the remote.
                                        if (selectedFileInfos.Count > 0)
                                        {
                                            string justTheFilename = Path.GetFileName(filename);
                                            if (Program.isMinix)
                                            {
                                                FileInformation fileInfo = selectedFileInfos[justTheFilename];
                                                remoteAccessedDateTime = Program.remoteAccess.UNIXtoDateTime(fileInfo.stat.st_atime);
                                                remoteModifiedDateTime = Program.remoteAccess.UNIXtoDateTime(fileInfo.stat.st_mtime);
                                                remoteCreatedDateTime = Program.remoteAccess.UNIXtoDateTime(fileInfo.stat.st_ctime);
                                            }
                                            else
                                            {
                                                FileInformation fileInfo = selectedFileInfos[justTheFilename];
                                                remoteModifiedDateTime = Program.remoteAccess.UNIXtoDateTime(fileInfo.stat.st_mtime);
                                            }
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        // we should never get here
                                        MsgBox.Show("what are we doing here");
                                        break;
                                    }
                                }
                                else
                                {
                                    // we should necer get here either
                                    DialogResult r = MsgBox.Show("No response from remote - Abort?", "No Response", MessageBoxButtons.YesNo);
                                    if (r == DialogResult.Yes)
                                    {
                                        error = true;
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MsgBox.Show(ex.Message);
                            error = true;
                        }
                    }
                }

                // put remote file datetime on the copy
                if (!error)
                {
                    File.SetLastWriteTime(localFilename, remoteModifiedDateTime);
                    File.SetCreationTime(localFilename, remoteModifiedDateTime);
                    File.SetLastAccessTime(localFilename, remoteModifiedDateTime);
                }
                else
                {
                    if (!checkBoxKeepZeroLengthFiles.Checked)
                        File.Delete(localFilename);         // do not keep partial file
                }
            }

            if (statusLabel != null)
            {
                currentFileProgress.Items.Clear();                      // make sure there is only ever one item in the status line
                statusLabel.Text = "Idle";                              // format the string
                currentFileProgress.Items.Add(statusLabel);             // add the item
                currentFileProgress.Refresh();                          // Force redraw
            }
            Application.DoEvents();                                 // let the system update the status bar.

            return error;
        }

        private bool SendFileNameAndSendFile(SerialPort serialPort, string localFilename, string remoteFilename, bool remoteFilenameIsDirectory, bool isDirectoryTransfer = false)
        {
            // sending to UniFLEX

            bool error = false;
            int response = 0;
            bool fileContainsOnlyText = false;
            string fileNameAtRemote = remoteFilename;
            byte[] sendBuffer = new byte[Constants.PACKETSIZE];
            int bytesToSend = 0;


            // we do not need to this if this is a directory transfer because the actual filename to create on
            // the remote end is already built in the 'remoteFilename' and set in fileNameAtRemote.

            if (!isDirectoryTransfer)
            {
                // If this is NOT a Full DirectoryTransfer then it is is either a single file select or
                // a multiple file select transfer. If it is a single file select then uniFLEXFilenameIsDirectory
                // will be fasle becasue the file name for the remote end is passed in as 'remoteFilename' and set
                // in fileNameAtRemote.

                if (remoteFilenameIsDirectory)
                {
                    // this is a multiple file select transfer - the remoteFilename is really a directory name
                    // so we need to build the true remote filename and set it in fileNameAtRemote..

                    fileNameAtRemote = Path.Combine(remoteFilename, Path.GetFileName(localFilename)).Replace(@"\", "/");
                }
            }

            currentFileProgress.Text = Path.GetFileName(localFilename);
            currentFileProgress.Items.Clear();

            //// Create and add a ToolStripStatusLabel programmatically

            //ToolStripStatusLabel statusLabel = new ToolStripStatusLabel(Path.GetFileName(localFilename) + " -> " + fileNameAtRemote);
            //currentFileProgress.Items.Add(statusLabel);

            //// Create and add a ToolStripProgressBar programmatically

            //ToolStripProgressBar progressBar = new ToolStripProgressBar();

            FileInfo fi = new FileInfo(localFilename);
            long sizeOfLocalFile = fi.Length;

            if (sizeOfLocalFile % Constants.PACKETSIZE == 0)
                progressBar.Maximum = (int)(sizeOfLocalFile / Constants.PACKETSIZE);
            else
                progressBar.Maximum = (int)(sizeOfLocalFile / Constants.PACKETSIZE) + 1;

            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Size = new System.Drawing.Size(490, 20);
            currentFileProgress.Items.Add(progressBar);

            Application.DoEvents();

            if (radioButtonCOMPort.Checked)
            {
                response = SendByte(serialPort, acceptFileName);
                if (response == 0x06)
                {
                    // we got an ACK - send the filename

                    byte[] nameBytes = ASCIIEncoding.ASCII.GetBytes(fileNameAtRemote);
                    response = SendBytes(serialPort, nameBytes, 0, nameBytes.Length, true);
                    if (response == 0x06)
                    {
                        // we got an ack to sending the filename - this means that UniFLEX is done doing
                        // what it needs to do to prepare for receiving the file content, so let's send
                        // it

                        if (checkBoxFixNewLines.Checked)
                            fileContainsOnlyText = FileContainsOnlyText(localFilename);

                        using (BinaryReader reader = new BinaryReader(File.Open(localFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            error = false;
                            int blockNumber = 1;

                            while (true)    // main loop to send blocks
                            {
                                Application.DoEvents();         // see if the user has clicked on Pause or Stop buttons

                                // Only allow Stop or Pause while getting ready to send the next block

                                while (pause)
                                {
                                    Application.DoEvents();
                                    if (stop)
                                    {
                                        // user clicked stop while paused = break out of pause state

                                        break;
                                    }
                                }

                                if (!stop)
                                {

                                    byte[] buffer = reader.ReadBytes(Constants.PACKETSIZE);
                                    if (buffer.Length > 0)
                                    {
                                        // this part only needs to be done once per block - don't do again on retry

                                        bytesToSend = 0;    // clear the number of bytes to send

                                        // see if we need to do <CR><LF> or <LF> replace if a text file

                                        byte previouscharacter = 0x00;
                                        if (checkBoxFixNewLines.Checked && fileContainsOnlyText)
                                        {
                                            if (Program.isMinix)
                                            {
                                                for (int i = 0, j = 0; i < buffer.Length; i++)
                                                {
                                                    // just strip all <CR> from the buffer we are going to send
                                                    if (buffer[i] != 0x0d)
                                                    {
                                                        sendBuffer[j++] = buffer[i];
                                                        bytesToSend++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int i = 0, j = 0; i < buffer.Length; i++)
                                                {
                                                    if (buffer[i] == 0x0a)
                                                    {
                                                        // strip line feed by replacing with carriage return unless the previous character was a carriage return
                                                        // in which case, just ignore the line feed.

                                                        if (previouscharacter != 0x0d)
                                                        {
                                                            sendBuffer[j++] = 0x0d;
                                                            previouscharacter = 0x0d;
                                                            bytesToSend++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        sendBuffer[j++] = buffer[i];
                                                        previouscharacter = buffer[i];
                                                        bytesToSend++;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // just copy the buffer over

                                            for (int i = 0; i < buffer.Length; i++)
                                                sendBuffer[i] = buffer[i];

                                            bytesToSend = buffer.Length;
                                        }

                                        // now calculate the ccitt value - only needs to be once per block

                                        ushort ccitt = CRCCCITT(sendBuffer, 0, bytesToSend, 0xffff, 0);

                                        // if we get a NACK back from the remote - resend the same block inside this while loop
                                        // receiving an ACK to the block sent will break out of the while loop.

                                        bool success = false;
                                        while (!success && !error)          // stay in here until we successfully sent this block or we get an error
                                        {                                   // again - error is NOT the same as NAK.
                                            Application.DoEvents();

                                            // send data in [bytesToSend] byte blocks as xxxx [dd ... dd] yyyy
                                            //      where xxxx = number of bytes being sent
                                            //            dd   = the data bytes from sendBuffer
                                            //            yyyy = ccitt value

                                            // this sends the buffer size word

                                            response = SendByte(serialPort, (byte)(bytesToSend / 256)); if (response == -1) { error = true; break; }
                                            response = SendByte(serialPort, (byte)(bytesToSend % 256)); if (response == -1) { error = true; break; }

                                            // now the data
                                            // do not send a null byte when done with the buffer - this is only done when sending the filename (strings)

                                            response = SendBytes(serialPort, sendBuffer, 0, bytesToSend, false);
                                            if (response != -1)                 // we did not timeout */
                                            {
                                                // now send the CCITT - the remote will not send an ACK after receiving the second
                                                // byte of the CCITT right away. It will wait to send either ACK or NAK depending
                                                // upon whether the calculated CCITT macthes the received CCITT.

                                                response = SendByte(serialPort, (byte)(ccitt / 256)); if (response == -1) { error = true; break; }

                                                // this call will not return until the remote either times out sending the ACK or NAK or we receive
                                                // either an ACK or NAK.

                                                response = SendByte(serialPort, (byte)(ccitt % 256));

                                                // show in the operator entertainment area, the byte received from the remote
                                                // after sending the low byte of the CCITT

                                                string operatorMessage = string.Format("    Block Number: {0} - response: 0x{1}", blockNumber.ToString("D6"), response.ToString("X2"));
                                                // textBoxResponses.AppendText(operatorMessage);

                                                if (response == 0x06 && tempbuffer.Count > 0)
                                                {
                                                    if (!tempbuffer[tempbuffer.Count - 1].StartsWith("Sending"))
                                                        tempbuffer[tempbuffer.Count - 1] = operatorMessage;
                                                    else
                                                        tempbuffer.Add(operatorMessage);
                                                }
                                                else
                                                    tempbuffer.Add(operatorMessage);

                                                textBoxResponses.Lines = tempbuffer.ToArray();
                                                textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                                                textBoxResponses.ScrollToCaret();

                                                // the response will either be -1 for timeout or 0x06 for ACK or 0x15 for NACK.

                                                switch (response)
                                                {
                                                    case -1:        // timed out waiting for a response
                                                        {
                                                            error = true;   // this will break us out of forever loop
                                                        }
                                                        break;

                                                    case 0x06:      // the calculated CCITT macthes the received CCITT.
                                                        {
                                                            // proceed on with the next block

                                                            progressBar.PerformStep();
                                                            Application.DoEvents();

                                                            success = true;
                                                            blockNumber++;
                                                        }
                                                        break;

                                                    case 0x15:          // the calculated CCITT does not macth the received CCITT.
                                                        {
                                                            // the only other valid response is 0x15 - NACK
                                                            //bool breakHere = true;
                                                        }
                                                        break;

                                                    default:            // something wicked this way cometh. (we should NEVER get here).
                                                        {
                                                            //bool breakHere = true;
                                                        }
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                // if we time out - the remote has stopped listening - abort
                                                // by setting error = true. error is NOT the same as NAK.

                                                error = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 0 bytes read from local file will end this transfer

                                        bytesToSend = 0;
                                    }

                                    if (bytesToSend == 0 || error)
                                        break;
                                }
                                else
                                    break;

                                if (error)
                                    break;
                            }

                            if (error)
                            {
                                MsgBox.Show(string.Format("An error has occurred - the file {0} is not correct on the remote end.", fileNameAtRemote));
                            }
                            else
                            {
                                // we are done with this file - inform the remote by sending a size of 0 to close thr file on
                                // the remote end and have the remote prepare for the next command byte. Always do this to
                                // close the file on the remote even if the Stop button was clicked.

                                response = SendByte(serialPort, 0x00);
                                response = SendByte(serialPort, 0x00);
                            }
                        }
                    }
                    else
                    {
                        MsgBox.Show($"non-ACK response returned after sending filename: {response.ToString("X2")}");
                    }
                }
                else
                {
                    MsgBox.Show($"non-ACK response returned after sending command: {response.ToString("X2")}");
                }
            }
            else
            {
                byte[] commandBuffer = new byte[fileNameAtRemote.Length + 2];     // buffer size if the length of the filename plus 1 for the command byte and 1 for the null terminater
                commandBuffer[0] = 0x02;                                        // retreive file command
                for (int i = 0; i < fileNameAtRemote.Length; i++)
                {
                    commandBuffer[i + 1] = (byte)fileNameAtRemote[i];
                }
                commandBuffer[fileNameAtRemote.Length + 1] = 0x00;

                try
                {
                    if (OpenTCPPort())
                    {
                        // is we were able to open the TCPIP Port, proceed
                        Program.remoteAccess.socket.Send(commandBuffer, 0, commandBuffer.Length, SocketFlags.None);

                        byte[] responseBuffer = new byte[512];
                        for (; ; )
                        {
                            int bytesRead = 0;
                            int bytesReceived = Program.remoteAccess.socket.Receive(responseBuffer);
                            if (bytesReceived == 1 && responseBuffer[0] == 0x06)
                            {
                                // UniFLEX accepted the finename and is now ready to start receiving data blocks
                                // read from the gile on the PC 256 bytes at a time and send count - data - CCITT
                                // and wait for ACK between data blcoks.

                                using (BinaryReader reader = new BinaryReader(File.Open(localFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                                {
                                    if (checkBoxFixNewLines.Checked)
                                        fileContainsOnlyText = FileContainsOnlyText(localFilename);

                                    while (true)    // main loop to send blocks
                                    {
                                        Application.DoEvents();         // see if the user has clicked on Pause or Stop buttons

                                        // Only allow Stop or Pause while getting ready to send the next block

                                        while (pause)
                                        {
                                            Application.DoEvents();
                                            if (stop)
                                            {
                                                // user clicked stop while paused = break out of pause state

                                                pause = false;
                                                break;
                                            }
                                        }

                                        if (!stop)
                                        {
                                            // read the file into memory  a block at a time - replaceing CRLF and LF if required.

                                            try
                                            {
                                                byte[] buffer = reader.ReadBytes(Constants.PACKETSIZE);
                                                bytesRead = buffer.Length;                          // we will use this being zero to trigger the exit from the forever loop

                                                if (bytesRead > 0)
                                                {
                                                    try
                                                    {
                                                        bytesToSend = 0;    // clear the number of bytes to send
                                                        byte[] bufferToSend = new byte[bytesRead + 4];   // this one needs to be big enough to hanlde the bytes from the file and the count and CCITT

                                                        try
                                                        {
                                                            // see if there are any CR/LF sequences to modify - we will only ever make the number of bytes to send smaller or leave as is - never bigger
                                                            byte previouscharacter = 0x00;
                                                            if (checkBoxFixNewLines.Checked && fileContainsOnlyText)
                                                            {
                                                                for (int i = 0, j = 2; i < bytesRead; i++)
                                                                {
                                                                    if (buffer[i] == 0x0a)
                                                                    {
                                                                        // strip line feed by replacing with carriage return unless the previous character was a carriage return
                                                                        // in which case, just ignore the line feed.

                                                                        if (previouscharacter != 0x0d)
                                                                        {
                                                                            bufferToSend[j++] = 0x0d;
                                                                            previouscharacter = 0x0d;
                                                                            bytesToSend++;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        bufferToSend[j++] = buffer[i];
                                                                        previouscharacter = buffer[i];
                                                                        bytesToSend++;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                // just copy the buffer over

                                                                for (int i = 0; i < bytesRead; i++)
                                                                    bufferToSend[i + 2] = buffer[i];

                                                                bytesToSend = bytesRead;
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            MsgBox.Show(e.Message);
                                                        }

                                                        // set how many bytes are to be received
                                                        bufferToSend[0] = (byte)(bytesToSend / 256);
                                                        bufferToSend[1] = (byte)(bytesToSend % 256);

                                                        // set the CCITT value in the buffer
                                                        ushort calculatedCCITT = CRCCCITT(bufferToSend, 2, bytesToSend, 0xffff, 0);
                                                        bufferToSend[bytesToSend + 2] = (byte)(calculatedCCITT / 256);  // factor size bytes at the start into the offset
                                                        bufferToSend[bytesToSend + 3] = (byte)(calculatedCCITT % 256);  // factor size bytes at the start into the offset

                                                        // bufferToSend is ready to be sent - send it and get ACK or NAK.
                                                        // we need a retry loop here in case remote send a NAK

                                                        while (true)
                                                        {
                                                            Program.remoteAccess.socket.Send(bufferToSend, 0, bytesToSend + 4, SocketFlags.None);
                                                            bytesReceived = Program.remoteAccess.socket.Receive(responseBuffer);
                                                            if (bytesReceived == 1 && responseBuffer[0] != 0x06)
                                                            {
                                                                using (StreamWriter sr = new StreamWriter(File.Open("D:/temp/errors.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                                                                {
                                                                    sr.Write("{0}\n", remoteFilename);
                                                                }
                                                            }
                                                            else
                                                                break;
                                                        }
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        MsgBox.Show(e.Message);
                                                        bytesRead = 0;                  // this will get us out of the forever loop
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        // time to tell the remote we are done - send 00 count

                                                        byte[] bufferToSend = new byte[2];   // this one needs to be big enough to hanlde the bytes from the file and the count and CCITT
                                                        bufferToSend[0] = 0x00;
                                                        bufferToSend[1] = 0x00;

                                                        Program.remoteAccess.socket.Send(bufferToSend, 0, 2, SocketFlags.None);
                                                        Program.remoteAccess.socket.Receive(responseBuffer);
                                                        if (responseBuffer[0] != 0x06)
                                                        {
                                                            MsgBox.Show("We need some retry logic here also");
                                                        }
                                                        break;  // we are done
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        MsgBox.Show(e.Message);
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                MsgBox.Show(e.Message);
                                            }
                                        }
                                        else
                                        {
                                            bytesRead = 0;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (bytesRead == 0)
                                break;
                        }
                    }
                    else
                    {
                        MsgBox.Show("Unable to connect to remote via TCP/IP");
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message);
                    error = true;
                }
            }

            currentFileProgress.Items.Clear();                      // make sure there is only ever one item in the status line
            statusLabel.Text = "Idle";                              // format the string
            currentFileProgress.Items.Add(statusLabel);             // add the item
            currentFileProgress.Refresh();                          // Force redraw
            Application.DoEvents();                                 // let the system update the status bar.

            return error;
        }

        private bool Transfer (string localFilename, string remoteFilename, bool uniFLEXFilenameIsDirectory, bool isDirectoryTransfer = false)
        {
            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Idle";
            currentFileProgress.Items.Add(statusLabel);

            Application.DoEvents();

            // error will be set if we should not proceed after theis transfer.
            // so return !error.
            bool error = false;

            bool proceed = true;
            if (radioButtonCOMPort.Checked)
            { 
                Program.OpenComPort(comboBoxCOMPorts.Text, comboBoxBaudRate.Text);
            }
            else if (radioButtonTCPIP.Checked)
            {
                if (!OpenTCPPort())
                    proceed = false;
            }

            if (proceed)
            {
                // Do your communication or other tasks with the open COM port here.

                if (radioButtonSend.Checked)
                {
                    error = SendFileNameAndSendFile(Program.remoteAccess.serialPort, localFilename, remoteFilename, uniFLEXFilenameIsDirectory, isDirectoryTransfer);
                }
                else
                {
                    // selectedFileInfos will contain the list of files to receive. If it is empty, use the name in textboxUniFLEXFileName.Text
                    int selectedCount = selectedFileInfos.Count;
                    if (selectedCount == 1)
                    {
                         string fileToGet = localFilename;

                        // do the single file in textBoxUniFLEXFileName.Text - send mode as 0x0000
                        if (fileToGet.Length == 0)
                        {
                            // if the filename in the local filename text box is empty use the filename from the UniFLEX
                            // filename (just the filename) combined with the local directory selected

                            fileToGet = Path.Combine(textBoxLocalDirName.Text, Path.GetFileName(textBoxUniFLEXFileName.Text));
                        }
                        else
                        {
                            // if the filename in the local filename text box is empty use the filename from the UniFLEX
                            // filename (just the filename) combined with the local directory selected

                            fileToGet = textBoxLocalFileName.Text;
                        }

                        error = SendFileNameAndRecieveFile(Program.remoteAccess.serialPort, fileToGet.Replace(@"\", "/"), textBoxUniFLEXFileName.Text.Replace(@"\", "/"), 0);
                    }
                    else
                    {
                        error = SendFileNameAndRecieveFile(Program.remoteAccess.serialPort, localFilename.Replace(@"\", "/"), remoteFilename.Replace(@"\", "/"), 0);
                    }
                }
            }
            else
            {
                MsgBox.Show("Unable to connect to remote via TCP/IP");
            }

            // all done - close the serial port if we opened it

            if (radioButtonCOMPort.Checked)
            {
                Program.remoteAccess.serialPort.Close(); // Close the port when done.
                Program.remoteAccess.serialPort = null;
            }

            return !error;
        }

        private void SetStartButtonStatus()
        {
            // first make sure we have a COM port selected and (either the local filename text box is enabled or local dir text box is enabled) and the uniflex file name length > 0
            if (comboBoxCOMPorts.Text.Length > 0 && ((textBoxLocalFileName.Text.Length > 0 && textBoxLocalFileName.Enabled) || (textBoxLocalDirName.Text.Length > 0 && textBoxLocalDirName.Enabled)) && textBoxUniFLEXFileName.Text.Length > 0)
            {
                buttonStart.Enabled = true;
                startToolStripMenuItem.Enabled = true;
            }
            else
            {
                // see if we are receiving multiple files
                int selectedCount = selectedFileInfos.Count;

                if (selectedCount > 0 && radioButtonReceive.Checked)
                {
                    buttonStart.Enabled = true;
                    startToolStripMenuItem.Enabled = true;
                }
                else
                {
                    // if not receiving multiple files, make sure the local filename text box does not contain a filename separator
                    if (textBoxLocalFileName.Text.Contains(";") || (textBoxLocalDirName.Text.Length > 0 && textBoxLocalDirName.Enabled))
                    {
                        // there are multiple files to trans fer so treat textBoxUniFLEXFileName as a directory and allow it to be blank

                        buttonStart.Enabled = true;
                        startToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        // otherwise - if there is a uniflex filename AND a local filename specified - we can enable the start button

                        if (textBoxLocalFileName.Text.Length > 0 && textBoxUniFLEXFileName.Text.Length > 0)
                        {
                            buttonStart.Enabled = true;
                            startToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            buttonStart.Enabled = false;
                            startToolStripMenuItem.Enabled = false;
                        }
                    }
                }
            }
        }

        private bool ComPortIsAvailable(string portName)
        {
            bool isAvaiable = false;

            try
            {
                using (SerialPort port = new SerialPort(portName))
                {
                    port.Open();
                    isAvaiable = true;
                    port.Close();
                }
            }
            catch (IOException ex)
            {
                isAvaiable = false;
                //MsgBox.Show($"COM Port {portName} is in use: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                isAvaiable = false;
                // MsgBox.Show($"Access to COM Port {portName} is unauthorized: {ex.Message}");
            }
            catch (Exception ex)
            {
                isAvaiable = false;
                // MsgBox.Show($"An error occurred: {ex.Message}");
            }

            return isAvaiable;
        }

        private string GetCurrentWorkingDirectory (SerialPort serialPort)
        {
            byte[] request = new byte[1];
            request[0] = sendCurrentDirectoryCommand;
            byte[] response = new byte[512];
            if (radioButtonTCPIP.Checked)
            {
                // retry 4 times before bothering the operator.
                int retryCount = 4;
                while (retryCount > 0)
                {
                    try
                    {
                        if (OpenTCPPort())
                        {
                            Program.remoteAccess.socket.Send(request);
                            Program.remoteAccess.socket.Receive(response, 512, SocketFlags.None);
                            retryCount = -1;
                        }
                        else
                        {
                            MsgBox.Show("Unable to connect to remote via TCP/IP");
                        }
                    }
                    catch
                    {
                        if (retryCount <= 0)
                        {
                            DialogResult r = MsgBox.Show("Unable to connect to remote via TCP/IP - would you like to retry?", "Unable to connect", MessageBoxButtons.RetryCancel);
                            if (r == DialogResult.Retry)
                            {
                                try
                                {
                                    if (OpenTCPPort())
                                    {
                                        Program.remoteAccess.socket.Send(request);
                                        Program.remoteAccess.socket.Receive(response, 512, SocketFlags.None);
                                    }
                                    else
                                    {
                                        MsgBox.Show("Unable to connect to remote via TCP/IP");
                                    }
                                }
                                catch
                                {
                                    MsgBox.Show("Still unable to connect to remote via TCP/IP");
                                }
                            }
                        }
                        else
                        {
                            retryCount--;
                        }
                    }
                }
            }
            else
            {
                Program.OpenComPort(comboBoxCOMPorts.Text, comboBoxBaudRate.Text);
                if (Program.remoteAccess.serialPort != null)
                {
                    try
                    {
                        Program.remoteAccess.serialPort.Write(request, 0, 1);
                        try
                        {
                            Program.remoteAccess.serialPort.Read(response, 0, 1);        // transfer will send an ACK before the directory name
                            if (response[0] == 0x06)
                            {
                                for (int i = 0; i < 511; i++)
                                {
                                    try
                                    {
                                        Program.remoteAccess.serialPort.Read(response, i, 1);    // stuff the bytes into response
                                        if (response[i] == 0x00)            // null character signals end of response
                                            break;
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            MsgBox.Show("serial read failed");
                        }
                    }
                    catch 
                    {
                        MsgBox.Show("serial write failed");
                    }
                    Program.remoteAccess.serialPort.Close();
                    Program.remoteAccess.serialPort = null;
                }
            }

            return ASCIIEncoding.ASCII.GetString(response).Trim().Replace("\0", "");
        }

        private void frmTransfer_Load(object sender, EventArgs e)
        {
            // we are going to set the width and height here because we want to allow
            // a larger size in the designer for placing controls that get relocated
            // when they need to be shown.

            this.Size = new Size(525, 515);

            try
            {
                registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\EvensonConsultingServices\TransferUniFLEX", true);
                if (registryKey == null)
                {
                    try
                    {
                        registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EvensonConsultingServices\TransferUniFLEX", true);
                    }
                    catch (Exception eRegistry1)
                    {
                        MsgBox.Show(string.Format("Unable to open registry", eRegistry1.Message));
                    }

                    if (registryKey == null)
                    {
                        MsgBox.Show("Unable to open registry");
                    }
                }
            }
            catch (Exception eRegistry2)
            {
                registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EvensonConsultingServices\TransferUniFLEX", true);
                if (registryKey == null)
                    MsgBox.Show(string.Format("Unable to open registry", eRegistry2.Message));
            }

            // get the available com ports loaded into the list box comboBoxCOMPorts

            string[] portNames = SerialPort.GetPortNames().OrderBy(port => port).ToArray();

            comboBoxCOMPorts.Items.Clear();
            foreach (string portName in portNames)
            {
                //if (portName != "COM15")
                {
                    if (ComPortIsAvailable(portName))
                        comboBoxCOMPorts.Items.Add(portName);
                }
            }

            if (registryKey != null)
            {
                string comPort   = (string)registryKey.GetValue("COM Port", "COM1");
                string baudRate  = (string)registryKey.GetValue("Baud Rate", "38400");
                string method    = (string)registryKey.GetValue("Method", "COM");
                string iPAddress = (string)registryKey.GetValue("IP Address", "");
                string port      = (string)registryKey.GetValue("Port", "");

                comboBoxCOMPorts.Text = comPort;
                comboBoxBaudRate.Text = baudRate;
                if (method == "COM")
                {
                    radioButtonCOMPort.Checked = true;
                    Program.currentSelectedTransport = (int)SELECTED_TRANSPORT.RS232;
                }
                else if (method == "TCP")
                {
                    radioButtonTCPIP.Checked = true;
                    Program.currentSelectedTransport = (int)SELECTED_TRANSPORT.TCPIP;
                }

                textBoxIPAddress.Text = iPAddress;
                textBoxPort.Text = port;

                string replaceLineFeeds = (string)registryKey.GetValue("Replace LineFeeds", "N");
                if (replaceLineFeeds == "Y")
                    checkBoxFixNewLines.Checked = true;
                else
                    checkBoxFixNewLines.Checked = false;

                string wariningsOff = (string)registryKey.GetValue("Warnings Off", "N");
                if (wariningsOff == "Y")
                    checkBoxWarningsOff.Checked = true;
                else
                    checkBoxWarningsOff.Checked = false;

                string keepZeroLengthFiles = (string)registryKey.GetValue("keep Zero Length Files", "N");
                if (keepZeroLengthFiles == "Y")
                    checkBoxKeepZeroLengthFiles.Checked = true;
                else
                    checkBoxKeepZeroLengthFiles.Checked = false;
            }

            groupBoxTCPIP.Top  = groupBoxCOMPort.Top;
            groupBoxTCPIP.Left = groupBoxCOMPort.Left;

            checkBoxMinix.Checked = Properties.Settings.Default.isMinix;

            // Not needed - this is taken care of when we set checkBoxMinix.Checked  in the above statement.
            //
            //// if Minix is selected at start up change the text
            //if (checkBoxMinix.Checked)
            //    checkBoxFixNewLines.Text = "Remove <CR> when Sending Text files";

            // Create and add a ToolStripStatusLabel programmatically

            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel("Idle");
            currentFileProgress.Items.Add(statusLabel);

            // Create and add a ToolStripProgressBar programmatically

            ToolStripProgressBar progressBar = new ToolStripProgressBar();

            currentFileProgress.Items.Clear();                      // make sure there is only ever one item in the status line
            statusLabel.Text = "Idle";                              // format the string
            currentFileProgress.Items.Add(statusLabel);             // add the item
            currentFileProgress.Refresh();                          // Force redraw

            ShowVersionInTitle(null);
        }

        #region control handlers
        #region buttons
        private void buttonStart_Click(object sender, EventArgs e)
        {
            stop = false;
            string currentWorkingDirectory = "";

            // save current configuration as the default
            Cursor = Cursors.WaitCursor;

            if (radioButtonSend.Checked)        // only doing send
            {
                // we only need the current working directory if we are sending

                currentWorkingDirectory = GetCurrentWorkingDirectory(Program.remoteAccess.serialPort);

                if (registryKey != null)
                {
                    //registryKey.SetValue("COM Port", comboBoxCOMPorts.Text);
                    //registryKey.SetValue("Baud Rate", comboBoxBaudRate.Text);
                    //registryKey.SetValue("Replace LineFeeds", checkBoxFixNewLines.Checked ? "Y" : "N");
                }

                // determine if we are doing file transfer(s) or a complete directory transfer

                if (textBoxLocalDirName.Enabled)
                {
                    // we are doing a Complete Directory Transfer - get all of the filenames from the directory

                    bool proceed = true;
                    if (radioButtonTCPIP.Checked)
                    {
                        if (!((Program.remoteAccess.socket != null) && Program.remoteAccess.socket.Connected))
                        {
                            proceed = false;
                        }
                    }
                    if (proceed)
                    {
                        DialogResult r = DialogResult.Yes;      // default to yes in case warnings are off
                        if (!checkBoxWarningsOff.Checked)
                        {
                            // if warnings are NOT turned off - issue warning
                            string os = "UniFLEX";
                            if (Program.isMinix)
                                os = "Minix";

                            r = MsgBox.Show($"Transferring multiple files means that the {os} filename is a directory - do you wish to continue?", "Warning", MessageBoxButtons.YesNo);
                        }
                        if (r == DialogResult.Yes)
                        {
                            if (!textBoxUniFLEXFileName.Text.StartsWith("/"))
                            {
                                DialogResult dr = MsgBox.Show(string.Format("The remote current working directory is: {0}", currentWorkingDirectory), "OK to proceed", MessageBoxButtons.YesNo);
                                if (dr != DialogResult.Yes)
                                {
                                    if (textBoxUniFLEXFileName.Text.Length == 0)
                                        textBoxUniFLEXFileName.Text = currentWorkingDirectory;

                                    proceed = false;
                                }
                            }
                            else
                                proceed = true;

                            //char[] trimChars = new char[2];
                            //trimChars[0] = '\\';
                            //trimChars[1] = (char)0x00;

                            //if (textBoxDirectoryReplaceString.Text.Length == 0)
                            //    textBoxDirectoryReplaceString.Text = Path.GetDirectoryName(textBoxLocalDirName.Text);

                            //if (textBoxDirectoryReplaceString.Text.EndsWith("\\"))
                            //    textBoxDirectoryReplaceString.Text.TrimEnd(trimChars);

                            //string targetDirectory = textBoxLocalDirName.Text.Replace(textBoxDirectoryReplaceString.Text, "");  // do this here so we only have to do it once

                            //if (targetDirectory.StartsWith("\\"))
                            //    targetDirectory.TrimStart(trimChars);

                            //targetDirectory += Path.Combine(targetDirectory, textBoxUniFLEXFileName.Text);

                            string targetDirectory = textBoxUniFLEXFileName.Text;

                            if (checkBoxRecursive.Checked)
                            {
                                // this we are going to copy an entire directory recursively, we need to know what part of the
                                // local filename to remove from the file name for the remote filename. For instance if the 
                                // local directory is D:\DefaultPath\DISKS\UNIFLEX\ufsources\utilitiesII and we want the files
                                // to go to the remote as /src/utilitiesII in order to build the path to send to the remote we
                                // need to know that we must strip "D:\DefaultPath\DISKS\UNIFLEX\ufsources\utilitiesII" from 
                                // the path before we send it to the remote as the filename, so when sending alarmclk.c from 
                                // D:\DefaultPath\DISKS\UNIFLEX\ufsources\utilitiesII\src\at we want to tell the remote to
                                // save the file as alarmclk.c in directory /src/utilitiesII/src/at by removing the path
                                // D:\DefaultPath\DISKS\UNIFLEX\ufsources\utilitiesII from the filename before we add in the
                                // path specified in the remote directory (filename) name

                                r = MsgBox.Show(string.Format("The files will be created in the directory {0} - do you wish to continue?", targetDirectory), "Warning", MessageBoxButtons.YesNo);
                                if (r != DialogResult.Yes)
                                {
                                    proceed = false;
                                }
                            }

                            if (proceed)
                            {
                                string[] filenames = Directory.GetFiles(textBoxLocalDirName.Text, "*", checkBoxRecursive.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                                foreach (string filename in filenames)
                                {
                                    // transfer the file to/from UniFLEX based on which radio button is checked - textBoxUniFLEXFileName is a directory name
                                    //
                                    // false will be returned if we should not proceed

                                    string remoteDirectory = Path.Combine(targetDirectory, filename.Replace(textBoxLocalDirName.Text + '\\', ""));
                                    remoteDirectory = remoteDirectory.Replace("\\", "/");
                                    remoteDirectory = textBoxUniFLEXFileName.Text + remoteDirectory;

                                    tempbuffer.Add(string.Format("Sending: {0} to {1}", filename, remoteDirectory));
                                    textBoxResponses.Lines = tempbuffer.ToArray();
                                    //textBoxResponses.AppendText(string.Format("Sending: {0} to {1}\r\n", filename, remoteDirectory));
                                    textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                                    textBoxResponses.ScrollToCaret();

                                    if (Transfer(filename.Replace(@"\", "/"), remoteDirectory.Replace(@"\", "/"), true, true))
                                    {
                                        if (stop)
                                        {
                                            buttonPause.Text = "Pause";
                                            break;
                                        }
                                    }
                                    else
                                        break;
                                }
                            }
                            else
                                MsgBox.Show("Transfer aborted");
                        }
                        else
                            MsgBox.Show("Transfer aborted");
                    }
                }
                else
                {
                    // we are doing file - not directory transfer(s)
                    // could be a single file transfer or multiple file transfer

                    string[] filenames = textBoxLocalFileName.Text.Split(';');

                    if (filenames.Length > 1)
                    {
                        // Multiple file transfers

                        DialogResult r = DialogResult.Yes;      // default to yes in case warnings are off
                        if (!checkBoxWarningsOff.Checked)
                        {
                            // if warnings are NOT turned off - issue warning
                            string os = "UniFLEX";
                            if (Program.isMinix)
                                os = "Minix";

                            r = MsgBox.Show($"Transferring multiple files means that the {os} filename is a directory - do you wish to continue?", "Warning", MessageBoxButtons.YesNo);
                        }
                        if (r == DialogResult.Yes)
                        {
                            bool proceed = true;

                            if (!textBoxUniFLEXFileName.Text.StartsWith("/"))
                            {
                                DialogResult dr = MsgBox.Show(string.Format("The remote current working directory is: {0}", currentWorkingDirectory), "OK to proceed", MessageBoxButtons.YesNo);
                                if (dr != DialogResult.Yes)
                                    proceed = false;
                            }

                            if (proceed)
                            {
                                foreach (string filename in filenames)
                                {
                                    string remotename = Path.GetFileName(filename).Replace(@"\", "/");
                                    string target = Path.Combine(textBoxUniFLEXFileName.Text, remotename).Replace(@"\", "/");
                                    string responsesText = string.Format("Sending: {0} to {1}", filename, target);

                                    tempbuffer.Add(responsesText);
                                    textBoxResponses.Lines = tempbuffer.ToArray();
                                    //textBoxResponses.AppendText(responsesText);                                    
                                    textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                                    textBoxResponses.ScrollToCaret();

                                    // transfer the file to/from UniFLEX based on which radio button is checked - textBoxUniFLEXFileName is a directory name
                                    //
                                    // false will be returned if we should not proceed

                                    if (Transfer(filename.Replace(@"\", "/"), textBoxUniFLEXFileName.Text.Replace(@"\", "/"), true, false))
                                    {
                                        if (stop)
                                        {
                                            buttonPause.Text = "Pause";
                                            break;
                                        }
                                    }
                                    else
                                        break;
                                }
                            }
                            else
                                MsgBox.Show("Transfer aborted");
                        }
                        else
                            MsgBox.Show("Transfer aborted");
                    }
                    else
                    {
                        // this is a singlr file transfer

                        tempbuffer.Add(string.Format("Sending: {0} to {1}", textBoxLocalFileName.Text, textBoxUniFLEXFileName.Text));
                        textBoxResponses.Lines = tempbuffer.ToArray();
                        //textBoxResponses.AppendText(string.Format("Sending: {0} to {1}\r\n", textBoxLocalFileName.Text, textBoxUniFLEXFileName.Text));                        
                        textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                        textBoxResponses.ScrollToCaret();

                        bool success = Transfer(textBoxLocalFileName.Text.Replace(@"\", "/"), textBoxUniFLEXFileName.Text.Replace(@"\", "/"), false, false);
                        if (stop)
                            buttonPause.Text = "Pause";
                    }
                }
            }
            else
            {
                int selectedCount = selectedFileInfos.Count;

                if (selectedCount > 1)
                {
                    DialogResult result = DialogResult.Yes;      // default to yes in case warnings are off
                    if (!checkBoxWarningsOff.Checked)
                    {
                        result = MsgBox.Show("Multple files selected so local directory will be used as the directory to save the files in. Do you wich to proceed?", "Multiple Files Selected", MessageBoxButtons.YesNo);
                    }
                    if (result == DialogResult.Yes)
                    {
                        // we are going to request multiple files specified in the selectedFileInfos list

                        foreach (KeyValuePair<string, FileInformation> fileInfo in selectedFileInfos)
                        {
                            // do all the selected files first
                            if (!fileInfo.Value.isDirectory)
                            {
                                // we will use textboxLocalFileName.Text as the target directory
                                string localFilename = textBoxLocalDirName.Text.Replace(@"\", "/");
                                if (textBoxLocalDirName.Text.Length > 0)
                                    localFilename += "/";
                                localFilename += fileInfo.Key;

                                string remoteFilename = fileInfo.Key;

                                // if the selected file count is > 1, the contents of textBoxUniFLEXFileName is a directory path
                                if (textBoxUniFLEXFileName.Text.Length > 0 && selectedFileInfos.Count > 1)
                                    remoteFilename = Path.Combine(textBoxUniFLEXFileName.Text, fileInfo.Key).Replace(@"\", "/");

                                bool success = Transfer(localFilename.Replace(@"\", "/"), remoteFilename.Replace(@"\", "/"), false, false);

                                if (!success)
                                    break;
                            }
                            else
                            {
                                // we are handling recursion at the main form - no selection dialog will be 
                                // presented - the user made their choic in the Directory Browse dialog
                                //
                                // this is a directory - we need to ask the UniFLEX machine for all of the files
                                // and and if doing recursive, the directories contained in this directory.

                                // the call to GetRemoteDirectory will fill Program.remoteAccess.sortedInformations, so if
                                // we will need the current contents we must save it here.

                                string currentDirectoryBrowsing = textBoxUniFLEXFileName.Text;
                                if (!currentDirectoryBrowsing.EndsWith("/") && !currentDirectoryBrowsing.EndsWith(@"\"))
                                    currentDirectoryBrowsing += "/";

                                Program.remoteAccess.GetRemoteDirectory(currentDirectoryBrowsing, checkBoxRecursive.Checked);
                            }
                        }
                        //// now do the directories - maybe recursively
                        //foreach (KeyValuePair<string, FileInformation> fileInfo in selectedFileInfos)
                        //{
                        //    if (fileInfo.Value.isDirectory)
                        //    {
                        //        MessageBox.Show($"{fileInfo.Key} is a directory");
                        //    }
                        //}
                    }
                }
                else
                {
                    // we want a single file specified in textboxUniFLEXFileName.Text. We cannot do a single directory download
                    // from the UniFLEX machine since there is no 'stat' information

                    foreach (KeyValuePair<string, FileInformation> fileInfo in selectedFileInfos)
                    {
                        if (fileInfo.Value.isDirectory)
                        {
                            // the user selected a directory to transfer. Do all the files in this directory. To do this, we
                            // need to get the list of files in the directory. textBoxUniFLEXFileName.Text will have the
                            // UniFLEX source directory name

                            MsgBox.Show("directory transfer not yet implemented");
                        }
                        else
                        {
                            bool success = Transfer(textBoxLocalFileName.Text.Replace(@"\", "/"), textBoxUniFLEXFileName.Text.Replace(@"\", "/"), false, false);
                        }
                    }
                }
            }
            Cursor = Cursors.Default;
        }

        private void buttonBrowseLocalFileName_Click(object sender, EventArgs e)
        {
            if (radioButtonSend.Checked)
            {
                // pick a file or files to transfer

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = true;

                DialogResult r = dlg.ShowDialog();
                if (r == DialogResult.OK)
                {
                    textBoxLocalFileName.Text = "";
                    foreach (string filename in dlg.FileNames)
                    {
                        if (textBoxLocalFileName.Text.Length > 0)
                            textBoxLocalFileName.Text += ";";
                        textBoxLocalFileName.Text += filename;
                    }

                    // if a single file was selected and the UniFLEX filename text box is empty - through just the file in there

                    if (dlg.FileNames.Length == 1) // && textBoxUniFLEXFileName.Text.Length == 0)
                    {
                        textBoxUniFLEXFileName.Text = Path.GetFileName(dlg.FileName);
                        if (checkBoxMinix.Checked)
                            labelUniFLEXFileName.Text = "Minix File Name";
                        else
                            labelUniFLEXFileName.Text = "UniFLEX File Name";
                    }
                    else
                    {
                        if (checkBoxMinix.Checked)
                            labelUniFLEXFileName.Text = "Minix Directory";
                        else
                            labelUniFLEXFileName.Text = "UniFLEX Directory";
                    }
                }
            }
            else
            {
                // we need a Save file dialog instead.

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = textBoxLocalFileName.Text;

                //// old code
                //DialogResult r = dlg.ShowDialog();
                //if (r == DialogResult.OK)
                //{
                //    textBoxLocalFileName.Text = dlg.FileName;
                //}
                ////
                ///
                // code from chatGPT
                if (!string.IsNullOrEmpty(Properties.Settings.Default.LastUsedFolder))
                {
                    dlg.InitialDirectory = Properties.Settings.Default.LastUsedFolder;
                }

                //dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Save the selected folder path for next time
                    string path = System.IO.Path.GetDirectoryName(dlg.FileName);

                    Properties.Settings.Default.LastUsedFolder = path;
                    Properties.Settings.Default.Save();

                    // Do something with dlg.FileName...

                    textBoxLocalFileName.Text = dlg.FileName;
                }
                // end of code from charGPT
            }
            SetStartButtonStatus();
        }

        private void buttonBrowseLocalDirectory_Click(object sender, EventArgs e)
        {
            // pick a directory or directories to transfer

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select a Directory";
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;
            dlg.FileName = "Folder Selection";

            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                string selectedDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);
                textBoxLocalDirName.Text = selectedDirectory;

                //if (textBoxUniFLEXFileName.Text.Length == 0)
                //{
                //    string [] parts = textBoxLocalDirName.Text.Replace(@"\", "/").Split('/');

                //    if (parts.Length > 0)
                //        textBoxUniFLEXFileName.Text = parts[parts.Length - 1];
                //}
                if (checkBoxMinix.Checked)
                    labelUniFLEXFileName.Text = "Minix Directory";
                else
                    labelUniFLEXFileName.Text = "UniFLEX Directory";
            }
            SetStartButtonStatus();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text == "Pause")
            {
                buttonPause.Text = "Resume";
                pause = true;
            }
            else
            {
                buttonPause.Text = "Pause";
                pause = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            stop = true;
        }

        // this button is used to clear the UniFLEX filename text box if Send is selected. If Receive a file
        // is selected, this button becomes the Browse button into the UniFLEX file system

        public Dictionary<string, FileInformation> allFileInfos      = new Dictionary<string, FileInformation>();
        public Dictionary<string, FileInformation> selectedFileInfos = new Dictionary<string, FileInformation>();

        // this is also the handler for the button when its label is 'Browse"
        private void buttonClearUniFLEXFilename_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (radioButtonSend.Checked)
            {
                textBoxUniFLEXFileName.Text = "";
            }
            else if (radioButtonReceive.Checked)
            {
                // this is now a browse button into the UniFLEX current Directory
                // pass in the currently selected list as the selecteFilesInfos

                frmUniFLEXBrowse dlg = null;
                if (radioButtonCOMPort.Checked)
                {
                    //Program.remoteAccess.serialPort = Program.OpenComPort(comboBoxCOMPorts.Text, comboBoxBaudRate.Text);
                    Program.OpenComPort(comboBoxCOMPorts.Text, comboBoxBaudRate.Text);
                    dlg = new frmUniFLEXBrowse(this, Program.remoteAccess.serialPort, textBoxUniFLEXFileName.Text, selectedFileInfos, checkBoxAllowDirectorySelection.Checked);

                    dlg.currentWorkingDirectory = GetCurrentWorkingDirectory(Program.remoteAccess.serialPort);
                }
                else
                {
                    if (OpenTCPPort())
                    {
                        dlg = new frmUniFLEXBrowse(this, Program.remoteAccess.socket, textBoxUniFLEXFileName.Text, selectedFileInfos, textBoxIPAddress.Text, textBoxPort.Text, checkBoxAllowDirectorySelection.Checked);
                        dlg.currentWorkingDirectory = GetCurrentWorkingDirectory(Program.remoteAccess.serialPort);
                    }
                    else
                    {
                        MsgBox.Show("Unable to connect to remote via TCP/IP");
                    }

                    // do not close the socket once it has been created
                    // socket.Close();
                }

                if (dlg != null)
                {
                    if (dlg.currentWorkingDirectory != "")      // GetCurrentWorkingDirectory will return an empty directory name if none found
                    {
                        // GetCurrentWorkingDirectory closes the com port - should fix. but for now just make sure it is open
                        // in case we are called from the Browse dialog
                        bool portWasOpen = Program.remoteAccess.serialPort == null ? false : true;
                        if (!portWasOpen)
                        {
                            Program.remoteAccess.serialPort = Program.OpenComPort(comboBoxCOMPorts.Text, comboBoxBaudRate.Text);
                        }

                        DialogResult result = dlg.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            selectedFileInfos = dlg.selectedFileInformations;

                            // user wants to get the single file selected in the list view.
                            if (selectedFileInfos.Count == 1)
                            {
                                // if the single file selected is a directory - add the filename to the current path
                                string selectedFile = dlg.selectedFile;
                                string selectedFileInfosFilename = Path.GetFileName(selectedFile);
                                
                                if (Program.isMinix)
                                {
                                    labelUniFLEXFileName.Text = "Minix Directory";
                                }
                                else
                                {
                                    labelUniFLEXFileName.Text = "UniFLEX Directory";
                                }

                                try 
                                { 
                                    bool isDirectory = selectedFileInfos[selectedFileInfosFilename].isDirectory;
                                    if (isDirectory)
                                    {
                                        textBoxUniFLEXFileName.Text = textBoxUniFLEXFileName.Text + "/" + selectedFile;

                                        buttonStart.Enabled = true;
                                        startToolStripMenuItem.Enabled = true;
                                        if (checkBoxMinix.Checked)
                                            labelUniFLEXFileName.Text = "Minix Directory";
                                        else
                                            labelUniFLEXFileName.Text = "UniFLEX Directory";

                                        // enabale and disable the appropriate text boxes
                                        textBoxLocalFileName.Enabled = false;
                                        buttonBrowseLocalFileName.Enabled = false;
                                        textBoxLocalDirName.Enabled = true;
                                        buttonBrowseLocalDirectory.Enabled = true;
                                    }
                                    else    // single file selected and it is NOT a directory - handle normal
                                    {       // fill in the textboxes appropriately

                                        string justTheSelectedFileName = Path.GetFileName(selectedFile);
                                        textBoxUniFLEXFileName.Text = selectedFile;

                                        string justTheLocalDirectoryName = "";
                                        if (textBoxLocalFileName.Text.Length > 0)
                                            justTheLocalDirectoryName = Path.GetDirectoryName(textBoxLocalFileName.Text);

                                        if (justTheLocalDirectoryName.Length > 0)
                                        {
                                            if (textBoxLocalFileName.Text.EndsWith("/") || textBoxLocalFileName.Text.EndsWith("\\"))
                                                textBoxLocalFileName.Text = textBoxLocalFileName.Text.Replace("\\", "/") + selectedFileInfosFilename;
                                            else
                                                textBoxLocalFileName.Text = textBoxLocalFileName.Text.Replace("\\", "/") + "/" + selectedFileInfosFilename;
                                        }
                                        else //textBoxLocalFileName.Text = selectedFileInfosFilename;
                                        {
                                            string lastUsedFolder = Properties.Settings.Default.LastUsedFolder;
                                            if (lastUsedFolder.Length > 0)
                                                textBoxLocalFileName.Text = Path.Combine(lastUsedFolder, justTheSelectedFileName);
                                            else
                                                textBoxLocalFileName.Text = justTheSelectedFileName;
                                        }

                                        buttonStart.Enabled = true;
                                        startToolStripMenuItem.Enabled = true;
                                        if (checkBoxMinix.Checked)
                                            labelUniFLEXFileName.Text = "Minix File Name";
                                        else
                                            labelUniFLEXFileName.Text = "UniFLEX File Name";

                                        // enabale and disable the appropriate text boxes
                                        textBoxLocalFileName.Enabled = true;
                                        buttonBrowseLocalFileName.Enabled = true;
                                        textBoxLocalDirName.Enabled = false;
                                        buttonBrowseLocalDirectory.Enabled = false;
                                    }
                                }
                                catch (Exception ex) 
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else
                            {
                                // user wants to get the multiple files from the list view or a directory was selected. so it's OK to enable Start

                                if (checkBoxMinix.Checked)
                                    labelUniFLEXFileName.Text = "Minix Directory ";
                                else
                                    labelUniFLEXFileName.Text = "UniFLEX Directory";

                                buttonStart.Enabled = true;
                                startToolStripMenuItem.Enabled = true;

                                textBoxUniFLEXFileName.Text = dlg.currentDirectoryNameToBrowse;
                                textBoxLocalFileName.Enabled = false; buttonBrowseLocalFileName.Enabled = false;
                                textBoxLocalDirName.Enabled = true; buttonBrowseLocalDirectory.Enabled = true;

                                // Browser control has already filled in our selectd file information dictionary weeding out the directories and
                                // only populating it with the files selected. So there is nothing more to do here
                            }
                        }

                        if (!portWasOpen)
                        {
                            if (Program.remoteAccess.serialPort != null)
                            {
                                Program.remoteAccess.serialPort.Close();
                                Program.remoteAccess.serialPort = null;
                            }
                        }
                    }
                }
            }
            Cursor = Cursors.Default;
        }

        private void buttonClearResponseWindow_Click(object sender, EventArgs e)
        {
            textBoxResponses.Text = "";
            tempbuffer.Clear();
        }
        #endregion
        #region text boxes
        private void textBoxUniFLEXFileName_TextChanged(object sender, EventArgs e)
        {
            SetStartButtonStatus();
        }

        private void textBoxLocalFileName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxLocalFileName.Text.Length > 0)
            {
                buttonBrowseLocalDirectory.Enabled = false;
                textBoxLocalDirName.Enabled = false;
                checkBoxRecursive.Enabled = false;

                if (textBoxUniFLEXFileName.Text.Length > 0)
                    buttonStart.Enabled = true;
            }
            else
            {
                buttonBrowseLocalDirectory.Enabled = true;
                textBoxLocalDirName.Enabled = true;
                checkBoxRecursive.Enabled = true;
            }

            if (radioButtonSend.Checked)
            {
                if (File.Exists(textBoxLocalFileName.Text))
                    SetStartButtonStatus();
                else
                {
                    buttonStart.Enabled = false;
                    startToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                // if the name exists as a directory in the path - do not allow transfer
                if (Directory.Exists(textBoxLocalFileName.Text))
                {
                    buttonStart.Enabled = false;
                    startToolStripMenuItem.Enabled = false;
                }
                else
                    SetStartButtonStatus();
            }
        }

        private void textBoxLocalDirName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxLocalDirName.Text.Length > 0)
            {
                buttonBrowseLocalFileName.Enabled = false;
                textBoxLocalFileName.Enabled = false;
                checkBoxRecursive.Enabled = true;

                int selectedCount = 0;
                if (Program.isMinix)
                    selectedCount = selectedFileInfos.Count;
                else
                    selectedCount = selectedFileInfos.Count;

                if (selectedCount == 0)
                    textBoxDirectoryReplaceString.Text = textBoxLocalDirName.Text;
            }
            else
            {
                buttonBrowseLocalFileName.Enabled = true;
                textBoxLocalFileName.Enabled = true;
                textBoxDirectoryReplaceString.Text = "";

                if (textBoxLocalFileName.Text.Length > 0)
                    checkBoxRecursive.Enabled = false;
                else
                    checkBoxRecursive.Enabled = true;
            }

            if (radioButtonSend.Checked)
            {
                if (Directory.Exists(textBoxLocalDirName.Text))
                    SetStartButtonStatus();
                else
                {
                    buttonStart.Enabled = false;
                    startToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                int selectedCount = 0;
                if (Program.isMinix)
                    selectedCount = selectedFileInfos.Count;
                else
                    selectedCount = selectedFileInfos.Count;

                if (selectedCount == 0)
                {
                    buttonStart.Enabled = false;
                    startToolStripMenuItem.Enabled = false;
                }
                else
                {
                    buttonStart.Enabled = true;
                    startToolStripMenuItem.Enabled = true;
                }
            }
        }
        #endregion
        #region other controls
        private void comboBoxCOMPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStartButtonStatus();

            registryKey.SetValue("COM Port", comboBoxCOMPorts.Text);
            Program.remoteAccess.comboBoxCOMPorts = comboBoxCOMPorts.Text;
        }

        private void comboBoxCOMPorts_DropDown(object sender, EventArgs e)
        {
            return;

            //string[] portNames = SerialPort.GetPortNames().OrderBy(port => port).ToArray();

            //comboBoxCOMPorts.Items.Clear();
            //foreach (string portName in portNames)
            //{
            //    if (portName != "COM15")
            //    {
            //        if (ComPortIsAvailable(portName))
            //            comboBoxCOMPorts.Items.Add(portName);
            //    }
            //}
        }

        private void comboBoxBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            registryKey.SetValue("Baud Rate", comboBoxBaudRate.Text);
            Program.remoteAccess.comboBoxBaudRate = comboBoxBaudRate.Text;
        }

        private void HandleDirectionChanged ()
        {
            if (radioButtonSend.Checked)
            {
                // enable for Send to UniFLEX
                checkBoxFixNewLines.Visible = true;
                checkBoxRecursive.Visible = true;
                checkBoxAllowDirectorySelection.Visible = false;
                textBoxDirectoryReplaceString.Visible = true;

                buttonClearUniFLEXFilename.Text = "Clear";
                Program.currentDirectionIsSending = true;
            }
            else
            {
                if (checkBoxRecursive.Checked)
                {
                    MsgBox.Show("This is still not working", "Notice", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                // enable for Receive from UniFLEX 
                checkBoxFixNewLines.Visible = false;
                checkBoxRecursive.Visible = true;      // still testing - not ready to implement recursive receiving from UniFLEX yet in release build
                checkBoxAllowDirectorySelection.Visible = true;
                textBoxDirectoryReplaceString.Visible = false;

                buttonClearUniFLEXFilename.Text = "Browse";
                Program.currentDirectionIsSending = false;
            }
        }

        private void radioButtonSend_CheckedChanged(object sender, EventArgs e)
        {
            HandleDirectionChanged();
        }

        private void radioButtonReceive_CheckedChanged(object sender, EventArgs e)
        {
            HandleDirectionChanged();
        }

        private void checkBoxFixNewLines_CheckedChanged(object sender, EventArgs e)
        {
            registryKey.SetValue("Replace LineFeeds", checkBoxFixNewLines.Checked ? "Y" : "N");
        }

        private void checkBoxTopLevelOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRecursive.Checked)
            {
                if (radioButtonReceive.Checked)
                {
                    MsgBox.Show("This is still not working", "Notice", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                textBoxLocalDirName.Enabled = true;
                textBoxLocalFileName.Enabled = false;
            }
            else
            {
                textBoxLocalFileName.Enabled = true;
            }
        }
        #endregion

        #endregion

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string executionDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string chmFilePath = Path.Combine(executionDirectory, "TransferUniFLEX_-_transfer.chm");
            if (!System.IO.File.Exists(chmFilePath))
            {
                MsgBox.Show("The specified .chm file does not exist.");
                return;
            }

            // Launch the default .chm viewer
            Process.Start(chmFilePath);
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonStart_Click(sender, e);
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonPause_Click(sender, e);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonStop_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonHelp_Click(sender, e);
        }

        bool IsTextAValidIPAddress(string text)
        {
            System.Net.IPAddress test;
            return System.Net.IPAddress.TryParse(text, out test);
        }

        private void textBoxIPAddress_TextChanged(object sender, EventArgs e)
        {
            registryKey.SetValue("IP Address", textBoxIPAddress.Text);
        }

        private void radioButtonCOMPort_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCOMPort.Checked)
            {
                groupBoxCOMPort.Visible = true;
                groupBoxTCPIP.Visible = false;
                Program.currentSelectedTransport = (int)SELECTED_TRANSPORT.RS232;

                registryKey.SetValue("Method", "COM");
            }
        }

        private void radioButtonTCPIP_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTCPIP.Checked)
            {
                groupBoxCOMPort.Visible = false;
                groupBoxTCPIP.Visible = true;
                Program.currentSelectedTransport = (int)SELECTED_TRANSPORT.TCPIP;

                registryKey.SetValue("Method", "TCP");
            }
        }

        private void textBoxPort_TextChanged(object sender, EventArgs e)
        {
            registryKey.SetValue("Port", textBoxPort.Text);
        }

        private void checkBoxWarningsOff_CheckedChanged(object sender, EventArgs e)
        {
            registryKey.SetValue("Warnings Off", checkBoxWarningsOff.Checked ? "Y" : "N");
        }

        private void checkBoxKeepZeroLengthFiles_CheckedChanged(object sender, EventArgs e)
        {
            registryKey.SetValue("Keep Zero Length Files", checkBoxKeepZeroLengthFiles.Checked ? "Y" : "N");
        }

        private void getABlockDevieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // this is only enabled if using TCP/IP - COM port would be TOO slow.

            bool error = false;

            int response;
            byte[] ackBuffer = new byte[] { 0x06 };
            byte[] nakBuffer = new byte[] { 0x15 };

            frmGetBlockDevice dlg = new frmGetBlockDevice();
            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                bool proceed = false;
                if (radioButtonTCPIP.Checked)
                {
                    if (OpenTCPPort(true))
                        proceed = true;
                }

                if (proceed)
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(dlg.saveAs, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        // build the request buffer to send to the remote
                        byte[] commandBuffer = new byte[dlg.deviceName.Length + 2]; // one extra for the command and one extra for the null terminator

                        commandBuffer[0] = getBlockDeviceCommand;
                        for (int i = 0; i < dlg.deviceName.Length; i++)
                        {
                            commandBuffer[i + 1] = (byte)dlg.deviceName[i];
                            commandBuffer[i + 2] = 0x00;        // self terminating
                        }

                        // device transfer start: 2024/05/11 07:26:07.292
                        // device transfer end:   2024/05/11 07:27:51.306

                        tempbuffer.Add(string.Format("device transfer start: {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));
                        textBoxResponses.Lines = tempbuffer.ToArray();
                        //textBoxResponses.AppendText(string.Format("device transfer start: {0}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));                        
                        textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                        textBoxResponses.ScrollToCaret();

                        // now send the request buffer to the remote and wait for the remote to say it is OK to
                        // start requesting blocks keep doing this until remotes back a packet with size word = 0
                        try
                        {
                            byte[] responseBuffer = new byte[512];

                            // send the command with the device name - remote will respond with ACK when it is OK to
                            // start requesting blocks of data
                            Program.remoteAccess.socket.Send(commandBuffer, 0, commandBuffer.Length, SocketFlags.None);

                            // wait for remote to say it's OK tos start requesting blocks
                            int bytesReceived = Program.remoteAccess.socket.Receive(responseBuffer);     // get the ack to the command
                            Program.remoteAccess.socket.Send(ackBuffer, 0, 1, SocketFlags.None);         // request the first block */

                            Cursor = Cursors.WaitCursor;

                            // the remote is ready - keep requesting blocks of data until we get a blcok size of 0
                            for (; ; )
                            {
                                bytesReceived = Program.remoteAccess.socket.Receive(responseBuffer);
                                if (bytesReceived > 4)
                                {
                                    int size = responseBuffer[0] * 256 + responseBuffer[1];

                                    ushort ccitt = 0;
                                    ccitt = (ushort)(responseBuffer[size + 2] * 256);
                                    ccitt += (ushort)((byte)responseBuffer[size + 3]);

                                    ushort calculatedCCITT = CRCCCITT(responseBuffer, 2, size, 0xffff, 0);
                                    if (calculatedCCITT == ccitt)
                                    {
                                        // calculated and received CCITT values match - good block - save to file
                                        writer.Write(responseBuffer, 2, size);
                                        writer.Flush();

                                        Application.DoEvents();

                                        // send ACK for next packet
                                        Program.remoteAccess.socket.Send(ackBuffer, 0, 1, SocketFlags.None);    // request another block of file data
                                    }
                                    else
                                    {
                                        Program.remoteAccess.socket.Send(nakBuffer, 0, 1, SocketFlags.None);    // request another block of file data
                                    }
                                }
                                else if (bytesReceived == 2)
                                {
                                    // only 2 bytes means no more packets remaining - we are done
                                    int size = responseBuffer[0] * 256 + responseBuffer[1];
                                    if (size == 0)
                                        break;
                                    else
                                    {
                                        // we should necer get here
                                        MsgBox.Show("what are we doing here");
                                    }
                                }
                                else
                                {
                                    // we should necer get here either
                                    MsgBox.Show("what are we doing here");
                                }
                            }
                            Cursor = Cursors.Default;
                        }
                        catch (Exception ex)
                        {
                            Cursor = Cursors.Default;
                            MsgBox.Show(ex.Message);
                            error = true;
                        }

                        tempbuffer.Add(string.Format("device transfer end:   {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));
                        textBoxResponses.Lines = tempbuffer.ToArray();
                        //textBoxResponses.AppendText(string.Format("device transfer end:   {0}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));                        
                        textBoxResponses.SelectionStart = textBoxResponses.Text.Length;
                        textBoxResponses.ScrollToCaret();
                    }
                }
            }
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (radioButtonTCPIP.Checked)
                getABlockDevieToolStripMenuItem.Enabled = true;
            else
                getABlockDevieToolStripMenuItem.Enabled = false;
        }

        private void frmTransfer_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void frmTransfer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.remoteAccess != null)
                Program.remoteAccess = null;
        }

        private void forceRemoteExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTCPPort();
            Program.remoteAccess.SendByte(0x55);
        }

        private void buttonForceRemoteExit_Click(object sender, EventArgs e)
        {
            OpenTCPPort();
            Program.remoteAccess.SendByte(0x55);
        }

        private void checkBoxMinix_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMinix.Checked)
            {
                Program.isMinix = true;
                radioButtonCOMPort.Text = "COM Port - uses DUART";
                radioButtonTCPIP.Enabled = false;
                radioButtonCOMPort.Checked = true;
                radioButtonSend.Text = "Send To Minix";
                radioButtonReceive.Text = "Receive From Minix";
                labelUniFLEXFileName.Text = labelUniFLEXFileName.Text.Replace("UniFLEX", "Minix");
                checkBoxAllowDirectorySelection.Enabled = false;
                Program.isDirMask = 0x4000;

                buttonForceRemoteExit.Enabled = false;      // the minix transfer program does not support this yet

                checkBoxFixNewLines.Text = "Remove <CR> when Sending Text files";

                Properties.Settings.Default.isMinix = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Program.isMinix = false;
                radioButtonCOMPort.Text = "COM Port - uses CPU09SR4";
                radioButtonTCPIP.Enabled = true;
                radioButtonSend.Text = "Send To UniFLEX";
                radioButtonReceive.Text = "Receive From UniFLEX";
                labelUniFLEXFileName.Text = labelUniFLEXFileName.Text.Replace("Minix", "UniFLEX");
                checkBoxAllowDirectorySelection.Enabled = true;
                Program.isDirMask = 0x0900;

                buttonForceRemoteExit.Enabled = true;   // the the UniFlex tuff and transfer programs support this

                checkBoxFixNewLines.Text = "Replace <CR><LF> and <LF> with <CR> on Send";

                Properties.Settings.Default.isMinix = false;
                Properties.Settings.Default.Save();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDialogOptions dlg = new frmDialogOptions();
            dlg.ShowDialog();
        }

        private void editorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "EditorPath", "");
            useExternalEditor = Program.GetConfigurationAttribute("Global/TransferUniFLEX", "UseExternalEditor", "N") == "Y" ? true : false;

            frmDialogGetEditor pDlg = new frmDialogGetEditor();
            pDlg.editor = editor;
            pDlg.useExternalEditor = useExternalEditor;

            DialogResult dr = pDlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                editor = pDlg.editor;

                if (pDlg.useExternalEditor)
                    useExternalEditor = true;
                else
                    useExternalEditor = false;

                Program.SaveConfigurationAttribute("Global/TransferUniFLEX", "EditorPath", editor);
                Program.SaveConfigurationAttribute("Global/TransferUniFLEX", "UseExternalEditor", useExternalEditor ? "Y" : "N");
            }
        }
    }
}
