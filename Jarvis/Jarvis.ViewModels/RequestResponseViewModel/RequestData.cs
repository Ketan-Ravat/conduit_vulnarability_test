using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class RequestData
    {
        public string title { get; set; }
        public string body { get; set; }
        public int type { get; set; }
        public string ref_id { get; set; }
        public object custom { get; set; }
        public string sound { get; set; }
        public string target_role { get; set; }
    }
}
