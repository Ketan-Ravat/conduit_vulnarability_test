using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpcomingSiteOpportunitiesDashboardResponseModel
    {
        public int active_assets_count { get; set; } 
        public int open_asset_issues_count { get; set; } 
        public int open_asset_pms_count { get; set; } 
        public int overdue_asset_pms_count { get; set; } 
        public int scheduled_asset_pms_count { get; set; }
        public List<class_code_due_month_wise_asset_pms> graph1 { get; set; }
        public List<class_code_due_month_wise_asset_pms> graph2 { get; set; }
        public List<class_code_due_month_wise_asset_pms> graph3 { get; set; }
    }
    public class class_code_due_month_wise_asset_pms
    {
        public string asset_class_code { get; set; }
        public int due_in_1 { get; set; }
        public int due_in_2 { get; set; }
        public int due_in_3 { get; set; }
        public int due_in_4 { get; set; }
        public int due_in_5 { get; set; }
        public int due_in_6 { get; set; }
        public int due_in_7 { get; set; }
        public int due_in_8 { get; set; }
        public int due_in_9{ get; set; }
        public int due_in_10 { get; set; }
        public int due_in_11 { get; set; }
        public int due_in_12 { get; set; }
    }
}
