using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class PMTriggersTasksResponseModel
    {
        public int response_status { get; set; }
        public string response_message { get; set; }
        public Guid trigger_task_id { get; set; }

        public Guid pm_trigger_id { get; set; }

        public Guid asset_pm_id { get; set; }

        public Guid asset_pm_task_id { get; set; }

        public Guid task_id { get; set; }

        public int status { get; set; }

        public Guid asset_id { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string created_by { get; set; }

        public string modified_by { get; set; }
    }
}
