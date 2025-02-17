using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddIssuesDirectlyToMaintenanceWORequestModel
    {
        public Guid wo_id { get; set; }
        public int? inspection_type { get; set; }
        public Guid asset_issue_id { get; set; }
    }
}
