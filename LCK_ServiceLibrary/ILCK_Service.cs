using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using LCK_DBcommunication.LCK_DataObjects;

namespace LCK_ServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ILCK_Service
    {
        #region SELECT/GET methods
        [OperationContract]
        List<Flavor> GetAllFlavors();

        [OperationContract]
        List<Cake> GetAllCakes();

        [OperationContract]
        Batch GetBatch(int BatchID);

        [OperationContract]
        List<Batch> GetBatches(int StoreID, string DayNumber);

        [OperationContract]
        List<Batch> GetDailyFlavors(int StoreID, string DayNumber);

        [OperationContract]
        StoreInfo GetStoreInfoByID(int StoreID);

        [OperationContract]
        StoreInfo GetStoreInfo(string Name);

        [OperationContract]
        SpecialOrder GetSpecialOrder(int SO_ID);

        [OperationContract]
        int GetLastSpecialOrderID();

        [OperationContract]
        List<SpecialOrder> GetSpecialOrders(int StoreID,string DayNumber);

        [OperationContract]
        DayInfo GetDayInfo(int StoreID, string DayNumber);

        [OperationContract]
        DayInfo GetDayInfoAll(string DayNumber);

        [OperationContract]
        List<StoreInfo> GetAllStoresInfo();

        [OperationContract]
        List<NewOrder> GetNewOrders();
        #endregion

        #region INSERT/Add methods
        [OperationContract]
        bool AddBatch(Batch bat);
        
        [OperationContract]
        bool AddFlavor(Flavor flav);

        [OperationContract]
        bool AddCake(Cake cke);

        [OperationContract]
        bool AddSpecialOrder(SpecialOrder SO);

        [OperationContract]
        bool AddSpecialOrderBatch(int SO_ID, SO_Batch batch);

        [OperationContract]
        bool AddSpecialOrderCakeBatch(int SO_ID, Cake_Batch cakeBatch);

        [OperationContract]
        bool AddStore(StoreInfo Store);

        [OperationContract]
        bool AddNewOrder(NewOrder newOrder);
        #endregion

        #region UPDATE methods
        [OperationContract]
        bool UpdateSpecialOrder(int SO_ID, SpecialOrder updatedSO);

        [OperationContract]
        bool UpdateSpecialOrderBatch(int SO_ID, SO_Batch updatedBatch);

        [OperationContract]
        bool UpdateBatch(int Batch_ID, Batch updatedBatch);

        [OperationContract]
        bool UpdateFlavor(int Flavor_ID, Flavor updatedFlavor);

        [OperationContract]
        bool UpdateCake(int Cake_ID, Cake updateCake);

        [OperationContract]
        bool UpdateStore(int Store_ID, StoreInfo updatedStore);

        [OperationContract]
        bool UpdateNewOrder(int NewOrder_ID, NewOrder updatedNewOrder);
        #endregion

        #region DELETE methods
        [OperationContract]
        bool DeleteSpecialOrder(int SO_ID);

        [OperationContract]
        bool DeleteAllSpecialOrders();

        [OperationContract]
        bool DeleteSpecialOrderBatch(int Batch_ID);

        [OperationContract]
        bool DeleteBatch(int Batch_ID);

        [OperationContract]
        bool DeleteAllBatches();

        [OperationContract]
        bool DeleteFlavor(int Flavor_ID);

        [OperationContract]
        bool DeleteAllFlavors();

        [OperationContract]
        bool DeleteCake(int Cake_ID);

        [OperationContract]
        bool DeleteStore(int Store_ID);

        [OperationContract]
        bool DeleteNewOrder(int NewOrder_ID);
        #endregion

        #region File Transfer
        [OperationContract]
        RemoteFileInfo DownloadFileFromServer(DownloadRequest request);

        [OperationContract]
        void UploadFileToServer(RemoteFileInfo request);
        #endregion
    }

    [MessageContract]
    public class DownloadRequest
    {
        [MessageBodyMember]
        public string FileName;
    }

    [MessageContract]
    public class RemoteFileInfo : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageHeader(MustUnderstand = true)]
        public long Length;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileByteStream;

        public void Dispose()
        { 
            if (FileByteStream != null)
            {
                FileByteStream.Close();
                FileByteStream = null;
            }
        }   
    }

}
