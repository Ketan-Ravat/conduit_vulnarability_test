using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOOBAssetsbyLocationHierarchyResponseModel
    {
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }
        public int status { get; set; }
        public Guid wo_id { get; set; }
        public string status_name { get; set; }
        public string QR_code { get; set; }
        public string temp_building { get; set; }
        public string temp_floor { get; set; }
        public string temp_room { get; set; }
        public string temp_section { get; set; }
        public int component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public Guid? toplevelcomponent_asset_id { get; set; }
        public Guid? asset_id { get; set; }
        public string form_nameplate_info { get; set; }
        public int? arc_flash_label_valid { get; set; }
        public int? maintenance_index_type { get; set; }
        public int temp_issues_count { get; set; }
        public string temp_master_building { get; set; }
        public string temp_master_floor { get; set; }
        public string temp_master_room { get; set; }
        public string temp_master_section { get; set; }
        public string asset_profile_image { get; set; }
        //public Guid? temp_master_building_id { get; set; }
        //public Guid? temp_master_floor_id { get; set; }
        //public Guid? temp_master_room_id { get; set; }
    }
}
