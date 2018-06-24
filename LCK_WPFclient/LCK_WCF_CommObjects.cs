using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using LCK_WPFclient.LCK_ServiceReference;
using LCK_WPFclient.Views;

namespace LCK_WPFclient
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

            public DayInfoEventArg(SpecialOrderWPF SO)
            {
                this.SpecialOrder_WPF = SO;
            }
        }
        #endregion

        public delegate void SpecialOrderEventHandler(object sender, DayInfoEventArg e);
        public SpecialOrderEventHandler AddSpecialOrderEvent;
        public SpecialOrderEventHandler UpateSpecialOrderEvent;
        public SpecialOrderEventHandler DeleteSpecialOrderEvent;

        private string _dayNumber;
        public string DayNumber
        {
            get { return _dayNumber; }
            set { _dayNumber = value; NotifyPropertyChanged("DayNumber"); NotifyPropertyChanged("DisplayDate"); }
        }

        private List<FlavorWPF> _flavors = new List<FlavorWPF>();
        public List<FlavorWPF> Flavors
        {
            get { return _flavors; }
            set { _flavors = value; NotifyPropertyChanged("Flavors"); }
        }

        private List<SpecialOrderWPF> _orders = new List<SpecialOrderWPF>();
        public List<SpecialOrderWPF> Orders
        {
            get { return _orders; }
            set { _orders = value; NotifyPropertyChanged("Orders"); }
        }

        private SpecialOrderWPF _selectedSO = new SpecialOrderWPF();
        public SpecialOrderWPF SelectedSO
        {
            get { return _selectedSO; }
            set { _selectedSO = value; NotifyPropertyChanged("SelectedSO"); }
        }

        public DayInfoWPF()
        {
            this.DayNumber = Globals.DateTime_to_DayNumber(DateTime.Now);
        }

        public DayInfoWPF(DayInfo dayInfo)
        {
            this.DayNumber = dayInfo.DayNumber;

            this.Flavors = new List<FlavorWPF>();
            foreach (Flavor flav in dayInfo.Flavors.ToList())
                this.Flavors.Add(new FlavorWPF(flav));

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

        #region Command Declarations
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

        private void AddSO()
        {
            // show SO window
            SpecialOrderWPF SO_WPF = new SpecialOrderWPF();
            SO_WPF.Day_Number = this.DayNumber;
            SpecialOrderWindow So_Win = new SpecialOrderWindow(SO_WPF);
            So_Win.ShowDialog();
            
            // check that cancel wasn't pressed then throw event (with SO object) so it can be sent to WCF and added to db
            if(So_Win.Cancel == false)
                if (AddSpecialOrderEvent != null) { AddSpecialOrderEvent(this,new DayInfoEventArg(SO_WPF)); }
        }

        private void UpdateSO()
        {
            SpecialOrderWindow SO_Win = new SpecialOrderWindow(SelectedSO);
            SO_Win.ShowDialog();

            if(SO_Win.Cancel == false)
                if (UpateSpecialOrderEvent != null) { UpateSpecialOrderEvent(this, new DayInfoEventArg(SelectedSO)); }
        }

        private void DeleteSO()
        {
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

        public FlavorWPF(Flavor flav)
        {
            this.ID = flav.ID;
            this.Name = flav.Name;
            this.Description = flav.Description;
        }

        public Flavor ToFlavor()
        {
            Flavor flav = new Flavor();
            flav.ID = this.ID;
            flav.Name = this.Name;
            flav.Description = this.Description;
            return flav;
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
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyPropertyChanged("Quantity"); }
        }

        public BatchWPF(Batch batch)
        {
            this.Id = batch.ID;
            this.Day_number = batch.Day_Number;
            this.Flavor = new FlavorWPF(batch.Flavor);
            this.Requested = batch.Requested;
            this.Store = new StoreWPF(batch.Store);
            this.Quantity = batch.Quantity;
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
            return bat;
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
        private string _specialInstructions;
        public string SpecialInstructions
        {
            get { return _specialInstructions; }
            set { _specialInstructions = value; NotifyPropertyChanged("SpecialInstructions"); }
        }

        public SO_BatchWPF(SO_Batch batch)
        {
            this.Id = batch.ID;
            this.So_id = batch.SO_ID;
            this.Day_number = batch.Day_Number;
            this.Flavor = new FlavorWPF(batch.Flavor);
            this.Quantity = batch.Quantity;
            this.IsMini = batch.Is_Mini;
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
            bat.Special_Instructions = this.SpecialInstructions;
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
                if (Deliver)
                    return "Yes";
                else
                    return "No";
            }
        }
        private string _customer_Name;
        public string Customer_Name
        {
            get { return _customer_Name; }
            set { _customer_Name = value; NotifyPropertyChanged("Customer_Name"); }
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
        private string _dueTime;
        public string DueTime
        {
            get { return _dueTime; }
            set { _dueTime = value; NotifyPropertyChanged("DueTime"); }
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
        private List<SO_BatchWPF> _batches = new List<SO_BatchWPF>();
        public List<SO_BatchWPF> Batches
        {
            get { return _batches; }
            set { _batches = value; NotifyPropertyChanged("Batches"); }
        }
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; NotifyPropertyChanged("LastModified"); }
        }

        public SpecialOrderWPF()
        { }

        public SpecialOrderWPF(SpecialOrder SO)
        {
            this.Id = SO.ID;
            this.Day_Number = SO.Day_Number;
            this.Store = new StoreWPF(SO.Store);
            this.Deliver = SO.Deliver;
            this.Customer_Name = SO.Customer_Name;
            this.Customer_Phone = SO.Customer_Phone;
            this.Customer_Address = SO.Customer_Address;
            this.ScanLink = SO.ScanLink;
            this.DueTime = SO.DueTime;
            this.Notes = SO.Notes;
            this.SpecialInstructions = SO.SpecialInstructions;
            this.Batches = new List<SO_BatchWPF>();
            foreach (SO_Batch bat in SO.Batches)
                this.Batches.Add(new SO_BatchWPF(bat));
            this.LastModified = SO.LastModified;
        }

        public SpecialOrder ToSpecialOrder()
        {
            SpecialOrder so = new SpecialOrder();
            so.ID = this.Id;
            so.Day_Number = this.Day_Number;
            so.Store = this.Store.ToStoreInfo();
            so.Deliver = this.Deliver;
            so.Customer_Name = this.Customer_Name;
            so.Customer_Phone = this.Customer_Phone;
            so.Customer_Address = this.Customer_Address;
            so.ScanLink = this.ScanLink;
            so.DueTime = this.DueTime;
            so.Notes = this.Notes;
            so.SpecialInstructions = this.SpecialInstructions;
            // batches. first convert each to SO_Batch obj then convert that list to Array
            List<SO_Batch> soBatches = new List<SO_Batch>();
            foreach (SO_BatchWPF bat in this.Batches)
                soBatches.Add(bat.ToSO_Batch());
            so.Batches = soBatches.ToArray();
            so.LastModified = this.LastModified;
            return so;
        }

        public string PreviewInfo
        {
            get
            {
                return DueTime + @" / " + Customer_Name + @" / " + _customer_Phone + @" / " + DeliverUI;
            }
        }

        public string DisplayDate
        {
            get
            {
                string[] pieces = Day_Number.Split('_');
                int days = int.Parse(pieces[1]);
                int year = int.Parse(pieces[0]);
                DateTime theDate = new DateTime(year, 1, 1).AddDays(days);
                string rtn = string.Format("{0:D}", theDate); // theDate.ToString("M/d/yyy");
                return rtn;
            }
        }
    }

}
