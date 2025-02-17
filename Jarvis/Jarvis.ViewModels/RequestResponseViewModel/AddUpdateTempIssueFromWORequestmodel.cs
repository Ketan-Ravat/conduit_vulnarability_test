using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateTempIssueFromWORequestmodel
    {
        public List<TempIssueListRequest> list_temp_issue { get; set; }
        public bool is_pm_clear { get; set; } = false;// in PM woline we are using this key to mark PM as complete or not 
        public List<PMAdditionalImagesRequest> list_pm_additional_images { get; set; }
        public List<IRVisualScanImages> list_pm_ir_scan_images { get; set; }

    }

    public class TempIssueListRequest
    {
        public Guid? wo_line_issue_id { get; set; }
        public int issue_type { get; set; }//Repair , Replacement , Compliance(NEC,Osha), Thermal
        public Guid? asset_form_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public int issue_caused_id { get; set; } // Osha , NEC , Thermal etc
        public string field_note { get; set; }
        public string back_office_note { get; set; }
        public Guid? wo_id { get; set; }
        public Guid? asset_id { get; set; }// Asset id to map this issue with an Asset
        public string form_retrived_asset_name { get; set; } // AT WO asset name to show in temp issue 
        public bool is_deleted { get; set; }
        public int temp_issue_status { get; set; }
        public string atmw_first_comment { get; set; }
        public bool is_issue_linked_for_fix { get; set; }
        public string pm_issue_identity_key { get; set; }
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
        public string dynamic_field_json { get; set; }
        public List<TempIssueImagesRequest> list_temp_issue_images { get; set; }
       
    }

    public class TempIssueImagesRequest
    {
        public Guid? woline_issue_image_mapping_id { get; set; }
        public Guid? wo_line_issue_id { get; set; }
        public string image_file_name { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public Guid? woonboardingassets_id { get; set; }
    }

    public class PMAdditionalImagesRequest
    {
        public Guid woline_assetpm_images_mapping_id { get; set; }
        public string image_name { get; set; }
        public bool is_deleted { get; set; }
        public int image_type { get; set; } //PM_Additional_General_photo = 6 , PM_Additional_Nameplate_photo = 7 , PM_Additional_Before_photo = 8 , PM_Additional_After_photo = 9 , PM_Additional_Environment_photo = 10
        public string pm_image_caption { get; set; }
        public Guid active_asset_pm_woline_mapping_id { get; set; }
    }

    public class IRVisualScanImages
    {
        public string ir_image_label { get; set; }
        public string visual_image_label { get; set; }
        public string s3_image_folder_name { get; set; }
        public bool is_deleted { get; set; }
        public string img_extension { get; set; }
    }
}
