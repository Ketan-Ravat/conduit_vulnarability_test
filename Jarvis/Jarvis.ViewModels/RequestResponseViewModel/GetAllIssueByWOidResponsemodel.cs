using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllIssueByWOidResponsemodel
    {
        public List<link_main_issue_list> main_issue_list { get; set; }
        public List<GetAllWOLineTempIssuesResponsemodel> temp_issue_list { get; set; }
    }
}
