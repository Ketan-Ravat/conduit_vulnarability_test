using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class RootRequestObjSingle
    {
        public string to { get; set; }

        public string priority { get; set; }

        public RequestData data { get; set; }

        public Notifications notification { get; set; }
    }
}
