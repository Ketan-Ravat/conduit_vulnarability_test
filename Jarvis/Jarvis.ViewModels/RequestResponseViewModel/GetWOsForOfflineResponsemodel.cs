using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOsForOfflineResponsemodel
    {
        public List<NewFlowWorkorderListResponseModel> workorders { get; set; }
        public List<WorkOrderAttachmentsResponseModel> WorkOrderAttachments { get; set; }
        public List<GetAllHierarchyAssetsResponseModel> asset_hierarchy { get; set; }
        public List<AssetsResponseModel> asset_list { get; set; }
        public List<MobileAssetParentsMapping> asset_parent_mapping_list { get; set; }
        public List<mobile_asset_class_form_io_mapping> form_io_asset_class_list { get; set; }
        
        public List<AssetProfileImageList> asset_profile_image_list { get; set; }
        public List<AssetIRVisualImageMappingList> asset_ir_visual_image_list { get; set; }
        public List<form_categoty_list> form_category_list { get; set; }
        public List<GetWOcategoryTaskByCategoryIDListResponsemodel> wo_category_task_list { get; set; }
        public List<GetFormByWOTaskIDResponsemodel> wo_task_forms_list{get; set;}
        public List<FormIOResponseModel> formio_master_forms {get; set;}

        public List<TaskResponseModel> MasterTasks { get; set; }
        public List<MobileAssetClassResponsemodel> MasterAssetClass { get; set; }
        public List<MobileAssetClassFormMappingResponsemodel> AssetClassFormMapping { get; set; }
        public List<MobileOBWOAssetListResponsemodel> OBWOAssetList { get; set; }
        public List<WOLineBuildingMappingOffline> WOLineBuildingMappingOffline { get; set; }  // new 
        public List<mobile_fed_by_asset_mapping> OBWOAssetFedByMapping { get; set; }  // new 
        public List<MobileOBWOAssetImagesResponsemodel> OBWOAssetImagesList { get; set; }
        public List<wo_ob_asset_image_label> OBWOAssetImagesLabelMapping { get; set; }
        public List<MobileFormIOMasterBuildings> formio_master_buildings { get; set; }
        public List<MobileFormIOMasterFloors> formio_master_floors { get; set; }
        public List<MobileFormIOMasterRooms> formio_master_rooms { get; set; }
        public List<MobileFormIOMasterSection> formio_master_section { get; set; }
        public List<AssetPmsOffline> asset_pms_mapping { get; set; }
        public List<AssetPMTriggerConditionmappingoffline> asset_pms_trigger_condition_mapping { get; set; }
        public List<AssetIssueListOffline> asset_issue_mapping { get; set; }
        public List<AssetIssueImageMapping> asset_issue_image_mapping { get; set; }
        public List<AssetWOlineIssueListOffline> asset_woline_issue_mapping { get; set; }
        public List<AssetAttachmentsOffline> asset_attachment_mapping { get; set; }
        public List<WolineToplevelAssetOffline> woline_toplevel_asset_mapping { get; set; }
        public List<WolineSublevelAssetOffline> woline_sublevel_asset_mapping { get; set; }
        public List<AssetToplevelAssetOffline> asset_toplevel_asset_mapping { get; set; }
        public List<AssetSublevelAssetOffline> asset_sublevel_asset_mapping { get; set; }
        public List<FormEquipmentsOffline> formio_master_equipments { get; set; }
        public List<GetPMMasterFormsOffline> pm_master_forms { get; set; }
        public List<PMPlanMasterOfffline> pm_plan_master_list { get; set; }
        public List<PMCategoryMasterOffline> pm_category_master_list { get; set; }
        public List<PMmasterOffline> pm_masters_list { get; set; }
        public List<GetPMSubmittedDataOffline> pm_submitted_form_data { get; set; }
        public List<TempLocationBuildingOffline> temp_formio_building { get; set; }
        public List<TempLocationFloorOffline> temp_formio_floor { get; set; }
        public List<TempLocationRoomOffline> temp_formio_room { get; set; }
        public List<TempLocationSectionOffline> temp_formio_section { get; set; }
        public List<TempLocationWolineMappingOffline> temp_location_woline_mapping { get; set; }

        public List<TempMasterBuildingMappingOffline> temp_master_building { get; set; }
        public List<TempMasterFloorMappingOffline> temp_master_floor { get; set; }
        public List<TempMasterRoomMappingOffline> temp_master_room { get; set; }
        public List<TempMasterBuildingWOMappingOffline> temp_master_building_wo_mapping { get; set; }
        public List<TempMasterFloorWOMappingOffline> temp_master_floor_wo_mapping { get; set; }
        public List<TempMasterRoomWOMappingOffline> temp_master_room_wo_mapping { get; set; }

        public List<TempActiveAssetPMWOlineMappingOffline> temp_active_assetpm_woline_mapping { get; set; } 
        public List<TempAssetPMsMappingOffline> temp_assetpm_mapping { get; set; }
        public List<TempAssetWOlineMappingOffline> temp_assetwoline_mapping { get; set; }
        public List<asset_woline_issue_images> asset_woline_issue_images_mapping { get; set; }
        public List<workorder_technician_mapping_class> workorder_technician_mapping { get; set; }

        public List<SitewalkthroughTempPmEstimationOffline>? sitewalkthrough_temp_pm_estimation {  get; set; }

        public bool force_to_reset { get; set; }
        public bool force_to_reset_master_forms { get; set; }

    }
    public class workorder_technician_mapping_class
    {
        public Guid wo_technician_mapping_id { get; set; }
        public Guid wo_id { get; set; }
        public Guid user_id { get; set; }
        public Guid site_id { get; set; }
        public bool is_deleted { get; set; }
    }

    public class asset_woline_issue_images
    {
        public Guid woline_issue_image_mapping_id { get; set; }
        public Guid wo_line_issue_id { get; set; }
        public Guid site_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Guid? irwoimagelabelmapping_id { get; set; }
    }
    public class TempAssetWOlineMappingOffline
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

        public Guid? new_created_asset_id { get; set; } // update main asset_id to this coloumn when complete WO 
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_formiosection_id { get; set; }
        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }
        public Guid? temp_master_room_id { get; set; }
        public string temp_master_section { get; set; }
        public Guid wo_id { get; set; }
        public Guid site_id { get; set; }
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
    }
    public class TempActiveAssetPMWOlineMappingOffline
    {
        public Guid temp_active_asset_pm_woline_mapping_id { get; set; }

        public Guid woonboardingassets_id { get; set; } // this is for PM woline
        public Guid temp_asset_pm_id { get; set; } // this is temp asset pm
        public string pm_form_output_data { get; set; }
        public bool is_deleted { get; set; }
        public bool is_active { get; set; }

    }
    public class TempAssetPMsMappingOffline
    {
        public Guid temp_asset_pm_id { get; set; }
        public Guid woonboardingassets_id { get; set; } // this will act as Main Asset fro this table
        public Nullable<Guid> pm_id { get; set; }
        public int status { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
        public bool is_archive { get; set; }
    }

    public class TempMasterBuildingMappingOffline
    {
        public Guid temp_master_building_id { get; set; }
        public string temp_master_building_name { get; set; }
        public Guid site_id { get; set; }
        public int? formiobuilding_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
    }
    public class TempMasterFloorMappingOffline
    {
        public Guid temp_master_floor_id { get; set; }
        public string temp_master_floor_name { get; set; }
        public Guid temp_master_building_id { get; set; }
        public Guid site_id { get; set; }
        public int? formiofloor_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
    }
    public class TempMasterRoomMappingOffline
    {
        public Guid temp_master_room_id { get; set; }
        public string temp_master_room_name { get; set; }
        public Guid temp_master_floor_id { get; set; }
        public Guid site_id { get; set; }
        public int? formioroom_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class TempMasterBuildingWOMappingOffline
    {
        public Guid temp_master_building_wo_mapping_id { get; set; }
        public Guid temp_master_building_id { get; set; }
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
    }
    public class TempMasterFloorWOMappingOffline
    {
        public Guid temp_master_floor_wo_mapping_id { get; set; }
        public Guid temp_master_floor_id { get; set; }
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
    }
    public class TempMasterRoomWOMappingOffline
    {
        public Guid temp_master_room_wo_mapping_id { get; set; }
        public Guid temp_master_room_id { get; set; }
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class TempLocationWolineMappingOffline
    {
        public Guid wo_ob_asset_temp_formiobuilding_id { get; set; }
        public DateTime? created_at { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_formiosection_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
    }

    public class TempLocationBuildingOffline
    {
        public Guid temp_formiobuilding_id { get; set; }
        public string temp_formio_building_name { get; set; }
        public Guid site_id { get; set; }
        public Guid wo_id { get; set; }
        public int? formiobuilding_id { get; set; }
        public bool is_deleted { get; set; }
        public Guid company_id { get; set; }
        public DateTime? created_at { get; set; }

    }
    public class TempLocationFloorOffline
    {
        public Guid temp_formiofloor_id { get; set; }
        public string temp_formio_floor_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid wo_id { get; set; }
        public int? formiofloor_id { get; set; }
        public bool is_deleted { get; set; }
        public Guid company_id { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
    }
    public class TempLocationRoomOffline
    {
        public Guid temp_formioroom_id { get; set; }
        public string temp_formio_room_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid wo_id { get; set; }
        public int? formioroom_id { get; set; }

        public bool is_deleted { get; set; }

        public Guid company_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
    }
    public class TempLocationSectionOffline
    {
        public Guid temp_formiosection_id { get; set; }
        public string temp_formio_section_name { get; set; }
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public int? formiosection_id { get; set; }
    }
    public class AssetPMTriggerConditionmappingoffline
    {
        public Guid asset_pm_trigger_condition_mapping_id { get; set; }
        public Nullable<int> datetime_repeates_every { get; set; } // total years or month count to repeat
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
        public Guid asset_pm_id { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
        public int condition_type_id { get; set; } // 1 - condition-1 , 2 - condition-2 ,3 - condition-3
        public Guid site_id { get; set; }
        public bool is_archive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }

    public class PMmasterOffline
    {
        public Guid pm_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }
        public int status { get; set; }
        public Guid pm_plan_id { get; set; }
        public Nullable<int> estimation_time { get; set; }
        public int pm_trigger_type { get; set; } // Fixed One Time -25 , Recurring - 26 
        public int pm_trigger_by { get; set; } //Time - 26 , Meter Hours - 27 , Time / Meter Hours  - 28
        public Nullable<int> datetime_repeat_time_period_type { get; set; } //29 - Month, 30 - Year, 39 - Week, 40 - Day
        public bool is_archive { get; set; }
        public Nullable<int> pm_inspection_type_id { get; set; } // 1 = Infrared ThermoGraphy
    }

    public class PMPlanMasterOfffline
    {
        public Guid pm_plan_id { get; set; }
        public string plan_name { get; set; }
        public bool is_default_pm_plan { get; set; }
        public Nullable<int> status { get; set; }
        public Guid pm_category_id { get; set; }
    }
    public class PMCategoryMasterOffline
    {
        public Guid pm_category_id { get; set; }
        public string category_name { get; set; }

        public string category_code { get; set; }
        public Nullable<int> status { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }

    }

    public class GetPMSubmittedDataOffline
    {
        public Guid active_asset_pm_woline_mapping_id { get; set; }
        public bool is_active { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public Guid asset_pm_id { get; set; }
        public string pm_form_output_data { get; set; }
        public bool is_deleted { get; set; }
    }

    public class GetPMMasterFormsOffline
    {
        public Guid pmitemmasterform_id { get; set; }
        public Guid? company_id { get; set; }
        public string form_json { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string plan_name { get; set; }
        public string pm_title { get; set; }
        public bool is_deleted { get; set; }
    }

    public class FormEquipmentsOffline
    {
        public Guid equipment_id { get; set; }
        public string equipment_number { get; set; }
        public string equipment_name { get; set; }
        public string manufacturer { get; set; }
        public string model_number { get; set; }
        public string serial_number { get; set; }
        public int calibration_interval { get; set; }
        public DateTime calibration_date { get; set; }
        public int? calibration_status { get; set; }
        public bool isarchive { get; set; }
    }
    public class AssetToplevelAssetOffline
    {
        public Guid asset_toplevelcomponent_mapping_id { get; set; }
        public Guid asset_id { get; set; }
        public Guid? site_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
    }
    public class AssetSublevelAssetOffline
    {
        public Guid asset_sublevelcomponent_mapping_id { get; set; }
        public Guid asset_id { get; set; }
        public Guid? site_id { get; set; }
        public Guid sublevelcomponent_asset_id { get; set; }
        public string circuit { get; set; }
        public string image_name { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
    }
    public class WolineToplevelAssetOffline
    {
        public Guid woline_toplevelcomponent_mapping_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public Guid? site_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public bool is_toplevelcomponent_from_ob_wo { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }

    }
    public class WolineSublevelAssetOffline
    {
        public Guid woline_sublevelcomponent_mapping_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public Guid? site_id { get; set; }
        public Guid sublevelcomponent_asset_id { get; set; }
        public bool is_sublevelcomponent_from_ob_wo { get; set; }
        public string circuit { get; set; }
        public string image_name { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
    }

    public class AssetAttachmentsOffline
    {
        public Guid assetatachmentmapping_id { get; set; }
        public string file_name { get; set; }
        public string file_url { get; set; }
        public string user_uploaded_file_name { get; set; }
        public bool is_deleted { get; set; }
        public Guid asset_id { get; set; }
    }
    public class AssetIRVisualImageMappingList
    {
        public Guid assetirwoimageslabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string visual_image_label { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
        public string s3_image_folder_name { get; set; }
        public Guid? asset_id { get; set; }
    }
    public class AssetIssueImageMapping
    {
        public Guid asset_issue_image_mapping_id { get; set; }
        public Guid asset_issue_id { get; set; }
        public Guid site_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public bool is_deleted { get; set; }
        public Guid? irwoimagelabelmapping_id { get; set; }
    }
    public class AssetIssueListOffline
    {
        public Guid asset_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public Guid? asset_id { get; set; }//asset id to map this Issue
        public string asset_name { get; set; }//asset id to map this Issue
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public string issue_status_name { get; set; }//Open , Scheduled , In Progress , Resolved
        public Guid? asset_form_id { get; set; } // if issue is generated from inspection tab
        public Guid? woonboardingassets_id { get; set; }  // if issue is generated from OB/IR WOline
        public bool is_issue_linked { get; set; }
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public Guid? site_id { get; set; }
        public Nullable<DateTime> created_at { get; set; }

        public Guid? wo_id { get; set; }
        public bool is_deleted { get; set; }
        public int? priority { get; set; }// low , medium , high

    }
    public class AssetWOlineIssueListOffline
    {
        public Guid wo_line_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public string issue_status_name { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? issue_caused_id { get; set; }// Osha , NEC , Thermal etc
        public Guid? asset_form_id { get; set; } // if issue is generated from inspection tab
        public Guid? woonboardingassets_id { get; set; }  // if issue is generated from OB/IR WOline
        public Guid? original_asset_form_id { get; set; } // origin inspection id from which temp issue is created 
        public Guid? original_woonboardingassets_id { get; set; } // origin woline id from which temp issue is created and this will also act as main temp asset for which issue is created
        public Guid? original_wo_id { get; set; } // origin wo id from which temp issue is created 
        public Guid? original_asset_id { get; set; } // origin asset id from which temp issue is created 
        public string pm_issue_identity_key { get; set; } // this key is to identify PM issue based on json key
        public bool is_issue_linked_for_fix { get; set; }// this will be true if issue is added to fix in WO line so create issue and resolve at same time        public Guid? wo_id { get; set; }
        public Guid? wo_id { get; set; }
        public bool is_deleted { get; set; }
        public Guid? asset_id { get; set; }//asset id to map this temp Issue with asset when completing WO
        public string asset_name { get; set; }//asset id to map this temp Issue with asset when completing WO
        public string form_retrived_asset_name { get; set; } // AT WO asset name to show in temp issue 
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public Nullable<DateTime> created_at { get; set; }
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
    public class AssetPmsOffline
    {
        public Guid asset_pm_id { get; set; }
        public string title { get; set; }

        public string description { get; set; }
        public int status { get; set; }
        public Guid? asset_form_id { get; set; }
        public Guid? wo_id { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
        public bool is_archive { get; set; }
      //  public string pm_form_output_data { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid asset_id { get; set; }
        public Nullable<Guid> pm_id { get; set; }
        public Guid asset_pm_plan_id { get; set; }
        public int pm_trigger_by { get; set; }
        public Nullable<DateTime> datetime_starting_at { get; set; }
        public DateTime? asset_pm_completed_date { get; set; }
        public bool is_pm_inspection_manual { get; set; }


    }
    public class MobileFormIOMasterBuildings
    {
        public int formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
    }
    public class MobileFormIOMasterFloors
    {
        public int formiofloor_id { get; set; }
        public string formio_floor_name { get; set; }
        public int? formiobuilding_id { get; set; }
    }
    public class MobileFormIOMasterRooms
    {
        public int formioroom_id { get; set; }
        public string formio_room_name { get; set; }
        public int? formiofloor_id { get; set; }
    }
    public class MobileFormIOMasterSection
    {
        public int formiosection_id { get; set; }
        public string formio_section_name { get; set; }
        public int? formioroom_id { get; set; }
    }
    public class WOLineBuildingMappingOffline
    {
        public int wolinebuildings_id { get; set; }
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
        public int? formiosection_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
    }

    public class MobileAssetParentsMapping
    {
        public Guid asset_parent_hierrachy_id { get; set; }
        public Guid? asset_id { get; set; }
        public Guid? parent_asset_id { get; set; }
        public string parent_asset_name { get; set; }
        public bool is_deleted { get; set; }
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public string length { get; set; }  // length of conductor
        public string style { get; set; }   // size of conductor
        public int? number_of_conductor { get; set; }
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
        public Guid? via_subcomponent_asset_id { get; set; }        //this will act as  OCP / current asset
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent this will act as main OCP 
    }

    public class mobile_asset_class_form_io_mapping
    {
        public Guid asset_class_formio_mapping_id { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public Guid form_id { get; set; }
        public bool isarchive { get; set; }
    }
    public class mobile_fed_by_asset_mapping
    {
        public Guid wo_ob_asset_fed_by_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid parent_asset_id { get; set; }
        public string parent_asset_name { get; set; }
        public bool is_parent_from_ob_wo { get; set; }
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public bool is_deleted { get; set; }
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
    public class wo_ob_asset_image_label
    {
        public Guid irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string ir_image_label_url { get; set; }
        public string visual_image_label { get; set; }
        public string visual_image_label_url { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public string s3_image_folder_name { get; set; }
        public bool is_deleted { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid? site_id { get; set; }
    }

    public class SitewalkthroughTempPmEstimationOffline
    {
        public Guid sitewalkthrough_temp_pm_estimation_id { get; set; }
        public Guid tempasset_id { get; set; }
        public Guid pm_plan_id { get; set; }
        public string title { get; set; }
        public Guid pm_id { get; set; }
        public string pm_name { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public int? estimation_time { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public bool is_deleted { get; set; }
    }
}
