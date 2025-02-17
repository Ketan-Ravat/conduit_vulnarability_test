using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class IssuesNameResponseModel
    {
        public Guid issue_uuid { get; set; }
        public string name { get; set; }
        public Nullable<Guid> asset_id { get; set; }
    }
}
