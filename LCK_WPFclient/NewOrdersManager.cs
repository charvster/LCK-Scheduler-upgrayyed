using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing.Printing;
using System.Diagnostics;

using LCK_ClientLibrary;

namespace LCK_WPFclient
{
    public class NewOrdersManager : ObservableObject
    {
        private NewOrderWPF _selectedNO = new NewOrderWPF();
        public NewOrderWPF SelectedNO
        {
            get { return _selectedNO; }
            set 
            { 
                _selectedNO = value; 
                NotifyPropertyChanged("SelectedNO"); 
            }
        }   
        private ObservableCollection<NewOrderWPF> _newOrders = new ObservableCollection<NewOrderWPF>();
        public ObservableCollection<NewOrderWPF> NewOrders
        {
            get { return _newOrders; }
            set { _newOrders = value; NotifyPropertyChanged("NewOrders"); }
        }

        private LCK_WCFcommunication local_lckComm;
        private string printFilepath = "";

        public EventHandler NewOrderAvailable;

        public bool NewOrdersAvailable
        {
            get
            {
                if (NewOrders.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public NewOrdersManager(LCK_WCFcommunication lck_comm)
        {
            local_lckComm = lck_comm;
        }

        #region Old Print method - retired v1.5
        private void PrintPage(object o, PrintPageEventArgs e)
        {
            //string printFilepath = @"J:\ELF_CERTIFICATE_v2.jpg";
            System.Drawing.Image i = System.Drawing.Image.FromFile(printFilepath);
            System.Drawing.Rectangle m = e.MarginBounds;

            //if ((double)i.Width / (double)i.Height > (double)m.Width / (double)m.Height) // image is wider
            //{
            //    m.Height = (int)((double)i.Height / (double)i.Width * (double)m.Width);
            //}
            //else
            //{
            //    m.Width = (int)((double)i.Width / (double)i.Height * (double)m.Height);
            //}
            //e.Graphics.DrawImage(i, m);

            e.Graphics.DrawImage(i, e.PageBounds);
        }
        #endregion

        private void PrintSO_Event(object sender, EventArgs e)
        {
            NewOrderWPF no_WPF = (NewOrderWPF)sender;

            try
            {
                // get image to be printed
                string serverFilename = no_WPF.SO.ScanLink;
                // look in local scan temp folder first, then download from server if not found
                List<string> files = System.IO.Directory.GetFiles(ConfigSettings_Static.ScanTempFolder).ToList();
                string found = files.Find(x => System.IO.Path.GetFileName(x) == serverFilename);
                if (found == null)
                {
                    // download from server
                    if (local_lckComm.DownloadFile(serverFilename, ConfigSettings_Static.ScanTempFolder))
                        printFilepath = ConfigSettings_Static.ScanTempFolder + @"\" + serverFilename;
                }
                else
                    printFilepath = found;

                if (!System.IO.File.Exists(printFilepath))
                {
                    string msg = "Unable to find Scanned Order";
                    System.Windows.Forms.MessageBox.Show(msg, "Error");
                    Log("PrintSO_Event(object,EventArgs) Error msg=" + msg, Logger.LogTypes.Error);
                    return;
                }
            }
            catch(Exception ex)
            {
                Log("PrintSO_Event(object,EventArgs):retrieve image section - Error msg=" + ex.Message, Logger.LogTypes.Error);
                return;
            }

            try
            {
                SendToPrinter(printFilepath);

                // Worked then began giving Out of Memory Error
                #region Old Print method - retired v1.5
                //PrintDocument pd = new PrintDocument();
                //pd.DefaultPageSettings.PrinterSettings.PrinterName = "Printer Name";
                //pd.DefaultPageSettings.Landscape = true; //or false!
                //pd.PrintPage += PrintPage;

                //System.Windows.Forms.PrintDialog dlg = new System.Windows.Forms.PrintDialog();
                //dlg.Document = pd;
                //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    //pd.Print();
                //    SendToPrinter(printFilepath);
                //    RemoveNewOrder(no_WPF);
                //}
                #endregion
            }
            catch(Exception ex)
            {
                Log("PrintSO_Event(object,EventArgs):print section - Error msg=" + ex.Message, Logger.LogTypes.Error);
                return;

            }
        }

        private void SendToPrinter(string filename)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = filename;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (false == p.CloseMainWindow())
                p.Kill();
        }

        private void RemoveNewOrder(object sender, EventArgs e)
        {
            NewOrderWPF no_WPF = (NewOrderWPF)sender;
            RemoveNewOrder(no_WPF);
        }

        private void RemoveNewOrder(NewOrderWPF NO_WPF)
        {
            if (System.Windows.Forms.MessageBox.Show("Remove entry with ID=" + NO_WPF.ID_UI + " from Queue?", "Error", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                bool rtn = local_lckComm.DeteleNewOrder(NO_WPF);
                RefreshNewOrders();
            }
        }

        public void RefreshNewOrders()
        {
            NewOrders.Clear(); // remove all previous
            List<NewOrderWPF> NOs = local_lckComm.GetNewOrders();
            if (NOs == null)
                return;

            int idx = 1;  
            foreach (NewOrderWPF no in NOs)
            {
                // only add entries added from the other location
                if (no.StoreOrigin.Id != Globals.SelectedStore.Id)
                {
                    no.PrintSpecialOrder += PrintSO_Event;
                    no.RemoveNewOrder += RemoveNewOrder;
                    no.ID_UI = idx;
                    NewOrders.Add(no);
                    idx++;
                }
            }

            // notify WCF_CommManager that new orders has changed
           NewOrder_Event();
        }

        private void NewOrder_Event()
        {
            if (NewOrderAvailable != null)
                NewOrderAvailable(this, null);
        }

        /// <summary>
        /// LCK_DB logging method
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        private static void Log(string msg, Logger.LogTypes logType = Logger.LogTypes.Debug)
        {
            Logger.Log(msg, "LCK_WPFclient.NewOrdersManager", logType);
        }

    }
}
