using Jarvis.Shared.StatusEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateWOOfflineRequestModel
    {
        public List<UpdateWOOfflineRequestmodel> Workorders { get; set; }
        public List<AddWOCategoryMappingOfflineRequestModel> WO_category { get; set; }  // insert 
        public List<AddWOCategoryTaskMappingOfflineRequestModel> WOCategoryTaskMapping { get; set; }
        public List<AddAssetFormioOfflineRequestModel> AssetFormio { get; set; }
        public List<DeleteAssetImages> delete_asset_images { get; set; }
        public List<DeleteOBAssetImages> delete_OB_asset_images { get; set; }
        public List<AddAssetImages>? asset_images { get; set; }
        public List<AddOBAssetImages>? OB_asset_images { get; set; }
        public List<OBWOAssetDetailsOfflineRequestmodel> OBWOAssetDetails { get; set; }
        public List<OBAssetFedByMapping>? OBWOFedByAssetMapping { get; set; }
        public List<IRWOImageLabelMapping>? IRWOImageLabelMappingList { get; set; }
        public List<AddFormIOBuildings>? form_io_buildings { get; set; }
        public List<AddFormIOFloors>? form_io_floors { get; set; }
        public List<AddFormIORooms>? form_io_rooms { get; set; }
        public List<UpdateAssetPMOffline>? asset_pms { get; set; }
        public List<UpdateAssetIssueOffline>? asset_issue { get; set; }
        public List<UpdateAssetIssueImageMappingOffline>? asset_issue_image_mapping { get; set; }
        public List<WolineToplevelAssetOffline> woline_toplevel_asset_mapping { get; set; }
        public List<WolineSublevelAssetOffline> woline_sublevel_asset_mapping { get; set; }
        public List<AssetToplevelAssetOffline> asset_toplevel_asset_mapping { get; set; }
        public List<AssetSublevelAssetOffline> asset_sublevel_asset_mapping { get; set; }
        public List<UpdatePMSubmittedDataOffline> pm_submitted_form_data { get; set; }
        public List<TempLocationBuildingOffline> temp_formio_building { get; set; }
        public List<TempLocationFloorOffline> temp_formio_floor { get; set; }
        public List<TempLocationRoomOffline> temp_formio_room { get; set; }
        public List<TempLocationWolineMappingOffline> temp_location_woline_mapping { get; set; }
        public List<TempActiveAssetPMWOlineMappingOffline> temp_active_assetpm_woline_mapping { get; set; } 
        public List<TempAssetPMsMappingOffline> temp_assetpm_mapping { get; set; }
        public List<UpdateTempAssetOffline> temp_asset_woline_mapping { get; set; }
        public List<TempIssueUpdateOffline> woline_temp_issue_mapping { get; set; }
        public List<TempIssueImageMappingUpdtaeoffline> woline_temp_issue_image_mapping { get; set; }

        public Guid trackmobilesyncoffline_id { get; set; } // to track sync request progress.
        public bool is_lambda_enable { get; set; } = false; // if false then execute request in BE , if true then execute request to lambda

        public List<TempMasterBuildingMappingOffline> temp_master_building { get; set; }
        public List<TempMasterFloorMappingOffline> temp_master_floor { get; set; }
        public List<TempMasterRoomMappingOffline> temp_master_room { get; set; }
        public List<TempMasterBuildingWOMappingOffline> temp_master_building_wo_mapping { get; set; }
        public List<TempMasterFloorWOMappingOffline> temp_master_floor_wo_mapping { get; set; }
        public List<TempMasterRoomWOMappingOffline> temp_master_room_wo_mapping { get; set; }

        public List<SitewalkthroughTempPmEstimationOfflineData>? sitewalkthrough_temp_pm_estimation{ get; set; }
    }
    public class TempIssueImageMappingUpdtaeoffline
    {
        public Guid woline_issue_image_mapping_id { get; set; }
        public Guid wo_line_issue_id { get; set; }
        public Guid site_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public bool is_deleted { get; set; }
        public Guid? irwoimagelabelmapping_id { get; set; }
    }
    public class TempIssueUpdateOffline
    {
        public Guid wo_line_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? issue_caused_id { get; set; }// Osha , NEC , Thermal etc
        public Guid? asset_form_id { get; set; } // woline in which issue is attached

        public Guid? woonboardingassets_id { get; set; }  // woline in which issue is attached it means repair/replace/general resolution

        public Guid? original_asset_form_id { get; set; } // origin inspection id from which temp issue is created 
        public Guid? original_woonboardingassets_id { get; set; } // origin woline id from which temp issue is created and this will also act as main temp asset for which issue is created
        public Guid? original_wo_id { get; set; } // origin wo id from which temp issue is created 
        public Guid? original_asset_id { get; set; } // origin asset id from which temp issue is created 
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string atmw_first_comment { get; set; } // AT and MWO Issue description for first comment in Asset Issue
        public string field_note { get; set; }
        public string back_office_note { get; set; }
        public bool is_main_issue_created { get; set; } // this will be true if main issue is created in AssetIssue table after completing WO t
        public bool is_issue_linked_for_fix { get; set; }// this will be true if issue is added to fix in WO line so create issue and resolve at same time
        public string pm_issue_identity_key { get; set; } // this key is to identify PM issue based on json key
        public Guid? site_id { get; set; }
        public Guid? wo_id { get; set; }
        public string form_retrived_asset_name { get; set; } // AT WO asset name to show in temp issue 
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public string thermal_anomaly_sub_componant { get; set; }
        public string thermal_anomaly_measured_amps { get; set; }
        public string thermal_anomaly_refrence_temps { get; set; }
        public string thermal_anomaly_measured_temps { get; set; }
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public string thermal_anomaly_location { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; }
        public int? thermal_anomaly_recommendation { get; set; }
        public int? thermal_classification_id { get; set; }
        public int? nec_violation { get; set; }
        public int? osha_violation { get; set; }
        public int? nfpa_70b_violation { get; set; }
        public string dynamic_field_json { get; set; }
        public int? type_of_ultrasonic_anamoly { get; set; }
        public string location_of_ultrasonic_anamoly { get; set; }
        public string size_of_ultrasonic_anamoly { get; set; }
        public string thermal_anomaly_problem_description { get; set; }
        public string thermal_anomaly_corrective_action { get; set; }
        public int? thermal_anomaly_severity_criteria { get; set; } // 1=similar, 2=ambient, 3=indirect
    }
    public class UpdateTempAssetOffline
    {
        public Guid tempasset_id { get; set; }
        public string asset_name { get; set; }
        public string QR_code { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public DateTime? commisiion_date { get; set; }
        public string form_nameplate_info { get; set; }
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? code_compliance { get; set; } // compliant  , non-compliant 
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Guid? asset_id { get; set; }// if WOline is added from main asset
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_formiosection_id { get; set; }
        public Guid wo_id { get; set; }
        public Guid site_id { get; set; }
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }
        public Guid? temp_master_room_id { get; set; }
        public string temp_master_section { get; set; }
    }
    public class UpdatePMSubmittedDataOffline
    {
        public Guid active_asset_pm_woline_mapping_id { get; set; }
        public bool is_active { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public Guid asset_pm_id { get; set; }
        public string pm_form_output_data { get; set; }
        public bool is_deleted { get; set; }

    }


    public class UpdateAssetIssueImageMappingOffline
    {
        public Guid asset_issue_image_mapping_id { get; set; }
        public Guid asset_issue_id { get; set; }
        public Guid site_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public bool is_deleted { get; set; }

    }
    public class UpdateAssetIssueOffline
    {
        public Guid asset_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public Guid? asset_id { get; set; }//asset id to map this Issue
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public Guid? asset_form_id { get; set; } // if issue is generated from inspection tab
        public Guid? woonboardingassets_id { get; set; }  // if issue is generated from OB/IR WOline
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string resolve_issue_reason { get; set; }
        public bool is_issue_linked { get; set; }
        public Guid? site_id { get; set; }
        public Guid? wo_id { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public bool is_deleted { get; set; }
        public int? priority { get; set; }
    }

    public class UpdateAssetPMOffline
    {
        public Guid asset_pm_id { get; set; }
        public string title { get; set; }

        public string description { get; set; }
        public int status { get; set; }
        public Guid? asset_form_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid? wo_id { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
        public bool is_archive { get; set; }
        public string pm_form_output_data { get; set; }
        public Nullable<DateTime> datetime_starting_at { get; set; }
        public bool is_pm_inspection_manual { get; set; }

    }
    public class AddFormIOBuildings
    {
        public int formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
    }
    public class AddFormIOFloors
    {
        public int formiofloor_id { get; set; }
        public string formio_floor_name { get; set; }
        public int? formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
    }
    public class AddFormIORooms
    {
        public int formioroom_id { get; set; }
        public string formio_room_name { get; set; }
        public int? formiofloor_id { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_building_name { get; set; }
    }
    public class AddWOCategoryMappingOfflineRequestModel
    {
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public Guid wo_id { get; set; }
        public Guid form_id { get; set; }
        public Guid? task_id { get; set; }
        public Guid? technician_user_id { get; set; }
        public Guid? asset_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? created_by { get; set; }
        public Guid? updated_by { get; set; }
        public bool is_archived { get; set; }
        public int status_id { get; set; }
        public string group_string { get; set; }
        // public List<AddWOCategoryTaskMappingOfflineRequestModel> WOCategoryTaskMapping { get; set; }
    }
    public class AddWOCategoryTaskMappingOfflineRequestModel
    {
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public Guid? task_id { get; set; }
        public Guid? wo_id { get; set; }
        public bool is_parent_task { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? created_by { get; set; }
        public Guid? updated_by { get; set; }
        public bool is_archived { get; set; }
        public bool is_newly_asset_created { get; set; }
        public string task_rejected_notes { get; set; }
        public int serial_number { get; set; }
        public Guid? assigned_asset { get; set; }
        public Guid? newly_created_asset_id { get; set; }
        public int? inspection_type { get; set; }
        // public AddAssetFormioOfflineRequestModel AssetFormio { get; set; }
    }
    public class AddAssetFormioOfflineRequestModel
    {
        public Guid asset_form_id { get; set; }
        public Guid? asset_id { get; set; }
        public Guid? site_id { get; set; }
        public Nullable<Guid> form_id { get; set; }
        public string asset_form_name { get; set; }
        public string asset_form_type { get; set; }
        public string asset_form_description { get; set; }
        public string asset_form_data { get; set; }
        public Nullable<Guid> requested_by { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public string accepted_by { get; set; }
        public int status { get; set; }
        public Guid? wo_id { get; set; }
        public string form_retrived_asset_name { get; set; }
        public string form_retrived_asset_id { get; set; }
        public string form_retrived_location { get; set; }
        public string form_retrived_data { get; set; }
        public DateTime? intial_form_filled_date { get; set; } // first time form field date 
        public string form_retrived_nameplate_info { get; set; }
        public string form_retrived_workOrderType { get; set; }
        public int? inspection_verdict { get; set; }
        public Guid? WOcategorytoTaskMapping_id { get; set; }
    }

    public class OBWOAssetDetailsOfflineRequestmodel
    {
        public Guid woonboardingassets_id { get; set; }
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
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public string task_rejected_notes { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public DateTime? commisiion_date { get; set; }
        public int? thermal_classification_id { get; set; }
        public Guid wo_id { get; set; }
        public int status { get; set; }
        public Guid site_id { get; set; }

        #region new_flow_of_Maintenenace_WO_details

        public int? inspection_type { get; set; }
        public DateTime? mwo_date { get; set; }
        public int? mwo_inspection_type_status { get; set; }
        public string problem_description { get; set; }
        public string solution_description { get; set; }
        public string inspection_further_details { get; set; }
        public string comments { get; set; }
        public int? repair_resolution { get; set; }
        public int? replacement_resolution { get; set; }
        public int? recommended_action { get; set; }
        public int? recommended_action_schedule { get; set; }
       
        public Guid? asset_id { get; set; }
        public Guid? technician_user_id { get; set; }
        public Guid? replaced_asset_id { get; set; }

        #endregion new_flow_of_Maintenenace_WO_details


        #region IR_scan_WO_new_Fields

        public string voltage { get; set; }
        public string rated_amps { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public int? location { get; set; }
        public string thermal_anomaly_sub_componant { get; set; }
        public string thermal_anomaly_measured_amps { get; set; }
        public string thermal_anomaly_refrence_temps { get; set; }
        public string thermal_anomaly_measured_temps { get; set; }
        public string thermal_anomaly_location { get; set; }
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; }
        public int? thermal_anomaly_recommendation { get; set; }
        public int? nec_violation { get; set; }
        public int? osha_violation { get; set; }
        public bool flag_issue_thermal_anamoly_detected { get; set; }
        public bool flag_issue_nec_violation { get; set; }
        public bool flag_issue_osha_violation { get; set; }
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? code_compliance { get; set; } // compliant  , non-compliant 
        #endregion IR_scan_WO_new_Fields
        public int? general_issue_resolution { get; set; }
        public bool is_wo_line_for_exisiting_asset { get; set; }
        public string other_notes { get; set; }
        public string form_nameplate_info { get; set; }
        public bool is_woline_from_other_inspection { get; set; }// if install woline is from issue/PM then this will be true and do not show this in wo datails screen
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public Guid? tempasset_id { get; set; }
        public Guid? issues_temp_asset_id { get; set; } // woline id of issue or PM i.e.  if issue or PM is from temp woline then give temp woline's Id here
        public int? issue_priority { get; set; } // attched issue priority
        public string dynmic_fields_json { get; set; }// we will use this key for dynamic new fields in woline 
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public string issue_title { get; set; } // attched issue title

        public DateTime? inspected_at { get; set; }
        public DateTime? initial_inspected_at { get; set; } // this key is will only add at the time of first inspection after that this date wont be update.
        public Guid? initial_inspected_by { get; set; }// this key is will only add at the time of first inspection after that this key wont be update.
        public bool is_nec_violation_resolved { get; set; }
        public bool is_osha_violation_resolved { get; set; }
        public bool is_thermal_anomaly_resolved { get; set; }
        public bool is_request_from_new_issue_flow { get; set; }// if request is from new issue flow then do not create issue from old flow which was from woline check box 
   
    }
    public class DeleteAssetImages
    {
        public Guid asset_profile_images_id { get; set; }
    }
    public class AddAssetImages
    {
        public Guid asset_profile_images_id { get; set; }
        public Guid asset_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }
    }
    public class AddOBAssetImages
    {
        public Guid woonboardingassetsimagesmapping_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }// 1 for profile and 2 for nameplate images
        public Guid woonboardingassets_id { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
    }
    public class DeleteOBAssetImages
    {
        public Guid woonboardingassetsimagesmapping_id { get; set; }
    }
    public class UpdateWOOfflineRequestmodel
    {
        public Guid wo_id { get; set; }
        public int wo_status_id { get; set; }
    }
    public class OBAssetFedByMapping
    {
        public Guid? wo_ob_asset_fed_by_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid? site_id { get; set; }
        public Guid parent_asset_id { get; set; }
        public bool is_parent_from_ob_wo { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public bool is_deleted { get; set; }
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public string length { get; set; }   // length of conductor
        public string style { get; set; }   // size of conductor
        public int? number_of_conductor { get; set; }    // # of conductor
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent
        public bool is_fed_by_via_subcomponant_asset_from_ob_wo { get; set; }
        public Guid? via_subcomponant_asset_id { get; set; } //this will act as main OCP / current asset
        public bool is_via_subcomponant_asset_from_ob_wo { get; set; }


    }
    public class IRWOImageLabelMapping
    {
        public Guid irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string visual_image_label { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public string s3_image_folder_name { get; set; }// job order id
        public bool is_deleted { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid? site_id { get; set; }
    }

    public class SitewalkthroughTempPmEstimationOfflineData
    {
        public Guid sitewalkthrough_temp_pm_estimation_id { get; set; }
        public Guid tempasset_id { get; set; }
        public Guid pm_plan_id { get; set; }
        public Guid pm_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public int? estimation_time { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }

        public bool is_deleted { get; set; }
    }

}
