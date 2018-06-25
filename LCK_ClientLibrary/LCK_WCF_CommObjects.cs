using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using LCK_ClientLibrary.LCK_ServiceReference;

namespace LCK_ClientLibrary
{
    public class StoreWPF : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; NotifyPropertyChanged("Phone"); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; NotifyPropertyChanged("Address"); }
        }

        public StoreWPF()
        { }

        public StoreWPF(StoreInfo store)
        {
            this.Id = store.ID;
            this.Name = store.Name;
            this.Phone = store.Phone;
            this.Address = store.Address;
        }

        public StoreInfo ToStoreInfo()
        {
            StoreInfo store = new StoreInfo();
            store.ID = this.Id;
            store.Name = this.Name;
            store.Address = this.Address;
            store.Phone = this.Phone;
            return store;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DayInfoWPF : ObservableObject
    {
        #region Custom Event Args
        public class DayInfoEventArg : EventArgs
        {
            public SpecialOrderWPF SpecialOrder_WPF { get; set; }
            public BatchWPF Batch_WPF { get; set; }

            public DayInfoEventArg()
            { }

            public DayInfoEventArg(SpecialOrderWPF SO)
            {
                this.SpecialOrder_WPF = SO;
            }

            public DayInfoEventArg(BatchWPF BatchWPF)
            {
                this.Batch_WPF = BatchWPF;
            }
        }
        #endregion

        public delegate void SpecialOrderEventHandler(object sender, DayInfoEventArg e);
        public SpecialOrderEventHandler AddSpecialOrderEvent;
        public SpecialOrderEventHandler UpateSpecialOrderEvent;
        public SpecialOrderEventHandler DeleteSpecialOrderEvent;
        public SpecialOrderEventHandler RefreshSpecialOrderEvent;
        public SpecialOrderEventHandler AddDailyFlavorEvent;
        public SpecialOrderEventHandler RemoveDailyFlavorEvent;

        public SpecialOrderEventHandler Clicked;

        private string _dayNumber;
        public string DayNumber
        {
            get { return _dayNumber; }
            set { _dayNumber = value; NotifyPropertyChanged("DayNumber"); NotifyPropertyChanged("DisplayDate"); }
        }

        private List<BatchWPF> _dailyBatches = new List<BatchWPF>();
        public List<BatchWPF> DailyBatches
        {
            get { return _dailyBatches; }
            set { _dailyBatches = value; NotifyPropertyChanged("DailyBatches"); }
        }

        // For UI Binding
        public string DailyBatchCount
        {
            get
            {
                List<BatchWPF> tmp = new List<BatchWPF>();
                // add daily flavors to tmp List
                foreach (BatchWPF bat in DailyBatches)
                    if (!bat.Flavor.NotAflavor)
                        tmp.Add(bat);

                // add SO flavors to tmp List
                foreach (SpecialOrderWPF SO in Orders)
                {
                    // check if any of the SO batches are part of the dailybatches and don't increment if they are
                    foreach(SO_BatchWPF bat in SO.Batches)
                        if (!bat.Flavor.NotAflavor) // case NotAFlavor=False [is a flavor] then add to list for later eval
                            tmp.Add(new BatchWPF() { Flavor = bat.Flavor });
                }

                // get count of distinct flavors in tmp List
                int distinctCount = tmp.Select(x => x.Flavor.Name).Distinct().Count();

                return distinctCount.ToString();
            }
        }
        public string FullSizeCount
        {
            get 
            {
                int dailyCount = 0;
                List<BatchWPF> dailys = DailyBatches.FindAll(x => x.IsMini == false);
                foreach (BatchWPF bat in dailys)
                    dailyCount += bat.Quantity;
                //int dailyCount = DailyBatches.FindAll(x => x.IsMini == false).Count;
                int SOCount = 0;
                foreach (SpecialOrderWPF SO in Orders)
                {
                    // check if any of the SO batches are part of the dailybatches and don't increment if they are
                    foreach (SO_BatchWPF bat in SO.Batches)
                    {
                        //if ((!bat.IsMini) && (!bat.Flavor.NotAflavor))
                        // add everything that is not a mini (included Flavors that are flagged @notAFlavor)
                        if (!bat.IsMini)
                        {
                            SOCount += bat.Quantity;
                            //BatchWPF found = DailyBatches.Find(x => x.Flavor.ID == bat.Flavor.ID && x.IsMini == false);
                            //if (found == null)
                            //    SOCount += bat.Quantity;
                        }
                    }
                }                
                //SOCount += SO.Batches.FindAll(x => x.IsMini == false).Count;
                return (dailyCount + SOCount).ToString();
            }
        }
        public string MiniSizeCount
        {
            get
            {
                int dailyCount = 0;
                //List<BatchWPF> dailys = DailyBatches.FindAll(x => x.IsMini == true);
                List<BatchWPF> dailys = DailyBatches.FindAll(x => x.QuantityMini > 0);
                foreach (BatchWPF bat in dailys)
                    dailyCount += bat.QuantityMini;

                //int dailyCount = DailyBatches.FindAll(x => x.IsMini == true).Count;
                int SOCount = 0;
                foreach (SpecialOrderWPF SO in Orders)
                {
                    // check if any of the SO batches are part of the dailybatches and don't increment if they are
                    foreach (SO_BatchWPF bat in SO.Batches)
                    {
                        if(bat.IsMini)
                        { 
                            //BatchWPF found = DailyBatches.Find(x => x.Flavor.ID == bat.Flavor.ID && x.IsMini == true);
                            BatchWPF found = DailyBatches.Find(x => x.Flavor.ID == bat.Flavor.ID && x.QuantityMini > 0);
                            if (found == null)
                                SOCount += bat.QuantityMini;
                        }
                    }
                }
                //SOCount += SO.Batches.FindAll(x => x.IsMini == true).Count;

                return (dailyCount + SOCount).ToString();
            }
        }
        public string CakeCount
        {
            get
            {
                int count = 0;
                foreach (SpecialOrderWPF so in Orders)
                    count += so.Cakes.Count;
                return count.ToString();
            }
        }

        private BatchWPF _selectedDailyBatch = new BatchWPF();
        public BatchWPF SelectedDailyBatch
        {
            get { return _selectedDailyBatch; }
            set { _selectedDailyBatch = value; NotifyPropertyChanged("SelectedDailyBatch"); }
        }
        
        private List<SpecialOrderWPF> _orders = new List<SpecialOrderWPF>();
        public List<SpecialOrderWPF> Orders
        {
            get { return _orders; }
            set { _orders = value; NotifyPropertyChanged("Orders"); }
        }
        public string SpecialOrderCount
        {
            get { return Orders.Count.ToString(); }
        }
        public List<SpecialOrderWPF> OrdersVista
        {
            get 
            {
                return Orders.FindAll(x => x.Store.Id == 2);    
            }
        }
        public List<SpecialOrderWPF> OrdersCarlsbad
        {
            get
            {
                return Orders.FindAll(x => x.Store.Id == 1);
            }
        }

        private SpecialOrderWPF _selectedSO = new SpecialOrderWPF();
        public SpecialOrderWPF SelectedSO
        {
            get { return _selectedSO; }
            set { _selectedSO = value; NotifyPropertyChanged("SelectedSO"); }
        }

        private List<FlavorWPF> _flavorsList = Globals.AllFlavors;
        public List<FlavorWPF> FlavorsList
        {
            get { return _flavorsList; }
            set { _flavorsList = value; }
        }

        private FlavorWPF _selectedFlavor = new FlavorWPF();
        public FlavorWPF SelectedFlavor
        {
            get { return _selectedFlavor; }
            set { _selectedFlavor = value; NotifyPropertyChanged("SelectedFlavor"); }
        }

        public DayInfoWPF()
        {
            this.DayNumber = Globals.DateTime_to_DayNumber(DateTime.Now);
        }

        public DayInfoWPF(DayInfo dayInfo)
        {
            this.DayNumber = dayInfo.DayNumber;

            this.DailyBatches = new List<BatchWPF>();
            foreach (Batch bat in dayInfo.DailyBatches.ToList())
                this.DailyBatches.Add(new BatchWPF(bat));

            this.Orders = new List<SpecialOrderWPF>();
            foreach (SpecialOrder so in dayInfo.Orders.ToList())
                this.Orders.Add(new SpecialOrderWPF(so));
        }

        public string DisplayDate
        {
            get
            {
                string[] pieces = DayNumber.Split('_');
                int days = int.Parse(pieces[1]) - 1; // DateTime obj is zero based
                int year = int.Parse(pieces[0]);
                DateTime theDate = new DateTime(year, 1, 1).AddDays(days);
                string rtn = string.Format("{0:D}", theDate); // theDate.ToString("M/d/yyy");
                return rtn;
            }
        }

        public string DisplayDateShort
        {
            get
            {
                string[] pieces = DayNumber.Split('_');
                int days = int.Parse(pieces[1]) - 1; // DateTime obj is zero based
                int year = int.Parse(pieces[0]);
                DateTime theDate = new DateTime(year, 1, 1).AddDays(days);
                string rtn = string.Format("{0:ddd, MMM d, yyyy}", theDate); // theDate.ToString("M/d/yyy");
                return rtn;
            }
        }

        #region Command Declarations
        private ICommand _addDailyFlavor;
        public ICommand AddDailyFlavorCmd
        {
            get
            {
                if (_addDailyFlavor == null)
                {
                    _addDailyFlavor = new RelayCommand(AddDailyFlavor);
                }

                return _addDailyFlavor;
            }
        }

        private ICommand _removeDailyFlavor;
        public ICommand RemoveDailyFlavorCmd
        {
            get
            {
                if (_removeDailyFlavor == null)
                    _removeDailyFlavor = new RelayCommand(RemoveDailyFlavor);

                return _removeDailyFlavor;
            }
        }
        
        private ICommand _addSpecialOrder;
        public ICommand AddSpecialOrder
        {
            get
            {
                if (_addSpecialOrder == null)
                {
                    _addSpecialOrder = new RelayCommand(AddSO);
                }

                return _addSpecialOrder;
            }
        }

        private ICommand _updateSpecialOrder;
        public ICommand UpdateSpecialOrder
        {
            get
            {
                if (_updateSpecialOrder == null)
                {
                    _updateSpecialOrder = new RelayCommand(UpdateSO);
                }

                return _updateSpecialOrder;
            }
        }

        private ICommand _deleteSpecialOrder;
        public ICommand DeleteSpecialOrder
        {
            get
            {
                if (_deleteSpecialOrder == null)
                {
                    _deleteSpecialOrder = new RelayCommand(DeleteSO);
                }

                return _deleteSpecialOrder;
            }
        }
        #endregion

        private void AddDailyFlavor()
        {
            BatchWPF bat = new BatchWPF();
            bat.Flavor = SelectedFlavor;

            if (AddDailyFlavorEvent != null) { AddDailyFlavorEvent(this, new DayInfoEventArg(bat)); }
        }

        private void RemoveDailyFlavor()
        {
            if (SelectedDailyBatch.Requested)
                return; // can't remove flavor that is part of special order

            if (RemoveDailyFlavorEvent != null) { RemoveDailyFlavorEvent(this, new DayInfoEventArg(SelectedDailyBatch)); }
        }

        private void AddSO()
        {
            // show SO window
            SpecialOrderWPF SO_WPF = new SpecialOrderWPF();
            SO_WPF.Day_Number = this.DayNumber;
            //SO_WPF.Store = Globals.SelectedStore;

            if (AddSpecialOrderEvent != null) { AddSpecialOrderEvent(this, new DayInfoEventArg(SO_WPF)); }
        }

        private void UpdateSO()
        {
            if (SelectedSO.Day_Number == null)
                return;

            SelectedSO.IsEditable = true;

            if (UpateSpecialOrderEvent != null) { UpateSpecialOrderEvent(this, new DayInfoEventArg(SelectedSO)); }
        }

        private void DeleteSO()
        {
            if (SelectedSO.Day_Number == null)
                return;

            // confirmation handled by event handler
            if (DeleteSpecialOrderEvent != null) { DeleteSpecialOrderEvent(this, new DayInfoEventArg(SelectedSO)); }
        }
    }

    public class FlavorWPF : ObservableObject
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("ID"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }

        private bool _notAflavor;
        public bool NotAflavor
        {
            get { return _notAflavor; }
            set { _notAflavor = value; NotifyPropertyChanged("NotAFlavor"); }
        }

        private bool _cakeFlavor;
        public bool CakeFlavor
        {
            get { return _cakeFlavor; }
            set { _cakeFlavor = value; NotifyPropertyChanged("CakeFlavor"); }
        }

        private bool _invisible;
        public bool Invisible
        {
            get { return _invisible; }
            set { _invisible = value; NotifyPropertyChanged("Invisible"); }
        }

        public FlavorWPF()
        { }

        public FlavorWPF(Flavor flav)
        {
            this.ID = flav.ID;
            this.Name = flav.Name;
            this.Description = flav.Description;
            this.NotAflavor = flav.NotAFlavor;
            this.CakeFlavor = flav.CakeFlavor;
            this.Invisible = flav.Invisible;
        }

        public Flavor ToFlavor()
        {
            Flavor flav = new Flavor();
            flav.ID = this.ID;
            flav.Name = this.Name;
            flav.Description = this.Description;
            flav.NotAFlavor = this.NotAflavor;
            flav.CakeFlavor = this.CakeFlavor;
            flav.Invisible = this.Invisible;
            return flav;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class CakeWPF : ObservableObject
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("ID"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        private string _abvName;
        public string AbvName
        {
            get { return _abvName; }
            set { _abvName = value; NotifyPropertyChanged("AbvName"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }

        public CakeWPF()
        { }

        public CakeWPF(Cake flav)
        {
            this.ID = flav.ID;
            this.Name = flav.Name;
            this.AbvName = flav.AbvName;
            this.Description = flav.Description;
        }

        public Cake ToCake()
        {
            Cake itm = new Cake();
            itm.ID = this.ID;
            itm.Name = this.Name;
            itm.AbvName = this.AbvName;
            itm.Description = this.Description;
            return itm;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class BatchWPF : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }
        private string _day_number;
        public string Day_number
        {
            get { return _day_number; }
            set { _day_number = value; NotifyPropertyChanged("Day_number"); }
        }
        private FlavorWPF _flavor = new FlavorWPF(new Flavor());
        public FlavorWPF Flavor
        {
            get { return _flavor; }
            set { _flavor = value; NotifyPropertyChanged("Flavor"); }
        }
        private bool _requested;
        public bool Requested
        {
            get { return _requested; }
            set { _requested = value; NotifyPropertyChanged("Requested"); }
        }
        private StoreWPF _store = new StoreWPF(new StoreInfo());
        public StoreWPF Store
        {
            get { return _store; }
            set { _store = value; NotifyPropertyChanged("Store"); }
        }
        public string StoreNameUI
        {
            get
            {
                if (!Requested)
                    return "DF";
                
                if (BothStores)
                    return "Both";
                else
                    return Store.Name;
            }
        }
        private bool _bothStores = false;
        public bool BothStores 
        {
            get { return _bothStores; }
            set { _bothStores = value; NotifyPropertyChanged("BothStores"); }
        }
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyPropertyChanged("Quantity"); }
        }
        public string QuantityUI
        {
            get
            {
                if ((QuantityMini == 0) & (Quantity == 0))
                    return "DF";
                else if (!IsMini)
                    return Quantity.ToString();
                else
                    return "0";
            }
        }
        private bool _isMini;
        public bool IsMini
        {
            get { return _isMini; }
            set { _isMini = value; NotifyPropertyChanged("IsMini"); }
        }
        private int _quantityMini;
        public int QuantityMini
        {
            get { return _quantityMini; }
            set { _quantityMini = value; NotifyPropertyChanged("QuantityMini"); }
        }
        public string IsMiniUI
        {
            get
            {
                return IsMini ? "Yes" : "No";
            }
        }
        public int MiniQty
        {
            get
            {
                if (IsMini)
                    return Quantity;
                else
                    return 0;
            }
        }

        public BatchWPF()
        { }

        public BatchWPF(Batch batch)
        {
            this.Id = batch.ID;
            this.Day_number = batch.Day_Number;
            this.Flavor = new FlavorWPF(batch.Flavor);
            this.Requested = batch.Requested;
            this.Store = new StoreWPF(batch.Store);
            this.Quantity = batch.Quantity;
            this.QuantityMini = batch.QuantityMini;
        }

        public Batch ToBatch()
        {
            Batch bat = new Batch();
            bat.ID = this.Id;
            bat.Day_Number = this.Day_number;
            bat.Flavor = this.Flavor.ToFlavor();
            bat.Requested = this.Requested;
            bat.Store = this.Store.ToStoreInfo();
            bat.Quantity = this.Quantity;
            bat.QuantityMini = this.QuantityMini;
            return bat;
        }

        public override string ToString()
        {
            return Flavor.Name;
        }
    }

    public class SO_BatchWPF : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }
        private string _day_number;
        public string Day_number
        {
            get { return _day_number; }
            set { _day_number = value; NotifyPropertyChanged("Day_number"); }
        }
        private FlavorWPF _flavor = new FlavorWPF(new Flavor());
        public FlavorWPF Flavor
        {
            get { return _flavor; }
            set { _flavor = value; NotifyPropertyChanged("Flavor"); }
        }
        private bool _requested;
        public bool Requested
        {
            get { return _requested; }
            set { _requested = value; NotifyPropertyChanged("Requested"); }
        }
        private StoreWPF _store = new StoreWPF(new StoreInfo());
        public StoreWPF Store
        {
            get { return _store; }
            set { _store = value; NotifyPropertyChanged("Store"); }
        }
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyPropertyChanged("Quantity"); }
        }
        private int _so_id;
        public int So_id
        {
            get { return _so_id; }
            set { _so_id = value; NotifyPropertyChanged("So_id"); }
        }
        private bool _isMini;
        public bool IsMini
        {
            get { return _isMini; }
            set { _isMini = value; NotifyPropertyChanged("IsMini"); }
        }
        private int _quantityMini;
        public int QuantityMini
        {
            get { return _quantityMini; }
            set { _quantityMini = value; NotifyPropertyChanged("QuantityMini"); }
        }
        private string _specialInstructions;
        public string SpecialInstructions
        {
            get { return _specialInstructions; }
            set { _specialInstructions = value; NotifyPropertyChanged("SpecialInstructions"); }
        }

        public SO_BatchWPF()
        { }

        public SO_BatchWPF(SO_Batch batch)
        {
            this.Id = batch.ID;
            this.So_id = batch.SO_ID;
            this.Day_number = batch.Day_Number;
            this.Flavor = new FlavorWPF(batch.Flavor);
            this.Quantity = batch.Quantity;
            this.IsMini = batch.Is_Mini;
            this.QuantityMini = batch.QuantityMini;
            this.SpecialInstructions = batch.Special_Instructions;
        }

        public SO_Batch ToSO_Batch()
        {
            SO_Batch bat = new SO_Batch();
            bat.ID = this.Id;
            bat.SO_ID = this.So_id;
            bat.Day_Number = this.Day_number;
            bat.Flavor = this.Flavor.ToFlavor();
            bat.Quantity = this.Quantity;
            bat.Is_Mini = this.IsMini;
            bat.QuantityMini = this.QuantityMini;
            bat.Special_Instructions = this.SpecialInstructions;
            return bat;
        }
    }

    public class Cake_BatchWPF : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }
        private int _so_id;
        public int So_id
        {
            get { return _so_id; }
            set { _so_id = value; NotifyPropertyChanged("So_id"); }
        }
        private CakeWPF _cake = new CakeWPF(new Cake());
        public CakeWPF Cake
        {
            get { return _cake; }
            set { _cake = value; NotifyPropertyChanged("Cake"); }
        }
        private FlavorWPF _flavor = new FlavorWPF(new Flavor());
        public FlavorWPF Flavor
        {
            get { return _flavor; }
            set { _flavor = value; NotifyPropertyChanged("Flavor"); }
        }
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyPropertyChanged("Quantity"); }
        }

        public Cake_BatchWPF()
        { }

        public Cake_BatchWPF(Cake_Batch cake)
        {
            this.Id = cake.ID;
            this.So_id = cake.SO_ID;
            this.Cake = new CakeWPF(cake.Cake);
            this.Flavor = new FlavorWPF(cake.Flavor);
            this.Quantity = cake.Quantity;
        }

        public Cake_Batch ToCake_Batch()
        {
            Cake_Batch bat = new Cake_Batch();
            bat.ID = this.Id;
            bat.SO_ID = this.So_id;
            bat.Cake = this.Cake.ToCake();
            bat.Flavor = this.Flavor.ToFlavor();
            bat.Quantity = this.Quantity;
            return bat;
        }
    }

    public class SpecialOrderWPF : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }
        private string _day_Number;
        public string Day_Number
        {
            get { return _day_Number; }
            set { _day_Number = value; NotifyPropertyChanged("Day_Number"); }
        }
        private StoreWPF _store = new StoreWPF(new StoreInfo());
        public StoreWPF Store
        {
            get { return _store; }
            set { _store = value; NotifyPropertyChanged("Store"); }
        }
        private List<StoreWPF> _storesUI = Globals.AllStores;
        public List<StoreWPF> StoresUI
        {
            get { return _storesUI; }
            set { _storesUI = value; NotifyPropertyChanged("StoresUI"); }
        }        
        private bool _deliver;
        public bool Deliver
        {
            get { return _deliver; }
            set { _deliver = value; NotifyPropertyChanged("Deliver"); NotifyPropertyChanged("DeliverUI"); }
        }
        public string DeliverUI
        {
            get
            {
                if (Deliver & Setup)
                    return "Yes & Setup";
                else if (Deliver)
                    return "Yes";
                else
                    return "No";
            }
        }
        private bool _setup;
        public bool Setup
        {
            get { return _setup; }
            set { _setup = value; NotifyPropertyChanged("Setup"); }
        }
        private string _customer_Name;
        public string Customer_Name
        {
            get { return _customer_Name; }
            set { _customer_Name = value; NotifyPropertyChanged("Customer_Name"); }
        }
        private string _customer_Email;
        public string Customer_Email
        {
            get { return _customer_Email; }
            set { _customer_Email = value; NotifyPropertyChanged("Customer_Email"); }
        }
        private string _customer_Phone;
        public string Customer_Phone
        {
            get { return _customer_Phone; }
            set { _customer_Phone = value; NotifyPropertyChanged("Customer_Phone"); }
        }
        private string _customer_Address;
        public string Customer_Address
        {
            get { return _customer_Address; }
            set { _customer_Address = value; NotifyPropertyChanged("Customer_Address"); }
        }
        private string _scanLink;
        public string ScanLink
        {
            get { return _scanLink; }
            set { _scanLink = value; NotifyPropertyChanged("ScanLink"); }
        }
        private int _dueTime;
        public int DueTime
        {
            get { return _dueTime; }
            set { _dueTime = value; NotifyPropertyChanged("DueTime"); }
        }
        public string DueTimeUI
        {
            get
            {
                if (DueTime < 12)
                    return DueTime.ToString() + "am";
                else
                    return (DueTime - 12).ToString() + "pm";
            }
            set
            {
                int tmp = int.Parse(value.Replace("am", "").Replace("pm", ""));
                if (value.Contains("pm"))
                    tmp += 12;
                DueTime = tmp;
            }
        }
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged("Notes"); }
        }
        private string _specialInstructions;
        public string SpecialInstructions
        {
            get { return _specialInstructions; }
            set { _specialInstructions = value; NotifyPropertyChanged("SpecialInstructions"); }
        }
        private string _cuttingCakeNotes;
        public string CuttingCakeNotes
        {
            get { return _cuttingCakeNotes; }
            set { _cuttingCakeNotes = value; NotifyPropertyChanged("CuttingCakeNotes"); }
        }
        private string _displayNotes;
        public string DisplayNotes
        {
            get { return _displayNotes; }
            set { _displayNotes = value; NotifyPropertyChanged("DisplayNotes"); }
        }
        private List<SO_BatchWPF> _batches = new List<SO_BatchWPF>();
        public List<SO_BatchWPF> Batches
        {
            get { return _batches; }
            set { _batches = value; NotifyPropertyChanged("Batches"); }
        }
        private List<Cake_BatchWPF> _cakes = new List<Cake_BatchWPF>();
        public List<Cake_BatchWPF> Cakes
        {
            get { return _cakes; }
            set { _cakes = value; NotifyPropertyChanged("Cakes"); }
        }
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; NotifyPropertyChanged("LastModified"); }
        }

        public string AddressLink
        {
            get { return @"https://www.google.com/maps/place/" + Customer_Address; }
        }

        private bool _isEditable = true;
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; NotifyPropertyChanged("IsEditable"); }
        }

        public SpecialOrderWPF()
        { }

        public SpecialOrderWPF(SpecialOrder SO)
        {
            this.Id = SO.ID;
            this.Day_Number = SO.Day_Number;
            this.Store = new StoreWPF(SO.Store);
            this.Deliver = SO.Deliver;
            this.Setup = SO.Setup;
            this.Customer_Name = SO.Customer_Name;
            this.Customer_Email = SO.Customer_Email;
            this.Customer_Phone = SO.Customer_Phone;
            this.Customer_Address = SO.Customer_Address;
            this.ScanLink = SO.ScanLink;
            this.DueTime = SO.DueTime;
            this.Notes = SO.Notes;
            this.SpecialInstructions = SO.SpecialInstructions;
            this.CuttingCakeNotes = SO.CuttingCakeNotes;
            this.DisplayNotes = SO.DisplayNotes;
            
            this.Batches = new List<SO_BatchWPF>();
            foreach (SO_Batch bat in SO.Batches)
                this.Batches.Add(new SO_BatchWPF(bat));
            
            this.Cakes = new List<Cake_BatchWPF>();
            foreach(Cake_Batch cke in SO.Cakes)
                this.Cakes.Add(new Cake_BatchWPF(cke));
            
            this.LastModified = SO.LastModified;
        }

        public SpecialOrder ToSpecialOrder()
        {
            SpecialOrder so = new SpecialOrder();
            so.ID = this.Id;
            so.Day_Number = this.Day_Number;
            so.Store = this.Store.ToStoreInfo();
            so.Deliver = this.Deliver;
            so.Setup = this.Setup;
            so.Customer_Name = this.Customer_Name;
            so.Customer_Email = this.Customer_Email;
            so.Customer_Phone = this.Customer_Phone;
            so.Customer_Address = this.Customer_Address;
            so.ScanLink = this.ScanLink;
            so.DueTime = this.DueTime;
            so.Notes = this.Notes;
            so.SpecialInstructions = this.SpecialInstructions;
            so.CuttingCakeNotes = this.CuttingCakeNotes;
            so.DisplayNotes = this.DisplayNotes;
            
            // batches. first convert each to SO_Batch obj then convert that list to Array
            List<SO_Batch> soBatches = new List<SO_Batch>();
            foreach (SO_BatchWPF bat in this.Batches)
                soBatches.Add(bat.ToSO_Batch());
            so.Batches = soBatches.ToArray();

            List<Cake_Batch> soCakes = new List<Cake_Batch>();
            foreach (Cake_BatchWPF cke in this.Cakes)
                soCakes.Add(cke.ToCake_Batch());
            so.Cakes = soCakes.ToArray();
            
            so.LastModified = this.LastModified;
            return so;
        }

        public string PreviewInfo
        {
            get
            {
                return DueTimeUI + @" / " + Customer_Name + @" / " + _customer_Phone + @" / " + DeliverUI;
            }
        }

        public string DisplayDate
        {
            get
            {
                try
                {
                    string[] pieces = Day_Number.Split('_');
                    int days = int.Parse(pieces[1]) - 1; // DateTime obj is zero based
                    int year = int.Parse(pieces[0]);
                    DateTime theDate = new DateTime(year, 1, 1).AddDays(days);
                    string rtn = string.Format("{0:ddd, MMM d, yyyy}", theDate); // theDate.ToString("M/d/yyy");
                    return rtn;
                }
                catch(Exception ex)
                {
                    return "";
                }
            }
        }

        public int FlavorCount
        {
            get { return Batches.Count; }
        }

        public int FullSizeCount
        {
            get
            {
                int count = 0;
                foreach(SO_BatchWPF bat in Batches)
                {
                    if ((!bat.IsMini)) // && (!bat.Flavor.NotAflavor))
                        count += bat.Quantity;
                }
                return count;
            }
        }

        public int MiniSizeCount
        {
            get
            {
                int count = 0;
                foreach (SO_BatchWPF bat in Batches)
                {
                    if (bat.IsMini)
                        count += bat.QuantityMini;
                }
                return count;
            }
        }

        public int CakeCount
        {
            get
            {
                return Cakes.Count;
            }
        }

        private List<FlavorWPF> _test = Globals.AllFlavors;
        public List<FlavorWPF> Test
        {
            get { return _test; }
            set { _test = value; NotifyPropertyChanged("Test"); }
        }
    }

    public class NewOrderWPF : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }
        public int ID_UI { get; set; }
        private SpecialOrderWPF _so;
        public SpecialOrderWPF SO
        {
            get { return _so; }
            set { _so = value; NotifyPropertyChanged("SO"); }
        }
        private StoreWPF _storeOrigin;
        public StoreWPF StoreOrigin
        {
            get { return _storeOrigin; }
            set { _storeOrigin = value; NotifyPropertyChanged("StoreOrigin"); }
        }
        private DateTime _created;
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; NotifyPropertyChanged("Created"); }
        }
        private bool _fulfilled;
        public bool Fulfilled
        {
            get { return _fulfilled; }
            set { _fulfilled = value; NotifyPropertyChanged("Fulfilled"); }
        }
        private bool _editted = false;
        public bool Editted
        {
            get { return _editted; }
            set { _editted = value; NotifyPropertyChanged("Editted"); }
        }

        public EventHandler PrintSpecialOrder;
        public EventHandler RemoveNewOrder;

        public NewOrderWPF()
        { }

        public NewOrderWPF(NewOrder NO)
        {
            this.Id = NO.ID;
            this.SO = new SpecialOrderWPF(NO.SO);
            this.StoreOrigin = new StoreWPF(NO.Store_Origin);
            this.Created = NO.Created;
            this.Fulfilled = NO.FulFilled;
            this.Editted = NO.Editted;
        }

        public NewOrder ToNewOrder()
        {
            NewOrder no = new NewOrder();
            no.ID = this.Id;
            no.SO = this.SO.ToSpecialOrder();
            no.Store_Origin = this.StoreOrigin.ToStoreInfo();
            no.Created = this.Created;
            no.FulFilled = this.Fulfilled;
            no.Editted = this.Editted;

            return no;
        }

        public string OrderDateUI
        {
            get { return Globals.DayNumber_to_DateTime(SO.Day_Number).ToString("MM/dd/yyyy"); }
        }

        public string OrderCreatedUI
        {
            get { return Created.ToString("MM/dd/yy HH:mm"); }
        }

        private ICommand _printCmd;
        public ICommand PrintCmd
        {
            get
            {
                if (_printCmd == null)
                {
                    _printCmd = new RelayCommand(Print);
                }

                return _printCmd;
            }
        }

        private ICommand _removeCmd;
        public ICommand RemoveCmd
        {
            get
            {
                if (_removeCmd == null)
                {
                    _removeCmd = new RelayCommand(RemoveNO);
                }

                return _removeCmd;
            }
        }

        private void Print()
        {
            if (PrintSpecialOrder != null)
                PrintSpecialOrder(this, new EventArgs());
        }

        private void RemoveNO()
        {
            if (RemoveNewOrder != null)
                RemoveNewOrder(this, new EventArgs());
        }
    }

}
