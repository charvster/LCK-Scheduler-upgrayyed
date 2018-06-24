using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCK_ClientLibrary
{
    public static class Globals
    {
        public static List<FlavorWPF> AllFlavors = new List<FlavorWPF>();
        public static List<StoreWPF> AllStores = new List<StoreWPF>();
        public static List<CakeWPF> AllCakes = new List<CakeWPF>();
        public static List<FlavorWPF> AllCakeFlavors = new List<FlavorWPF>();
        public static StoreWPF SelectedStore = new StoreWPF();

        public static string ScanOrderFolder = @"";

        public static string DateTime_to_DayNumber(DateTime date)
        {
            int year = date.Year;
            int day = date.DayOfYear;
            return year + "_" + day;
        }

        public static DateTime DayNumber_to_DateTime(string day_number)
        {
            string[] pieces = day_number.Split('_');
            int days = int.Parse(pieces[1]) - 1; // dateTime days are zero based
            int year = int.Parse(pieces[0]);
            DateTime theDate = new DateTime(year, 1, 1).AddDays(days);
            return theDate;
        }

    }

    public static class ConfigSettings_Static
    {
        private static int _storeID = 1;
        public static int StoreID
        {
            get { return _storeID; }
            set { _storeID = value; }
        }
        private static bool _adminRights = false;
        public static bool AdminRights
        {
            get { return _adminRights; }
            set { _adminRights = value; }
        }
        private static string _scanOrderDefaultPath = "";
        public static string ScanOrderDefaultPath
        {
            get { return _scanOrderDefaultPath; }
            set { _scanOrderDefaultPath = value; }
        }
        private static UInt32 _refreshInterval = 10 * 60 * 1000; // min * sec * millisec => default 10minutes
        public static UInt32 RefreshInterval
        {
            get { return ConfigSettings_Static._refreshInterval; }
            set { ConfigSettings_Static._refreshInterval = value; }
        }
        public static string ScanTempFolder = AppDomain.CurrentDomain.BaseDirectory + "temp";
        public static string EndpointAddress = @"endpoint_localhost";
    }

}
