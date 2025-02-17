using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class LinkIssueToWOLineRequestmodel
    {
        public List<Guid>? asset_issue_id { get; set; } // for main issue
        public List<Guid>? wo_line_issue_id { get; set; } // for temp issue
        public Guid? wo_id { get; set; }
        public Guid? asset_form_id { get; set; } // for inspection wo line
        public Guid? woonboardingassets_id { get; set; } // for OB/IR WO line
        public Guid? main_asset_id { get; set; } // if selected asset is from main asset list
        public Guid? issues_temp_asset_id { get; set; } // if selected asset is from main asset list
        public List<Guid>? deleted_asset_issue_id { get; set; }
        public List<Guid>? deleted_wo_line_issue_id { get; set; }
    }
}
