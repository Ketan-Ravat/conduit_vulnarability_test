using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MRCancelRequestModel
    {
        public Guid mr_id { get; set; }
        public int reason_id { get; set; }
        public string notes { get; set; }
        public bool is_archive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }
}
