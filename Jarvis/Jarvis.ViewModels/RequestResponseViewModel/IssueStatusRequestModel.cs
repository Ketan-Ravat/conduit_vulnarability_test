using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class IssueStatusRequestModel
    {
        public Guid issue_status_id { get; set; }

        public Guid issue_id { get; set; }

        public int status { get; set; }

        public string modified_by { get; set; }

        public DateTime modified_at { get; set; }
    }
}
