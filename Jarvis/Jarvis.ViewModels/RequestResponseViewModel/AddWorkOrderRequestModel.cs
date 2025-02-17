using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddWorkOrderRequestModel
    {
        public Guid wo_id { get; set; }

        public long wo_number { get; set; }

        //public List<Guid> mr_id { get; set; }
        public Guid mr_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public Guid asset_id { get; set; }

        public int priority { get; set; }

        public DateTime due_at { get; set; }

        public int wo_type { get; set; }

        public int status { get; set; }

        public Nullable<DateTime> completed_date { get; set; }

        public Guid site_id { get; set; }

        public Guid? service_dealer_id { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public List<WorkOrderTasksRequestModel> WorkOrderTasks { get; set; }

        public List<WorkOrderAttachmentsRequestModel> WorkOrderAttachments { get; set; }

        public List<WorkOrderIssuesRequestModel> Issue { get; set; }
    }
}
