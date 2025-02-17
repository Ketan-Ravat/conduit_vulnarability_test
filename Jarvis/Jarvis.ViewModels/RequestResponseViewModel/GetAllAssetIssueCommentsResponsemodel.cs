using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetIssueCommentsResponsemodel
    {
        public Guid asset_issue_comments_id { get; set; }
        public Guid? asset_issue_id { get; set; }
        public string comment { get; set; }
        public Guid? comment_user_id { get; set; }
        public string comment_user_name { get; set; }
        public Guid? comment_user_role_id { get; set; }
        public string comment_user_role_name { get; set; }
    }
}
