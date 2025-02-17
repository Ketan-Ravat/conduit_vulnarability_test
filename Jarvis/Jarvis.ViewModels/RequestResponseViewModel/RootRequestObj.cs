using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class RootRequestObj
    {
        public List<string> registration_ids { get; set; }

        public string priority { get; set; }

        public RequestData data { get; set; }

        public Notifications notification { get; set; }

    }

    public class Notifications
    {
        public string body { get; set; }

        public string title { get; set; }

        public string click_action { get; set; }

        public string target_role { get; set; }
    }
}
