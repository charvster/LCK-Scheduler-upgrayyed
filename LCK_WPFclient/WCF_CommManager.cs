using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Windows.Threading;
using System.Configuration;

using LCK_WPFclient.Views;
using LCK_ClientLibrary;

namespace LCK_WPFclient
{
    public class WCF_CommManager : ObservableObject
    {
        public List<StoreWPF> Stores
        {
            get { return Globals.AllStores; }
            set { Globals.AllStores = value; NotifyPropertyChanged("Stores"); }
        }

        private DayInfoWPF _selectedDay = new DayInfoWPF();
        public DayInfoWPF SelectedDay
        {
            get { return _selectedDay; }
            set { _selectedDay = value; EnableSelectedDayEventHandlers(); NotifyPropertyChanged("SelectedDay"); }
        }

        private const int DayInfoCount = 7;
        private ObservableCollection<DayInfoWPF> _dayInfoPreviews = new ObservableCollection<DayInfoWPF>();
        public ObservableCollection<DayInfoWPF> DayInfoPreviews
        {
            get { return _dayInfoPreviews; }
            set { _dayInfoPreviews = value; NotifyPropertyChanged("DayInfoPreviews"); }
        }

        public StoreWPF SelectedStore
        {
            get 
            {
                return Globals.SelectedStore; 
            }
            set 
            {
                Globals.SelectedStore = value; 
                NotifyPropertyChanged("SelectedStore"); 
                RefreshSelectedDay(); 
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; NotifyPropertyChanged("SelectedDate"); RefreshSelectedDay(); }
        }

        private LCK_WCFcommunication lck_Comm = new LCK_WCFcommunication();

        private Timer _timer = new Timer();
        public UInt32 TimerInterval
        {
            get { return ConfigSettings_Static.RefreshInterval; }
            set { ConfigSettings_Static.RefreshInterval = value; NotifyPropertyChanged("TimerInterval"); }
        }

        private ConfigSettings Settings = new ConfigSettings();
        private string settingsFilename = AppDomain.CurrentDomain.BaseDirectory + "config.xml";

        private NewOrdersManager _no_manager;
        public NewOrdersManager NO_Manager
        {
            get { return _no_manager; }
            set { _no_manager = value; NotifyPropertyChanged("NO_Manager"); }
        }

        private bool _newOrdersAvailable = false;
        public bool NewOrdersAvailable
        {
            get { return _newOrdersAvailable; }
            set { _newOrdersAvailable = value; NotifyPropertyChanged("NewOrdersAvailable"); }
        }
        private bool NO_RefreshEnabled = true;

        public bool HasAdminRights
        {
            get { return Settings.AdminRights;  }
        }

        private void EnableSelectedDayEventHandlers()
        {
            if (SelectedDay == null)
                return;

            SelectedDay.AddSpecialOrderEvent -= HandleAddSpecialOrder;
            SelectedDay.AddSpecialOrderEvent += HandleAddSpecialOrder;

            SelectedDay.UpateSpecialOrderEvent -= HandleUpdateSpecialOrder;
            SelectedDay.UpateSpecialOrderEvent += HandleUpdateSpecialOrder;

            SelectedDay.DeleteSpecialOrderEvent -= HandleDeleteSpecialOrder;
            SelectedDay.DeleteSpecialOrderEvent += HandleDeleteSpecialOrder;

            SelectedDay.RefreshSpecialOrderEvent -= HandleRefreshSpecialOrder;
            SelectedDay.RefreshSpecialOrderEvent += HandleRefreshSpecialOrder;

            SelectedDay.AddDailyFlavorEvent -= HandleAddDailyFlavor;
            SelectedDay.AddDailyFlavorEvent += HandleAddDailyFlavor;

            SelectedDay.RemoveDailyFlavorEvent -= HandleRemoveDailyFlavor;
            SelectedDay.RemoveDailyFlavorEvent += HandleRemoveDailyFlavor;

            for (int i = 0; i < DayInfoCount;i++ )
            {
                if (DayInfoPreviews[i] != null)
                {
                    DayInfoPreviews[i].Clicked -= HandleDayInfoClicked;
                    DayInfoPreviews[i].Clicked += HandleDayInfoClicked;
                }
            }

        }

