using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ExecutiveDailyNewIssuesRequestModel
    {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        public List<IssueResponseModel> issues { get; set; }
    }
}
