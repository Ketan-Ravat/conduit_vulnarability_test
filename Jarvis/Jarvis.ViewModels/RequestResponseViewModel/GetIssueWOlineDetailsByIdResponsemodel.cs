using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetIssueWOlineDetailsByIdResponsemodel
    {

        public GetOBWOAssetDetailsByIdResponsemodel issue_woline_details { get; set; }
        public GetOBWOAssetDetailsByIdResponsemodel install_woline_details { get; set; }
        public IssueDetailsForView issue_details { get; set; }
    }

    public class IssueDetailsForView
    {
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public int? inspection_type { get; set; } // reapir / replace / general issue resolution
        public Guid? asset_issue_id { get; set; } // if selected issue is main issue
        public Guid? selected_asset_id { get; set; } // Asset id if linked and new_issue_asset_type is not verify-on-field
        public string selected_asset_name { get; set; }
        public bool is_selected_asset_id_main { get; set; } // if selected asset id is main then true else flase
        public int issue_creation_type { get; set; }// 1 - existing/Main issue , 2 - New issue 
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public string issue_title { get; set; }
        public string problem_description { get; set; }
        public int? priority { get; set; }// low , medium , high
        public string resolution_description { get; set; }
        public string inspection_further_details { get; set; } //further details for inspection if selected resolved
        public int? resolution_type { get; set; } // 1 - resolved , 2- not resolved
    }
}