        public WCF_CommManager()
        {
            if (!File.Exists(settingsFilename))
                SaveSettings(settingsFilename);  // not found so create
            else
                LoadSettings(settingsFilename);  // found so open and load
            
            // Check for WCF communication. if not present, notify user then close application
            if(!LckHostConnected())
            {
                string errorMsg = "Unable to communicate with LCK WCF server. Please verify host is running and DNS service is working.";
                MessageBox.Show(errorMsg + Environment.NewLine + "Exiting application", "Error - LCK Scheduler");
                if (MessageBox.Show(errorMsg + Environment.NewLine + "Exiting application", "Error - LCK Scheduler", MessageBoxButton.OK) == MessageBoxResult.OK)
                {  }

                Log(errorMsg);
                System.Environment.Exit(0);
            }

            // get info for default store
            SelectedStore = lck_Comm.GetStoreInfo(Settings.StoreID);

            // initialize DayInfoPreview array
            for (int i = 0; i < DayInfoCount; i++)
                DayInfoPreviews.Add(new DayInfoWPF());

            ReSyncGlobalVariables();
            NO_Manager = new NewOrdersManager(lck_Comm);
            NO_Manager.NewOrderAvailable += NewOrderAvailable_Event;

            RefreshNewOrders();

            SelectedDate = DateTime.Now;
            StartRefreshTimer();
        }

        private bool LoadSettings(string filename)
        {
            if (!File.Exists(filename)) return false;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ConfigSettings));
                TextReader reader = new StreamReader(filename);
                object obj = deserializer.Deserialize(reader);
                Settings = (ConfigSettings)obj;
                reader.Close();

                //SelectedStore = lck_Comm.GetStoreInfo(Settings.StoreID);

                Log("Config Load Successful. Filename: " + filename);

