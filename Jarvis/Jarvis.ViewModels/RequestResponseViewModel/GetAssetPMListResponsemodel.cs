using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMListResponsemodel
    {
        public Guid asset_pm_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public Nullable<Guid> pm_id { get; set; }
        public string title { get; set; }

        public string description { get; set; }
        public string facility_name { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public Guid asset_pm_plan_id { get; set; }
        public string asset_plan_name { get; set; }
        public string frequency { get; set; } // frequency number like 10 months,1 year , etc
        public string due_in { get; set; } // like 3 months , 1 month 1 week etc..
        public int pm_timer_status { get; set; } // pm timer status = 1 - overdue , 2- due 
        public string asset_class_name { get; set; } 
        public string asset_class_code { get; set; }
        public string asset_class_type { get; set; }
        public DateTime? due_date { get; set; }
        public DateTime? pm_starting_date { get; set; }
        public bool is_assetpm_enabled { get; set; } // false = disabled , true = enable
        public Nullable<int> pm_due_overdue_flag { get; set; }
        public DateTime? over_due_date { get; set; }    
        public DateTime? last_completed_date { get; set; }
        public bool is_overdue { get; set; } = false;
        public bool is_current_assetpm { get; set; } = false;
        public string form_name { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string top_level_asset_name { get; set; }
        public string internal_asset_id { get; set; }
        public string manual_wo_number { get; set; }
        public int? criticality_index_type { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public Nullable<int> estimation_time { get; set; } // number of minutes 

        public int? work_procedure_type { get; set; }

        public Guid? form_id { get; set; }
    }
}
