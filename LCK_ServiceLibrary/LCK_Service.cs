using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

using LCK_DBcommunication;
using LCK_DBcommunication.LCK_DataObjects;

namespace LCK_ServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class LCK_Service : ILCK_Service
    {
        private LCK_DB lck = new LCK_DB();
        private string scansFolder = AppDomain.CurrentDomain.BaseDirectory + @"scans";

        #region SELECT/Get methods
        public List<Flavor> GetAllFlavors()
        {
            return lck.GetAllFlavors();
        }

        public List<Cake> GetAllCakes()
        {
            return lck.GetAllCakes();
        }

        public Batch GetBatch(int BatchID)
        {
            return lck.GetBatch(BatchID);
        }

        public List<Batch> GetBatches(int StoreID, string DayNumber)
        {
            return lck.GetBatches(StoreID, DayNumber);
        }

        public List<Batch> GetDailyFlavors(int StoreID, string DayNumber)
        {
            return lck.GetBatches(StoreID, DayNumber);
        }

        public StoreInfo GetStoreInfoByID(int StoreID)
        {
            return lck.GetStoreInfo(StoreID);
        }

        public StoreInfo GetStoreInfo(string Name)
        {
            return lck.GetStoreInfo(Name);
        }

        public SpecialOrder GetSpecialOrder(int SO_ID)
        {
            return lck.GetSpecialOrder(SO_ID);
        }

        public List<SpecialOrder> GetSpecialOrders(int StoreID, string DayNumber)
        {
            return lck.GetSpecialOrders(StoreID, DayNumber);
        }

        public int GetLastSpecialOrderID()
        {
            return lck.GetLastSpecialOrderID();
        }

        public DayInfo GetDayInfo(int StoreID, string DayNumber)
        {
            return lck.GetDayInfo(StoreID, DayNumber);
        }

        public DayInfo GetDayInfoAll(string DayNumber)
        {
            DayInfo carlsbad = lck.GetDayInfo(1, DayNumber);
            DayInfo vista = lck.GetDayInfo(2, DayNumber);

            // combine the 2, add vista stuff to carlsbad
            carlsbad.DayNumber = DayNumber;
            foreach (Batch bat in vista.DailyBatches)
                carlsbad.DailyBatches.Add(bat);

            foreach (SpecialOrder SO in vista.Orders)
                carlsbad.Orders.Add(SO);

            return carlsbad;
        }

        public List<StoreInfo> GetAllStoresInfo()
        {
            return lck.GetAllStoreInfo();
        }

        public List<NewOrder> GetNewOrders()
        {
            return lck.GetNewOrders();
        }
        #endregion

        #region INSERT/Add methods
        public bool AddBatch(Batch bat)
        {
            return lck.AddBatch(bat);
        }

        public bool AddFlavor(Flavor flav)
        {
            return lck.AddFlavor(flav);
        }

        public bool AddCake(Cake cke)
        {
            return lck.AddCake(cke);
        }

        public bool AddSpecialOrderBatch(int SO_ID, SO_Batch batch)
        {
            return lck.AddSO_Batch(SO_ID, batch);
        }

        public bool AddSpecialOrderCakeBatch(int SO_ID, Cake_Batch cakeBatch)
        {
            return lck.AddSO_Cake(SO_ID, cakeBatch);
        }

        public bool AddSpecialOrder(SpecialOrder SO)
        {
            return lck.AddSpecialOrder(SO);
        }

        public bool AddStore(StoreInfo Store)
        {
            return lck.AddStore(Store);
        }

        public bool AddNewOrder(NewOrder newOrder)
        {
            return lck.AddNewOrder(newOrder);
        }
        #endregion

        #region UPDATE methods
        public bool UpdateSpecialOrder(int SO_ID, SpecialOrder SO)
        {
            return lck.UpdateSpecialOrder(SO_ID, SO);
        }

        public bool UpdateSpecialOrderBatch(int SO_ID, SO_Batch updatedBatch)
        {
            return lck.UpdateSpecialOrderBatch(SO_ID, updatedBatch);
        }

        public bool UpdateBatch(int Batch_ID, Batch updatedBatch)
        {
            return lck.UpdateBatch(Batch_ID, updatedBatch);
        }

        public bool UpdateFlavor(int Flavor_ID, Flavor updatedFlavor)
        {
            return lck.UpdateFlavor(Flavor_ID, updatedFlavor);
        }

        public bool UpdateCake(int Cake_ID, Cake updateCake)
        {
            return lck.UpdateCake(Cake_ID, updateCake);
        }

        public bool UpdateStore(int Store_ID, StoreInfo updatedStore)
        {
            return lck.UpdateStore(Store_ID, updatedStore);
        }

        public bool UpdateNewOrder(int NewOrder_ID, NewOrder updatedNewOrder)
        {
            return lck.UpdateNewOrder(NewOrder_ID, updatedNewOrder);
        }
        #endregion

        #region DELETE methods
        public bool DeleteSpecialOrder(int SO_ID)
        {
            return lck.DeleteSpecialOrder(SO_ID);
        }

        public bool DeleteAllSpecialOrders()
        {
            return lck.DeleteAllSpecialOrders();
        }

        public bool DeleteSpecialOrderBatch(int Batch_ID)
        {
            return lck.DeleteSpecialOrderBatch(Batch_ID);
        }

        public bool DeleteBatch(int Batch_ID)
        {
            return lck.DeleteBatch(Batch_ID);
        }

        public bool DeleteAllBatches()
        {
            return lck.DeleteAllBatches();
        }

        public bool DeleteFlavor(int Flavor_ID)
        {
            return lck.DeleteFlavor(Flavor_ID);
        }

        public bool DeleteAllFlavors()
        {
            return lck.DeleteAllFlavors();
        }

        public bool DeleteCake(int Cake_ID)
        {
            return lck.DeleteCake(Cake_ID);
        }

        public bool DeleteStore(int Store_ID)
        {
            return lck.DeleteStore(Store_ID);
        }

        public bool DeleteNewOrder(int NewOrder_ID)
        {
            return lck.DeleteNewOrder(NewOrder_ID);
        }
        #endregion

        #region File Transfer
        public RemoteFileInfo DownloadFileFromServer(DownloadRequest request)
        {
            RemoteFileInfo result = new RemoteFileInfo();
            try
            {
                string filePath = System.IO.Path.Combine(scansFolder, request.FileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);

                // check if exists
                if (!fileInfo.Exists)
                    throw new System.IO.FileNotFoundException("File not found", request.FileName);

                // open stream
                //using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                //{
                System.IO.FileStream stream = new System.IO.FileStream(filePath,
                          System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // return result 
                result.FileName = request.FileName;
                result.Length = fileInfo.Length;
                result.FileByteStream = stream;
                //}
            }
            catch (Exception ex)
            {
                Log("DownloadFileFromServer(DownloadRequest) - Error msg:" + ex.Message);
                return null;
            }
            return result;
        }

        public void UploadFileToServer(RemoteFileInfo request)
        {
            try
            {
                FileStream targetStream = null;
                Stream sourceStream = request.FileByteStream;

                string filePath = Path.Combine(scansFolder, request.FileName);

                using (targetStream = new FileStream(filePath, FileMode.Create,
                                      FileAccess.Write, FileShare.None))
                {
                    //read from the input stream in 65000 byte chunks

                    const int bufferLen = 65000;
                    byte[] buffer = new byte[bufferLen];
                    int count = 0;
                    while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0)
                    {
                        // save to output stream
                        targetStream.Write(buffer, 0, count);
                    }
                    targetStream.Close();
                    sourceStream.Close();
                }
                //return true;
            }
            catch (Exception ex)
            {
                Log("UploadFileToServer(RemoteFileInfo) - Error msg:" + ex.Message);
                //return false;
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
            Logger.Log(msg, "LCK_ServiceLibrary.LCK_Service", logType);
        }

    }
}