                return true;
            }
            catch(Exception ex)
            {
                Log("Config Load Unsuccessful. Filename: " + filename + " ErrorMsg=" + ex.Message);
                return false;
            }
        }

        private bool SaveSettings(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigSettings));
                using (TextWriter writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, Settings);
                }
                Log("Config Save Successful. Filename: " + filename);
                return true;
            }
            catch (Exception ex)
            {
                Log("Config Save Unsuccessful. Filename: " + filename + " ErrorMsg=" + ex.Message);
                return false;
            }
        }

        #region Custom Event Handlers
        private void NewOrderAvailable_Event(object sender, EventArgs e)
        {
            NewOrdersAvailable = NO_Manager.NewOrdersAvailable;
        }

        private void HandleAddSpecialOrder(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            SpecialOrderWPF SO_WPF = e.SpecialOrder_WPF;

            // show SO window
            SpecialOrderWindow So_Win = new SpecialOrderWindow(SO_WPF,lck_Comm);
            So_Win.ShowDialog();
            if (So_Win.Cancel == false)
            {
                // add to Special Order table
                lck_Comm.AddSpecialOrder(SO_WPF);

                // add entry to NewOrders table
                int lastSO_id = lck_Comm.GetLastSpecialOrderID();
                if (lastSO_id != 0) // check for bad return
                {
                    NewOrderWPF no_wpf = new NewOrderWPF();
                    SO_WPF.Id = lastSO_id;
                    no_wpf.SO = SO_WPF;
                    no_wpf.StoreOrigin = Globals.SelectedStore;
                    no_wpf.Created = DateTime.Now;
                    no_wpf.Fulfilled = false;
                    lck_Comm.AddNewOrder(no_wpf);
                }
                else
                    Log("Error Adding new entry to NewOrder. Check NewOrders table for 'so_id'=0");
            }
            
            RefreshSelectedDay();
        }

        private void HandleUpdateSpecialOrder(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            SpecialOrderWPF SO_WPF = e.SpecialOrder_WPF;

            SpecialOrderWindow SO_Win = new SpecialOrderWindow(SO_WPF,lck_Comm);
            SO_Win.ShowDialog();

            if (SO_Win.Cancel == false)
            {
                MessageBoxResult res = MessageBox.Show("Update selected Special Order?", "Update Special Order?", MessageBoxButton.YesNo);
                if (res != MessageBoxResult.Yes)
                    return;

                // update SO
                lck_Comm.UpdateSpecialOrder(SO_WPF.Id, SO_WPF);

                SO_WPF.SpecialInstructions = "--Editted--" + SO_WPF.SpecialInstructions;
                lck_Comm.UpdateSpecialOrder(SO_WPF.Id, SO_WPF);

                // add editted SO to NewOrder table
                NewOrderWPF no_wpf = new NewOrderWPF();
                no_wpf.Id = SO_WPF.Id;
                //SO_WPF.Id = lastSO_id;
                no_wpf.SO = SO_WPF;
                no_wpf.StoreOrigin = Globals.SelectedStore;
                no_wpf.Created = DateTime.Now;
                no_wpf.Fulfilled = false;
                no_wpf.Editted = true;
                lck_Comm.AddNewOrder(no_wpf);
            }
            
            RefreshSelectedDay();
        }

        private void HandleDeleteSpecialOrder(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            SpecialOrderWPF SO_WPF = e.SpecialOrder_WPF;

            // confirm with user then exe
            MessageBoxResult res = MessageBox.Show("Delete selected Special Order?", "Delete Special Order?",MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
                return;
            
            bool success = lck_Comm.DeleteSpecialOrder(SO_WPF);

            RefreshSelectedDay();
        }

        private void HandleRefreshSpecialOrder(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            RefreshSelectedDay();
        }

        private void HandleAddDailyFlavor(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            BatchWPF tmp = e.Batch_WPF;
            tmp.Store = Globals.SelectedStore;
            tmp.Day_number = Globals.DateTime_to_DayNumber(SelectedDate);

            bool success = lck_Comm.AddBatch(tmp);
            RefreshSelectedDay();
        }

        private void HandleRemoveDailyFlavor(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            // confirm with user then exe
            MessageBoxResult res = MessageBox.Show("Delete Flavor '" + e.Batch_WPF.Flavor.Name + "'?", "Delete Flavor?", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
                return;

            bool success = lck_Comm.DeleteBatch(e.Batch_WPF);

            RefreshSelectedDay();
        }

        private void HandleDayInfoClicked(object sender, DayInfoWPF.DayInfoEventArg e)
        {
            try
            {
                if (sender != null)
                {
                    DayInfoWPF tmp = (DayInfoWPF)sender;
                    SelectedDate = Globals.DayNumber_to_DateTime(tmp.DayNumber);
                    //SelectedDay = tmp;
                }
            }
            catch(Exception ex)
            { }
        }
        #endregion

        public bool LckHostConnected()
        {
            // Check for WCF communication. if not present, notify user then close application
            try
            {
                // Case: try saved endpoint
                List<StoreWPF> check = new List<StoreWPF>();
                string savedEndpoint = Settings.EndpointAddress;
                if ((savedEndpoint != "") && (savedEndpoint != null))
                {
                    lck_Comm = new LCK_WCFcommunication(savedEndpoint);
                    check = lck_Comm.GetAllStores();
                    if (check != null)
                        return true;
                }
                // try multiple endpoints until one works
                // Case: localHost [endpoint_localhost]
                lck_Comm = new LCK_WCFcommunication(@"endpoint_localhost");    
                check = lck_Comm.GetAllStores();
                if (check != null)
                {
                    Settings.EndpointAddress = @"endpoint_localhost";
                    SaveSettings(settingsFilename);                   
                    return true;
                }

                // Case: local IP
                lck_Comm = new LCK_WCFcommunication(@"endpoint_localIP");                
                check = lck_Comm.GetAllStores();
                if (check != null)
                {
                    Settings.EndpointAddress = @"endpoint_localIP";
                    SaveSettings(settingsFilename);  
                    return true;
                }

                // Case DDNS address
                lck_Comm = new LCK_WCFcommunication(@"endpoint_ddns");
                check = lck_Comm.GetAllStores();
                if (check != null)
                {
                    Settings.EndpointAddress = @"endpoint_ddns";
                    SaveSettings(settingsFilename);   
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public void ShowFlavorEditor()
        {
            try
            {
                FlavorEditorWindow win = new FlavorEditorWindow(lck_Comm);
                win.ShowDialog();
                ReSyncGlobalVariables();
                RefreshSelectedDay();
            }
            catch(Exception ex)
            {

            }
        }

        public void ShowCakeEditor()
        {
            try
            {
                CakeEditorWindow win = new CakeEditorWindow(lck_Comm);
                win.ShowDialog();
                ReSyncGlobalVariables();
                RefreshSelectedDay();
            }
            catch (Exception ex)
            {

            }
        }

        public void ShowNewOrders()
        {
            try
            {
                NO_RefreshEnabled = false;
                NewOrdersWindow noWin = new NewOrdersWindow(NO_Manager);
                noWin.ShowDialog();
                RefreshNewOrders();
                NO_RefreshEnabled = true;
            }
            catch(Exception ex)
            {

            }
        }

        private void ReSyncGlobalVariables()
        {
            //Globals.AllFlavors = lck_Comm.GetAllFlavors();
            Globals.AllFlavors = lck_Comm.GetVisibleFlavors();
            Globals.AllStores = lck_Comm.GetAllStores();
            Globals.AllCakes = lck_Comm.GetAllCakes();
            Globals.AllCakeFlavors = lck_Comm.GetAllCakeFlavors();
        }

        private void RefreshSelectedDay()
        {
            string dayNum = Globals.DateTime_to_DayNumber(SelectedDate);
            if (dayNum == "1_1")
                return;

            // check that store is selected
            //if (SelectedStore.Id == 0)
            //    return;
            
            //SelectedDay = lck_Comm.GetDayInfo(SelectedStore.Id, dayNum,true);
            SelectedDay = lck_Comm.GetDayInfo(dayNum, true);

            RefreshDayPreviews();
        }

        private void RefreshDayPreviews()
        {
            // figure out 'dayNum' day of week then populate the day previews
            int dayOffset = 0;
            switch(SelectedDate.DayOfWeek.ToString())
            {
                case "Monday":
                    dayOffset = 0;
                    break;
                case "Tuesday":
                    dayOffset = -1;
                    break;
                case "Wednesday":
                    dayOffset = -2;
                    break;
                case "Thursday":
                    dayOffset = -3;
                    break;
                case "Friday":
                    dayOffset = -4;
                    break;
                case "Saturday":
                    dayOffset = -5;
                    break;
                case "Sunday":
                    dayOffset = -6;
                    break;
            }

            for (int i = 0; i < DayInfoCount; i++)
            {
                //DayInfoPreviews[i] = lck_Comm.GetDayInfo(SelectedStore.Id, Globals.DateTime_to_DayNumber(SelectedDate.AddDays(i + dayOffset)));
                DayInfoPreviews[i] = lck_Comm.GetDayInfo(Globals.DateTime_to_DayNumber(SelectedDate.AddDays(i + dayOffset)));

                DayInfoPreviews[i].Clicked -= HandleDayInfoClicked;
                DayInfoPreviews[i].Clicked += HandleDayInfoClicked;
            }
        }

        private void RefreshNewOrders()
        {
            if (!NO_RefreshEnabled)
                return; 

            // force add on main thread
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                NO_Manager.RefreshNewOrders();
            }));
        }

        private void StartRefreshTimer()
        {
            _timer.Interval = TimerInterval;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true; // Enable it
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            RefreshSelectedDay();
            RefreshNewOrders();
            _timer.Start();
        }

        /// <summary>
        /// LCK_DB logging method
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        private static void Log(string msg, Logger.LogTypes logType = Logger.LogTypes.Debug)
        {
            Logger.Log(msg, "LCK_WPFclient.WCF_CommManager", logType);
        }
    }
}
