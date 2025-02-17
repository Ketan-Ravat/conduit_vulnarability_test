using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class NewFlowWorkorderListResponseModel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public string manual_wo_number { get; set; }
        public int wo_type { get; set; }
        public string wo_type_name { get; set; }
        public DateTime? due_date { get; set; }
        public int? wo_due_overdue_flag { get; set; }
        public string due_in { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public int wo_status_id { get; set; }
        public string wo_status { get; set; }
        public Nullable<DateTime> completed_date { get; set; }
        public string description { get; set; }
        public string timezone { get; set; }
        public string client_company_name { get; set; }
        public Guid client_company_id { get; set; }
        public bool is_archive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string ir_wo_pdf_report { get; set; }
        public int? ir_wo_pdf_report_status { get; set; }
        public string po_number { get; set; }
        public Guid? responsible_party_id { get; set; }
        public int? quote_status_id { get; set; } // accepted , rejected, Defered
        public string quote_status_name { get; set; } // accepted , rejected, Defered
        public status_wise_asset_count_obj status_wise_asset_count_obj { get; set; }
        public bool is_watcher { get; set; }
        
        public List<watcher_users_list>? watcher_users_list { get; set; }
        
        //public string technician_name { get; set; }
        //public Guid? technician_id { get; set; }
        public List<WorkOrderAttachments_data> WorkOrderAttachments_list { get; set; }
    }
    public class watcher_users_list
    {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }

    public class status_wise_asset_count_obj
    {
        public int open_obwo_asset { get; set; }
        public int inprogress_obwo_asset { get; set; }
        public int completed_obwo_asset { get; set; }
        public int submitted_obwo_asset { get; set; }
        public int hold_obwo_asset { get; set; }
        public int ready_for_review_obwo_asset { get; set; }
        public int recheck_obwo_asset { get; set; }
        public int total_count { get; set; }
    }

    public class WorkOrderAttachments_data
    {
        public Guid wo_attachment_id { get; set; }
        public string user_uploaded_name { get; set; }

        public string filename { get; set; }
    }

}
