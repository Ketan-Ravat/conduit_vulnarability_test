using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class PMLastCompletedDateReportResponsemodel
    {
        public string asset_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public Guid asset_id { get; set; }
        public string pm_title { get; set; }
        public string pm_plan_title { get; set; }
        public Guid asset_pm_id { get; set; }
        public DateTime? pm_last_completed_date { get; set; }
        public DateTime? due_date { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string pm_interval_frequency { get; set; } // frequency number like 10 months,1 year , etc
        public string top_level_asset_name { get; set; }
        public string internal_asset_id { get; set; }
        public bool is_assetpm_enabled { get; set; } // true = disabled , false = enable
    }
}
