using System;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Xml;

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

        public static string configFileName = "configuration.xml";
        static bool isDebugBuild = false;

        public static bool IsDebugBuild()
        {
#if DEBUG
            isDebugBuild = true;
            return true;
#else
            isDebugBuild = false;
            return false;
#endif
        }

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
            if (!File.Exists(configFileName))
            {
                string defaults = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <Global>
    <TransferUniFLEX EditorPath="""" UseExternalEditor=""N"" LogOS9FloppyWrites=""N"" os9FloppyWritesFile="""">
    </TransferUniFLEX>
  </Global>
</configuration>";

                // create a default config file if one does not already exist.

                using (StreamWriter cf = new StreamWriter(File.Open(configFileName, FileMode.Create, FileAccess.ReadWrite)))
                {
                    cf.WriteLine(defaults);
                }
            }

            version = Assembly.GetEntryAssembly().GetName().Version;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTransfer());
        }

        #region COnfiguration File Access Routines
        public static void SaveConfigurationAttribute(string xpath, string attribute, string value)
        {
            XmlReader reader = null;
            FileStream xmlDocStream = null;

            try
            {
                xmlDocStream = File.OpenRead(Program.configFileName);
                reader = XmlReader.Create(xmlDocStream);
            }
            catch
            {
                File.Create(Program.configFileName);
                try
                {
                    xmlDocStream = File.OpenRead(Program.configFileName);
                    reader = XmlReader.Create(xmlDocStream);
                }
                catch (Exception e)
                {
                    MsgBox.Show($"Unable to open the configuration file: {e.Message}");
                }
            }

            XmlDocument newDoc = null;

            if (reader != null)
            {
                XmlDocument doc = new XmlDocument();
                if (doc != null)
                {
                    doc.Load(reader);

                    Program.SaveConfigurationAttribute(doc, xpath, attribute, value);

                    newDoc = (XmlDocument)doc.Clone();
                }
                reader.Close();
                reader.Dispose();

                xmlDocStream.Close();

                newDoc.Save(Program.configFileName);
            }
        }

        public static void SaveConfigurationAttribute(XmlDocument doc, string xpath, string attribute, string value)
        {
            XmlNode configurationNode = doc.SelectSingleNode("/configuration");
            XmlNode node = configurationNode.SelectSingleNode(xpath);
            if (node != null)
            {
                XmlAttributeCollection coll = node.Attributes;
                if (coll != null)
                {
                    XmlNode valueNode = coll.GetNamedItem(attribute);

                    if (valueNode != null)
                    {
                        if (value != valueNode.Value)
                            valueNode.Value = value;
                    }
                    else
                    {
                        XmlAttribute attr = doc.CreateAttribute(attribute);
                        attr.Value = value;
                        node.Attributes.Append(attr);
                    }
                }
            }
            else
            {
                // need to add this xpath node to the keyboard map

                string[] uriParts = xpath.Split('/');
                string name = uriParts[uriParts.Length - 1];

                XmlNode finalNode = configurationNode;
                XmlNode previousNode = finalNode;
                for (int i = 0; i < uriParts.Length - 1; i++)
                {
                    finalNode = finalNode.SelectSingleNode(uriParts[i]);
                    if (finalNode == null)
                    {
                        XmlNode newNode = doc.CreateNode(XmlNodeType.Element, uriParts[i], "");
                        previousNode.AppendChild(newNode);

                        finalNode = previousNode.SelectSingleNode(uriParts[i]);
                    }
                    previousNode = finalNode;
                }

                if (finalNode != null)
                {
                    XmlNode newNode = doc.CreateNode(XmlNodeType.Element, name, "");
                    XmlAttribute attr = doc.CreateAttribute(attribute);
                    attr.Value = value;

                    newNode.Attributes.Append(attr);
                    finalNode.AppendChild(newNode);
                }
            }
        }

        public static string GetConfigurationAttribute(string xpath, string attribute, string defaultvalue)
        {
            string value = defaultvalue;

            try
            {
                FileStream xmlDocStream = File.OpenRead(configFileName);
                XmlReader reader = XmlReader.Create(xmlDocStream);

                if (reader != null)
                {
                    XmlDocument doc = new XmlDocument();
                    if (doc != null)
                    {
                        doc.Load(reader);

                        XmlNode configurationNode = doc.SelectSingleNode("/configuration");
                        XmlNode node = configurationNode.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            XmlAttributeCollection coll = node.Attributes;
                            if (coll != null)
                            {
                                XmlNode valueNode = coll.GetNamedItem(attribute);

                                if (valueNode != null)
                                    value = valueNode.Value;
                            }
                        }
                    }
                    reader.Close();
                }
                xmlDocStream.Close();
            }
            catch
            {

            }
            return value;
        }

        // Modified to allow numbers to be specified as eothe decimal or hex if preceeded with "0x" or "0X"
        public static int GetConfigurationAttribute(string xpath, string attribute, int defaultvalue)
        {
            int value = defaultvalue;

            FileStream xmlDocStream = File.OpenRead(configFileName);
            XmlReader reader = XmlReader.Create(xmlDocStream);

            if (reader != null)
            {
                XmlDocument doc = new XmlDocument();
                if (doc != null)
                {
                    doc.Load(reader);

                    XmlNode configurationNode = doc.SelectSingleNode("/configuration");
                    XmlNode node = configurationNode.SelectSingleNode(xpath);
                    if (node != null)
                    {
                        XmlAttributeCollection coll = node.Attributes;
                        if (coll != null)
                        {
                            XmlNode valueNode = coll.GetNamedItem(attribute);
                            if (valueNode != null)
                            {
                                string strvalue = valueNode.Value;
                                if (strvalue.StartsWith("0x") || strvalue.StartsWith("0X"))
                                {
                                    value = Convert.ToInt32(strvalue, 16);
                                }
                                else
                                    Int32.TryParse(strvalue, out value);
                            }
                        }
                    }
                }
                reader.Close();
            }
            xmlDocStream.Close();
            return value;
        }

        public static string GetConfigurationAttribute(string xpath, string attribute, string ordinal, string defaultvalue)
        {
            string value = defaultvalue;
            bool foundOrdinal = false;

            FileStream xmlDocStream = File.OpenRead(configFileName);
            XmlReader reader = XmlReader.Create(xmlDocStream);

            if (reader != null)
            {
                XmlDocument doc = new XmlDocument();
                if (doc != null)
                {
                    doc.Load(reader);

                    XmlNode configurationNode = doc.SelectSingleNode("/configuration");
                    XmlNode node = configurationNode.SelectSingleNode(xpath);
                    while (!foundOrdinal && node != null)
                    {
                        if (node != null)
                        {
                            XmlAttributeCollection coll = node.Attributes;
                            if (coll != null)
                            {
                                foreach (XmlAttribute a in coll)
                                {
                                    if (a.Name == "ID")
                                    {
                                        string index = a.Value;
                                        if (index == ordinal)
                                        {
                                            XmlNode valueNode = coll.GetNamedItem(attribute);

                                            if (valueNode != null)
                                            {
                                                value = valueNode.Value;
                                                foundOrdinal = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (!foundOrdinal)
                                node = node.NextSibling;
                        }
                    }
                }
                reader.Close();
            }
            xmlDocStream.Close();
            return value;
        }
        public static int GetConfigurationAttribute(string xpath, string attribute, string ordinal, int defaultvalue)
        {
            int value = defaultvalue;
            bool foundOrdinal = false;

            FileStream xmlDocStream = File.OpenRead(configFileName);
            XmlReader reader = XmlReader.Create(xmlDocStream);

            if (reader != null)
            {
                XmlDocument doc = new XmlDocument();
                if (doc != null)
                {
                    doc.Load(reader);

                    XmlNode configurationNode = doc.SelectSingleNode("/configuration");
                    XmlNode node = configurationNode.SelectSingleNode(xpath);
                    while (!foundOrdinal && node != null)
                    {
                        XmlAttributeCollection coll = node.Attributes;
                        if (coll != null)
                        {
                            foreach (XmlAttribute a in coll)
                            {
                                if (a.Name == "ID")
                                {
                                    string index = a.Value;
                                    if (index == ordinal)
                                    {
                                        XmlNode valueNode = coll.GetNamedItem(attribute);

                                        if (valueNode != null)
                                        {
                                            string strvalue = valueNode.Value;
                                            if (strvalue.StartsWith("0x") || strvalue.StartsWith("0X"))
                                            {
                                                value = Convert.ToInt32(strvalue, 16);
                                            }
                                            else
                                                Int32.TryParse(strvalue, out value);
                                            foundOrdinal = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (!foundOrdinal)
                            node = node.NextSibling;
                    }
                }
                reader.Close();
            }
            xmlDocStream.Close();
            return value;
        }
        public static int GetConfigurationAttributeHex(string xpath, string attribute, string ordinal, int defaultValue)
        {
            int value = defaultValue;

            try
            {
                string strValue = GetConfigurationAttribute(xpath, attribute, ordinal, defaultValue.ToString("X4"));
                value = Convert.ToUInt16(strValue, 16);
            }
            catch
            {
            }

            return value;
        }
        #endregion
    }

    public static class Constants
    {
        public const int PACKETSIZE = 256;
    }
}
