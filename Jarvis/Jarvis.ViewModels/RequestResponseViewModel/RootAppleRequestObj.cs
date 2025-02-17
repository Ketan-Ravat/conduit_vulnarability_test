using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class RootAppleRequestObj
    {
        public List<string> registration_ids { get; set; }
        public string priority { get; set; }
        public RequestData notification { get; set; }
        public RootAppleRequestObj()
        {
            priority = "high";
        }
    }
}
