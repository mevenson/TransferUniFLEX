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
    // this class gets instantiated by Program. All access to it's properties and method must be called through:
    //
    //      Program.serialPort.<method or propertiy>
    //

    class RemoteAccess
    {
        // these must be set whenever the selection in comboBoxCOMPorts or comboBoxBaudRate changes.
        public string comboBoxCOMPorts, comboBoxBaudRate;

        public SerialPort serialPort = null;                        // RemoteAccess owns the serial port.
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

            int year   = t.Year;
            int month  = t.Month;
            int day    = t.Day;
            int hour   = t.Hour;
            int minute = t.Minute;
            int second = t.Second;

            if (Program.isMinix)
                year -= 10;

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

        public RemoteAccess(string _comboBoxCOMPorts, string _comboBoxBaudRate)
        {
            comboBoxCOMPorts = _comboBoxCOMPorts;
            comboBoxBaudRate = _comboBoxBaudRate;
        }

        public int SendByte(byte b)
        {
            int response = -1;
            byte[] byteToSend = new byte[1];
            byteToSend[0] = b;

            try
            {
                if (Program.currentSelectedTransport == (int)SELECTED_TRANSPORT.RS232)
                {
                    if (serialPort == null || !serialPort.IsOpen)
                    {
                        Program.OpenComPort(comboBoxCOMPorts, comboBoxBaudRate);
                    }

                    if (serialPort != null && serialPort.IsOpen)
                    {
                        serialPort.Write(byteToSend, 0, 1);
                        response = serialPort.ReadByte();
                    }
                    else
                    {
                        MsgBox.Show("Com port is not open", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
                MsgBox.Show($"Read operation timed out waiting for ack: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            catch (Exception ex)
            {
                MsgBox.Show($"Read operation failed waiting for ack: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
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

            if (Program.currentSelectedTransport == (int)SELECTED_TRANSPORT.RS232)
            {
                if (serialPort == null || !serialPort.IsOpen)
                {
                    Program.OpenComPort(comboBoxCOMPorts, comboBoxBaudRate);
                }

                if (serialPort != null && serialPort.IsOpen)
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
                                if (Program.isMinix)
                                {
                                    int sizeOfStatBuffer = 30;
                                    FileInformation fileInfo = new FileInformation();

                                    byte[] filename = new byte[15];     // when you declare a byte array, all elements are set to 0 by default.
                                    int fnIndex = 0;

                                    // first get the statBuffer - it will be 24 bytes
                                    serialPort.Write(ackByte, 0, 1);    // request a statBuffer and filename

                                    for (int i = 0; i < sizeOfStatBuffer; i++)
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
                                else
                                {
                                    int sizeOfStatBuffer = 24;
                                    FileInformation fileInfo = new FileInformation();

                                    byte[] filename = new byte[17];     // when you declare a byte array, all elements are set to 0 by default.
                                    int fnIndex = 0;

                                    // first get the statBuffer - it will be 24 bytes
                                    serialPort.Write(ackByte, 0, 1);    // request a statBuffer and filename

                                    for (int i = 0; i < sizeOfStatBuffer; i++)
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
                        }

                        // this is where we will sort the allFileInfos

                        if (Program.isMinix)
                        {
                            sortedInformations.Clear();                             // make sure it is empty.
                            var sortedKeys = allFileInfos.Keys.OrderBy(k => k);     // build the Sorting keys

                            // Iterate through the sorted keys to build the sorted file informations that we will use.
                            //
                            // Let's put the directories first and then the files. This saves us having to do this when
                            // we load the list control.

                            foreach (var key in sortedKeys)
                            {
                                if ((allFileInfos[key].stat.st_mode & Program.isDirMask) == Program.isDirMask)
                                    sortedInformations.Add(key, allFileInfos[key]);
                            }

                            // now do the files

                            foreach (var key in sortedKeys)
                            {
                                if ((allFileInfos[key].stat.st_mode & Program.isDirMask) != Program.isDirMask)
                                    sortedInformations.Add(key, allFileInfos[key]);
                            }
                        }
                    }
                    else
                    {
                        sortedInformations.Clear();                                 // make sure it is empty.
                        var sortedKeys = allFileInfos.Keys.OrderBy(k => k);     // build the Sorting keys

                        // Iterate through the sorted keys to build the sorted file informations that we will use.
                        //
                        // Let's put the directories first and then the files. This saves us having to do this when
                        // we load the list control.

                        foreach (var key in sortedKeys)
                        {
                            if ((allFileInfos[key].stat.st_mode & Program.isDirMask) == Program.isDirMask)
                                sortedInformations.Add(key, allFileInfos[key]);
                        }
                            // now do the files

                        foreach (var key in sortedKeys)
                        {
                            if ((allFileInfos[key].stat.st_mode & Program.isDirMask) != Program.isDirMask)
                                sortedInformations.Add(key, allFileInfos[key]);
                        }
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
                            if ((allFileInfos[key].stat.st_mode & Program.isDirMask) == Program.isDirMask)
                                sortedInformations.Add(key, allFileInfos[key]);
                        }

                        // now do the files

                        foreach (var key in sortedKeys)
                        {
                            if ((allFileInfos[key].stat.st_mode & Program.isDirMask) != Program.isDirMask)
                                sortedInformations.Add(key, allFileInfos[key]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }

            //Cursor = Cursors.Default;
        }
    }

    // Side note: The maximum file length in both UniFLEX and minix is 14 characters since they both use the 
    // original “V7 layout” for directory entries (2 + 14 bytes).
    //
    //      minix:
    //
    //          struct direct
    //          {
    //              uint16_t d_ino;         // 2 bytes
    //              char d_name[14];        // 14 bytes, NOT null-terminated if full
    //          };
    //
    //      UniFLEX:
    //
    //         struct direct
    //         {
    //             unsigned short d_ino;    /* inode number (0 = empty entry) */
    //             char d_name[14];         /* filename, not null-terminated if full */
    //         };
    //
    // these cannot be the first classes in the file. The designer barfs if they are - so leave them here.
    //      
    //      UniFLEX (21 bytes)
    //      struct stat // structure returned by stat (size = 21)
    //      {
    //          int             st_dev     ;    // 00 0x00 - device number (first byte = major, second byte = minor)
    //          int             st_ino     ;    // 02 0x02 - fdn number
    //          unsigned        st_mode    ;    // 04 0x04 - file mode and permissions
    //          char            st_nlink   ;    // 06 0x06 - file link count
    //          int             st_uid     ;    // 07 0x07 - file owner's user id
    //          long            st_size    ;    // 09 0x09 - file size in bytes
    //          long            st_mtime   ;    // 13 0x0D - last modified time
    //          long            st_spr     ;    // 17 0x11 - spare - future use only
    //                                          // 21 0x15 - next byte after
    //      };
    //          
    //      #define S_IFMT      0xff00	  /* type of file */
    //      #define S_IFDIR     0x0900	  /* directory */
    //      #define S_IFCHR     0x0500	  /* character special */
    //      #define S_IFBLK     0x0300	  /* block special */
    //      #define S_IFREG     0x0100	  /* regular */
    //      #define S_ISUID     0x40	  /* set user id on execution */
    //      #define S_IREAD     0x01	  /* read permission, owner */
    //      #define S_IWRITE    0x02	  /* write permission, owner */
    //      #define S_IEXEC     0x04	  /* execute/search permission, owner */
    //      #define S_IOREAD    0x08	  /* others read */
    //      #define S_IOWRITE   0x10	  /* others write */
    //      #define S_IOEXEC    0x20	  /* others execute */
    //      #define S_IPRM      0xff	  /* mask for permission bits */
    //      /*#define S_IFMPC 0030000*/   /* multiplexed char special */
    //      /*#define S_IFMPB 0070000*/   /* multiplexed block special */
    //      /*#define S_ISGID 0002000*/   /* set group id on execution */
    //      /*#define S_ISVTX 0001000*/   /* save swapped text even after use */

    //      minix (30 bytes)
    //      struct stat {
    //          short int       st_dev     ;    // 00 0x00 - device number (first byte = major, second byte = minor)
    //          unsigned short  st_ino     ;    // 02 0x02
    //          unsigned short  st_mode    ;    // 04 0x04 - file mode and permissions
    //          short int       st_nlink   ;    // 06 0x06 - file link count
    //          short int       st_uid     ;    // 08 0x08 - file owner's user id
    //          short int       st_gid     ;    // 10 0x0A - 2 bytes of gid
    //          short int       st_rdev    ;    // 12 0x0C - 2 bytes of ???
    //          long            st_size    ;    // 14 0x0E - 4 bytes of size info
    //          long            st_atime   ;    // 18 0x12 - 4 bytes of last accessed time
    //          long            st_mtime   ;    // 22 0x16 - 4 bytes of last modified time
    //          long            st_ctime   ;    // 26 0x1A - 4 bytes of created time
    //                                          // 30 0x1E - next byte after
    //      };

    //      /* Some common definitions. (in octal) */
    //      #define S_IFMT  0170000		/* type of file */
    //      #define S_IFDIR 0040000  	/* directory */
    //      #define S_IFCHR 0020000		/* character special */
    //      #define S_IFBLK 0060000		/* block special */
    //      #define S_IFREG 0100000		/* regular */
    //      #define S_ISUID   04000		/* set user id on execution */
    //      #define S_ISGID   02000		/* set group id on execution */
    //      #define S_ISVTX   01000		/* save swapped text even after use */
    //      #define S_IREAD   00400		/* read permission, owner */
    //      #define S_IWRITE  00200		/* write permission, owner */
    //      #define S_IEXEC   00100		/* execute/search permission, owner */
    //      

    public class FILE_STAT
    {
        // common between       Minix (30 bytes)                 and                               UniFLEX (21 bytes)
        public short st_dev;    // 00 0x00 - device number (first byte = major, second byte = min  // 00 0x00 - device number             
        public short st_ino;    // 02 0x02                                                         // 02 0x02 - fdn number                
        public short st_mode;   // 04 0x04 - file mode and permissions                             // 04 0x04 - file mode and permissions 
        public short st_nlink;  // 06 0x06 - file link count (2 bytes in Minix)                    // 06 0x06 - file link count           (only 1 byte in UniFLEX)
        public short st_uid;    // 08 0x08 - file owner's user id                                  // 07 0x07 - file owner's user id      

        // Minix only
        public short st_gid;    // 10 0x0A - 2 bytes of gid                                        
        public short st_rdev;   // 12 0x0C - 2 bytes of ???                                        

        // common between UniFLEX and Minix
        public int   st_size;   // 14 0x0E - 4 bytes of size info                                  // 08 0x08 - file size in bytes        

        // Minix only
        public int   st_atime;  // 18 0x12 - 4 bytes of last accessed time

        // common between UniFLEX and Minix
        public int   st_mtime;  // 22 0x16 - 4 bytes of last modified time                        // 12 0x0C - last modified time        

        // Minix only
        public int   st_ctime;  // 26 0x1A - 4 bytes of created time

        // UniFLEX only
        public int   st_spr;                                                                      // 16 0x11 - spare - future use only   
                            
                                // 30 0x1E - next byte after                                      // 20 0x14 - next byte after

    }

    public class FileInformation
    {
        public byte[] statBytes = new byte[30];
        public string filename;

        public FILE_STAT stat = new FILE_STAT();
        
        public bool isDirectory
        {
            get {return (stat.st_mode & Program.isDirMask) == Program.isDirMask ? true : false;}
        }

        public void fillStat()
        {
            if (Program.isMinix)        // 30 bytes (all thirty are defines)
            {
                stat.st_dev     = (short)(statBytes[ 0] * 256 + statBytes[ 1]);
                stat.st_ino     = (short)(statBytes[ 2] * 256 + statBytes[ 3]);
                stat.st_mode    = (short)(statBytes[ 4] * 256 + statBytes[ 5]);
                stat.st_nlink   = (short)(statBytes[ 6] * 256 + statBytes[ 7]);     // <- two bytes in minix
                stat.st_uid     = (short)(statBytes[ 8] * 256 + statBytes[ 9]);
                stat.st_gid     = (short)(statBytes[10] * 256 + statBytes[11]);
                stat.st_rdev    = (short)(statBytes[12] * 256 + statBytes[13]);
                stat.st_size    = statBytes[14] * 256 * 256 * 256 + statBytes[15] * 256 * 256 + statBytes[16] * 256 + statBytes[17];
                stat.st_atime   = statBytes[18] * 256 * 256 * 256 + statBytes[19] * 256 * 256 + statBytes[20] * 256 + statBytes[21];
                stat.st_mtime   = statBytes[22] * 256 * 256 * 256 + statBytes[23] * 256 * 256 + statBytes[24] * 256 + statBytes[25];
                stat.st_ctime   = statBytes[26] * 256 * 256 * 256 + statBytes[27] * 256 * 256 + statBytes[28] * 256 + statBytes[29];
            }
            else                        // UniFLEX tuff and transfer send 24 bytes of stat info. (only twenty one are defined).
            { 
                stat.st_dev   = (short)(statBytes[ 0] * 256 + statBytes[ 1]);
                stat.st_ino   = (short)(statBytes[ 2] * 256 + statBytes[ 3]);
                stat.st_mode  = (short)(statBytes[ 4] * 256 + statBytes[ 5]);
                stat.st_nlink = statBytes[ 6];                                      // <- only one byte in UniFLEX
                stat.st_uid   = (short)(statBytes[ 7] * 256 + statBytes[8]);
                stat.st_size  = statBytes[ 9] * 256 * 256 * 256 + statBytes[10] * 256 * 256 + statBytes[11] * 256 + statBytes[12];
                stat.st_mtime = statBytes[13] * 256 * 256 * 256 + statBytes[14] * 256 * 256 + statBytes[15] * 256 + statBytes[16];
                stat.st_spr   = statBytes[17] * 256 * 256 * 256 + statBytes[18] * 256 * 256 + statBytes[19] * 256 + statBytes[20];
            }
        }
    }
}
