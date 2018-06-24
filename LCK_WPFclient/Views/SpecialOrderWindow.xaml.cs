using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using System.Windows.Forms;
using System.Drawing;
//using System.Windows.Navigation;


using LCK_ClientLibrary;

namespace LCK_WPFclient.Views
{
    public class ZeroToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = (int)value;

            if (count == 0)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for SpecialOrderWindow.xaml
    /// </summary>
    public partial class SpecialOrderWindow : Window
    {
        private SpecialOrderWPF localSO = new SpecialOrderWPF();
        private LCK_WCFcommunication localLckComm = new LCK_WCFcommunication();
        //private string localScanTempFolder = AppDomain.CurrentDomain.BaseDirectory + "temp";
        public bool Cancel = true;

        private List<SO_BatchWPF> _miniSize = new List<SO_BatchWPF>();
        private List<SO_BatchWPF> _fullSize = new List<SO_BatchWPF>();

        #region Constructors
        public SpecialOrderWindow()
        {
            InitializeComponent();

            SetDefaults();
        }

        public SpecialOrderWindow(SpecialOrderWPF SO_WPF)
        {
            InitializeComponent();

            this.localSO = SO_WPF;

            SetDefaults();
        }

        public SpecialOrderWindow(SpecialOrderWPF SO_WPF, LCK_WCFcommunication lckComm)
        {
            InitializeComponent();

            this.localSO = SO_WPF;
            this.localLckComm = lckComm;

            SetDefaults();
        }
        #endregion

        #region Form Event Handlers (Buttons, etc.)
        private void btnSOBatchAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SO_BatchWPF so_wpf = new SO_BatchWPF();
                //so_wpf.Flavor = (FlavorWPF)cboFlavors.SelectedItem;
                so_wpf.Flavor = (FlavorWPF)ddmFlavor.SelectedItem;
                so_wpf.Quantity = int.Parse(txtBatchAdd_Qty.Text);
                so_wpf.Day_number = localSO.Day_Number;
                so_wpf.Store = Globals.SelectedStore;
                so_wpf.IsMini = false;

                _fullSize.Add(so_wpf);
                RefreshBatches();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSOBatchMiniAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SO_BatchWPF so_wpf = new SO_BatchWPF();
                //so_wpf.Flavor = (FlavorWPF)cboFlavorsMini.SelectedItem;
                so_wpf.Flavor = (FlavorWPF)ddmFlavorMini.SelectedItem;
                //so_wpf.Quantity = int.Parse(txtBatchMiniAdd_Qty.Text);
                so_wpf.QuantityMini = int.Parse(txtBatchMiniAdd_Qty.Text); ;
                so_wpf.Day_number = localSO.Day_Number;
                so_wpf.Store = Globals.SelectedStore;
                so_wpf.IsMini = true;

                _miniSize.Add(so_wpf);
                RefreshBatches();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // combine _miniSize and _fullSize
            localSO.Batches = _fullSize.Concat(_miniSize).ToList<SO_BatchWPF>();
            // extract store, if not selected use this store
            if (cboStores.SelectedValue == null)
                localSO.Store = Globals.SelectedStore;
            else
                localSO.Store = Globals.AllStores.Find(x => x.Name == cboStores.SelectedValue.ToString());

            // parse Pickup/drop-off/setup
            if ((bool)rdoPickUp.IsChecked)
            {
                localSO.Deliver = false;
                localSO.Setup = false;
            }
            else if ((bool)rdoDropOff.IsChecked)
            {
                localSO.Deliver = true;
                localSO.Setup = false;
            }
            else if ((bool)rdoDeliverSetup.IsChecked)
            {
                localSO.Deliver = true;
                localSO.Setup = true;
            }

