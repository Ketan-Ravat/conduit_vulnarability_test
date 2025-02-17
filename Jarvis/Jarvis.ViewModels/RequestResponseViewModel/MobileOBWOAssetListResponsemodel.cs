using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileOBWOAssetListResponsemodel
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
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int? general_issue_resolution { get; set; }
        public bool is_wo_line_for_exisiting_asset { get; set; }

        // public List<OBWOAssetImages> asset_image_list { get; set; }
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
        public string technician_name { get; set; }
        public string other_notes { get; set; }
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
        #endregion  IR_scan_WO_new_Fields

        public string form_nameplate_info { get; set; }
        public string dynmic_fields_json { get; set; }// we will use this key for dynamic new fields in woline 
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public DateTime? inspected_at { get; set; }
        public string issue_title { get; set; } // attched issue title
        public int? issue_priority { get; set; } // attched issue priority
        public Guid? tempasset_id { get; set; }
        public bool is_woline_from_other_inspection { get; set; }// if install woline is from issue/PM then this will be true and do not show this in wo datails screen
        public Guid? issues_temp_asset_id { get; set; } // woline id of issue i.e.  if issue is from temp woline then give temp woline's Id here
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public DateTime? initial_inspected_at { get; set; } // this key is will only add at the time of first inspection after that this date wont be update.
        public Guid? initial_inspected_by { get; set; }// this key is will only add at the time of first inspection after that this key wont be update.
        public bool is_nec_violation_resolved { get; set; }
        public bool is_osha_violation_resolved { get; set; }
        public bool is_thermal_anomaly_resolved { get; set; }

    }
}
