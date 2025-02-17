using System;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ClientCompanyListRequestModel
    {
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public DateTime? from_date { get; set; }  // used for calender events
        public DateTime? to_date { get; set; }    // used for calender events
        public string search_string { get; set; } // used for search using string       
    }
}
