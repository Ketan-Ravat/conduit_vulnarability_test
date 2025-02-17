using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class EditAssetDetailsRequestmodel
    {
        public Guid? asset_id { get; set; }
        public int? component_level_type_id { get; set; } = 1;
        public string parent_asset_internal_id { get; set; }
        public string asset_name { get; set; }
        public int? status { get; set; }
        public int? criticality_index { get; set; }
        public double? condition_index { get; set; }
        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; }
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? thermal_classification_id { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string form_retrived_nameplate_info { get; set; }

        public int formiobuilding_id { get; set; } = 0;
        public int formiofloor_id { get; set; } = 0;
        public int formioroom_id { get; set; } = 0;
        public string section { get; set; }
        public DateTime? commisiion_date { get; set; }
        public DateTime? visual_insepction_last_performed { get; set; }
        public DateTime? mechanical_insepction_last_performed { get; set; }
        public DateTime? electrical_insepction_last_performed { get; set; }
        public DateTime? infrared_insepction_last_performed { get; set; }
        public DateTime? arc_flash_study_last_performed { get; set; }
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? asset_placement { get; set; } // indooe,outdoor
        public string QR_code { get; set; }
        public string Location { get; set; }
        public int? code_compliance { get; set; }
        public int? asset_expected_usefull_life { get; set; }
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No , 3 = Missing
        public Guid? asset_group_id { get; set; }
        public int x_axis { get; set; }
        public int y_axis { get; set; }
        public string asset_node_data_json { get; set; }

        public List<EditAssetParentsMapping> asset_parent_mapping_list { get; set; }
        public List<asset_subcomponents_mapping_list_class> asset_subcomponents_mapping_list { get; set; }
        public EditAssetToplevel asset_toplevel_componenent { get; set; }
        public List<AssetsProfileImageListRequest> asset_profile_images { get; set; }

    }

    public class asset_subcomponents_mapping_list_class
    {
        public Guid? asset_sublevelcomponent_mapping_id { get; set; }
        public Guid? asset_id { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public bool is_deleted { get; set; }
        public string circuit { get; set; }
        public string sublevelcomponent_asset_name { get; set; }
        public Guid? sublevelcomponent_asset_id { get; set; }
        public List<subcomponentasset_image_list_class>? subcomponentasset_image_list { get; set; }
    }

    public class EditAssetToplevel
    {
        public Guid? asset_toplevelcomponent_mapping_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public string toplevelcomponent_asset_name { get; set; }

    }
    public class EditAssetParentsMapping
    {
        public Guid? asset_parent_hierrachy_id { get; set; }
        public Guid? parent_asset_id { get; set; }
        public bool is_deleted { get; set; }
        public string via_subcomponent_asset_name { get; set; }
        public string via_subcomponant_asset_class_code { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }        //this will act as  Main-OCP / current asset
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent this will act as OCP 
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public string length { get; set; }  // length of conductor
        public string style { get; set; }   // size of conductor
        public int? number_of_conductor { get; set; }
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
    }
    public class AssetsProfileImageListRequest
    {
        public int asset_photo_type { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public bool is_deleted { get; set; }
        public Guid? asset_profile_images_id { get; set; }
        public string file_url { get; set; }
        public string thumbnail_file_url { get; set; }
        public string image_extracted_json { get; set; }
    }
    public class subcomponentasset_image_list_class
    {
        public Guid? asset_profile_images_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public string asset_photo_url { get; set; }
        public int asset_photo_type { get; set; }
        public bool is_deleted { get; set; }
    }
}
