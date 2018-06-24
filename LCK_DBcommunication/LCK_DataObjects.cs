using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LCK_DBcommunication.LCK_DataObjects
{
    [DataContract]
    public class Flavor
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool NotAFlavor { get; set; }
        [DataMember]
        public bool CakeFlavor { get; set; }
    }

    [DataContract]
    public class Cake
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string AbvName { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

    [DataContract]
    public class Cake_Batch
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int SO_ID { get; set; }
        [DataMember]
        public Cake Cake { get; set; }
        [DataMember]
        public Flavor Flavor { get; set; }
        [DataMember]
        public int Quantity { get; set; }
    }

    [DataContract]
    public class Batch
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Day_Number { get; set; }
        [DataMember]
        public Flavor Flavor { get; set; }
        [DataMember]
        public bool Requested { get; set; }
        [DataMember]
        public StoreInfo Store { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public int QuantityMini { get; set; }
    }

    [DataContract]
    public class SO_Batch : Batch
    {
        [DataMember]
        public int SO_ID { get; set; }
        [DataMember]
        public bool Is_Mini { get; set; }
        [DataMember]
        public string Special_Instructions { get; set; }
    }

    [DataContract]
    public class DailyFlavors
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Day_Number { get; set; }
        [DataMember]
        public Flavor Flavor { get; set; }
    }

    [DataContract]
    public class SpecialOrder
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Day_Number { get; set; }
        [DataMember]
        public StoreInfo Store { get; set; }
        [DataMember]
        public bool Deliver { get; set; }
        [DataMember]
        public bool Setup { get; set; }
        [DataMember]
        public string Customer_Name { get; set; }
        [DataMember]
        public string Customer_Email { get; set; }
        [DataMember]
        public string Customer_Phone { get; set; }
        [DataMember]
        public string Customer_Address { get; set; }
        [DataMember]
        public string ScanLink { get; set; }
        [DataMember]
        public int DueTime { get; set; }
        [DataMember]
        public string Notes { get; set; }
        [DataMember]
        public string SpecialInstructions { get; set; }
        [DataMember]
        public string CuttingCakeNotes { get; set; }
        [DataMember]
        public string DisplayNotes { get; set; }
        [DataMember]
        public List<SO_Batch> Batches { get; set; }
        [DataMember]
        public List<Cake_Batch> Cakes { get; set; }
        [DataMember]
        public DateTime LastModified { get; set; }
    }

    [DataContract]
    public class DayInfo
    {
        [DataMember]
        public string DayNumber { get; set; }
        [DataMember]
        public List<Batch> DailyBatches { get; set; }
        [DataMember]
        public List<SpecialOrder> Orders { get; set; }
    }

    public class DayPreview
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int SO_Qty { get; set; }
        [DataMember]
        public int DailyFlavorCount { get; set; }
    }

    [DataContract]
    public class StoreInfo
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Phone { get; set; }
    }

    [DataContract]
    public class NewOrder
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public SpecialOrder SO { get; set; }
        [DataMember]
        public StoreInfo Store_Origin { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public bool FulFilled { get; set; }
        [DataMember]
        public bool Editted { get; set; }
    }
}
