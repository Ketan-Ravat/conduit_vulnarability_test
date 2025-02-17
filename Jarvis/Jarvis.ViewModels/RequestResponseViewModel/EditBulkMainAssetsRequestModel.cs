using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class EditBulkMainAssetsRequestModel
    {
        public List<EditBulkMainAssets_Class> assets_list { get; set; }
    }
    public class EditBulkMainAssets_Class
    {
        public Guid asset_id { get; set; }
        public string name { get; set; }
        public int? asset_placement { get; set; }
        public int? condition_index_type { get; set; }
        public string QR_code { get; set; }
        public int? criticality_index_type { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public DateTime? commisiion_date { get; set; }
        public int? maintenance_index_type { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public int status { get; set; }
    }
}
