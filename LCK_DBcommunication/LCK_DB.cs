using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Data.Sql;
using System.Data.SqlClient;
using LCK_DBcommunication.LCK_DataObjects;
using System.Data.SQLite;

namespace LCK_DBcommunication
{
    public class LCK_DB
    {
        //private DBcommunication dbComm = new DBcommunication(@"Data Source=HARVEYMAIN-PC\SQLEXPRESSWCF;Initial Catalog=LCK_WCF;Integrated Security=True;Pooling=False");
        //private SqlDataReader rdr;
        private SQLiteComm dbComm = new SQLiteComm();
        //private SQLiteDataReader rdr;

        public LCK_DB()
        {
            Logger._LogEnabled = true;
        }

        #region SELECT methods

        public Flavor GetFlavor(int id, bool closeDB = true)
        {
            try 
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Flavor 
                                            WHERE Id=" + id))
                {
                    rdr.Read();

                    Flavor flav = new Flavor();
                    flav.ID = int.Parse(rdr["id"].ToString());
                    flav.Name = rdr["name"].ToString();
                    flav.Description = rdr["description"].ToString();
                    flav.NotAFlavor = bool.Parse(rdr["not_a_flavor"].ToString());
                    flav.CakeFlavor = bool.Parse(rdr["cake_flavor"].ToString());
                    if (closeDB)
                        dbComm.Close();

                    return flav;
                }
            }
            catch (Exception ex)
            {
                Log("GetFlavor(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<Flavor> GetAllFlavors()
        {
            List<Flavor> flavors = new List<Flavor>();

            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand("SELECT * FROM Flavor WHERE Id>0"))
                {
                    while (rdr.Read())
                    {
                        Flavor flav = new Flavor();
                        flav.ID = int.Parse(rdr["id"].ToString());
                        flav.Name = rdr["Name"].ToString();
                        flav.Description = rdr["Description"].ToString();
                        flav.NotAFlavor = bool.Parse(rdr["not_a_flavor"].ToString());
                        flav.CakeFlavor = bool.Parse(rdr["cake_flavor"].ToString());
                        flavors.Add(flav);
                    }
                    dbComm.Close();

                    return flavors;
                }
            }
            catch (Exception ex)
            {
                Log("GetAllFlavors() - Error msg:" + ex.Message);
                return null;
            }
        }
        
        public Cake GetCake(int id, bool closeDB = true)
        {
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Cake 
                                            WHERE Id=" + id))
                {
                    rdr.Read();

                    Cake itm = new Cake();
                    itm.ID = int.Parse(rdr["id"].ToString());
                    itm.Name = rdr["name"].ToString();
                    itm.AbvName = rdr["abv_name"].ToString();
                    itm.Description = rdr["description"].ToString();
                    if (closeDB)
                        dbComm.Close();

                    return itm;
                }
            }
            catch (Exception ex)
            {
                Log("GetCake(int,bool) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<Cake> GetAllCakes()
        {
            List<Cake> cakes = new List<Cake>();

            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand("SELECT * FROM Cake WHERE Id>0"))
                {
                    while (rdr.Read())
                    {
                        Cake itm = new Cake();
                        itm.ID = int.Parse(rdr["id"].ToString());
                        itm.Name = rdr["Name"].ToString();
                        itm.Description = rdr["Description"].ToString();
                        itm.AbvName = rdr["abv_name"].ToString();
                        cakes.Add(itm);
                    }
                    dbComm.Close();

                    return cakes;
                }
            }
            catch (Exception ex)
            {
                Log("GetAllCakes() - Error msg:" + ex.Message);
                return null;
            }
        }

