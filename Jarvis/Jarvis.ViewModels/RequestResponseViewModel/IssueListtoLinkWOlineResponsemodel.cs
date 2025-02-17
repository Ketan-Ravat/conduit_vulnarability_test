using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class IssueListtoLinkWOlineResponsemodel
    {
        public List<link_main_issue_list> main_issue_list { get; set; }    
        public List<link_temp_issue_list> temp_issue_list { get; set; }    
    }
    public class link_main_issue_list
    {
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? priority { get; set; }// low , medium , high
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string issue_number { get; set; }

        public Guid? asset_issue_id { get; set; }
        public int issue_type { get; set; }
        public DateTime? created_at { get; set; }
        public string asset_name { get; set; }
        public string origin_manual_wo_number { get; set; }
        public Guid? origin_wo_id { get; set; }
        public Guid? origin_wo_line_id { get; set; }
        public int origin_wo_type { get; set; }
    }
    public class link_temp_issue_list
    {
        public int? issue_status { get; set; }//Open , Scheduled , In Progress , Resolved
        public int? priority { get; set; }// low , medium , high
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public Guid wo_line_issue_id { get; set; }
        public int issue_type { get; set; }
        public DateTime? created_at { get; set; }
        public string asset_name { get; set; }
    }
}
