using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOLineIssue
    {
        [Key]
        public Guid wo_line_issue_id { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        [ForeignKey("StatusMaster")]
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? issue_caused_id { get; set; }// Osha , NEC , Thermal etc

        [ForeignKey("AssetFormIO")]
        public Guid? asset_form_id { get; set; } // woline in which issue is attached

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }  // woline in which issue is attached it means repair/replace/general resolution

        public Guid? original_asset_form_id { get; set; } // origin inspection id from which temp issue is created 
        public Guid? original_woonboardingassets_id { get; set; } // origin woline id from which temp issue is created and this will also act as main temp asset for which issue is created
        public Guid? original_wo_id { get; set; } // origin wo id from which temp issue is created 
        public Guid? original_asset_id { get; set; } // origin asset id from which temp issue is created 
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string atmw_first_comment { get; set; } // AT and MWO Issue description for first comment in Asset Issue
        public string field_note { get;set; }
        public string back_office_note { get; set; }
        public bool is_main_issue_created { get; set; } // this will be true if main issue is created in AssetIssue table after completing WO t
        public bool is_issue_linked_for_fix { get; set; }// this will be true if issue is added to fix in WO line so create issue and resolve at same time
        public string pm_issue_identity_key { get; set; } // this key is to identify PM issue based on json key
        public string thermal_anomaly_sub_componant { get; set; }
        public string thermal_anomaly_measured_amps { get; set; }
        public string thermal_anomaly_refrence_temps { get; set; }
        public string thermal_anomaly_measured_temps { get; set; }
        public string thermal_anomaly_additional_ir_photo { get; set; }
        public string thermal_anomaly_location { get; set; }
        public int? thermal_anomaly_probable_cause { get; set; } // desprecated now we use as string below 🔽
        public string thermal_anomaly_problem_description { get; set; }
        public int? thermal_anomaly_recommendation { get; set; } // desprecated now we use as string below 🔽
        public string thermal_anomaly_corrective_action { get; set; }
        public int? thermal_anomaly_severity_criteria { get; set; } // 1=similar, 2=ambient, 3=indirect
        public int? thermal_classification_id { get; set; }
        public int? nec_violation { get; set; }
        public int? osha_violation { get; set; }
        public int? nfpa_70b_violation { get; set; }
        public string dynamic_field_json { get; set; }
        public bool is_abc_phase_required_for_report { get; set; }
        public int? type_of_ultrasonic_anamoly { get; set; }
        public string location_of_ultrasonic_anamoly { get; set; }
        public string size_of_ultrasonic_anamoly { get; set; }

        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        
        [ForeignKey("WorkOrders")]
        public Guid? wo_id { get; set; }

        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }//asset id to map this temp Issue with asset when completing WO so we can identify from which woline main issue has generated.
        public string form_retrived_asset_name { get; set; } // AT WO asset name to show in temp issue 
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Sites Sites { get; set; }
        public AssetFormIO AssetFormIO { get; set; }
        public WorkOrders WorkOrders { get; set; }
        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public AssetIssue AssetIssue { get; set; }
        public Asset Asset { get; set; }
        public StatusMaster StatusMaster { get; set; }
        public ICollection<WOlineIssueImagesMapping> WOlineIssueImagesMapping { get; set; }

    }
}
