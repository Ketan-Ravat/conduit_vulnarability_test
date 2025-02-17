using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jarvis.Shared.Utility
{
    public class Logger
    {

        private static ILoggerFactory _Factory = null;
        private static ILogger _Ilogger;
        private static Logger _logger;
        private static readonly object _syncLock = new object();

        private Logger() { }

        public static Logger GetInstance<T>()
        {
            if (_logger == null)
            {
                lock (_syncLock)
                {
                    if (_logger == null)
                    {
                        _logger = new Logger();
                        _Ilogger = LoggerFactory.CreateLogger<T>();
                    }
                }
            }

            return _logger;
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        public void LogError(String message)
        {
            _Ilogger.LogError(message);
            WriteLog("", message);
        }

        public void LogDebug(String message)
        {
            _Ilogger.LogDebug(message);
            WriteLog("", message);
        }

        public void LogWarning(String message)
        {
            _Ilogger.LogWarning(message);
            WriteLog("", message);
        }

        public void LogInformation(String message)
        {
            _Ilogger.LogInformation(message);
            WriteLog("", message);
        }

        public void LogError(String id, String message)
        {
            _Ilogger.LogError(id, message);
            WriteLog(id, message);
        }

        public void LogDebug(String id, String message)
        {
            _Ilogger.LogDebug(id, message);
            WriteLog(id, message);
        }

        public void LogWarning(String id, String message)
        {
            _Ilogger.LogWarning(id, message);
            WriteLog(id, message);
        }

        public void LogInformation(String id, String message)
        {
            _Ilogger.LogInformation(id, message);
            WriteLog(id, message);
        }
        public void WriteLog(string id, String Message)
        {
            try
            {
                String filename = DateTime.Now.ToString("dd-MM-yyyy") + ".log";
                string path = AppDomain.CurrentDomain.BaseDirectory + "var/log/jarvis_backend-" + filename;
                //string path = AppDomain.CurrentDomain.BaseDirectory + "\\VSI_logfile\\" + filename;

                //create Folder if not exists
                //string folderpath = AppDomain.CurrentDomain.BaseDirectory + "\\VSI_logfile";
                string folderpath = AppDomain.CurrentDomain.BaseDirectory + "var/log";
                bool exists = Directory.Exists(folderpath);

                if (!exists)
                {
                    Directory.CreateDirectory(folderpath);
                }

                //Write to file
                StreamWriter w = File.AppendText(path);
                w.WriteLine(string.Concat(DateTime.Now.ToString() + ": " + id + " : " + Message));
                w.Close();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Exception in Write Log: " + e.Message);
            }
        }
    }
}
