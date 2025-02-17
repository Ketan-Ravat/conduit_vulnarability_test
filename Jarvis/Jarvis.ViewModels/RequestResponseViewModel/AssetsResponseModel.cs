using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetsResponseModel
    {
        public int response_status { get; set; }
        public string message { get; set; }

        [ScaffoldColumn(false)]
        public int role { get; set; }
        public string QR_code { get; set; }
        public Guid asset_id { get; set; }

        public string asset_photo { get; set; }

        public string internal_asset_id { get; set; }

        public Nullable<int> status { get; set; }

        public string status_name { get; set; }

        public Guid inspectionform_id { get; set; }
        //public string asset_profile_image_url { get; set; } // this profile is from AssetPhotos list based on priority conditions 
        public string asset_profile_image { get; set; } // this profile is from DB 
        public string notes { get; set; }

        public Nullable<int> usage { get; set; }

        public Nullable<long> meter_hours { get; set; }

        public string name { get; set; }

        public string asset_type { get; set; }

        public string product_name { get; set; }

        public string model_name { get; set; }

        public string asset_serial_number { get; set; }

        public string model_year { get; set; }

        public string site_location { get; set; }
        public string Location { get; set; }

        public string current_stage { get; set; }

        public string parent { get; set; }
        public Guid? parent_asset_id { get; set; }

        public string children { get; set; }
        public Guid? client_company_id { get; set; }
        public string client_company_name { get; set; }
        public Guid site_id { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }

        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }

        public string timeelapsed { get; set; }

        public int condition_state { get; set; }

        public string levels { get; set; }

        public Guid form_id { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }
        public int issue_count { get; set; }

        public double condition_index { get; set; }
        public string asset_pm_form_json { get; set; } // structure: {"pm_json":[{"pm_title":"mechanicalservicing","name":"torqueDetails","json":""}]}
        public int criticality_index { get; set; }
        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; }
        public string criticality_index_type_name { get; set; }
        public string condition_index_type_name { get; set; }
        public int? thermal_classification_id { get; set; }
        public string thermal_classification_name { get; set; }
        public string asset_barcode_id { get; set; }

        public List<AssetsValueJsonObjectViewModel> lastinspection_attribute_values { get; set; }

        //public virtual SitesViewModel Sites { get; set; }

        public virtual AssetInspectionFormResponseModel InspectionForms { get; set; }

        public int inspectionlistsize { get; set; }

        public int issuelistsize { get; set; }
        public int openIssuesCount { get; set; }

        public string timezone { get; set; }

        public virtual List<AssetInspectionViewModel> Inspections { get; set; }

        public virtual List<IssueResponseModel> Issues { get; set; }
        public virtual List<IssueResponseModel> WorkOrders { get; set; }

        //public virtual ICollection<AssetTransactionHistoryViewModel> AssetTransactionHistory { get; set; }
        public int assetPMCount { get; set; }

        public string client_internal_id { get; set; }

        public virtual List<AssetsResponseModel> nodes { get; set; }

        public bool is_child_available { get; set; }
        public bool isAutoApprove { get; set; }
        public int? inspection_verdict { get; set; }
        public string inspection_verdict_name { get; set; }

        public AssetLocationHierarchy AssetLocationHierarchy { get; set; }
        public string form_retrived_nameplate_info { get; set; }

        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }
        public string formio_section_name { get; set; }
        public string formio_location_notes { get; set; }
        public bool is_LVCB_Asset { get; set; }
        public DateTime? commisiion_date { get; set; }
        public List<AssetProfileImageList> asset_profile_images { get; set; }
        public List<AssetNameplateImageList> asset_nameplate_images { get; set; }
        public List<AssetIRScanImageList> asset_IR_scan_images { get; set; }
        public DateTime? visual_insepction_last_performed { get; set; }
        public DateTime? mechanical_insepction_last_performed { get; set; }
        public DateTime? electrical_insepction_last_performed { get; set; }
        public DateTime? infrared_insepction_last_performed { get; set; }
        public DateTime? arc_flash_study_last_performed { get; set; }
        public string visual_insepction_due_in { get; set; }
        public string mechanical_insepction_due_in { get; set; }
        public string electrical_insepction_due_in { get; set; }
        public string infrared_insepction_due_in { get; set; }
        public string arc_flash_study_insepction_due_in { get; set; }

        public DateTime? mwo_date { get; set; }
        public int? mwo_inspection_type_status { get; set; } // Open,Inprogress,Done
        public string problem_description { get; set; }
        public string solution_description { get; set; }
        public string inspection_further_details { get; set; }
        public string comments { get; set; }
        public int? repair_resolution { get; set; }
        public int? replacement_resolution { get; set; }
        public int? recommended_action { get; set; }
        public int? recommended_action_schedule { get; set; }
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? asset_placement { get; set; } // indooe,outdoor
        public int? code_compliance { get; set; }// compliant  , non-compliant 
        public Guid? replaced_asset_id { get; set; }
        public string replaced_asset_name { get; set; }
        public int? asset_expected_usefull_life { get; set; }
        public int component_level_type_id { get; set; }
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public string asset_profile { get; set; }
        public Guid? asset_group_id { get; set; }
        public List<Asset_OBIRImage_labels> ob_ir_Image_label_list { get; set; }
        public List<AssetParentsMapping> asset_parent_mapping_list { get; set; }
        public AssetToplevelComponenent asset_toplevel_componenent { get; set; }
        public List<asset_subcomponents_mapping_listview_class> asset_subcomponents_mapping_list { get; set; }
    }
    public class asset_subcomponents_mapping_listview_class
    {
        public Guid? asset_sublevelcomponent_mapping_id { get; set; }
        public Guid? asset_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public bool is_deleted { get; set; }
        public string circuit { get; set; }
        public string sublevelcomponent_asset_name { get; set; }
        public Guid? sublevelcomponent_asset_id { get; set; }
        public Guid? sublevelcomponent_asset_class_id { get; set; }

        public List<subcomponentasset_image_list_class>? subcomponentasset_image_list { get; set; }
    }
    public class AssetToplevelComponenent
    {
        public Guid asset_toplevelcomponent_mapping_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public string toplevelcomponent_asset_name { get; set; }
    }
    public class AssetParentsMapping
    {
        public Guid asset_parent_hierrachy_id { get; set; }
        public Guid? parent_asset_id { get; set; }
        public string parent_asset_name { get; set; }
        public bool is_deleted { get; set; }
        public string length { get; set; }
        public string style { get; set; }
        public int? fed_by_usage_type_id { get; set; }
        public int? number_of_conductor { get; set; }
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }        //this will act as  Main-OCP / current asset
        public Guid? fed_by_via_subcomponant_asset_id { get; set; }// fed by subcomponent this will act as OCP 
        public string via_subcomponent_asset_name { get; set; }
        public string fed_by_via_subcomponant_asset_name { get; set; }
    }
    public class Asset_OBIRImage_labels
    {
        public Guid? irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string ir_image_label_url { get; set; }
        public string visual_image_label { get; set; }
        public string visual_image_label_url { get; set; }
        public Guid? asset_id { get; set; }
        public bool is_deleted { get; set; }
        public string s3_image_folder_name { get; set; }
    }
    public class AssetProfileImageList
    {
        public Guid asset_profile_images_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }
        public string image_extracted_json { get; set; }
    }
    public class AssetNameplateImageList
    {
        public Guid asset_profile_images_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }
        public string image_extracted_json { get; set; }
        public string image_actual_json { get; set; }
    }
    public class AssetIRScanImageList
    {
        public Guid asset_profile_images_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }
    }
    public class AssetLocationHierarchy
    {
        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }
        public string formio_section_name { get; set; }
        public string formio_location_notes { get; set; }
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
        public int? formiosection_id { get; set; }

    }
}
