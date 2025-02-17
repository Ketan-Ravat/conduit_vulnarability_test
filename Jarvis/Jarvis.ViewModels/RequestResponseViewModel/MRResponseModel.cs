using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MRResponseModel
    {
        public int response_status { get; set; }

        public Guid mr_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public Guid asset_id { get; set; }

        public int priority { get; set; }

        public int mr_type { get; set; }

        public Nullable<Guid> mr_type_id { get; set; }

        public Nullable<Guid> wo_id { get; set; }

        public string time_elapsed { get; set; }

        public int status { get; set; }

        public Guid site_id { get; set; }

        public Guid requested_by { get; set; }

        public string requested_by_name { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public string status_name { get; set; }

        public string mr_type_name { get; set; }

        public string priority_name { get; set; }

        public int work_order_status { get; set; }

        public string work_order_number { get; set; }

        public Guid inspection_id { get; set; }

        public string meter_at_inspection { get; set; }

        public virtual WorkOrderResponseModel WorkOrders { get; set; }
        public virtual AssetsResponseModel Asset { get; set; }
    }
}
