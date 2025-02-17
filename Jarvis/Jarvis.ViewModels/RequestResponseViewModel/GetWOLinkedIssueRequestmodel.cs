using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOLinkedIssueRequestmodel
    {
        public Guid? asset_form_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public string search_string { get; set; }
        public bool is_request_from_issue_service { get; set; } = false; 
    }
}
