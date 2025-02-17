using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateAssetIssueRequestmodel
    {
        public Guid? asset_issue_id { get; set; }
        public Guid? asset_id { get; set; }//asset id to map this Issue
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? priority { get; set; }// low , medium , high
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string field_note { get; set; }
        public string back_office_note { get; set; }
        public bool is_deleted { get; set; }
        public int? issue_type { get; set; } = 0;
        public string resolve_issue_reason { get; set; }    
        public Guid? wo_id { get; set; }
        public Guid? asset_form_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public bool is_issue_linked { get; set; } = false;
        public List<AddupdateIssueImg>? issue_image_list { get; set; }
    }
    public class AddupdateIssueImg
    {
        public Guid? asset_issue_image_mapping_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public bool is_deleted { get; set; }
        public int? image_duration_type_id { get; set; }
        
    }
}
