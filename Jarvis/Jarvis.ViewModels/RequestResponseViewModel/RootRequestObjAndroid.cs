using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class RootRequestObjAndroid
    {
        public string to { get; set; }

        public string collapse_key { get; set; }

        public Notifications notification { get; set; }

        public Data data { get; set; }
    }

    public class Data
    {
        public string body { get; set; }

        public string title { get; set; }

        public string key_1 { get; set; }

        public string key_2 { get; set; }
    }
}
