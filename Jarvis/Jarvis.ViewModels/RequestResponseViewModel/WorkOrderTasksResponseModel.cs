using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class WorkOrderTasksResponseModel
    {
        public Guid wo_task_id { get; set; }

        public Guid task_id { get; set; }

        public Guid wo_id { get; set; }

        public int time_spent_minutes { get; set; }

        public int time_spent_hours { get; set; }

        public string time_remaining_display { get; set; }

        public double hourly_rate { get; set; }

        public int status { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public TaskResponseModel Tasks { get; set; }
    }
}
