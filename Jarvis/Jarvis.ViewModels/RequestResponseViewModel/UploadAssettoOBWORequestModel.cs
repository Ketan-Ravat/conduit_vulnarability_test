using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UploadAssettoOBWORequestModel
    {
       public List<uploadassetONWOdata> Asset_data { get; set; }
       public List<assets_fedby_mappings_class>? assets_fedby_mappings { get; set; }
       public List<asset_subcomponents_mappings_class>? asset_subcomponents_mappings { get; set; }
    }
    public class asset_subcomponents_mappings_class
    {
        public string toplevel_asset_name { get; set; }
        public string subcomponent_asset_name { get; set; }
        public string subcomponent_asset_class_code { get; set; }
        public Guid? subcomponents_woonboardingassets_id { get; set; }// for BE use only
    }
    public class assets_fedby_mappings_class
    {
        public string asset_name { get; set; }
        public string ocp_asset_name { get; set; }
        public string fedby_asset_name { get; set; }
        public string fedby_ocp_asset_name { get; set; }
        public string length { get; set; }// length of conductor
        public string style { get; set; }// size of conductor
        public int? number_of_conductor { get; set; }    // # of conductor
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
        public int? fed_by_usage_type_id { get; set; }    // 1 = Normal , 2 = Emergency
    }
    public class uploadassetONWOdata
    {
        public Guid? asset_id { get; set; }
        public string asset_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string back_office_note { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string QR_code { get; set; }
        public string field_note { get; set; }
        public string voltage { get; set; }
        public string rated_amps { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public Guid wo_id { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public DateTime? commisiion_date { get; set; }
        public int? thermal_classification_id { get; set; }
        public int? dynmic_fields_json { get; set; }
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
    }
}
