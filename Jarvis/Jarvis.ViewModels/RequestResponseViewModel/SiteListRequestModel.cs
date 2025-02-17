using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class SiteListRequestModel
    {
        public Guid client_company_id { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public DateTime? from_date { get; set; }  // used for calender events
        public DateTime? to_date { get; set; }    // used for calender events
        public string search_string { get; set; } // used for search using string  
    }
}
