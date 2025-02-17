using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOLinkedIssueResponsemodel
    {
        public List<link_main_issue_list> main_issue_list { get; set; }
        public List<link_temp_issue_list> temp_issue_list { get; set; }
    }

}
