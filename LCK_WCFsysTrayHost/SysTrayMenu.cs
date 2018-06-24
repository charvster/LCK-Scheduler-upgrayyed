using System;
using System.Diagnostics;
using System.Windows.Forms;
using LCK_WCFsysTrayHost.Properties;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;

using LCK_ServiceLibrary;

namespace LCK_WCFsysTrayHost
{
	/// <summary>
	/// 
	/// </summary>
	class SysTrayMenu
	{

		/// <summary>
		/// Is the About box displayed?
		/// </summary>
        bool isSettingsLoaded = false;

        private bool Enabled = true;
        private List<string> _logEntries = new List<string>();
        private bool DebugMode = false;

        private ContextMenuStrip menu = new ContextMenuStrip();
        private ToolStripMenuItem statusItem;

        private static ServiceHost Sh = null;

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ContextMenuStrip</returns>
		public ContextMenuStrip Create()
		{
            // Init Service first so values are updated for BuildMenu items
            InitServiceHost();
            
            BuildMenu();

			return menu;
		}

        private void BuildMenu()
        {
            // Add the default menu options.
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            try
            {
                // remove all items before adding more
                menu.Items.Clear();

                // server version number
                item = new ToolStripMenuItem();
                Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                item.Text = "LCK WCF Service v" + ver.Major.ToString() + "." + ver.Minor.ToString(); 
                menu.Items.Add(item);

                // Separator.
                sep = new ToolStripSeparator();
                menu.Items.Add(sep);

                // Local IP
                item = new ToolStripMenuItem();
                item.Text = "Local IP: " + Globals.LocalIP + @":" + Globals.LocalPort;
                menu.Items.Add(item);

                // Endpoint
                item = new ToolStripMenuItem();
                item.Text = "Endpoint addr: " + Globals.LocalIP + @":" + Globals.LocalPort + @"/lck"; // + Globals.httpBaseAddress.ToString();
                menu.Items.Add(item);

                // Mex endpoint
                item = new ToolStripMenuItem();
                item.Text = "MEX addr: " + Globals.LocalIP + @":" + Globals.LocalPort + @"/lck/mex"; // +Globals.mexAddr;
                menu.Items.Add(item);

                // Separator.
                sep = new ToolStripSeparator();
                menu.Items.Add(sep);

                // Exit.
                item = new ToolStripMenuItem();
                item.Text = "Exit";
                item.Click += new System.EventHandler(Exit_Click);
                //item.Image = Resources.Exit;
                menu.Items.Add(item);

                //// Status
                //statusItem = new ToolStripMenuItem();
                //if (Enabled)
                //    statusItem.Text = "Disable";
                //else
                //    statusItem.Text = "Enable";
                //statusItem.Click += Enable_Click;
                //menu.Items.Add(statusItem);

                //// Settings Item
                //item = new ToolStripMenuItem();
                //item.Text = "Settings";
                //item.Click += new EventHandler(Settings_Click);
                //item.Image = Resources.settings;
                //menu.Items.Add(item);

                //if (DebugMode)
                //{
                //    // Show log
                //    item = new ToolStripMenuItem();
                //    item.Text = "Show Log";
                //    item.Click += new EventHandler(ShowLog_Click);
                //    item.Image = Resources.About;
                //    menu.Items.Add(item);
                //}

                //// Separator.
                //sep = new ToolStripSeparator();
                //menu.Items.Add(sep);

                //// Exit.
                //item = new ToolStripMenuItem();
                //item.Text = "Exit";
                //item.Click += new System.EventHandler(Exit_Click);
                //item.Image = Resources.Exit;
                //menu.Items.Add(item);
                //// *** End Menu Build ***
            }
            catch(Exception ex)
            {
                Log("BuildMenu() failed.");
            }

            
        }

        static void InitServiceHost()
        {
            Uri httpBaseAddress = new Uri("http://localhost:6790/lck");
            string mexAddr = "http://localhost:6790/lck/mex";

            // declare for both
            Sh = new ServiceHost(typeof(LCK_Service), httpBaseAddress);

            // create Binding - settings much match binding settings on client
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = 2147483647;
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxBufferSize = 2147483647;
            //binding.ReceiveTimeout = new TimeSpan(0, 0, 0, 30, 0);            
            // create binding.ReaderQuotas
            binding.ReaderQuotas.MaxDepth = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;

            ServiceEndpoint httpSe = Sh.AddServiceEndpoint(typeof(ILCK_Service), binding, httpBaseAddress);
            //ServiceEndpoint httpSe = Sh.AddServiceEndpoint(typeof(ILCK_Service), new WSHttpBinding(), httpBaseAddress);

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = false;
            Sh.Description.Behaviors.Add(smb);

            ServiceEndpoint httpSeMex = Sh.AddServiceEndpoint(typeof(IMetadataExchange),
                                                                MetadataExchangeBindings.CreateMexHttpBinding(),
                                                                mexAddr);
            Sh.Open();

            Globals.LocalIP = GetLocalIPAddress();

        }

        public static string GetLocalIPAddress()
        {
            try
            {
                string localIP;
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }

                return localIP;
            }
            catch (Exception ex)
            {
                return "0.0.0.0";
            }
        }

        #region Menu Event Handlers

        private void ShowLog_Click(object sender, EventArgs e)
        {
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string tmp = ver.Major + "." + ver.Minor + "." + ver.Build;
            string display = "Log Entries [count=" + _logEntries.Count + "]:" + Environment.NewLine;
            display += String.Join(Environment.NewLine,_logEntries);
            MessageBox.Show(display, "v" + tmp + " Log Entries");
        }

		/// <summary>
		/// Processes a menu item.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Exit_Click(object sender, EventArgs e)
		{
            Sh.Close();

			// Quit without further ado.
			Application.Exit();
		}

        #endregion

        private void Log(string msg)
        {
            if (!DebugMode)
                return;

            msg = DateTime.Now.ToString("[MM/dd/yyyy hh:mm:ss]") + msg;
            _logEntries.Add(msg);
        }
	}
}