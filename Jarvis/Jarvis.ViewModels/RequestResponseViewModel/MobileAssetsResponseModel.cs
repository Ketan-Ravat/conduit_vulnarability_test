using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileAssetsResponseModel
    {
        public string name { get; set; }
        public string levels { get; set; }
        public Guid asset_id { get; set; }
        public bool is_child_available { get; set; }
        public string internal_asset_id { get; set; }
        public Nullable<int> status { get; set; }
        public string site_name { get; set; }
        public double condition_index { get; set; }
        public int criticality_index { get; set; }
        public string asset_photo { get; set; }
        public string status_name { get; set; }
        public string inspection_verdict_name { get; set; }
        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }
        public string formio_section_name { get; set; }
        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; }
        public string condition_index_type_name { get; set; }
        public string criticality_index_type_name { get; set; }
        public List<AssetProfileImageList> asset_profile_images { get; set; }
        public List<AssetNameplateImageList> asset_nameplate_images { get; set; }
        public List<AssetIRScanImageList> asset_IR_scan_images { get; set; }
        public DateTime? commisiion_date { get; set; }

        public DateTime? visual_insepction_last_performed { get; set; }
        public DateTime? mechanical_insepction_last_performed { get; set; }
        public DateTime? electrical_insepction_last_performed { get; set; }
        public DateTime? infrared_insepction_last_performed { get; set; }
        public DateTime? arc_flash_study_last_performed { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
    }
}
