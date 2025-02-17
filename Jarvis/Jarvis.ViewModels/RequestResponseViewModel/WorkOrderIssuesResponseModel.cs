using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class WorkOrderIssuesResponseModel
    {
        public int response_status { get; set; }

        public Guid wo_issue_id { get; set; }

        public Guid wo_id { get; set; }

        public Guid issue_id { get; set; }

        public bool is_archive { get; set; }
    } 
}
