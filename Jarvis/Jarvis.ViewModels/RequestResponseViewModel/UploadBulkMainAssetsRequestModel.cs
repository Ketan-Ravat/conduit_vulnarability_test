using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UploadBulkMainAssetsRequestModel
    {
        public List<UploadBulkMainAsset_Data> asset_data { get; set; }
        public List<assets_fedby_mappings_class>? assets_fedby_mappings { get; set; }
        public List<asset_subcomponents_mappings_class>? asset_subcomponents_mappings { get; set; }
    }
    public class UploadBulkMainAsset_Data
    {
        public Guid? asset_id { get; set; }
        public string asset_name { get; set; }
        public int component_level_type_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string back_office_note { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string QR_code { get; set; }
        public string field_note { get; set; }
        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; }
        public int? thermal_classification_id { get; set; }
        public int? asset_placement { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public int? status { get; set; } 

        //public string voltage { get; set; }
        //public string rated_amps { get; set; }
        //public string manufacturer { get; set; }
        //public string model { get; set; }
    }
}
