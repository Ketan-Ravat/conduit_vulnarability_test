using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetIssueDetailsForReportResponsemodel
    {
        public string site_name { get; set; }
        public string company_name { get; set; }
        public string client_company_name { get; set; }
        public int schedual_issue_count { get; set; }
        public int open_issue_count { get; set; }
        public int inprogress_issue_count { get; set; }
        public int completed_issue_count { get; set; }
    }
}
