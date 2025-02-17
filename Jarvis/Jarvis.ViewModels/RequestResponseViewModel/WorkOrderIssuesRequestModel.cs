using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class WorkOrderIssuesRequestModel
    {
        public Guid issue_id { get; set; }

        public Guid mr_id { get; set; }

        public bool is_archive { get; set; }

    }
}
