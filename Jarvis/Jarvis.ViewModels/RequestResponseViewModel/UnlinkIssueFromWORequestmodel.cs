using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UnlinkIssueFromWORequestmodel
    { 
        public List<Guid>? asset_issue_id { get; set; }
        public List<Guid>? wo_line_issue_id { get; set; }
    }
}
