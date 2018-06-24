using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

//using System.Runtime.Serialization.Formatters.Binary;
using LCK_WPFclient.LCK_ServiceReference;

namespace LCK_WPFclient
{
    public class LCK_WCFcommunication
    {
        LCK_ServiceClient lck = new LCK_ServiceClient();

        public LCK_WCFcommunication()
        {
            Logger._LogEnabled = true;
        }

        #region Get/Select methods

        public List<FlavorWPF> GetAllFlavors()
        {
            try
            {
                List<FlavorWPF> flavWPFs = new List<FlavorWPF>();
                List<Flavor> flavs = lck.GetAllFlavors().ToList();

                foreach (Flavor flav in flavs)
                    flavWPFs.Add(new FlavorWPF(flav));

                return flavWPFs;
            }
            catch (Exception ex)
            {
                Log("GetAllFlavors() - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<StoreWPF> GetAllStores()
        {
            try
            {
                List<StoreWPF> storeWPFs = new List<StoreWPF>();
                List<StoreInfo> stores = lck.GetAllStoresInfo().ToList();

                foreach (StoreInfo store in stores)
                    storeWPFs.Add(new StoreWPF(store));

                return storeWPFs;
            }
            catch(Exception ex)
            {
                Log("GetAllStores() - Error msg:" + ex.Message);
                return null;
            }
        }

        public DayInfoWPF GetDayInfo(int StoreID, string DayNumber)
        {
            try
            {
                DayInfo day = lck.GetDayInfo(StoreID, DayNumber);
                day.DayNumber = DayNumber;

                // ToDo - sort DayInfo.Orders by DueTime


                return new DayInfoWPF(day);
            }
            catch(Exception ex)
            {
                Log("GetDayInfo(int,string) - Error msg:" + ex.Message);
                return null;
            }
        }

        public StoreWPF GetStoreInfo(int StoreID)
        {
            try
            {
                StoreInfo store = lck.GetStoreInfoByID(StoreID);
                return new StoreWPF(store);
            }
            catch (Exception ex)
            {
                Log("GetStoreInfo(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public StoreWPF GetStoreInfo(string StoreName)
        {
            try
            {
                StoreInfo store = lck.GetStoreInfo(StoreName);
                return new StoreWPF(store);
            }
            catch (Exception ex)
            {
                Log("GetStoreInfo(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public SpecialOrderWPF GetSpecialOrder(int SO_ID)
        {
            try
            {
                SpecialOrder so = lck.GetSpecialOrder(SO_ID);
                return new SpecialOrderWPF(so);
            }
            catch (Exception ex)
            {
                Log("GetSpecialOrder(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Used to get all SpecialOrders for a specific day. To ensure updated data use GetSpecialOrder() when particular SO is selected.
        /// </summary>
        /// <param name="StoreID"></param>
        /// <param name="DayNumber"></param>
        /// <returns></returns>
        public List<SpecialOrderWPF> GetSpecialOrders(int StoreID, string DayNumber)
        {
            try
            {
                List<SpecialOrderWPF> soWPFs = new List<SpecialOrderWPF>();
                List<SpecialOrder> sos = lck.GetSpecialOrders(StoreID, DayNumber).ToList();

                foreach (SpecialOrder so in sos)
                    soWPFs.Add(new SpecialOrderWPF(so));

                return soWPFs;
            }
            catch (Exception ex)
            {
                Log("GetSpecialOrders(int,string) - Error msg:" + ex.Message);
                return null;
            }
        }

        #endregion

        #region Add/Insert methods
        public bool AddSpecialOrderBatch(int SO_ID, SO_BatchWPF SoBatch_WPF)
        {
            try
            {
                bool rtn = lck.AddSpecialOrderBatch(SO_ID, SoBatch_WPF.ToSO_Batch());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddSpeicalOrderBatch(int,SO_BatchWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddSpecialOrder(SpecialOrderWPF SO_WPF)
        {
            try
            {
                SO_WPF.LastModified = DateTime.Now;
                bool rtn = lck.AddSpecialOrder(SO_WPF.ToSpecialOrder());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddSpecialOrder(SpecialOrderWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddFlavor(FlavorWPF Flavor_WPF)
        {
            try
            {
                bool rtn = lck.AddFlavor(Flavor_WPF.ToFlavor());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddFlavor(FlavorWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool AddStore(StoreWPF Store_WPF)
        {
            try
            {
                bool rtn = lck.AddStore(Store_WPF.ToStoreInfo());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddStore(StoreWPF) - Error msg:" + ex.Message);
                return false;
            }
        }
        #endregion

        #region Update methods
        public bool UpdateSpecialOrderBatch(int SO_ID, SO_BatchWPF SoBatch_WPF)
        {
            try
            {
                bool rtn = lck.UpdateSpecialOrderBatch(SO_ID, SoBatch_WPF.ToSO_Batch());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpdateSpecialOrderBatch(int,SO_BatchWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateSpecialOrder(int SO_ID, SpecialOrderWPF SO_WPF)
        {
            try
            {
                bool rtn = lck.UpdateSpecialOrder(SO_ID, SO_WPF.ToSpecialOrder());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpdateSpecialOrder(int,SpecialOrderWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateBatch(int Batch_ID, BatchWPF Batch_WPF)
        {
            try
            {
                bool rtn = lck.UpdateBatch(Batch_ID, Batch_WPF.ToBatch());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpdateBatch(int,BatchWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateFlavor(int Flavor_ID, FlavorWPF Flavor_WPF)
        {
            try
            {
                bool rtn = lck.UpdateFlavor(Flavor_ID, Flavor_WPF.ToFlavor());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpdateFlavor(int,FlavorWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UpdateStore(int Store_ID, StoreWPF Store_WPF)
        {
            try
            {
                bool rtn = lck.UpdateStore(Store_ID, Store_WPF.ToStoreInfo());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpdateStore(int,StoreWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        #endregion

        #region Delete methods
        public bool DeleteSpecialOrder(SpecialOrderWPF SO_WPF)
        {
            try
            {
                bool rtn = lck.DeleteSpecialOrder(SO_WPF.Id);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteSpecialOrder(SpecialOrderWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool DeleteSpecialOrderBatch(SO_BatchWPF SoBatch_WPF)
        {
            try
            {
                bool rtn = lck.DeleteSpecialOrderBatch(SoBatch_WPF.Id);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteSpcialOrderBatch(SO_BatchWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool DeleteBatch(BatchWPF Batch_WPF)
        {
            try
            {
                bool rtn = lck.DeleteBatch(Batch_WPF.Id);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteBatch(BatchWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool DeleteFlavor(FlavorWPF Flavor_WPF)
        {
            try
            {
                bool rtn = lck.DeleteFlavor(Flavor_WPF.ID);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteFlavor(FlavorWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool DeleteStore(StoreWPF Store_WPF)
        {
            try
            {
                bool rtn = lck.DeleteStore(Store_WPF.Id);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteStore(StoreWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// LCK_DB logging method
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        private static void Log(string msg, Logger.LogTypes logType = Logger.LogTypes.Debug)
        {
            Logger.Log(msg, "LCK_WPFClient.LCK_WCFcommunication", logType);
        }

    }
}
