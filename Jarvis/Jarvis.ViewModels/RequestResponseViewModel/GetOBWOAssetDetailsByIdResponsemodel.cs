using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetOBWOAssetDetailsByIdResponsemodel
    {
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public Guid? ob_existing_asset_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_type { get; set; }
        public int? asset_class_type_id { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string back_office_note { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string temp_master_building { get; set; }
        public string temp_master_floor { get; set; }
        public string temp_master_room { get; set; }
        public string temp_master_section { get; set; }
        public string temp_building { get; set; }
        public string temp_floor { get; set; }
        public string temp_room { get; set; }
        public string temp_section { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_formiosection_id { get; set; }
        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }
        public Guid? temp_master_room_id { get; set; }
        public string qr_code { get; set; }
        public string field_note { get; set; }
        public Guid wo_id { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public string condition_index_type_name { get; set; }
        public string criticality_index_type_name { get; set; }
        public DateTime? commisiion_date { get; set; }
        public int? thermal_classification_id { get; set; }
        public string thermal_classification_name { get; set; }
        public int? work_time_spend { get; set; } // number of seconds spend 

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
        public Guid? mwo_asset_id { get; set; }
        public string mwo_asset_name { get; set; }
        public bool is_wo_line_for_exisiting_asset { get; set; }
        public Guid? technician_user_id { get; set; }

        #endregion new_flow_of_Maintenenace_WO_details

        // IR WO new fields
        public Guid? fed_by { get; set; }
        public string fed_by_name { get; set; }
        public Guid? fed_by_ob_asset_id { get; set; }
        public string fed_by_ob_asset_name { get; set; }
        public bool is_fed_by_ob_asset { get; set; }
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
        public bool flag_issue_Thermal_Anamoly_Detected { get; set; }
        public bool flag_issue_NEC_Violation { get; set; }
        public bool flag_issue_OSHA_Violation { get; set; }
        // IR WO new fields ends
        public string other_notes { get; set; }
        public int? code_compliance { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public bool is_replaced_asset_id_is_main { get; set; }
        public Guid? replaced_asset_id { get; set; }
        public string replaced_asset_name { get; set; }
        public int? general_issue_resolution { get; set; }
        public string form_nameplate_info { get; set; }
        public string dynmic_fields_json { get; set; }
        public int formiobuilding_id { get; set; }
        public int formiofloor_id { get; set; }
        public int formioroom_id { get; set; }
        public Guid? asset_id { get; set; }
        public int? Component_level_type_id { get; set; }
        public Guid? temp_woonboardingassets_id { get; set; }// this is for temp woline/Asset to which repair/replace/general issue is made for.  
        public Guid? issues_temp_asset_id { get; set; }
        public string issues_temp_asset_name { get; set; }
        public bool is_nec_violation_resolved { get; set; }
        public bool is_osha_violation_resolved { get; set; }
        public bool is_thermal_anomaly_resolved { get; set; }
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public string issue_title { get; set; } // attched issue title
        public int? issue_priority { get; set; } // attched issue priority
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public Guid? asset_group_id { get; set; }
        public string asset_group_name { get; set; }
        public Guid? pm_plan_id { get; set; }
        public string plan_name { get; set; }
        public List<OBWOAssetImages> asset_image_list { get; set; }
        public List<View_OBIRImage_label> ob_ir_Image_label_list { get; set; }
        public List<WOOBAssetFedByMap> wo_ob_asset_fed_by_mapping { get; set; }
        public List<WOOBAssetToplevelAssetMapping> wo_ob_asset_toplevelcomponent_mapping { get; set; }
        public List<WOOBAssetSublevelAssetMapping> wo_ob_asset_sublevelcomponent_mapping { get; set; }
        public GetWOLinkedIssueResponsemodel linked_issues { get; set; } // this is for linked issue list for repair/replace/general resolution woline
        public List<woline_issue_response_obj> woline_issue_list { get; set; } // this is for repair/replace issue check mark in install woline details
        public TempAssetDetailsForWoline temp_asset_details { get; set; }
        public List<feeding_circuit_list_class> feeding_circuit_list { get; set; }

        public List<PMEstimationdata>? pm_estimation_list { get; set; }

    }
    public class feeding_circuit_list_class
    {
        public string children_asset_name { get; set; }
        public Guid? children_asset_id { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }
        public Guid? fed_by_via_subcomponant_asset_id { get; set; }
        public string via_subcomponent_asset_name { get; set; }
        public string fed_by_via_subcomponant_asset_name { get; set; }
        public string circuit { get; set; }
        public string amps { get; set; }
        public string nameplate_json { get; set; }
    }
    public class TempAssetDetailsForWoline
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
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public Guid? asset_group_id { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Guid? asset_id { get; set; }// if WOline is added from main asset
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public Guid temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_formiosection_id { get; set; }
        public string temp_formio_building_name { get; set; }
        public string temp_formio_floor_name { get; set; }
        public string temp_formio_room_name { get; set; }
        public string temp_formio_section_name { get; set; }
        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }
        public Guid? temp_master_room_id { get; set; }
        public string temp_master_building { get; set; }
        public string temp_master_floor { get; set; }
        public string temp_master_room { get; set; }
        public string temp_master_section { get; set; }
    }

    public class WOOBAssetToplevelAssetMapping
    {
        public Guid woline_toplevelcomponent_mapping_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public string toplevelcomponent_asset_name { get; set; }

        public bool is_toplevelcomponent_from_ob_wo { get; set; }
        public bool is_deleted { get; set; }

    }

    public class WOOBAssetSublevelAssetMapping
    {
        public Guid woline_sublevelcomponent_mapping_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public Guid sublevelcomponent_asset_id { get; set; }
        public string sublevelcomponent_asset_name { get; set; }
        public Guid sublevelcomponent_asset_class_id { get; set; }
        public string  sublevelcomponent_asset_class_name { get; set; }
        public string  sublevelcomponent_asset_class_code { get; set; }
        public bool is_sublevelcomponent_from_ob_wo { get; set; }
        public string circuit { get; set; }
        public string image_name { get; set; }
        public string image_url { get; set; }
        public bool is_deleted { get; set; }
        public List<subcomponent_image_list_class>? subcomponent_image_list { get; set; }
    }
    public class WOOBAssetFedByMap
    {
        public Guid wo_ob_asset_fed_by_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid parent_asset_id { get; set; }
        public string parent_asset_name{ get; set;}
        public string via_subcomponant_asset_name{ get; set;}
        public bool is_parent_from_ob_wo { get; set; }
        public int? fed_by_usage_type_id { get; set; }
        public Guid? via_subcomponant_asset_id { get; set; }
        public bool is_via_subcomponant_asset_from_ob_wo { get; set; }
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent
        public bool is_fed_by_via_subcomponant_asset_from_ob_wo { get; set; }
        public string fed_by_via_subcomponent_asset_name { get; set; }
        public bool is_deleted { get; set; }
        public string length { get; set; }
        public string style { get; set; }
        public int number_of_conductor { get; set; }    
        public int conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
    }
    public class OBWOAssetImages
    {
        public Guid? woonboardingassetsimagesmapping_id { get; set; }
        public Guid? woline_issue_image_mapping_id { get; set; }
        public Guid? wo_line_issue_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public Guid? irwoimagelabelmapping_id { get; set; }

        //public string visual_imgage_name { get; set; }
        //public string visual_thumbnail_imgage_name { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }// 1 for profile and 2 for nameplate images
        public Guid? woonboardingassets_id { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after , 3 - ir_visual
        public string image_extracted_json { get; set; }
        public string image_actual_json { get; set; }
    }
    public class View_OBIRImage_label
    {
        public Guid? irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string ir_image_label_url { get; set; }
        public string visual_image_label { get; set; }
        public string visual_image_label_url { get; set; }
        public string s3_image_folder_name { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public bool is_deleted { get; set; }
    }
    public class woline_issue_response_obj
    {
        public Guid? wo_line_issue_id { get; set; }
        public string issue_title { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public int? issue_caused_id { get; set; }// Osha , NEC , Thermal etc
        public string issue_description { get; set; }
        public bool is_issue_linked_for_fix { get; set; }
        public string thermal_anomaly_sub_componant { get; set; }
        public string thermal_anomaly_measured_amps { get; set; }
        public string thermal_anomaly_refrence_temps { get; set; }
        public string thermal_anomaly_measured_temps { get; set; }
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public string thermal_anomaly_location { get; set; }
        public string dynamic_field_json { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; } //problem_description
        public int? thermal_anomaly_recommendation { get; set; }  // corrective_action 
        public string thermal_anomaly_corrective_action { get; set; }
        public string thermal_anomaly_problem_description { get; set; }
        public int? thermal_anomaly_severity_criteria { get; set; } // 1=similar, 2=ambient, 3=indirect
        public int? thermal_classification_id { get; set; }
        public int? nec_violation { get; set; }
        public int? osha_violation { get; set; }
        public int? nfpa_70b_violation { get; set; }
        public int? type_of_ultrasonic_anamoly { get; set; }
        public int? nfpa_violation { get; set; }
        public string location_of_ultrasonic_anamoly { get; set; }
        public string size_of_ultrasonic_anamoly { get; set; }
        public bool is_abc_phase_required_for_report { get; set; }
        public List<OBWOAssetImages>? woline_issue_image_list { get; set; }
    }
    public class woline_issue_image_mapping
    {
        public Guid? woline_issue_image_mapping_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public Guid? irwoimagelabelmapping_id { get; set; }

        //public string visual_imgage_name { get; set; }
        //public string visual_thumbnail_imgage_name { get; set; }
        public bool is_deleted { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
    }

    public class PMEstimationdata
    {
        public Guid pm_id { get; set; }

        public string title { get; set; }

        public Nullable<int> estimation_time { get; set; }

        public Guid? sitewalkthrough_temp_pm_estimation_id { get; set; }
    }
}
