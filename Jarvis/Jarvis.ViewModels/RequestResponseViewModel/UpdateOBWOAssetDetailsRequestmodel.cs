using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateOBWOAssetDetailsRequestmodel
    {
        public Guid? woonboardingassets_id { get; set; }
        public Guid wo_id { get; set; }
        public Guid site_id { get; set; } // for BE purpose 
        public bool is_request_from_issue_service { get; set; } = false; // for BE purpose only : if request is from Issue service then do not create temp Asset
        public string issue_title { get; set; } // for BE purpose only : if request is from Issue service then only update issue title
        public int? issue_priority { get; set; } // for BE purpose only : if request is from Issue service then only update issue priority
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_name { get; set; }
        public List<Guid>? asset_issue_id { get; set; }
        public List<Guid>? deleted_asset_issue_id { get; set; }
        public List<Guid>? woline_issue_id { get; set; }
        public List<Guid>? deleted_woline_issue_id { get; set; }

        public Guid? ob_existing_asset_id { get; set; } // old flow -- Do OB/IR wo flow for existing asset so all values in OB/IR WO line will be update when complete WO , new flow - now we are using asset_id from this model for update exisitng asset
        public bool is_wo_line_for_exisiting_asset { get; set; } // in MWO if asset is added from drop down then send true and if it is added from text box then send false
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string back_office_note { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }

        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }

        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }
        public Guid? temp_master_room_id { get; set; }

        public string QR_code { get; set; }
        public string field_note { get; set; }
        public int? thermal_classification_id { get; set; }
        public int status { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }

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
        public int? general_issue_resolution { get; set; }

        // IR WO new fields
        public Guid? fed_by { get; set; }
        public Guid? fed_by_ob_asset_id { get; set; }
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
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public string thermal_anomaly_location { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; }
        public int? thermal_anomaly_recommendation { get; set; }
        public int? nec_violation { get; set; }
     
        public int? osha_violation { get; set; }
        public bool flag_issue_thermal_anamoly_detected { get; set; }
        public bool flag_issue_nec_violation { get; set; }
        public bool flag_issue_osha_violation { get; set; }
        public bool flag_issue_repair { get; set; }
        // IR WO new fields ends
        public bool is_nec_violation_resolved { get; set; }
        public bool is_osha_violation_resolved { get; set; }
        public bool is_thermal_anomaly_resolved { get; set; }
        public string other_notes { get; set; }
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public DateTime? commisiion_date { get; set; }
        public int? code_compliance { get; set; }
        public bool is_replaced_asset_id_is_main { get; set; } = true; //if replaced asset is main then true else false
        public Guid? replaced_asset_id { get; set; }
        public string form_nameplate_info { get; set; }
        public string dynmic_fields_json { get; set; }
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public Guid? temp_woonboardingassets_id { get; set; }// this is for temp woline/Asset to which repair/replace/general issue is made for.  
        public Guid? issues_temp_asset_id { get; set; } // woline id of issue i.e.  if issue is from temp woline then give temp woline's Id here
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public bool is_woline_from_other_inspection { get; set; } = false; // if install woline is from issue/PM then this will be true and do not show this in wo datails screen
      public Guid? tempasset_id { get; set; }
        public string temp_building { get; set; }
        public string temp_floor { get; set; }
        public string temp_room { get; set; }
        public string temp_section { get; set; }
        public int? work_time_spend { get; set; } // number of seconds spend 
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public Guid? asset_group_id { get; set; }

        public Guid? pm_plan_id { get; set;}
        public List<OBWOAssetImages>? asset_image_list { get; set; }
        public List<OBIRImage_label>? ob_ir_Image_label_list { get; set; }
        public List<ob_asset_fed_by_mapping>? wo_ob_asset_fed_by_mapping { get; set; }
        public List<ob_asset_toplevelcomponent_mapping>? wo_ob_asset_toplevelcomponent_mapping { get; set; }
        public List<ob_asset_sublevelcomponent_mapping>? wo_ob_asset_sublevelcomponent_mapping { get; set; }
        public List<woline_issue_request_obj>? woline_issue_list { get; set; }

        public List<PMEstimationData>? pm_estimation_data { get; set; }
    }
    public class ob_asset_fed_by_mapping
    {
        public Guid? wo_ob_asset_fed_by_id { get; set; }
        public Guid parent_asset_id { get; set; }
        public bool is_parent_from_ob_wo { get; set; }
        public string via_subcomponant_asset_name { get; set; }
        public string via_subcomponant_asset_class_code { get; set; }
        public Guid? via_subcomponant_asset_id { get; set; }//this will act as main OCP / current asset
        public bool is_via_subcomponant_asset_from_ob_wo { get; set; } = false;
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent
        public bool is_fed_by_via_subcomponant_asset_from_ob_wo { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public bool is_deleted { get; set; }
        public int? fed_by_usage_type_id { get; set; } = 1; // 1- normal , 2- emergency
        public string length { get;set; }// length of conductor
        public string style { get; set; }// size of conductor
        public int? number_of_conductor { get; set; }    // # of conductor
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
    }
    public class OBIRImage_label
    {
        public Guid? irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string visual_image_label { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public bool is_deleted { get; set; }
        public string manual_wo_number { get; set; }
    }

    public class ob_asset_toplevelcomponent_mapping
    {
        public Guid? woline_toplevelcomponent_mapping_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public bool is_toplevelcomponent_from_ob_wo { get; set; }
        public bool is_deleted { get; set; }
    }
    public class ob_asset_sublevelcomponent_mapping
    {
        public Guid? woline_sublevelcomponent_mapping_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public Guid? sublevelcomponent_asset_id { get; set; }
        public bool is_sublevelcomponent_from_ob_wo { get; set; }
        public bool is_deleted { get; set; }
        public string circuit { get; set; }
        public string image_name { get; set; }
        public string sublevelcomponent_asset_name { get; set; }
        public Guid sublevelcomponent_asset_class_id  { get; set; }
        public List<subcomponent_image_list_class>? subcomponent_image_list { get; set; }
    }
    public class woline_issue_request_obj
    {
        //public int? nfpa_violation { get; set; }
        public Guid? wo_line_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public string issue_title { get; set; }
        public string issue_description { get; set; }

        public bool is_issue_linked_for_fix { get; set; }
        public bool is_deleted { get; set; }
        public int? issue_caused_id { get; set; }// new key

        //public List<woline_issue_image_mapping>? woline_issue_image_list { get; set; }

        // for nec / osha /thermal anamoly 
        public string thermal_anomaly_sub_componant { get; set; }
        public string thermal_anomaly_measured_amps { get; set; }
        public string thermal_anomaly_refrence_temps { get; set; }
        public string thermal_anomaly_measured_temps { get; set; }
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public string thermal_anomaly_location { get; set; }
        public string dynamic_field_json { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; } // store in problem_description
        public int? thermal_anomaly_recommendation { get; set; } // store in corrective_action
        public string thermal_anomaly_problem_description { get; set; }
        public string thermal_anomaly_corrective_action { get; set; }
        public int? thermal_anomaly_severity_criteria { get; set; } // 1=similar, 2=ambient, 3=indirect
        public int? thermal_classification_id { get; set; }
        public int? nec_violation { get; set; }
        public int? nfpa_70b_violation { get; set; }
        public int? osha_violation { get; set; }
        public int? type_of_ultrasonic_anamoly { get; set; }
        public string location_of_ultrasonic_anamoly { get; set; }
        public string size_of_ultrasonic_anamoly { get; set; }
        public bool is_abc_phase_required_for_report { get; set; }
        public List<OBWOAssetImages>? issue_image_list { get; set; }

    }

    public class subcomponent_image_list_class
    {
        public Guid? woonboardingassetsimagesmapping_id { get; set; }
        public string image_name { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public string thumbnail_url { get; set; }
        public string image_url { get; set; }
        public int image_type { get; set; }
        public bool is_deleted { get; set; }
    }


    public class PMEstimationData
    {

        public Guid pm_id { get; set; }

        public int? estimation_time { get; set; }
        public bool is_deleted { get; set; }

        public Guid? sitewalkthrough_temp_pm_estimation_id { get; set;}


    }

}
