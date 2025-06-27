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
    class RemoteAccess
    {
        public SerialPort serialPort = null;
        public Socket socket = null;

        public byte acceptDirectoryNameToBrowse = 0x03;             // tells the remote to accept a directory name to browse

        public Dictionary<string, FileInformation> sortedInformations = new Dictionary<string, FileInformation>();

        public string ipAddress = "";
        public string port = "";

        public DateTime UNIXtoDateTime(long seconds)
        {
            double secs = Convert.ToDouble(seconds);
            DateTime dt = new DateTime(1980, 1, 1, 0, 0, 0).AddSeconds(secs);

            return System.TimeZone.CurrentTimeZone.ToLocalTime(dt);
        }

        public string ConvertDateTime(int fileTime)
        {
            DateTime t = UNIXtoDateTime((long)fileTime);

            int year = t.Year;
            int month = t.Month;
            int day = t.Day;
            int hour = t.Hour;
            int minute = t.Minute;
            int second = t.Second;

            //if (year < 100)
            //    year += 1900;
            //else 
            //{
            //    year -= 100;
            //    year += 2000;
            //}

            string strDateTime = string.Format("{0}/{1}/{2} {3}:{4}:{5}", month.ToString("00"), day.ToString("00"), year.ToString("0000"), hour.ToString("00"), minute.ToString("00"), second.ToString("00"));
            return strDateTime;
        }

        public string ConvertDateTime(byte[] value)
        {
            int fileTime = (value[0] * 16777216) + (value[1] * 65536) + (value[2] * 256) + (value[3]);
            return ConvertDateTime(fileTime);
        }

        public RemoteAccess()
        {
        }

        public int SendByte(byte b)
        {
            int response = -1;
            byte[] byteToSend = new byte[1];
            byteToSend[0] = b;

            try
            {
                if (serialPort != null)
                {
                    serialPort.Write(byteToSend, 0, 1);
                    response = serialPort.ReadByte();
                }
                else
                {
                    byte[] commandBuffer = new byte[2];
                    commandBuffer[0] = b;
                    commandBuffer[1] = 0x00;
                    try
                    {

                        if (socket == null)
                            socket = Program.OpenSocket(ipAddress, port);

                        if (!socket.Connected)
                        {
                            try
                            {
                                socket.Close();
                            }
                            catch
                            {
                            }
                            socket = Program.OpenSocket(ipAddress, port);
                        }

                        socket.Send(commandBuffer, 0, commandBuffer.Length, SocketFlags.None);

                        byte[] responseBuffer = new byte[512];
                        int bytesReceived = socket.Receive(responseBuffer);

                        // the remote now knows which directory to get the list from - so see if
                        // the remote sent us an ACK saying it is ready to start receiving requests
                        // for the next entry in the list.

                        if (responseBuffer[0] != 0x06)
                        {
                            // we got an error from the remote
                        }
                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show($"Read operation timed out waiting for ack: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read operation failed waiting for ack: {ex.Message}");
            }

            return response;
        }

        public int SendBytes(byte[] b, int startIndex, int length, bool sendNull)
        {
            int response = -1;

            int i;

            for (i = startIndex; i < length; i++)
            {
                response = SendByte(b[i]);

                if (response == -1)
                    break;
            }
            if (sendNull)
            {
                response = SendByte(0x00);
            }

            return response;
        }

        // Only call this with recursive true when being called from code that is NOT loading a listview. This would
        // exclude the remote directory browse for. It would however include the calls to get a list of files and
        // directories in a directory when the list of files to receive include directories.

        public void GetRemoteDirectory(string directoryNameToBrowse, bool recursive)
        {
            //Cursor = Cursors.WaitCursor;

            byte command = acceptDirectoryNameToBrowse;
            byte[] ackByte = new byte[1];
            ackByte[0] = 0x06;

            // allFileInfos is only used initailly to get the list - it is NEVER used outside this function
            // once we have the list sorted, we ONLY use that dictionary to work with

            Dictionary<string, FileInformation> allFileInfos = new Dictionary<string, FileInformation>();  // this is the total list

            if (recursive)
                command |= 0x40; // make it recursive - DO NOT use values above 0x7F

            // Start by sending a command to the UniFLEX box to send the directory listing of the 
            // currently selected directory on the UniFLEX box specified int the UniFLEX filename
            // if the UniFLEX filename text box is blank. If it is not empty, get the directory
            // listing of the directory specified in the text box.

            if (serialPort != null)
            {
                int response = SendByte(command);
                if (response == 0x06)
                {
                    // we got an ACK - send the directory name to browse

                    byte[] nameBytes = ASCIIEncoding.ASCII.GetBytes(directoryNameToBrowse);
                    response = SendBytes(nameBytes, 0, nameBytes.Length, true);
                    if (response == 0x06)
                    {
                        // the remote as acknowledged the requset for a list of available files
                        // loop on receiving bytes building filename until 0x00 received. The first byte will be the mode
                        while (true)
                        {
                            FileInformation fileInfo = new FileInformation();

                            byte[] filename = new byte[15];     // when you declare a byte array, all elements are set to 0 by default.
                            int fnIndex = 0;

                            // first get the statBuffer - it will be 24 bytes
                            serialPort.Write(ackByte, 0, 1);    // request a statBuffer and filename
                            for (int i = 0; i < 24; i++)
                            {
                                response = serialPort.ReadByte();
                                fileInfo.statBytes[i] = (byte)response;
                            }
                            fileInfo.fillStat();

                            // now get the filename - up to 15 bytes - null terminated
                            do
                            {
                                response = serialPort.ReadByte();   // 0x00 will signal end of filename
                                filename[fnIndex++] = (byte)response;
                            } while (response != 0x00);

                            // receiving an empty filename signals end of filename transmissions
                            if (filename[0] == 0x00)
                                break;
                            else
                            {
                                fileInfo.filename = Encoding.ASCII.GetString(filename).TrimEnd('\0');
                                allFileInfos.Add(Encoding.ASCII.GetString(filename).TrimEnd('\0'), fileInfo);
                            }
                        }
                    }

                    // this is where we will sort the allFileInfos

                    sortedInformations.Clear();                                 // make sure it is empty.
                    var sortedKeys = allFileInfos.Keys.OrderBy(k => k);     // build the Sorting keys

                    // Iterate through the sorted keys to build the sorted file informations that we will use.
                    //
                    // Let's put the directories first and then the files. This saves us having to do this when
                    // we load the list control.

                    foreach (var key in sortedKeys)
                    {
                        if ((allFileInfos[key].stat.st_mode & 0x0900) == 0x0900)
                            sortedInformations.Add(key, allFileInfos[key]);
                    }

                    // now do the files

                    foreach (var key in sortedKeys)
                    {
                        if ((allFileInfos[key].stat.st_mode & 0x0900) != 0x0900)
                            sortedInformations.Add(key, allFileInfos[key]);
                    }
                }
            }
            else
            {
                // use TCPIP - start with building the command buffer to send that will request
                // a directory listing from the remote. First tell the remote what directory
                // we wish to get a list files along with their stats from. If the passed in
                // parameter 'directoryNameToBrowse' is empty, then we want the current working
                // directory of the remote. Otherwiae the 'directoryNameToBrowse' parameter will
                // either be a relative path to the remote's cuurent working directory or it
                // will be an absolute path from the root (starts with /).

                int bufferOffset = 0;
                byte[] nameBytes = ASCIIEncoding.ASCII.GetBytes(directoryNameToBrowse);
                byte[] commandBuffer = new byte[nameBytes.Length + 2];
                commandBuffer[bufferOffset] = command;

                for (int i = 0; i < nameBytes.Length; i++)
                {
                    commandBuffer[i + 1] = nameBytes[i];
                }
                commandBuffer[commandBuffer.Length - 1] = 0x00;
                try
                {

                    if (socket == null)
                        socket = Program.OpenSocket(ipAddress, port);

                    if (!socket.Connected)
                    {
                        try
                        {
                            socket.Close();
                        }
                        catch
                        {
                        }
                        socket = Program.OpenSocket(ipAddress, port);
                    }

                    socket.Send(commandBuffer, 0, commandBuffer.Length, SocketFlags.None);

                    byte[] responseBuffer = new byte[512];
                    int bytesReceived = socket.Receive(responseBuffer);

                    // the remote now knows which directory to get the list from - so see if
                    // the remote sent us an ACK saying it is ready to start receiving requests
                    // for the next entry in the list.

                    if (responseBuffer[0] == 0x06)
                    {
                        // start asking for filenames by sending an ACK character
                        // loop on receiving bytes building filename until 0x00 received. The first byte
                        // will be the mode. When the remote has no more stat/filename entries to send, 
                        // it will send a block of 25 bytes, the first 24 being a zeroed ot stat structure
                        // and the last byte being a 0x00 indicating we are done.

                        while (true)
                        {
                            FileInformation fileInfo = new FileInformation();

                            byte[] filename = new byte[15];

                            // first get the statBuffer - it will be 24 bytes
                            socket.Send(ackByte, 0, 1, SocketFlags.None);    // request a statBuffer and filename
                            bytesReceived = socket.Receive(responseBuffer);
                            for (int i = 0; i < 24; i++)
                            {
                                fileInfo.statBytes[i] = responseBuffer[i];
                            }
                            fileInfo.fillStat();

                            // now get the filename - up to 15 bytes - null terminated (filenames are 14 bytes max plus null terminator
                            for (int i = 0; i < 16; i++)
                            {
                                filename[i] = responseBuffer[i + 24];   // skip past stat bytes already processed
                                if (filename[i] == 0x00)
                                    break;
                            }

                            // receiving an empty filename signals end of filename transmissions
                            if (filename[0] == 0x00)
                                break;
                            else
                            {
                                // we got a filename - add it to the Dictionary, but first get an ASCII representation
                                // of the filename.
                                fileInfo.filename = Encoding.ASCII.GetString(filename).TrimEnd('\0');

                                // if we are being recursive, do not add the . and .. directories to the list
                                if (!recursive || (fileInfo.filename != "." && fileInfo.filename != ".."))
                                    allFileInfos.Add(Encoding.ASCII.GetString(filename).TrimEnd('\0'), fileInfo);
                            }
                        }

                        // now that we have the complete list - this is where we will sort the allFileInfos
                        sortedInformations.Clear();                                 // make sure it is empty.
                        var sortedKeys = allFileInfos.Keys.OrderBy(k => k);     // build the Sorting keys

                        // Iterate through the sorted keys to build the sorted file informations that we will use.
                        //
                        // Let's put the directories first and then the files. This saves us having to do this when
                        // we load the list control.

                        foreach (var key in sortedKeys)
                        {
                            if ((allFileInfos[key].stat.st_mode & 0x0900) == 0x0900)
                                sortedInformations.Add(key, allFileInfos[key]);
                        }

                        // now do the files

                        foreach (var key in sortedKeys)
                        {
                            if ((allFileInfos[key].stat.st_mode & 0x0900) != 0x0900)
                                sortedInformations.Add(key, allFileInfos[key]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            //Cursor = Cursors.Default;
        }
    }

    // these cannot be the first classes in the file. The designer barfs if they are - so leave them here.

    public class STAT
    {
        public short  st_dev;    // 0x00 - device number             
        public short  st_ino;    // 0x02 - fdn number                
        public short  st_mode;   // 0x04 - file mode and permissions 
        public byte   st_nlink;  // 0x06 - file link count           
        public int    st_uid;    // 0x07 - file owner's user id      
        public int    st_size;   // 0x09 - file size in bytes        
        public int    st_mtime;  // 0x0D - last modified time        
        public int    st_spr;    // 0x11 - spare - future use only   
    }

    public class FileInformation
    {
        public byte[] statBytes = new byte[24];
        public string filename;

        public STAT stat = new STAT();
        
        public bool isDirectory
        {
            get {return (stat.st_mode & 0x0900) == 0x0900 ? true : false;}
        }

        public void fillStat()
        {
            stat.st_dev   = (short)(statBytes[ 0] * 256 + statBytes[ 1]);
            stat.st_ino   = (short)(statBytes[ 2] * 256 + statBytes[ 3]);
            stat.st_mode  = (short)(statBytes[ 4] * 256 + statBytes[ 5]);
            stat.st_nlink = statBytes[6];
            stat.st_uid   = statBytes[ 7] * 256 + statBytes[ 8];
            stat.st_size  = statBytes[ 9] * 256 * 256 * 256 + statBytes[10] * 256 * 256 + statBytes[11] * 256 + statBytes[12];
            stat.st_mtime = statBytes[13] * 256 * 256 * 256 + statBytes[14] * 256 * 256 + statBytes[15] * 256 + statBytes[16];
            stat.st_spr   = statBytes[17] * 256 * 256 * 256 + statBytes[18] * 256 * 256 + statBytes[19] * 256 + statBytes[20];
        }
    }
}
