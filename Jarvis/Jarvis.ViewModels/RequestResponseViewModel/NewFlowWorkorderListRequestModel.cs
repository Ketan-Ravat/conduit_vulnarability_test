using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class NewFlowWorkorderListRequestModel
    {
        public int pagesize { get; set; } 
        public int pageindex { get; set; } 
        public Guid? technician_user_id { get; set; }
        public List<int>? wo_status { get; set; }
        public List<int>? quote_status { get; set; }
        public DateTime? from_date { get; set; }  // used for calender events
        public DateTime? to_date { get; set; }// used for calender events
        public string search_string { get; set; }
        public List<int>? wo_type { get; set; }
        public List<string>? site_id { get; set; }
        public bool is_request_for_backlogcards { get; set; } = false;
        public bool is_requested_from_workorders_tab { get; set; } = true;
    }
}
