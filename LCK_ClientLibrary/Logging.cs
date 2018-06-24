using System.Web;
using System.IO;
using System.Configuration;
using System;

namespace LCK_ClientLibrary
{
    /// <summary>
    /// Summary description for Logger.
    /// </summary>
    public class Logger
    {
 
        private static System.IO.StreamWriter _Output = null;
        private static Logger _Logger = null;
        private static Object _classLock = typeof(Logger);
        public static string _LogFileDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string _LogFile 
        {
            get 
            {
                string rtn = "DebugLog";
                if(FilenameAppend_Day)
                    rtn += @"-" + DateTime.Now.Day.ToString();
                if (FilenameAppend_Month)
                    rtn += @"-" + DateTime.Now.Month.ToString();
                if (FilenameAppend_Year)
                    rtn += @"-" + DateTime.Now.Year.ToString();
                if(FilenameAppend_Custom != "")
                    rtn += @"-" + FilenameAppend_Custom;

                return _LogFileDirectory + @"/" + rtn + @".log"; // "//DebugLog.log";
            }
        }
            
        public static bool _LogEnabled = false;
        public static LogTypes _LogThreshold = LogTypes.Debug;

        // variables that can alter output filename
        public static bool FilenameAppend_Day = false;
        public static bool FilenameAppend_Month = false;
        public static bool FilenameAppend_Year = false;
        public static string FilenameAppend_Custom = "";

        public enum LogTypes
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            Fatal = 4
        }
 
        private Logger()
        {
                 
        }
 
        public static Logger getInstance()
        {
            //lock object to make it thread safe
            lock(_classLock)
            {
                if(_Logger==null)
                {
                    _Logger = new Logger();
                 
                }   
            }
            return _Logger;
        }
 
        public static void Log(string s, string objectName,LogTypes severity = LogTypes.Debug)
        {
            if (!_LogEnabled) { return; }
            if (severity < _LogThreshold) { return; }
            try
            {
                lock (_classLock)
                {
                    if (_Output == null)
                        _Output = new System.IO.StreamWriter(_LogFile, true, System.Text.UnicodeEncoding.Default);

                    _Output.WriteLine(System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToString("hh:mm:ss.fff tt") +
                                            "[" + severity.ToString() + "]" + " [" + objectName + "] - " + s, new object[0]);

                    if (_Output != null)
                    {
                        _Output.Close();
                        _Output = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, new object[0]);
            }
        }

        public static void LogSeperator()
        {
            if (!_LogEnabled) { return; }
            try
            {
                lock (_classLock)
                {
                    if (_Output == null)
                    {
                        _Output = new System.IO.StreamWriter(_LogFile, true, System.Text.UnicodeEncoding.Default);
                    }

                    _Output.WriteLine("----------------------------------------------------------------------------------------------------------");

                    if (_Output != null)
                    {
                        _Output.Close();
                        _Output = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, new object[0]);
            }
        }
         
        public static void closeLog()
        {
            try
            {
                lock (_classLock)
                {
                    if (_Output != null)
                    {
                        _Output.Close();
                        _Output = null;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message,new object[0]);
            }
        }
    }
}