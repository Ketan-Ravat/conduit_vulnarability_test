using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetIssueImagesMapping
    {
        [Key]
        public Guid asset_issue_image_mapping_id { get; set; }
       
        [ForeignKey("AssetIssue")]
        public Guid asset_issue_id { get; set; }
        [ForeignKey("Sites")]
        public Guid site_id { get; set; }
        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Sites Sites { get; set; }
        public AssetIssue AssetIssue { get; set; }

    }
}
