using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class WorkOrderResponseModel
    {
        public int response_status { get; set; }

        public Guid wo_id { get; set; }

        public long wo_number { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public Guid asset_id { get; set; }

        public int priority { get; set; }

        public DateTime due_at { get; set; }

        public int wo_type { get; set; }

        public int status { get; set; }

        public Guid site_id { get; set; }

        public Guid? service_dealer_id { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public string asset_name { get; set; }

        public string status_name { get; set; }

        public string wo_type_name { get; set; }

        public string priority_name { get; set; }

        public Guid? inspection_id { get; set; }

        public string meter_at_inspection { get; set; }

        public string task_total_estimate_time { get; set; }

        public string task_total_time_spent { get; set; }

        public List<WorkOrderTasksResponseModel> WorkOrderTasks { get; set; }

        public List<WorkOrderAttachmentsResponseModel> WorkOrderAttachments { get; set; }

        public virtual AssetsResponseModel Asset { get; set; }

        public List<WorkOrderMRResponseModel> MaintenanceRequests { get; set; }

        public ServiceDealerViewModel ServiceDealers { get; set; }

        public SitesViewModel Sites { get; set; }
    }
}
