using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

//using System.Runtime.Serialization.Formatters.Binary;
using LCK_ClientLibrary.LCK_ServiceReference;

namespace LCK_ClientLibrary
{
    public class LCK_WCFcommunication
    {
        LCK_ServiceClient lck = new LCK_ServiceClient("endpoint_localhost");

        public LCK_WCFcommunication()
        {
            Logger._LogEnabled = true;
        }

        public LCK_WCFcommunication(string endPointName)
        {
            Logger._LogEnabled = true;

            lck = new LCK_ServiceClient(endPointName);
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

                // order output list alphabetically
                flavWPFs = flavWPFs.OrderBy(p => p.Name).ToList();

                return flavWPFs;
            }
            catch (Exception ex)
            {
                Log("GetAllFlavors() - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<FlavorWPF> GetVisibleFlavors()
        {
            try
            {
                List<FlavorWPF> flavWPFs = new List<FlavorWPF>();
                List<Flavor> flavs = lck.GetAllFlavors().ToList();

                foreach (Flavor flav in flavs)
                {
                    if(!flav.Invisible)
                        flavWPFs.Add(new FlavorWPF(flav));
                }

                return flavWPFs;
            }
            catch (Exception ex)
            {
                Log("GetAllFlavors() - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<CakeWPF> GetAllCakes()
        {
            try
            {
                List<CakeWPF> listWPFs = new List<CakeWPF>();
                List<Cake> ckes = lck.GetAllCakes().ToList();

                foreach (Cake cke in ckes)
                    listWPFs.Add(new CakeWPF(cke));

                return listWPFs;
            }
            catch (Exception ex)
            {
                Log("GetAllCakes() - Error msg:" + ex.Message);
                return null;
            }
        }

        public List<FlavorWPF> GetAllCakeFlavors()
        {
            try
            {
                List<FlavorWPF> flavWPFs = new List<FlavorWPF>();
                List<Flavor> flavs = lck.GetAllFlavors().ToList();

                foreach (Flavor flav in flavs)
                {
                    if(flav.CakeFlavor)
                        flavWPFs.Add(new FlavorWPF(flav));
                }

                return flavWPFs;
            }
            catch (Exception ex)
            {
                Log("GetAllCakeFlavors() - Error msg:" + ex.Message);
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

        public BatchWPF GetBatch(int BatchID)
        {
            try
            {
                Batch bat = lck.GetBatch(BatchID);
                BatchWPF batWPF = new BatchWPF(bat);
                return batWPF;
            }
            catch (Exception ex)
            {
                Log("GetBatch(int) - Error msg:" + ex.Message);
                return null;
            }
        }

        public DayInfoWPF GetDayInfo(int StoreID, string DayNumber, bool appendSO2DailyFlavors = false)
        {
            try
            {
                DayInfo day = lck.GetDayInfo(StoreID, DayNumber);
                day.DayNumber = DayNumber;

                DayInfoWPF dayWPF = new DayInfoWPF(day);
                // extract all flavors from SOs and append to dailyBatches
                if(appendSO2DailyFlavors)
                {
                    foreach(SpecialOrderWPF SO in dayWPF.Orders)
                    {
                        foreach(SO_BatchWPF bat in SO.Batches)
                        {
                            BatchWPF tmp = new BatchWPF();
                            tmp.Day_number = bat.Day_number;
                            tmp.Store = bat.Store;
                            tmp.Flavor = bat.Flavor;
                            tmp.Quantity = bat.Quantity;
                            tmp.Requested = true;
                            tmp.IsMini = bat.IsMini;
                            // check to see if item already in list
                            BatchWPF found =  dayWPF.DailyBatches.Find(x => x.Flavor.ID == tmp.Flavor.ID && x.IsMini == tmp.IsMini);
                            if (found == null)
                            {
                                dayWPF.DailyBatches.Add(tmp);
                            }
                        }
                    }
                }

                return dayWPF;
            }
            catch(Exception ex)
            {
                Log("GetDayInfo(int,string,bool) - Error msg:" + ex.Message);
                return null;
            }
        }

        public DayInfoWPF GetDayInfo(string DayNumber, bool appendSO2DailyFlavors = false)
        {
            try
            {
                DayInfo day = lck.GetDayInfoAll(DayNumber);
                day.DayNumber = DayNumber;

                DayInfoWPF dayWPF = new DayInfoWPF(day);
                // extract all flavors from SOs and append to dailyBatches
                if (appendSO2DailyFlavors)
                {
                    foreach (SpecialOrderWPF SO in dayWPF.Orders)
                    {
                        // extract flavor info from Special Orders and add to Daily Flavors panel
                        foreach (SO_BatchWPF bat in SO.Batches)
                        {
                            BatchWPF tmp = new BatchWPF();
                            tmp.Day_number = bat.Day_number;
                            tmp.Store = SO.Store;
                            tmp.Flavor = bat.Flavor;
                            tmp.Quantity = bat.Quantity;
                            tmp.QuantityMini = bat.QuantityMini;
                            tmp.Requested = true;
                            tmp.IsMini = bat.IsMini;
                            // check to see if item already in list
                            BatchWPF found = dayWPF.DailyBatches.Find(x => x.Flavor.ID == tmp.Flavor.ID && x.IsMini == tmp.IsMini);
                            if (found == null)
                            {
                                dayWPF.DailyBatches.Add(tmp);
                            }
                            else
                            {
                                // add qty
                                found.Quantity += tmp.Quantity;
                                found.QuantityMini += tmp.QuantityMini;
                                // updated Request flag
                                found.Requested = true;
                                // check if at both locations
                                if (found.Store.Name != tmp.Store.Name)
                                    found.BothStores = true;
                            }
                        }
                        // extract Cake info from SO and add to Daily Flavors panel
                        foreach(Cake_BatchWPF cakeBat in SO.Cakes)
                        {
                            BatchWPF tmp = new BatchWPF();
                            tmp.Day_number = SO.Day_Number;
                            tmp.Store = SO.Store;
                            // create temporary flavorWPF that I manipulate and make custom name for
                            FlavorWPF flav = new FlavorWPF();
                            flav.Name = cakeBat.Cake.AbvName + " " + cakeBat.Flavor.Name;
                            tmp.Flavor = flav;
                            tmp.Quantity = cakeBat.Quantity;
                            tmp.Requested = true;
                            tmp.IsMini = false;

                            // check if already present in list
                            BatchWPF found = dayWPF.DailyBatches.Find(x => x.Flavor.Name == tmp.Flavor.Name);
                            if (found == null)  // not found, so add
                                dayWPF.DailyBatches.Add(tmp);
                            else  // found so sum up quantities
                                found.Quantity += tmp.Quantity;
                        }
                    }

                    // scan thru all DailyBatches and combine mini and Full for same flavor
                    List<BatchWPF> deleteList = new List<BatchWPF>();
                    for (int i = dayWPF.DailyBatches.Count - 1; i >= 0; i--)
                    {
                        BatchWPF current = dayWPF.DailyBatches[i];
                        List<BatchWPF> found = dayWPF.DailyBatches.FindAll(x => x.Flavor.Name == current.Flavor.Name);
                        if (found.Count > 1)
                        {
                            // combine qty/qtyMini
                            found[0].Quantity += found[1].Quantity;
                            found[0].QuantityMini += found[1].QuantityMini;
                            // then remove duplicate from list
                            dayWPF.DailyBatches.RemoveAt(i);
                        }
                    }
                }

                return dayWPF;
            }
            catch (Exception ex)
            {
                Log("GetDayInfo(string,bool) - Error msg:" + ex.Message);
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

        public int GetLastSpecialOrderID()
        {
            try
            {
                int rtn = lck.GetLastSpecialOrderID();
                return rtn;
            }
            catch (Exception ex)
            {
                Log("GetLastSpecialOrderID() - Error msg:" + ex.Message);
                return 0;
            }
        }

        public List<NewOrderWPF> GetNewOrders()
        {
            try
            {
                List<NewOrderWPF> NO_WPFs = new List<NewOrderWPF>();
                List<NewOrder> NOs = lck.GetNewOrders().ToList();

                foreach (NewOrder no in NOs)
                {
                    //if(!no.FulFilled)   // only add orders that haven't been processed
                    //    NO_WPFs.Add(new NewOrderWPF(no));
                    NO_WPFs.Add(new NewOrderWPF(no));
                }

                return NO_WPFs;
            }
            catch(Exception ex)
            {
                Log("GetNewOrders() - Error msg:" + ex.Message);
                return null;
            }
        }

        #endregion

        #region Add/Insert methods
        public bool AddBatch(BatchWPF batch)
        {
            try
            {
                bool rtn = lck.AddBatch(batch.ToBatch());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddFlavor(FlavorWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

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

        public bool AddCake(CakeWPF Cake_WPF)
        {
            try
            {
                bool rtn = lck.AddCake(Cake_WPF.ToCake());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddCake(CakeWPF) - Error msg:" + ex.Message);
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

        public bool AddNewOrder(NewOrderWPF NO_WPF)
        {
            try
            {
                bool rtn = lck.AddNewOrder(NO_WPF.ToNewOrder());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("AddNewOrder(NewOrderWPF) - Error msg:" + ex.Message);
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

        public bool UpdateCake(int Cake_ID, CakeWPF Cake_WPF)
        {
            try
            {
                bool rtn = lck.UpdateCake(Cake_ID, Cake_WPF.ToCake());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpdateCake(int,CakeWPF) - Error msg:" + ex.Message);
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

        public bool UpateNewOrder(int NO_ID, NewOrderWPF NO_WPF)
        {
            try
            {
                bool rtn = lck.UpdateNewOrder(NO_ID, NO_WPF.ToNewOrder());
                return rtn;
            }
            catch (Exception ex)
            {
                Log("UpateNewOrder(int,NewOrderWPF) - Error msg:" + ex.Message);
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

        public bool DeleteAllSpecialOrders()
        {
            try
            {
                bool rtn = lck.DeleteAllSpecialOrders();
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteAllSpecialOrders() - Error msg:" + ex.Message);
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

        public bool DeleteAllBatches()
        {
            try
            {
                bool rtn = lck.DeleteAllBatches();
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteAllBatches() - Error msg:" + ex.Message);
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

        public bool DeleteAllFlavors()
        {
            try
            {
                bool rtn = lck.DeleteAllFlavors();
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteAllFlavors() - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool DeleteCake(CakeWPF Cake_WPF)
        {
            try
            {
                bool rtn = lck.DeleteCake(Cake_WPF.ID);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeleteCake(CakeWPF) - Error msg:" + ex.Message);
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

        public bool DeteleNewOrder(NewOrderWPF NO_WPF)
        {
            try
            {
                bool rtn = lck.DeleteNewOrder(NO_WPF.Id);
                return rtn;
            }
            catch (Exception ex)
            {
                Log("DeteleNewOrder(NewOrderWPF) - Error msg:" + ex.Message);
                return false;
            }
        }

        #endregion

        #region File Transfer methods

        public bool DownloadFile(string requestFilename, string WriteToFolderPath)
        {
            string WriteToFilePath = "";
            try
            {
                ILCK_Service clientDownload = new LCK_ServiceClient(ConfigSettings_Static.EndpointAddress);
                DownloadRequest dlRequest = new DownloadRequest();
                RemoteFileInfo remFile = new RemoteFileInfo();
                dlRequest.FileName = requestFilename;

                // request file from server
                remFile = clientDownload.DownloadFileFromServer(dlRequest);

                // build local filepath to save to
                WriteToFilePath = Path.Combine(WriteToFolderPath, requestFilename);

                using (FileStream fileStream = new FileStream(WriteToFilePath, FileMode.Create, FileAccess.Write))
                {
                    remFile.FileByteStream.CopyTo(fileStream);
                }

                return true;
            }
            catch(Exception ex)
            {
                Log("DownloadFile() [requestedFile=" + requestFilename + "][WriteToFilePath=" + WriteToFilePath + "] - Error msg:" + ex.Message);
                return false;
            }
        }

        public bool UploadFile(string ReadFromFilePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(ReadFromFilePath);
                ILCK_Service clientUpload = new LCK_ServiceClient(ConfigSettings_Static.EndpointAddress);
                RemoteFileInfo updateRequest = new RemoteFileInfo();

                if (!fileInfo.Exists)
                    return false;

                using (FileStream stream = new FileStream(ReadFromFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    updateRequest.FileName = Path.GetFileName(ReadFromFilePath);
                    updateRequest.Length = fileInfo.Length;
                    updateRequest.FileByteStream = stream;
                    clientUpload.UploadFileToServer(updateRequest);
                }

                return true;
            }
            catch(Exception ex)
            {
                Log("UploadFile() - Error msg:" + ex.Message);
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
