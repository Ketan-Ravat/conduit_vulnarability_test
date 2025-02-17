using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterWorkOrderIssueRequestModel
    {
        public int pageindex { get; set; }
        public int pagesize { get; set; }
        public string searchstring { get; set; }
        public string asset_id { get; set; }
        public string mr_id { get; set; }
    }
}
