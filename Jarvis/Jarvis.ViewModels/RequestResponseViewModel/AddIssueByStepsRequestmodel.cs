using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddIssueByStepsRequestmodel
    {
        public AddIssueDetailsForSteps issue_details { get; set; }
        public UpdateOBWOAssetDetailsRequestmodel install_woline_details { get; set; }
    }

    public class AddIssueDetailsForSteps
    {
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public int? inspection_type { get; set; } // reapir / replace / general issue resolution
        public Guid? asset_issue_id { get; set; } // if selected issue is main issue
        public Guid? selected_asset_id { get; set; } // Asset id if linked and new_issue_asset_type is not verify-on-field
        public bool is_selected_asset_id_main { get; set; } // if selected asset id is main then true else flase
        public int issue_creation_type { get; set; }// 1 - existing/Main issue , 2 - New issue 
        public int? new_issue_asset_type { get; set; }// if user creates new issue and selects create-asset then  1 , if selects from existing then 2 , and if verify on field then 3 (Enums : NewIssueAssettype)
        public string issue_title { get; set; }
        public string problem_description { get; set; }
        public int? priority { get; set; }// low , medium , high
        public Guid wo_id { get; set; }
        public string resolution_description { get; set; }
        public int? resolution_type { get; set; } // 1 - resolved , 2- not resolved
        public bool is_replaced_asset_id_is_main { get; set; } //if replaced asset is main then true else false
        public Guid? replaced_asset_id { get; set; }// to be replaced asset id
        public int issue_woline_status { get; set; }// issue woline status 
        public string inspection_further_details { get; set; } //further details for inspection if selected resolved
        public List<IssueImagesMapping> issue_images { get; set; } // we will always add images when request is to add woline 
    }

    public class IssueImagesMapping
    {
        public Guid? asset_issue_image_mapping_id { get; set; } // if image is from main issue if added new when create issue then store image in woline bucket
        public Guid? woline_issue_image_mapping_id { get; set; } // if image is from temp issue 
        public string image_file_name_url { get; set; }
        public string image_file_name { get; set; }
        public bool is_deleted { get; set; }
        public string image_thumbnail_file_name_url { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public int image_duration_type_id { get; set; }
    }

}
