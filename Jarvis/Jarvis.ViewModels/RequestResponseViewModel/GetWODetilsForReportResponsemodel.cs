using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWODetilsForReportResponsemodel
    {
        public string manual_wo_number { get; set; }
        public string site_name { get; set; }
        public string company_name { get; set; }
        public string client_company_name { get; set; }
        public string status { get; set; }
        public DateTime start_date { get; set; }
        public DateTime due_date { get; set; }
        public DateTime? modified_at { get; set; }
        public string technician_users { get; set; }
        public int open_obwo_asset { get; set; }
        public int inprogress_obwo_asset { get; set; }
        public int completed_obwo_asset { get; set; }
        public int hold_obwo_asset { get; set; }
        public int ready_for_review_obwo_asset { get; set; }
        public int recheck_obwo_asset { get; set; }
        public int new_issue_open_count { get; set; }
        public int new_issue_resolved_count { get; set; }

    }
}
