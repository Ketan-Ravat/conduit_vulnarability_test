using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DashboardPropertiescountsResponseModel
    {
        public int Asset_count { get; set; }
        public int test_report_count { get; set; }
        public int maintainance_request_count { get; set; }
        public int annual_maintainance_schedule_count { get; set; }
        public int inspection_ready_for_review_count { get; set; }
        public int inspection_completed_count { get; set; }
        public int open_asset_issue_count { get; set; }
        public int overdue_asset_pm_count { get; set; }


    }
}
