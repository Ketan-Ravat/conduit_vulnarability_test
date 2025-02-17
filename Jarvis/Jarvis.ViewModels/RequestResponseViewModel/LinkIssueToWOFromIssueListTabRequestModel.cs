using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class LinkIssueToWOFromIssueListTabRequestModel
    {
        public List<LinkIssueList> link_issue_list { get; set; }
    }

    public class LinkIssueList
    {
        public Guid asset_issue_id { get; set; }
        public Guid wo_id { get; set; }
        public int? wo_type { get; set; }
    }
}
