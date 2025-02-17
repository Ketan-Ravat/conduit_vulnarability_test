using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileIssueResponseModel
    {
        public int status { get; set; }
        public string status_name { get; set; }
        public int priority { get; set; }
        public string name { get; set; }
        public Guid issue_uuid { get; set; }
        public Nullable<Guid> asset_id { get; set; }
        public string asset_name { get; set; }
        public string internal_asset_id { get; set; }
        public long? issue_number { get; set; }
        public string notes { get; set; }
        public string timezone { get; set; }
        public Nullable<Guid> inspection_id { get; set; }
        public List<CommentJsonObjectViewModel> comments { get; set; }
    }
}
