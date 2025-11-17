using System;
using System.Reflection;
using System.Windows.Forms;

using System.Net.Sockets;
using System.IO.Ports;

namespace TransferUniFLEX
{
    public enum SELECTED_TRANSPORT
    {
        RS232,
        TCPIP
    };

    static class Program
    {
        public static Version version = new Version();

        // this will be used for access to the remote from all forms that need it.
        // it is the one and only instamce of this object - use it wisely

        public static RemoteAccess remoteAccess = new RemoteAccess("", "");
        public static bool isMinix = false;
        public static ushort isDirMask = 0x0900;
        public static bool currentDirectionIsSending = false;
        public static int currentSelectedTransport;

        public static SerialPort OpenComPort(string comboBoxCOMPorts, string comboBoxBaudRate)
        {
            string portName = comboBoxCOMPorts;

            // if there is an open serial port - close it in case the selected port has changed
            if (Program.remoteAccess.serialPort != null && Program.remoteAccess.serialPort.IsOpen)
            {
                Program.remoteAccess.serialPort.Close();
            }

            // now open the one specified in the comboBoxCOMPorts.Text combo box
            try
            {
                Program.remoteAccess.serialPort = new SerialPort(portName);

                // Optional: Configure other SerialPort settings such as BaudRate, Parity, DataBits, StopBits, etc.

                string stringBaudRate = comboBoxBaudRate;
                int baudRate = 2400;

                bool success = Int32.TryParse(stringBaudRate, out baudRate);

                //serialPort.BaudRate = baudRate;
                Program.remoteAccess.serialPort.BaudRate = baudRate;
                Program.remoteAccess.serialPort.Parity = Parity.None;
                Program.remoteAccess.serialPort.DataBits = 8;
                Program.remoteAccess.serialPort.StopBits = StopBits.One;
                Program.remoteAccess.serialPort.ReadTimeout = 5000;      // set timeout to 5 seconds

                Program.remoteAccess.serialPort.Open();

                // MsgBox.Show($"COM Port {portName} opened successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                // MsgBox.Show($"Access to COM Port {portName} is unauthorized: {ex.Message}");
            }
            catch (Exception ex)
            {
                // MsgBox.Show($"An error occurred: {ex.Message}");
            }
            return Program.remoteAccess.serialPort;
        }

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
