using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class ExecutivePMDueResponseModel {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        public List<PMDueReportEmailResponse> pmItems { get; set; }
        public List<ExecutiveSiteWiseReport> siteWiseReport { get; set; }
    }

    public class ExecutiveSiteWiseReport {
        public string site_name { get; set; }
        public int total_pm_items_overdue { get; set; }
        public string time_elapsed { get; set; }
    }

    public class PMDueReportEmailResponse {
        public Guid pm_trigger_id { get; set; }
        public string pm_title { get; set; }
        public string internal_asset_id { get; set; }
        public string asset_name { get; set; }
        public string pm_plan_name { get; set; }
        public string site_name { get; set; }
        public string time_elapsed { get; set; }
        public string status { get; set; }
        public string meters_at_due { get; set; }
        public string current_meters { get; set; }
        public string Total_meters_run_after_PM_overdue { get; set; }
        public string service_dealer_name { get; set; }
        public string service_dealer_email { get; set; }
        public Guid service_dealer_id { get; set; }
        public bool sent_first_notifcation { get; set; }
        public bool sent_second_notifcation { get; set; }
        public bool sent_pm_due_notifcation { get; set; }
        //public string due_in { get; set; }
        //public int total_est_time_hours { get; set; }
        //public int total_est_time_minutes { get; set; }
    }
}