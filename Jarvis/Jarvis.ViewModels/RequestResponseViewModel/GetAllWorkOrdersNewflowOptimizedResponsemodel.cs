using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllWorkOrdersNewflowOptimizedResponsemodel
    {
        public int wo_type { get; set; }
        public string manual_wo_number { get; set; }
        public string wo_type_name { get; set; }
        public string site_name { get; set; }
        public Guid site_id { get; set; }
        public string client_company_name { get; set; }
        public DateTime start_date { get; set; }
        public status_wise_asset_count_obj status_wise_asset_count_obj { get; set; }
        public int wo_status_id { get; set; }
        public string due_in { get; set; }
        public int? wo_due_overdue_flag { get; set; }
        public DateTime? due_date { get; set; }
        public List<watcher_users_list> watcher_users_list { get; set; }
        public Guid wo_id { get; set; }
        public bool is_watcher { get; set; }
        public int? quote_status_id { get; set; } // accepted , rejected, Defered
        public string quote_status_name { get; set; } // accepted , rejected, Defered
    }
}
