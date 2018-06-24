using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

using System.ServiceModel;
using System.ServiceModel.Description;

using LCK_ServiceLibrary;

namespace LCK_WCFsysTrayHost
{
    static class Program
    {
        static ServiceHost Sh;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // check to see if app is already running
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("LCK_WCFsysTrayHost");
            int count = 0;
            foreach (System.Diagnostics.Process p in processes)
            {
                count++;
                if (count > 1)
                    return;    // found more than one so another must be running
            }

            // Show the system tray icon.					
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();

                // Make sure the application runs!
                Application.Run();
            }

            //Application.Run(new Form1());
        }


    }
}
