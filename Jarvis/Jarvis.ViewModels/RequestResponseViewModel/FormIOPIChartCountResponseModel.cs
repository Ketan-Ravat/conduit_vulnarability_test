using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FormIOPIChartCountResponseModel
    {
        public int total_assets_count { get; set; }
        public int good_condition_asset_count { get; set; }
        public int average_condition_asset_count { get; set; }
        //public int Serious_condition_asset_count { get; set; }
        public int poor_condition_asset_count { get; set; }
        public int poor_dusty_condition_asset_count { get; set; }
        public int zero_value_asset_count { get; set; }

        /// new values based on assets operating condition
        /// 
        public int Operating_Normally { get; set; }
        public int Repair_Needed { get; set; }
        public int Replacement_Needed { get; set; }
        public int Repair_Scheduled { get; set; }
        public int Replacement_Scheduled { get; set; }
        public int Decomissioned { get; set; }
        public int Spare { get; set; }
        public int Repair_Inprogress { get; set; }
        public int Replacement_Inprogress { get; set; }
        public int open_issue_count { get; set; }
        public int inprogress_issue_count { get; set; }
        public int schedule_issue_count { get; set; }
    }

}
