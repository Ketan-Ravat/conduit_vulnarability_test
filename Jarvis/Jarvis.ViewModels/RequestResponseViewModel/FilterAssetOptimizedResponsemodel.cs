using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAssetOptimizedResponsemodel
    {
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public int? condition_index_type { get; set; }
        public string criticality_index_type_name { get; set; }
        public int? criticality_index_type { get; set; }
        public string condition_index_type_name { get; set; }
        public int? code_compliance { get; set; }// compliant  , non-compliant 
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }
        public string room { get; set; }
        public Nullable<int> status { get; set; }
        public string status_name { get; set; }
        public int issue_count { get; set; }
        public string asset_profile_image { get; set; }
    }
}
