using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jarvis.Shared
{
    public class Logger
    {
        public static void Log(string message, string reqid = "")
        {
            try
            {
                //string status = ConfigurationManager.AppSettings["logstatus"];
                //if (status == "on")
                //{
                //string path = @"D:\\Temp\Example.txt";
                String filename = DateTime.Now.ToString("dd-MM-yyyy") + ".log";
                string path = AppDomain.CurrentDomain.BaseDirectory + "var/log/jarvis_backend-" + filename;

                //create Folder if not exists
                string folderpath = AppDomain.CurrentDomain.BaseDirectory + "var/log";
                bool exists = Directory.Exists(folderpath);

                if (!exists)
                {
                    Directory.CreateDirectory(folderpath);
                }

                //Write to file
                StreamWriter w = File.AppendText(path);
                w.WriteLine(string.Concat(reqid + " : ", DateTime.Now.ToString() + " " + message));
                w.Close();
                //}
            }
            catch (Exception e)
            {

            }
        }

        public static void Log(object p)
        {
            throw new NotImplementedException();
        }
    }
}
