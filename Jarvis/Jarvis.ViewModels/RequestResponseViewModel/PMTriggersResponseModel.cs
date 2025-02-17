using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class PMTriggersResponseModel
    {
        public int response_status { get; set; }
        public string response_message { get; set; }
        public Guid pm_trigger_id { get; set; }

        public Guid asset_pm_id { get; set; }

        public Guid asset_id { get; set; }

        public int status { get; set; }
        public Nullable<int> asset_pm_status { get; set; }

        public Nullable<DateTime> due_datetime { get; set; }

        public Nullable<int> due_meter_hours { get; set; }

        public Nullable<int> estimated_due_date_meter_hour { get; set; }

        public Nullable<int> total_est_time_minutes { get; set; }

        public Nullable<int> total_est_time_hours { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public List<PMTriggersTasksResponseModel> PMTriggersTasks { get; set; }
    }
}
