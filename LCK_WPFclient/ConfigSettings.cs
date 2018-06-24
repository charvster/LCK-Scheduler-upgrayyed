using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LCK_ClientLibrary;

namespace LCK_WPFclient
{
    //public static class ConfigSettings_Static
    //{
    //    private static int _storeID = 1;
    //    public static int StoreID
    //    {
    //        get { return _storeID; }
    //        set { _storeID = value; }
    //    }
    //    private static bool _adminRights = false;
    //    public static bool AdminRights
    //    {
    //        get { return _adminRights; }
    //        set { _adminRights = value; }
    //    }
    //    private static string _scanOrderDefaultPath = "";
    //    public static string ScanOrderDefaultPath
    //    {
    //        get { return _scanOrderDefaultPath; }
    //        set { _scanOrderDefaultPath = value; }
    //    }
    //    private static UInt32 _refreshInterval = 10 * 60 * 1000; // min * sec * millisec => default 10minutes
    //    public static UInt32 RefreshInterval
    //    {
    //        get { return ConfigSettings_Static._refreshInterval; }
    //        set { ConfigSettings_Static._refreshInterval = value; }
    //    }
    //    public static string ScanTempFolder = AppDomain.CurrentDomain.BaseDirectory + "temp";
    //    public static string EndpointAddress = @"endpoint_localhost";
    //}

    /// <summary>
    /// ConfigSettings serialization object
    /// </summary>
    public class ConfigSettings 
    {
        public int StoreID
        {
            get { return ConfigSettings_Static.StoreID; }
            set { ConfigSettings_Static.StoreID = value; }
        }

        public bool AdminRights
        {
            get { return ConfigSettings_Static.AdminRights; }
            set { ConfigSettings_Static.AdminRights = value; }
        }

        public string ScanOrderDefaultPath
        {
            get { return ConfigSettings_Static.ScanOrderDefaultPath; }
            set { ConfigSettings_Static.ScanOrderDefaultPath = value; }
        }

        public UInt32 RefreshInterval
        {
            get { return ConfigSettings_Static.RefreshInterval; }
            set { ConfigSettings_Static.RefreshInterval = value; }
        }

        public string EndpointAddress
        {
            get { return ConfigSettings_Static.EndpointAddress; }
            set { ConfigSettings_Static.EndpointAddress = value; }
        }
    }
}