        public StoreInfo GetStoreInfo(int id, bool closeDB = true)
        {
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Stores 
                                            WHERE id=" + id))
                {
                    rdr.Read();

                    StoreInfo store = new StoreInfo();
                    store.ID = int.Parse(rdr["id"].ToString());
                    store.Name = rdr["name"].ToString();
                    store.Address = rdr["address"].ToString();
                    store.Phone = rdr["phone"].ToString();
                    if (closeDB)
                        dbComm.Close();

                    return store;
                }
            }
            catch (Exception ex)
            {
                Log("GetStoreInfo(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public StoreInfo GetStoreInfo(string Name)
        {
            try 
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Stores 
                                            WHERE name LIKE " + Name + "%"))
                {
                    rdr.Read();

                    StoreInfo store = new StoreInfo();
                    store.ID = int.Parse(rdr["id"].ToString());
                    store.Name = rdr["name"].ToString();
                    store.Address = rdr["address"].ToString();
                    store.Phone = rdr["phone"].ToString();

                    return store;
                }
            }
            catch (Exception ex)
            {
                Log("GetStoreInfo(str) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<StoreInfo> GetAllStoreInfo()
        {
            List<StoreInfo> stores = new List<StoreInfo>();

            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand("SELECT * FROM Stores WHERE id>0")) // id>0 filters out 'not found' option
                {
                    while (rdr.Read())
                    {
                        StoreInfo store = new StoreInfo();
                        store.ID = int.Parse(rdr["id"].ToString());
                        store.Name = rdr["name"].ToString();
                        store.Address = rdr["address"].ToString();
                        store.Phone = rdr["phone"].ToString();
                        stores.Add(store);
                    }
                    dbComm.Close();

                    return stores;
                }
            }
            catch (Exception ex)
            {
                Log("GetAllStoreInfo() - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<Flavor> GetDailyFlavors(int StoreID, string DayNumber)
        {
            List<Flavor> flavors = new List<Flavor>();

            try 
            { 
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Batch " +
                                            "WHERE store_id=" + StoreID + " " +
                                            "AND day_number='" + DayNumber + "'"))
                {
                    while (rdr.Read())
                    {
                        Flavor tmp = GetFlavor(int.Parse(rdr["flavor"].ToString()));
                        if(tmp == null)
                            tmp = GetFlavor(0,false);
                        flavors.Add(tmp);
                    }
                    dbComm.Close();

                    return flavors;
                }
            }
            catch (Exception ex)
            {
                Log("GetDailyFlavors(int,str) - Error msg:" + ex.Message);
                return null;
            }
        }

        public Batch GetBatch(int Batch_ID)
        {
            try
            {
                Batch bat = new Batch();
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Batch WHERE id='" + Batch_ID + @"'"))
                {
                    bat.ID = int.Parse(rdr["id"].ToString());
                    bat.Day_Number = rdr["day_number"].ToString();
                    bat.Flavor = GetFlavor(int.Parse(rdr["flavor"].ToString()), false);
                    // check to see if flavor is null, if so replace ID = 0 (not found) then re-call GetFlavor()
                    if (bat.Flavor == null)
                        bat.Flavor = GetFlavor(0, false);
                    bat.Quantity = int.Parse(rdr["qty"].ToString());
                    bat.QuantityMini = int.Parse(rdr["qty_mini"].ToString());
                    bat.Requested = (bool)rdr["requested"];
                    bat.Store = GetStoreInfo(int.Parse(rdr["store_id"].ToString()), false);
                    // check to see if store is null, if so replace ID = 0 (not found) then re-call GetStore()
                    if (bat.Store == null)
                        bat.Store = GetStoreInfo(0, false);

                    dbComm.Close();
                }

                return bat;
            }
            catch (Exception ex)
            {
                Log("GetBatch(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<Batch> GetBatches(int StoreID, string Day_Number)
        {
            List<Batch> batches = new List<Batch>();
            try {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Batch 
                                            WHERE day_number='" + Day_Number + @"' AND store_id='" + StoreID + "'"))
                {
                    while (rdr.Read())
                    {
                        Batch bat = new Batch();
                        bat.ID = int.Parse(rdr["id"].ToString());
                        bat.Day_Number = rdr["day_number"].ToString();
                        bat.Flavor = GetFlavor(int.Parse(rdr["flavor"].ToString()), false);
                        // check to see if flavor is null, if so replace ID = 0 (not found) then re-call GetFlavor()
                        if (bat.Flavor == null)
                            bat.Flavor = GetFlavor(0, false);
                        bat.Quantity = int.Parse(rdr["qty"].ToString());
                        bat.QuantityMini = int.Parse(rdr["qty_mini"].ToString());
                        bat.Requested = (bool)rdr["requested"];
                        bat.Store = GetStoreInfo(int.Parse(rdr["store_id"].ToString()), false);
                        // check to see if store is null, if so replace ID = 0 (not found) then re-call GetStore()
                        if (bat.Store == null)
                            bat.Store = GetStoreInfo(0, false);
                        batches.Add(bat);
                    }
                    dbComm.Close();

                    return batches;
                }
            }
            catch (Exception ex)
            {
                Log("GetBatches(int,str) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<SO_Batch> GetSOBatches(int SO_ID,bool closeDB = true)
        {
            List<SO_Batch> batches = new List<SO_Batch>();
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM SO_Batch WHERE so_id =" + SO_ID))
                {
                    //SQLiteDataReader tmpRdr = dbComm.ExecuteCommand(@"SELECT * FROM SO_Batch WHERE so_id =" + SO_ID);
                    while (rdr.Read())
                    {
                        SO_Batch bat = new SO_Batch();
                        bat.ID = int.Parse(rdr["id"].ToString());
                        bat.SO_ID = int.Parse(rdr["so_id"].ToString());
                        bat.Day_Number = rdr["day_number"].ToString();
                        bat.Flavor = GetFlavor(int.Parse(rdr["flavor"].ToString()), false);
                        // check to see if flavor is null, if so replace ID = 0 (not found) then re-call GetFlavor()
                        if (bat.Flavor == null)
                            bat.Flavor = GetFlavor(0, false);
                        bat.Quantity = int.Parse(rdr["qty"].ToString());
                        bat.Is_Mini = (bool)rdr["is_mini"];
                        bat.QuantityMini = int.Parse(rdr["qty_mini"].ToString());
                        bat.Special_Instructions = rdr["special_instructions"].ToString();
                        batches.Add(bat);
                    }
                    if (closeDB)
                        dbComm.Close();
                }
                return batches;
            }
            catch (Exception ex)
            {
                Log("GetSOBatches(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<Cake_Batch> GetSOCakes(int SO_ID, bool closeDB = true)
        {
            List<Cake_Batch> cakes = new List<Cake_Batch>();
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM Cake_Batch WHERE so_id =" + SO_ID))
                {
                    while (rdr.Read())
                    {
                        Cake_Batch itm = new Cake_Batch();
                        itm.ID = int.Parse(rdr["id"].ToString());
                        itm.SO_ID = int.Parse(rdr["so_id"].ToString());
                        itm.Cake = GetCake(int.Parse(rdr["cake_id"].ToString()), false);
                        itm.Flavor = GetFlavor(int.Parse(rdr["flavor"].ToString()), false);
                        // check to see if flavor is null, if so replace ID = 0 (not found) then re-call GetFlavor()
                        if (itm.Flavor == null)
                            itm.Flavor = GetFlavor(0, false);
                        itm.Quantity = int.Parse(rdr["qty"].ToString());
                        cakes.Add(itm);
                    }
                    if (closeDB)
                        dbComm.Close();
                }
                return cakes;
            }
            catch (Exception ex)
            {
                Log("GetSOCakes(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public SpecialOrder GetSpecialOrder(int SO_ID,bool closeDB = true)
        {
            SpecialOrder so = new SpecialOrder();
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM SpecialOrder 
                                            WHERE id=" + SO_ID))
                {
                    rdr.Read();

                    so.ID = int.Parse(rdr["id"].ToString());
                    so.Day_Number = rdr["day_number"].ToString();
                    so.Store = GetStoreInfo(int.Parse(rdr["store_id"].ToString()), false);
                    so.Deliver = bool.Parse(rdr["deliver"].ToString());
                    so.Setup = bool.Parse(rdr["setup"].ToString());
                    so.Customer_Name = rdr["cust_name"].ToString();
                    so.Customer_Email = rdr["cust_email"].ToString();
                    so.Customer_Phone = rdr["cust_phone"].ToString();
                    so.Customer_Address = rdr["cust_address"].ToString();
                    so.ScanLink = rdr["scan_link"].ToString();
                    so.DueTime = int.Parse(rdr["due_time"].ToString());
                    so.Notes = rdr["notes"].ToString();
                    so.SpecialInstructions = rdr["special_instructions"].ToString();
                    so.CuttingCakeNotes = rdr["cutting_cake"].ToString();
                    so.DisplayNotes = rdr["displays"].ToString();
                    string tmp = rdr["last_modified"].ToString();
                    so.LastModified = DateTime.Parse(tmp);

                    // get so_batches
                    so.Batches = GetSOBatches(so.ID,false);

                    // get cake_batchs
                    so.Cakes = GetSOCakes(so.ID,false);

                    if(closeDB)
                        dbComm.Close();

                    return so;
                }
            }
            catch (Exception ex)
            {
                Log("GetSpecialOrder(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<SpecialOrder> GetSpecialOrders(int StoreID, string Day_Number)
        {
            List<SpecialOrder> orders = new List<SpecialOrder>();

            try {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM SpecialOrder 
                                            WHERE day_number='" + Day_Number + @"' AND store_id=" + StoreID + 
                                            " ORDER BY due_time ASC"))
                {
                    while (rdr.Read())
                    {
                        SpecialOrder so = new SpecialOrder();
                        so.ID = int.Parse(rdr["id"].ToString());
                        so.Day_Number = rdr["day_number"].ToString();
                        so.Store = GetStoreInfo(int.Parse(rdr["store_id"].ToString()), false);
                        so.Deliver = bool.Parse(rdr["deliver"].ToString());
                        so.Setup = bool.Parse(rdr["setup"].ToString());
                        so.Customer_Name = rdr["cust_name"].ToString();
                        so.Customer_Email = rdr["cust_email"].ToString();
                        so.Customer_Phone = rdr["cust_phone"].ToString();
                        so.Customer_Address = rdr["cust_address"].ToString();
                        so.ScanLink = rdr["scan_link"].ToString();
                        so.DueTime = int.Parse(rdr["due_time"].ToString());
                        so.Notes = rdr["notes"].ToString();
                        so.SpecialInstructions = rdr["special_instructions"].ToString();
                        so.CuttingCakeNotes = rdr["cutting_cake"].ToString();
                        so.DisplayNotes = rdr["displays"].ToString();
                        string tmp = rdr["last_modified"].ToString();
                        so.LastModified = DateTime.Parse(tmp);
                        // populate batches
                        so.Batches = GetSOBatches(so.ID, false);
                        // populate cakes
                        so.Cakes = GetSOCakes(so.ID,false);

                        orders.Add(so);
                    }
                    dbComm.Close();

                    return orders;
                }
            }
            catch (Exception ex)
            {
                Log("GetSpecialOrders(int,str) - Error msg:" + ex.Message);
                return null;
            }
        }

        public DayInfo GetDayInfo(int StoreID, string DayNumber)
        {
            DayInfo day = new DayInfo();

            try
            {
                // get all daily flavors for a day
                //day.Flavors = GetDailyFlavors(StoreID, DayNumber);
                day.DailyBatches = GetBatches(StoreID, DayNumber);

                // get all special orders for a day
                day.Orders = GetSpecialOrders(StoreID, DayNumber);

                return day;
            }
            catch (Exception ex)
            {
                Log("BuildDayInfo(int,str) - Error msg:" + ex.Message);
                return null;
            }
        }

        public int GetLastSpecialOrderID()
        {
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT id FROM SpecialOrder 
                                            ORDER BY id DESC"))
                {
                    rdr.Read();

                    int rtn = int.Parse(rdr["id"].ToString());

                    return rtn;
                }
            }
            catch (Exception ex)
            {
                Log("GetLastSpecialOrderID() - Error msg:" + ex.Message);
                return -1;
            }
        }

        public List<NewOrder> GetNewOrders(bool closeDB = true)
        {
            List<NewOrder> newOrders = new List<NewOrder>();
            try
            {
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM NewOrders WHERE fulfilled = 'False'"))
                {
                    while (rdr.Read())
                    {
                        NewOrder itm = new NewOrder();
                        itm.ID = int.Parse(rdr["id"].ToString());
                        itm.SO = GetSpecialOrder(int.Parse(rdr["so_id"].ToString()),false);
                        itm.Store_Origin = GetStoreInfo(int.Parse(rdr["store_origin"].ToString()), false);
                        string tmp = rdr["created"].ToString();
                        itm.Created = DateTime.Parse(tmp);
                        itm.FulFilled = bool.Parse(rdr["fulfilled"].ToString());
                        itm.Editted = bool.Parse(rdr["editted"].ToString());
                        newOrders.Add(itm);
                    }
                    if (closeDB)
                        dbComm.Close();
                }
                return newOrders;
            }
            catch (Exception ex)
            {
                Log("GetNewOrders(bool) - Error msg:" + ex.Message);
                return null;
            }
        }

        #endregion

        #region UPDATE methods

        public bool UpdateSpecialOrderBatch(int SO_ID, SO_Batch updateBatch)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("so_id", SO_ID.ToString());
                data.Add("day_number", updateBatch.Day_Number);
                data.Add("flavor", updateBatch.Flavor.ID.ToString());
                data.Add("qty", updateBatch.Quantity.ToString());
                data.Add("is_mini", updateBatch.Is_Mini.ToString());
                data.Add("qty_mini", updateBatch.QuantityMini.ToString());
                data.Add("special_instructions", updateBatch.Special_Instructions);

                dbComm.Update("SO_Batch", data, String.Format("id = {0}", updateBatch.ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateSpecialOrderBatch(int,SO_Batch) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateSpecialOrderCake(int SO_ID, Cake_Batch updateCake)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("so_id", SO_ID.ToString());
                data.Add("cake_id", updateCake.Cake.ID.ToString());
                data.Add("flavor", updateCake.Flavor.ID.ToString());
                data.Add("qty", updateCake.Quantity.ToString());

                dbComm.Update("Cake_Batch", data, String.Format("id = {0}", updateCake.ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateSpecialOrderBatch(int,SO_Batch) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateSpecialOrder(int SO_ID, SpecialOrder updatedSO)
        {
            bool SO_rtn = false;
            try
            {
                // add Special Order
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("day_number", updatedSO.Day_Number);
                data.Add("store_id", updatedSO.Store.ID.ToString());
                data.Add("deliver", updatedSO.Deliver.ToString());
                data.Add("setup", updatedSO.Setup.ToString());
                data.Add("cust_name", updatedSO.Customer_Name);
                data.Add("cust_email", updatedSO.Customer_Email);
                data.Add("cust_phone", updatedSO.Customer_Phone);
                data.Add("cust_address", updatedSO.Customer_Address);
                data.Add("scan_link", updatedSO.ScanLink);
                data.Add("due_time", updatedSO.DueTime.ToString());
                data.Add("notes", updatedSO.Notes);
                data.Add("special_instructions", updatedSO.SpecialInstructions);
                data.Add("cutting_cake", updatedSO.CuttingCakeNotes);
                data.Add("displays", updatedSO.DisplayNotes);
                data.Add("last_modified", updatedSO.LastModified.ToString("yyyy-MM-dd HH:mm:ss"));

                dbComm.Update("SpecialOrder", data, String.Format("id = {0}", SO_ID));
                SO_rtn = true;

                // remove all SO_Batch rows for this SO_ID, then add
                DeleteAllSpecialOrderBatches(SO_ID);
                // add entries 
                foreach (SO_Batch bat in updatedSO.Batches)
                {
                    if(AddSO_Batch(SO_ID,bat))
                    { }
                }
                // remove all cakes then re-add
                DeleteAllSpecialOrderCakes(SO_ID);
                foreach(Cake_Batch cke in updatedSO.Cakes)
                {
                    if(AddSO_Cake(SO_ID,cke))
                    { }
                }

            }
            catch (Exception ex)
            {
                Log("UpdateSpecialOrder(int,SpecialOrder) - Error msg:" + ex.Message);
                SO_rtn = false;
            }

            return SO_rtn;
        }

        public bool UpdateBatch(int Batch_ID, Batch updateBatch)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("day_number", updateBatch.Day_Number);
                data.Add("flavor", updateBatch.Flavor.ID.ToString());
                data.Add("requested", updateBatch.Requested.ToString());
                data.Add("store_id", updateBatch.Store.ID.ToString());
                data.Add("qty", updateBatch.Quantity.ToString());
                data.Add("qty_mini", updateBatch.QuantityMini.ToString());

                dbComm.Update("Batch", data, String.Format("id = {0}", Batch_ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateBatch(int,Batch) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateFlavor(int Flavor_ID, Flavor updateFlavor)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("name", updateFlavor.Name);
                data.Add("description", updateFlavor.Description);
                data.Add("not_a_flavor", updateFlavor.NotAFlavor.ToString());
                data.Add("cake_flavor", updateFlavor.CakeFlavor.ToString());

                dbComm.Update("Flavor", data, String.Format("id = {0}", Flavor_ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateFlavor(int,Flavor) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateCake(int Cake_ID, Cake updateCake)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("name", updateCake.Name);
                data.Add("description", updateCake.Description);
                data.Add("abv_name", updateCake.AbvName);

                dbComm.Update("Cake", data, String.Format("id = {0}", Cake_ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateCake(int,Cake) - Error msg:" + ex.Message);
                return false;
            }
        }
        
        public bool UpdateStore(int Store_ID, StoreInfo updateStore)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("name", updateStore.Name);
                data.Add("address", updateStore.Address);
                data.Add("phone", updateStore.Phone);

                dbComm.Update("Stores", data, String.Format("id = {0}", Store_ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateStore(int,StoreInfo) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateNewOrder(int NewOrder_ID, NewOrder updateNewOrder)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("so_id", updateNewOrder.SO.ID.ToString());
                data.Add("store_origin", updateNewOrder.Store_Origin.ID.ToString());
                data.Add("created", updateNewOrder.Created.ToString("yyyy-MM-dd HH:mm:ss"));
                data.Add("fulfilled", updateNewOrder.FulFilled.ToString());
                data.Add("editted", updateNewOrder.Editted.ToString());

                dbComm.Update("NewOrders", data, String.Format("id = {0}", NewOrder_ID));
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateNewOrder(int,NewOrder) - Error msg:" + ex.Message);
                return false;
            }
        }

        #endregion

        #region INSERT methods

        public bool AddBatch(Batch batch)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("day_number", batch.Day_Number);
                data.Add("flavor", batch.Flavor.ID.ToString());
                data.Add("requested", batch.Requested.ToString());
                data.Add("store_id", batch.Store.ID.ToString());
                data.Add("qty", batch.Quantity.ToString());
                data.Add("qty_mini", batch.QuantityMini.ToString());

                dbComm.Insert("Batch", data);
                return true;
            }
            catch (Exception ex)
            {
                Log("AddBatch(Batch) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddSO_Batch(int SO_ID, SO_Batch batch)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("so_id", SO_ID.ToString());
                data.Add("day_number", batch.Day_Number);
                data.Add("flavor", batch.Flavor.ID.ToString());
                data.Add("qty", batch.Quantity.ToString());
                data.Add("is_mini", batch.Is_Mini.ToString());
                data.Add("qty_mini", batch.QuantityMini.ToString());
                data.Add("special_instructions", batch.Special_Instructions);

                dbComm.Insert("SO_Batch", data);
                return true;
            }
            catch (Exception ex)
            {
                Log("AddSO_Batch(int,SO_Batch) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddSO_Cake(int SO_ID, Cake_Batch batch)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("so_id", SO_ID.ToString());
                data.Add("cake_id", batch.Cake.ID.ToString());
                data.Add("flavor", batch.Flavor.ID.ToString());
                data.Add("qty", batch.Quantity.ToString());

                dbComm.Insert("Cake_Batch", data);
                return true;
            }
            catch (Exception ex)
            {
                Log("AddSO_Cake(int,Cake_Batch) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddSpecialOrder(SpecialOrder SO)
        {
            bool SO_rtn = false;
            try
            {
                // add Special Order
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("day_number", SO.Day_Number);
                data.Add("store_id", SO.Store.ID.ToString());
                data.Add("deliver", SO.Deliver.ToString());
                data.Add("setup", SO.Setup.ToString());
                data.Add("cust_name", SO.Customer_Name);
                data.Add("cust_email", SO.Customer_Email);
                data.Add("cust_phone", SO.Customer_Phone);
                data.Add("cust_address", SO.Customer_Address);
                data.Add("scan_link", SO.ScanLink);
                data.Add("due_time", SO.DueTime.ToString());
                data.Add("notes", SO.Notes);
                data.Add("special_instructions", SO.SpecialInstructions);
                data.Add("cutting_cake", SO.CuttingCakeNotes);
                data.Add("displays", SO.DisplayNotes);
                data.Add("last_modified", SO.LastModified.ToString("yyyy-MM-dd HH:mm:ss"));

                dbComm.Insert("SpecialOrder", data);
                SO_rtn = true;

                // extract newly added 'id'
                int SO_id = GetLastSpecialOrderID();

                // add each batch to SO_Batch table with link to SO_id
                foreach (SO_Batch bat in SO.Batches)
                {
                    if (AddSO_Batch(SO_id,bat) == false)
                    { }
                }

                // add each cake_batch with link to SO_id
                foreach(Cake_Batch cke in SO.Cakes)
                {
                    if(AddSO_Cake(SO_id,cke)== false)
                    { }
                }
            }
            catch (Exception ex)
            {
                Log("AddSpecialOrder(SpecialOrder) - Error msg:" + ex.Message);
                SO_rtn = false;
            }
                
            return SO_rtn;
        }

        public bool AddFlavor(Flavor flav)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("name", flav.Name);
                data.Add("description", flav.Description);
                data.Add("not_a_flavor", flav.NotAFlavor.ToString());
                data.Add("cake_flavor", flav.NotAFlavor.ToString());

                dbComm.Insert("Flavor", data);
                return true;
            }
            catch(Exception ex)
            {
                Log("AddFlavor(Flavor) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddCake(Cake cke)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("name", cke.Name);
                data.Add("description", cke.Description);
                data.Add("abv_name", cke.AbvName);

                dbComm.Insert("Cake", data);
                return true;
            }
            catch (Exception ex)
            {
                Log("AddCake(Flavor) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddStore(StoreInfo store)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("name", store.Name);
                data.Add("address", store.Address);
                data.Add("phone", store.Phone);

                dbComm.Insert("Stores", data);
                return true;
            }
            catch (Exception ex)
            {
                Log("AddStore(StoreInfo) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddNewOrder(NewOrder newOrder)
        {
            try
            {
                bool updateItem = false;
                // look to see if it is already in the database using SO_ID field
                int NO_ID=0;
                using (SQLiteDataReader rdr = dbComm.ExecuteCommand(@"SELECT * FROM NewOrders WHERE so_id ='" + newOrder.SO.ID + @"'"))
                {
                    if (rdr.HasRows)
                    {
                        updateItem = true;
                        rdr.Read();
                        NO_ID = int.Parse(rdr["id"].ToString());
                    }
                }

                if (updateItem)
                {
                    UpdateNewOrder(NO_ID, newOrder);
                    return true;
                }
                else
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("so_id", newOrder.SO.ID.ToString());
                    data.Add("store_origin", newOrder.Store_Origin.ID.ToString());
                    data.Add("created", newOrder.Created.ToString("yyyy-MM-dd HH:mm:ss"));
                    data.Add("fulfilled", newOrder.FulFilled.ToString());
                    data.Add("editted", newOrder.Editted.ToString());

                    dbComm.Insert("NewOrders", data);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log("AddNewOrder(NewOrder) - Error msg:" + ex.Message);
                return false;
            }
        }
        
        #endregion

        #region DELETE methods

        public bool DeleteSpecialOrderBatch(int batch_id)
        {
            bool rtn = false;
            try
            {
                // remove all SO_Batch table rows first
                dbComm.Delete("SO_Batch", String.Format("id = {0}", batch_id));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteSpecialOrder(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteAllSpecialOrderBatches(int SO_ID)
        {
            bool rtn = false;
            try
            {
                // remove all SO_Batch table rows first
                dbComm.Delete("SO_Batch", String.Format("so_id = {0}", SO_ID));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteAllSpecialOrderBatches(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteAllSpecialOrderCakes(int SO_ID)
        {
            bool rtn = false;
            try
            {
                // remove all Cake_Batch table rows first
                dbComm.Delete("Cake_Batch", String.Format("so_id = {0}", SO_ID));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteAllSpecialOrderCakes(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteSpecialOrder(int SO_ID)
        {
            bool rtn = false;
            try
            {
                // remove all SO_Batch table rows first
                dbComm.Delete("SO_Batch", String.Format("so_id = {0}", SO_ID));
                // remove row from SpecialOrder table
                dbComm.Delete("SpecialOrder", String.Format("id = {0}", SO_ID));
                // remove row from NewOrders table
                dbComm.Delete("NewOrders", String.Format("so_id = {0}", SO_ID));
                
                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteSpecialOrder(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteAllSpecialOrders()
        {
            bool rtn = false;
            try
            {
                // remove all from So_Batch table too
                dbComm.ExecuteNonQuery("Delete * FROM SO_Batch;");
                // remove all rows from table 
                dbComm.ExecuteNonQuery("Delete * FROM SpecialOrder;");
                // remove all rows from table 
                dbComm.ExecuteNonQuery("Delete * FROM NewOrders;");

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteAllSpecialOrders() - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteFlavor(int flavor_id)
        {
            bool rtn = false;
            try
            {
                // remove row from table 
                dbComm.Delete("Flavor", String.Format("id = {0}", flavor_id));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteFlavor(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteAllFlavors()
        {
            bool rtn = false;
            try
            {
                // remove all rows from table 
                dbComm.ExecuteNonQuery("Delete * FROM Flavor;");

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteAllFlavors() - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteCake(int cake_id)
        {
            bool rtn = false;
            try
            {
                // remove row from table 
                dbComm.Delete("Cake", String.Format("id = {0}", cake_id));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteCake(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteStore(int store_id)
        {
            bool rtn = false;
            try
            {
                // remove row from table 
                dbComm.Delete("Stores", String.Format("id = {0}", store_id));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteStore(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteBatch(int batch_id)
        {
            bool rtn = false;
            try
            {
                // remove row from table 
                dbComm.Delete("Batch", String.Format("id = {0}", batch_id));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteBatch(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        public bool DeleteAllBatches()
        {
            bool rtn = false;
            try
            {
                // remove all rows from table 
                dbComm.ExecuteNonQuery("Delete * FROM Batch;");

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteAllBatches() - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }
        
        public bool DeleteNewOrder(int newOrder_id)
        {
            bool rtn = false;
            try
            {
                // remove row from table 
                dbComm.Delete("NewOrders", String.Format("id = {0}", newOrder_id));

                rtn = true;
            }
            catch (Exception ex)
            {
                Log("DeleteNewOrder(int) - Error msg:" + ex.Message);
                rtn = false;
            }
            return rtn;
        }

        #endregion

        /// <summary>
        /// LCK_DB logging method
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        private static void Log(string msg, Logger.LogTypes logType = Logger.LogTypes.Debug)
        {
            Logger.Log(msg, "LCK_DBcommunication.LCK_DB", logType);
        }
    }
}
