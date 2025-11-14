using System;
using System.Reflection;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace TransferUniFLEX
{
    static class Program
    {
        public static Version version = new Version();

        // this will be used for access to the remote from all forms that need it.
        // it is the one and only instamce of this object - use it wisely

        public static RemoteAccess remoteAccess = new RemoteAccess();
        public static bool isMinix = false;
        public static ushort isDirMask = 0x0900;

        static public Socket OpenSocket(string _ipAddress, string _port)
        {
            Socket socket = null;
            int port = 1410;

            bool success = Int32.TryParse(_port, out port);
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(_ipAddress, port);
            }
            catch (Exception e)
            {
                MsgBox.Show(string.Format("Unable to connect to UniFLEX machine via TCP/IP\r\n{0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            return socket;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            version = Assembly.GetEntryAssembly().GetName().Version;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTransfer());
        }
    }

    public static class Constants
    {
        public const int PACKETSIZE = 256;
    }
}
