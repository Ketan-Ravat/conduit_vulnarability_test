using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetIssueComments
    {
        [Key]
        public Guid asset_issue_comments_id { get; set; }
       
        [ForeignKey("AssetIssue")]
        public Guid? asset_issue_id { get; set; }
        public string comment { get; set; }

        [ForeignKey("User")]
        public Guid? comment_user_id { get; set; }

        [ForeignKey("Roles")]
        public Guid? comment_user_role_id { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public AssetIssue AssetIssue { get; set; }
        public User User { get; set; }
        public Role Roles { get; set; }
    }
}
