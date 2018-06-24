using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCK_WCFsysTrayHost
{
    public static class Globals
    {
        public static Uri httpBaseAddress = new Uri("http://localhost:6790/lck");
        public static string mexAddr = "http://localhost:6790/lck/mex";
        public static string LocalIP;
        public static string LocalPort = "6790";
    }
}
