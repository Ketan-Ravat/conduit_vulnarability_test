using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ViewAssetIssueDetailsByIdResponsemodel
    {
        public Guid? asset_issue_id { get; set; }
        public Guid? asset_id { get; set; }//asset id to map this Issue
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? priority { get; set; }// low , medium , high
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string issue_number { get; set; }
        public string field_note { get; set; }
        public string back_office_note { get; set; }
        public bool is_deleted { get; set; }
        public string manual_wo_number { get; set; }
        public Guid? wo_id { get; set; }
        public string asset_name { get; set; }
        public int issue_type { get; set; }
        public Guid? issue_created_wo_id { get; set; }
        public Guid? client_company_id { get; set; }
        public Guid? site_id { get; set; }
        public string site_name { get; set; }
        public string client_company_name { get; set; }
        public List<AssetIssueImagesListResponse> issue_image_list { get; set; }
    }

    public class AssetIssueImagesListResponse
    {
        public Guid asset_issue_image_mapping_id { get; set; }
        public Guid asset_issue_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public string image_file_name_url { get; set; }
        public string image_thumbnail_file_name_url { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public bool is_deleted { get; set; }
        public int? image_duration_type_id { get; set; }// 1 - before , 2 - after
    }
}
