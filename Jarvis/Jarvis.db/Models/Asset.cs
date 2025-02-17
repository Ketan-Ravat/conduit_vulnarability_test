using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class Asset
    {
        [Key]
        public Guid asset_id { get; set; }

        public string internal_asset_id { get; set; }

        public string asset_photo { get; set; }
        public string QR_code { get; set; }
        public string company_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        [ForeignKey("InspectionForms")]
        public Guid? inspectionform_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }

        public string notes { get; set; }
        public string field_notes { get; set; }

        public Nullable<int> asset_request_status { get; set; }

        public string asset_requested_by { get; set; }

        public Nullable<DateTime> asset_requested_on { get; set; }

        public string asset_approved_by { get; set; }

        public Nullable<DateTime> asset_approved_on { get; set; }

        [Column(TypeName = "jsonb")]
        public List<AssetsValueJsonObject> lastinspection_attribute_values { get; set; }

        public Nullable<int> usage { get; set; }

        public Nullable<long> meter_hours { get; set; }

        public string name { get; set; }

        public string asset_type { get; set; }

        public string product_name { get; set; }

        public string model_name { get; set; }

        public string asset_serial_number { get; set; }

        public string model_year { get; set; }

        public string site_location { get; set; }

        public string current_stage { get; set; }

        public string parent { get; set; }

        public string children { get; set; }

        [ForeignKey("ConditionMaster")]
        public int? condition_state { get; set; } // depricated
        public string levels { get; set; }
        
        [ForeignKey("InspectionsTemplateFormIO")]
        public Guid? form_id { get; set; }
        public double? condition_index { get; set; } // depricated

        public int? criticality_index { get; set; } // depricated
        public int? criticality_index_type { get; set; }
        public int? condition_index_type { get; set; } // good , average , corrosive, dusty
        public DateTime? commisiion_date { get; set; }
        public string asset_barcode_id { get; set; }

        public string client_internal_id { get; set; }
        public string Install_Date { get; set; }
        public string Catalog_Number { get; set; }
        public string Location   { get; set; }
        public string Identification { get; set; }
        public DateTime? last_inspected_formio_date { get; set; }
        public DateTime? visual_insepction_last_performed { get; set; }
        public DateTime? mechanical_insepction_last_performed { get; set; }
        public DateTime? electrical_insepction_last_performed { get; set; }
        public DateTime? infrared_insepction_last_performed { get; set; }
        public DateTime? arc_flash_study_last_performed { get; set; }
        public int? thermal_classification_id { get; set; }
        public int? code_compliance { get; set; } // compliant  , non-compliant 
        public int? asset_expected_usefull_life { get; set; }
        public string form_retrived_nameplate_info { get; set; }
        public string asset_pm_form_json { get; set; } // structure: {"pm_json":[{"pm_title":"mechanicalservicing","name":"torqueDetails","json":""}]}

        [ForeignKey("FormIOType")]
        public Nullable<int> form_type_id { get; set; }

        [ForeignKey("Assethierarchy")]
        public Nullable<int> asset_hierarchy_id { get; set; }

        [ForeignKey("InspectionTemplateAssetClass")]
        public Guid? inspectiontemplate_asset_class_id { get; set; }

        [ForeignKey("AssetGroup")]
        public Guid? asset_group_id { get; set; }
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No
        public int x_axis { get; set; }
        public int y_axis { get; set; }
        public string asset_node_data_json { get; set; }

        #region new_flow_of_Maintenenace_WO_details

        public DateTime? mwo_date { get; set; }
        public int? mwo_inspection_type_status { get; set; } // Open,Inprogress,Done
        public string problem_description { get; set; }
        public string solution_description { get; set; }
        public string inspection_further_details { get; set; }
        public string comments { get; set; }
        public int? repair_resolution { get; set; }
        public int? replacement_resolution { get; set; }
        public int? recommended_action { get; set; }
        public int? recommended_action_schedule { get; set; } //
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? asset_placement { get; set; } // indooe,outdoor
        public int component_level_type_id { get; set; } = 1;  // 1 - top level , 2 - subcomponant
        public string asset_profile_image { get; set; }

        #endregion new_flow_of_Maintenenace_WO_details
        public virtual Assethierarchy Assethierarchy { get; set; }
        public virtual InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }
        public virtual FormIOType FormIOType { get; set; }
        public virtual StatusMaster ConditionMaster { get; set; }  
        public virtual InspectionsTemplateFormIO InspectionsTemplateFormIO { get; set; }

        public virtual Sites Sites { get; set; }

        public virtual InspectionForms InspectionForms { get; set; }
        public virtual AssetGroup AssetGroup { get; set; }

        public virtual ICollection<AssetTransactionHistory> AssetTransactionHistory { get; set; }

        public virtual ICollection<Inspection> Inspection { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }

        public virtual ICollection<IssueRecord> IssueRecords { get; set; }
        public virtual ICollection<AssetPMs> AssetPMs { get; set; }
        public virtual ICollection<AssetPMPlans> AssetPMPlans { get; set; }
        public virtual AssetPMNotificationConfigurations AssetPMNotificationConfigurations { get; set; }
        public virtual ICollection<AssetFormIO> AssetFormIO { get; set; }

        public virtual ICollection<Tasks> Tasks { get; set; }

       // public virtual WOcategorytoTaskMapping WOcategorytoTaskMapping { get; set; }

        [InverseProperty("Asset")]
        public virtual WOcategorytoTaskMapping WOcategorytoTaskMapping { get; set; }

        [InverseProperty("_assigned_asset")]
        public virtual ICollection<WOcategorytoTaskMapping> _assigned_asset_WOcategorytoTaskMapping { get; set; }

        public virtual AssetFormIOBuildingMappings AssetFormIOBuildingMappings { get; set; }

        public int? inspection_verdict { get; set; }
        public virtual ICollection<AssetProfileImages> AssetProfileImages { get; set; }
        public virtual ICollection<AssetNotes> AssetNotes { get; set; }
        public virtual ICollection<WOOnboardingAssets> WOOnboardingAssets { get; set; }
        public virtual ICollection<AssetIRWOImagesLabelMapping> AssetIRWOImagesLabelMapping { get; set; }
        public virtual ICollection<AssetChildrenHierarchyMapping> AssetChildrenHierarchyMapping { get; set; }
        public virtual ICollection<AssetParentHierarchyMapping> AssetParentHierarchyMapping { get; set; }
        public virtual ICollection<AssetReplacementMapping> AssetReplacementMapping { get; set; }
        public virtual ICollection<AssetIssue> AssetIssue { get; set; }
        public virtual ICollection<AssetAttachmentMapping> AssetAttachmentMapping { get; set; }
        public virtual ICollection<AssetTopLevelcomponentMapping> AssetTopLevelcomponentMapping { get; set; }
        public virtual ICollection<AssetSubLevelcomponentMapping> AssetSubLevelcomponentMapping { get; set; }
        public virtual ICollection<TempAsset> TempAsset { get; set; }

        //public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