            Cancel = false;
            this.Close();
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SO_BatchWPF selected = (SO_BatchWPF)lvwSoBatchesFull.SelectedItem;
                _fullSize.Remove(selected);
                RefreshBatches();
            }
            catch (Exception ex)
            { }
        }

        private void mnuDeleteMini_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SO_BatchWPF selected = (SO_BatchWPF)lvwSoBatchesMini.SelectedItem;
                _miniSize.Remove(selected);
                RefreshBatches();
            }
            catch (Exception ex)
            { }
        }

        private void btnCakeAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cake_BatchWPF cakeWPF = new Cake_BatchWPF();
                cakeWPF.So_id = localSO.Id;
                cakeWPF.Cake = (CakeWPF)cboCakeTypes.SelectedItem;
                //cakeWPF.Flavor = (FlavorWPF)cboCakeFlavors.SelectedItem;
                cakeWPF.Flavor = (FlavorWPF)ddmCakeFlavors.SelectedItem;
                cakeWPF.Quantity = int.Parse(txtCakeAdd_Qty.Text);
                localSO.Cakes.Add(cakeWPF);
                RefreshCakes();
            }
            catch (Exception ex)
            { }
        }

        private void mnuDeleteCake_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cake_BatchWPF selected = (Cake_BatchWPF)lvwCakes.SelectedItem;
                localSO.Cakes.Remove(selected);
                RefreshCakes();
            }
            catch (Exception ex)
            { }
        }

        private void rdoOptions_Check(object sender, RoutedEventArgs e)
        {
            if (cboStores == null)
                return;
            //if ((bool)rdoPickUp.IsChecked)
            //    cboStores.IsEnabled = true;
            //else if ((bool)rdoDropOff.IsChecked)
            //    cboStores.IsEnabled = false;
            //else if ((bool)rdoDeliverSetup.IsChecked)
            //    cboStores.IsEnabled = false;
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "PDF Files (*.pdf)|*.pdf";
            dlg.InitialDirectory = ConfigSettings_Static.ScanOrderDefaultPath;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (localLckComm.UploadFile(dlg.FileName))
                {
                    UploadScanImage(dlg.FileName);
                }
            }
        }

        private void btnCreatePDF_Click(object sender, RoutedEventArgs e)
        {
            GeneratePDFWindow pdfWin = new GeneratePDFWindow();
            pdfWin.Topmost = true;
            pdfWin.ShowDialog();
            string destFilename = pdfWin.DestinationFilename;

            if (destFilename == "")
                return;

            UploadScanImage(destFilename);
        }

        private void mnuClickImage(object sender, RoutedEventArgs e)
        {
            Print();
        }

        #endregion
        
        private void SetDefaults()
        {
            this.DataContext = this.localSO;
            //cboFlavors.ItemsSource = Globals.AllFlavors;
            //cboFlavorsMini.ItemsSource = Globals.AllFlavors;
            //cboCakeFlavors.ItemsSource = Globals.AllCakeFlavors;
            cboCakeTypes.ItemsSource = Globals.AllCakes;
            cboStores.ItemsSource = Globals.AllStores;
            cboStores.SelectedItem = Globals.AllStores.Find(x => x.Name == this.localSO.Store.Name);

            // split SO_Batches into Full and Mini size
            foreach(SO_BatchWPF bat in localSO.Batches)
            {
                if (bat.IsMini)
                    _miniSize.Add(bat);
                else
                    _fullSize.Add(bat);
            }
            RefreshBatches();

            // load Pick-up options
            if (localSO.Deliver && localSO.Setup)
            {
                rdoDeliverSetup.IsChecked = true;
                //cboStores.IsEnabled = false;
            }
            else if (localSO.Deliver && !localSO.Setup)
            {
                rdoDropOff.IsChecked = true;
                //cboStores.IsEnabled = false;
            }
            else if (!localSO.Deliver && !localSO.Setup)
            {
                rdoPickUp.IsChecked = true;
                //cboStores.IsEnabled = true;
            }

            // populate Due Time combo
            cboDueTime.Items.Add("6am");
            cboDueTime.Items.Add("7am");
            cboDueTime.Items.Add("8am");
            cboDueTime.Items.Add("9am");
            cboDueTime.Items.Add("10am");
            cboDueTime.Items.Add("11am");
            cboDueTime.Items.Add("12pm");
            cboDueTime.Items.Add("1pm");
            cboDueTime.Items.Add("2pm");
            cboDueTime.Items.Add("3pm");
            cboDueTime.Items.Add("4pm");
            cboDueTime.Items.Add("5pm");
            cboDueTime.Items.Add("6pm");
            cboDueTime.Items.Add("7pm");
            cboDueTime.Items.Add("8pm");
            cboDueTime.Items.Add("9pm");

            // load scanned order preview
            DownloadScanImage(localSO.ScanLink);
        }

        private void RefreshBatches()
        {
            lvwSoBatchesFull.ItemsSource = null;
            lvwSoBatchesFull.ItemsSource = _fullSize;

            lvwSoBatchesMini.ItemsSource = null;
            lvwSoBatchesMini.ItemsSource = _miniSize;
        }

        private void RefreshCakes()
        {
            lvwCakes.ItemsSource = null;
            lvwCakes.ItemsSource = localSO.Cakes;
        }

        private string DownloadScanImage(string serverFilename, bool refreshPreview = true)
        {
            using (new WaitCursor())
            {
                try
                {
                    if ((localSO.ScanLink == null) || (localSO.ScanLink == ""))
                        return "";

                    string localFilepath = "";
                    // look in local scan temp folder first, then download from server if not found
                    List<string> files = System.IO.Directory.GetFiles(ConfigSettings_Static.ScanTempFolder).ToList();
                    string found = files.Find(x => System.IO.Path.GetFileName(x) == serverFilename);
                    if (found == null)
                    {
                        // download from server
                        if (localLckComm.DownloadFile(serverFilename, ConfigSettings_Static.ScanTempFolder))
                            localFilepath = ConfigSettings_Static.ScanTempFolder + @"\" + serverFilename;
                    }
                    else
                        localFilepath = found;

                    if (refreshPreview)
                        RefreshScanImage(localFilepath);

                    return localFilepath;
                }
                catch (Exception ex)
                {
                    Log("DownloadScanImage(string,bool) - Error msg:" + ex.Message);
                    return "";
                }
            }
            
        }

        private bool UploadScanImage(string filename)
        {
            using (new WaitCursor())
            {
                try
                {
                    localSO.ScanLink = System.IO.Path.GetFileName(filename);
                    RefreshScanImage(filename);
                    return false;
                }
                catch (Exception ex)
                {
                    Log("UploadScanImage(string) - Error msg:" + ex.Message);
                    return false;
                }
            }
        }

        private void RefreshScanImage(string filename)
        {
            try
            {
                if (filename == "")
                    return;

                webImage.Navigate(new Uri(filename));

                //imgScan.Source = null;
                //imgScan.Source = new BitmapImage(new Uri(filename));
            }
            catch(Exception ex)
            {
                Log("RefreshScanImage(string) - Error msg:" + ex.Message);
            }
        }

        private void Print()
        {
            PrintDialog dlg = new System.Windows.Controls.PrintDialog();
            if(dlg.ShowDialog() == true)
            {
                //dlg.PrintVisual(imgScan, "Scan Image");
            }
        }

        private void Address_doubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenGoogleMap();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            OpenGoogleMap();
        }

        private void OpenGoogleMap()
        {
            // shift focus so datacontext binding updates model
            txtPhone.Focus();
            txtAddress.Focus();

            if((localSO.Customer_Address == "") || (localSO.Customer_Address == null))
                return;

            System.Diagnostics.Process.Start(localSO.AddressLink);
        }

        /// <summary>
        /// LCK_DB logging method
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        private static void Log(string msg, Logger.LogTypes logType = Logger.LogTypes.Debug)
        {
            Logger.Log(msg, "LCK_WPFclient.Views.SpecialOrderWindow", logType);
        }

        private void CleanUpZombieFoxItReader()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("FoxitReader");

            foreach (System.Diagnostics.Process p in processes)
                p.Kill();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // release pdf file in web viewer
            webImage.Navigate("about:blank");
            System.Threading.Thread.Sleep(250);
            CleanUpZombieFoxItReader();
        }

    }

    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public WaitCursor()
        {
            _previousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
