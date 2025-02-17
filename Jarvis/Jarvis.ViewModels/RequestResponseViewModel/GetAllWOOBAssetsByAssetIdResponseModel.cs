using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllWOOBAssetsByAssetIdResponseModel
    {
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public int status { get; set; }
        public Guid wo_id { get; set; }
        public int wo_type { get; set; }
        public string manual_wo_number { get; set; }
        public Guid? asset_pm_id { get; set; }
        public Guid? pm_id { get; set; }
        public string asset_pm_title { get; set; }
        public Nullable<int> pm_inspection_type_id { get; set; } // 1 = Infrared ThermoGraphy
        public int? component_level_type_id { get; set; }
        public int? inspection_type { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public string QR_code { get; set; }

        //public string temp_building { get; set; }
        //public string temp_floor { get; set; }
        //public string temp_room { get; set; }
        //public string temp_section { get; set; }
        //public Guid? toplevelcomponent_asset_id { get; set; }
    }
}
