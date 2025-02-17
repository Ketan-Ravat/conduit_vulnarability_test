using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.ExcludePropertiesfromDBHelper
{
    public class AssetPMListExcludeProperties
    {
        public Guid asset_pm_id { get; set; }
        public string title { get; set; }
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public Guid asset_pm_plan_id { get; set; }
        public string asset_plan_name { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_type { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public string due_in { get; set; }
        public Nullable<int> pm_due_overdue_flag { get; set; }
        public DateTime? due_date { get; set; }
        public string facility_name { get; set; }
        public bool is_current_assetpm { get; set; } = false;
        public Nullable<int> estimation_time { get; set; } // number of minutes 
    }
}
