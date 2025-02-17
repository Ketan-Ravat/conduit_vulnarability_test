using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared.Helper.FormIORequestmodel
{
    public class LoginRequestmodel
    {
        public Data data { get; set; }
        public Metadata metadata { get; set; }
        public string state { get; set; }
        public string _vnote { get; set; }
    }
    public class Data
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool login { get; set; }
    }

    public class Metadata
    {
        public string timezone { get; set; }
        public int offset { get; set; }
        public string origin { get; set; }
        public string referrer { get; set; }
        public string browserName { get; set; }
        public string userAgent { get; set; }
        public string pathName { get; set; }
        public bool onLine { get; set; }
    }
}
