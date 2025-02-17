using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetIssuesOptimizedResponsemodel
    {
        public Guid asset_issue_id { get; set; }
        public string issue_number { get; set; }
        public string asset_name { get; set; }
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public string issue_status_name { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? issue_caused_id { get; set; }// Osha , NEC , Thermal etc
        public string manual_wo_number { get; set; }
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public Guid? asset_id { get; set; }//asset id to map this Issue
        public string asset_class_type { get; set; }
        public int? priority { get; set; }// low , medium , high
        public string issue_title { get; set; }
        public string issue_time_elapsed { get; set; }
        public Guid? wo_id { get; set; }

        public string asset_class_name { get; set; }
        public int? asset_condition_index_type { get; set; }
        public int? asset_criticality_index_type { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public string issue_description { get; set; }
        public string thermal_anomaly_corrective_action { get; set; }
        public string site_name { get; set; }
        public string client_company_name { get; set; }
        public string company_name { get; set; }
        public string comments { get; set; }

        //public Guid? asset_form_id { get; set; } // if issue is linked to inspection tab
        //public Guid? issue_created_asset_form_id { get; set; } // if issue is generated from inspection tab
        //public Guid? woonboardingassets_id { get; set; }  // if issue is linked to OB/IR WOline
        //public Guid? issue_created_woonboardingassets_id { get; set; }  // if issue is generated from OB/IR WOline
        //public string issue_description { get; set; }
        //public string field_note { get; set; }
        //public string back_office_note { get; set; }
        //public Guid? issue_created_wo_id { get; set; }
    }
}
