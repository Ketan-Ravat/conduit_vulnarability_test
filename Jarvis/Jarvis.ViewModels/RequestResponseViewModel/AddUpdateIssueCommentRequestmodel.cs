using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateIssueCommentRequestmodel
    {
        public Guid? asset_issue_comments_id { get; set; }
        public Guid? asset_issue_id { get; set; }
        public string comment { get; set; }
        public bool is_deleted { get; set; }
    }
}
