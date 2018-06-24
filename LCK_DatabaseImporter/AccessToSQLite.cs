using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LCK_ClientLibrary;
using System.Data.OleDb;

namespace LCK_DatabaseImporter
{
    public class AccessToSQLite
    {
        // connection object for new WCF communication
        private LCK_WCFcommunication new_WCF = new LCK_WCFcommunication();

        // connection info for old MS Access database communication
        private string _databaseFileLocation = @"E:\Consulting\LCCK\Scheduler upgrade\development\database\Access\Scheduler.mdb";
        public string DatabaseFileLocation
        {
            get { return _databaseFileLocation; }
            set { _databaseFileLocation = value; }
        }
        public string ConnectionString
        {
            //get { return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFileLocation + ";"; }
            get { return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DatabaseFileLocation + ";"; }
        }

        private OleDbConnection old_Access;

        public AccessToSQLite()
        {
            old_Access = new OleDbConnection(ConnectionString);
        }

        public bool ClearBatchTable()
        {
            try
            {
                new_WCF.DeleteAllBatches();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool ClearFlavorTable()
        {
            try
            {
                new_WCF.DeleteAllFlavors();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ClearSpecialOrderTable()
        {
            try
            {
                new_WCF.DeleteAllSpecialOrders();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int BatchTable()
        {
            int count = 0;
            // will be used to link flavor in new db entry
            List<FlavorWPF> AllFlavors = new_WCF.GetAllFlavors();

            try
            {
                // read from old db
                old_Access.Open();
                string strCmd = @"SELECT * from Batch";
                OleDbCommand cmd = new OleDbCommand(strCmd, old_Access);
                OleDbDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    BatchWPF itm_WPF = new BatchWPF();
                    itm_WPF.Day_number = rdr["Day_Number"].ToString();

                    // using flavor name, find flavor in new db to make foreign key link
                    string tmp = rdr["Flavor"].ToString();
                    FlavorWPF flav = AllFlavors.Find(x => x.Name.Contains(tmp));
                    itm_WPF.Flavor = flav;

                    itm_WPF.Requested = rdr["Requested"].ToString() == "Yes" ? true : false;
                    itm_WPF.Quantity = 0;
                    itm_WPF.Store = new_WCF.GetStoreInfo(2);    // ID=>2 is Vista by default

                    new_WCF.AddBatch(itm_WPF);
                    count++;
                }
                rdr.Close();
                old_Access.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }

            return count;
        }

        public int FlavorTable()
        {
            int count = 0;

            try
            {
                // read from old db
                old_Access.Open();
                string strCmd = @"SELECT * from Flavors";
                OleDbCommand cmd = new OleDbCommand(strCmd, old_Access);
                OleDbDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    FlavorWPF itm_WPF = new FlavorWPF();
                    itm_WPF.ID = int.Parse(rdr["ID"].ToString());
                    itm_WPF.Name = rdr["Flavor_Name"].ToString();
                    itm_WPF.Description = rdr["Description"].ToString();

                    new_WCF.AddFlavor(itm_WPF);
                    count++;
                }
                rdr.Close();
                old_Access.Close();
            }
            catch(Exception ex)
            {
                return -1;
            }

            return count;
        }

        public int SpecialOrderTable()
        {
            int count = 0;
            // will be used to link flavor in new db entry
            List<FlavorWPF> AllFlavors = new_WCF.GetAllFlavors();

            try
            {
                // read from old db
                old_Access.Open();
                string strCmd = @"SELECT * from Special_Orders";
                OleDbCommand cmd = new OleDbCommand(strCmd, old_Access);
                OleDbDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    SpecialOrderWPF itm_WPF = new SpecialOrderWPF();
                    itm_WPF.Id = int.Parse(rdr["SO_Number"].ToString());
                    itm_WPF.Day_Number = rdr["Day_Number"].ToString();
                    
                    // scanLink has to be truncated to match new scan folder structure on server
                    string tmpLink = rdr["ScanLink"].ToString();
                    tmpLink = System.IO.Path.GetFileName(tmpLink);
                    itm_WPF.ScanLink = tmpLink;                    
                    
                    itm_WPF.Customer_Name = rdr["Customer_Name"].ToString();
                    itm_WPF.Customer_Phone = rdr["Contact_Phone"].ToString();
                    itm_WPF.Customer_Address = rdr["Address"].ToString();
                    
                    // convert to 24hr time
                    string value = String.Format("{0:HH}", DateTime.Parse(rdr["Due_Time"].ToString()));
                    itm_WPF.DueTime = int.Parse(value);

                    itm_WPF.Deliver = rdr["Delivery"].ToString() == "Yes" ? true : false;
                    itm_WPF.Setup = rdr["Setup"].ToString() == "Yes" ? true : false;
                    itm_WPF.SpecialInstructions = rdr["Special_Instructions"].ToString();
                    itm_WPF.Notes = rdr["Special_Instructions"].ToString();

                    new_WCF.AddSpecialOrder(itm_WPF);
                    count++;
                }
                rdr.Close();
                old_Access.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }

            return count;
        }

        public int SO_BatchTable()
        {
            int count = 0;
            // will be used to link flavor in new db entry
            List<FlavorWPF> AllFlavors = new_WCF.GetAllFlavors();

            try
            {
                // read from old db
                old_Access.Open();
                string strCmd = @"SELECT * from SpecialOrder_Batch";
                OleDbCommand cmd = new OleDbCommand(strCmd, old_Access);
                OleDbDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    SO_BatchWPF itm_WPF = new SO_BatchWPF();
                    itm_WPF.So_id = int.Parse(rdr["SO_Number"].ToString());
                    itm_WPF.Day_number = rdr["Day_Number"].ToString();

                    // using flavor name, find flavor in new db to make foreign key link
                    string tmp = rdr["Flavor"].ToString();
                    FlavorWPF flav = AllFlavors.Find(x => x.Name.Contains(tmp));
                    itm_WPF.Flavor = flav;
                    
                    itm_WPF.Quantity = int.Parse(rdr["Quantity"].ToString());
                    itm_WPF.IsMini = rdr["Mini"].ToString() == "Yes" ? true : false;
                    itm_WPF.SpecialInstructions = rdr["Special_Instructions"].ToString();

                    new_WCF.AddSpecialOrderBatch(itm_WPF.Id, itm_WPF);
                    count++;
                }
                rdr.Close();
                old_Access.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }

            return count;
        }

    }
}
