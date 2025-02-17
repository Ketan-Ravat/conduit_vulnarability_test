using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Jarvis.db.Models
{
    public class WOOnboardingAssets
    {
        [Key]
        public Guid woonboardingassets_id { get; set; }
      
        public string asset_name { get; set; }
        public Guid? ob_existing_asset_id { get; set; }//old flow-- Do OB/IR wo flow for existing asset so all values in OB/IR WO line will be update when complete WO , new flow- now we wr using asset_id from this table for MWO as well as OB,IR WOs
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string back_office_note { get; set; }
        public string building { get; set; } //we are using these direct location keys to show location in repair/replace/general/PM/Isssu woline for exsitng Asset
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
        public string form_nameplate_info { get; set; }
        public string dynmic_fields_json { get; set; }// we will use this key for dynamic new fields in woline 

        #region new_flow_of_Maintenenace_WO_details

        public int? inspection_type { get; set; }
        public DateTime? mwo_date { get; set; }
        public int? mwo_inspection_type_status { get; set; }
        public string problem_description { get; set; }
        public string solution_description { get; set; }
        public string issue_title { get; set; } // attched issue title
        public int? issue_priority { get; set; } // attched issue priority

        public string inspection_further_details { get; set; }
        public string comments { get; set; }
        public int? repair_resolution { get; set; }
        public int? replacement_resolution { get; set; }
        public int? recommended_action { get; set; } // from MWO and general issue resolution recommended action {inspection , repair , replace , no action required}
        public int? recommended_action_schedule { get; set; }
        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }// Do OB/IR wo flow for existing asset so all values in OB/IR WO line will be update when complete WO also this key is used in MWO for all OB cateory.
        public Guid? issues_temp_asset_id { get; set; } // woline id of issue or PM i.e.  if issue or PM is from temp woline then give temp woline's Id here
        public bool is_wo_line_for_exisiting_asset { get; set; }
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public Guid? technician_user_id { get; set; }
        public bool is_replaced_asset_id_is_main { get; set; } //if replaced asset is main then true else false
        public Guid? replaced_asset_id { get; set; } // asset id to be replace from replace category in MWO.
        public int? general_issue_resolution { get; set; }
        #endregion new_flow_of_Maintenenace_WO_details


        #region IR_scan_WO_new_Fields

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
        public string thermal_anomaly_location { get; set; }
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; }
        public int? thermal_anomaly_recommendation { get; set; }
        public int? nec_violation { get; set; }
        public int? osha_violation { get; set; }
        public bool flag_issue_thermal_anamoly_detected { get; set; }
        public bool flag_issue_nec_violation { get; set; }
        public bool flag_issue_osha_violation { get; set; }
        public bool is_nec_violation_resolved { get; set; }
        public bool is_osha_violation_resolved { get; set; }
        public bool is_thermal_anomaly_resolved { get; set; }
        //public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? code_compliance { get; set; } // compliant  , non-compliant 
        #endregion IR_scan_WO_new_Fields

        public string pdf_report_url { get; set; }
        public string other_notes { get; set; }
        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("TempAsset")]
        public Guid? tempasset_id { get; set; }

        public DateTime? inspected_at { get; set; }
        public DateTime? initial_inspected_at { get; set; } // this key is will only add at the time of first inspection after that this date wont be update.
        public Guid? initial_inspected_by { get; set; }// this key is will only add at the time of first inspection after that this key wont be update.
        public DateTime? completed_at { get; set; }
        public bool is_main_asset_created { get; set; }
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        
        //public Guid? temp_woonboardingassets_id { get; set; }// this is for temp woline/Asset to which repair/replace/general issue is made for.  
        public bool is_woline_from_other_inspection { get; set; }// if install woline is from issue/PM then this will be true and do not show this in wo datails screen
        public WorkOrders WorkOrders { get; set; }
        public StatusMaster StatusMaster { get; set; }
        public Asset Asset { get; set; }
        public AssetPMs AssetPMs { get; set; } // deprecated now we are using ActiveAssetPMWOlineMapping
        public WOLineBuildingMapping WOLineBuildingMapping { get; set; }
        public virtual Sites Sites { get; set; }
        public ICollection<WOOnboardingAssetsImagesMapping> WOOnboardingAssetsImagesMapping { get; set; }
        public ICollection<IRWOImagesLabelMapping> IRWOImagesLabelMapping { get; set; }
        public ICollection<WOOBAssetFedByMapping> WOOBAssetFedByMapping { get; set; }
        public ICollection<WOLineIssue> WOLineIssue { get; set; }
        public ICollection<AssetIssue> AssetIssue { get; set; }
        public ICollection<WOlineTopLevelcomponentMapping> WOlineTopLevelcomponentMapping { get; set; }
        public ICollection<WOlineSubLevelcomponentMapping> WOlineSubLevelcomponentMapping { get; set; }
        public ICollection<SitewalkthroughTempPmEstimation> SitewalkthroughTempPmEstimation { get; set; }
        public ICollection<TempAssetPMs> TempAssetPMs { get; set; }
        public virtual WOOBAssetTempFormIOBuildingMapping WOOBAssetTempFormIOBuildingMapping { get; set; }
        public virtual ActiveAssetPMWOlineMapping ActiveAssetPMWOlineMapping { get; set; }
        public virtual TempActiveAssetPMWOlineMapping TempActiveAssetPMWOlineMapping { get; set; }
        public virtual TempAsset TempAsset { get; set; }
        public virtual WOOnboardingAssetsDateTimeTracking WOOnboardingAssetsDateTimeTracking { get; set; }
    }
}
