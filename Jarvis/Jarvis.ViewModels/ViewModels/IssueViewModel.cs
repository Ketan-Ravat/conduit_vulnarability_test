using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class IssueViewModel
    {
        public Guid issue_uuid { get; set; }
        public Guid work_order_uuid { get; set; }
        public Guid asset_id { get; set; }
        public Guid attribute_id { get; set; }
        public Guid inspection_id { get; set; }
        public string internal_asset_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string notes { get; set; }
        public int status { get; set; }
        public int priority { get; set; }
        public DateTime checkout_datetime { get; set; }
        public DateTime requested_datetime { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public Guid? site_id { get; set; }
        public string company_id { get; set; }
        public string status_name { get; set; }
        public string maintainence_staff_id { get; set; }
        public long? issue_number { get; set; }
        public long? work_order_number { get; set; }
        public List<CommentJsonObjectViewModel> comments { get; set; }
    }
}
