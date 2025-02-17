using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class OfflineIssueRequestModel
    {
        public Guid uuid { get; set; }
        public string userid { get; set; }
        public Guid issue_uuid { get; set; }
        public Guid work_order_uuid { get; set; }
        public int status { get; set; }
        public string updated_at { get; set; }
        public List<CommentJsonObjectViewModel> comment { get; set; }
        public bool is_sync { get; set; }
    }
}
