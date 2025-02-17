using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class TaskRequestModel
    {
        public Guid task_id { get; set; }
        public Guid company_id { get; set; }

        public long task_code { get; set; }

        public string task_title { get; set; }

        public int task_est_minutes { get; set; } = 0;

        public int task_est_hours { get; set; } = 0;

        public string task_est_display { get; set; }

        public double? hourly_rate { get; set; }

        public Guid? form_id { get; set; }

        public string description { get; set; }

        public string notes { get; set; }

        public bool isArchive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public List<AssetTasksRequestModel>? AssetTasks { get; set; }
    }
}
