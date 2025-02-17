using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllWOLineTempIssuesResponsemodel
    {
        public Guid wo_line_issue_id { get; set; }
        public int issue_type { get; set; }
        public int? issue_caused_id { get; set; }
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public string field_note { get; set; }
        public string back_office_note { get; set; }
        public Guid? woonboardingassets_id { get; set; }  // if issue is generated from OB/IR WOline
        public Guid? asset_form_id { get; set; } // if issue is generated from inspection tab
        public Guid? wo_id { get; set; }
        public Guid? asset_id { get; set; }//asset id to map this temp Issue with asset when completing WO
        public string asset_name { get; set; }//asset id to map this temp Issue with asset when completing WO
        public int? issue_status { get; set; }
        public string issue_status_name { get; set; }
        public string origin_manual_wo_number { get; set; }
        public Guid? origin_wo_id { get; set; }
        public Guid? origin_wo_line_id { get; set; }


        public int origin_wo_type { get; set; }
    }
}
