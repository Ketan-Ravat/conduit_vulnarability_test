using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class TaskResponseModel
    {
        public int response_status { get; set; }
        
        public string response_message { get; set; }

        public Guid task_id { get; set; }
        public Guid company_id { get; set; }

        public long task_code { get; set; }

        public string task_title { get; set; }

        public int task_est_minutes { get; set; }

        public int task_est_hours { get; set; }

        public string task_est_display { get; set; }

        public double? hourly_rate { get; set; }

        public string description { get; set; }

        public string notes { get; set; }

        public bool isArchive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public Guid form_id { get; set; }

        public string form_name { get; set; }
        public string form_type { get; set; }
        public string work_procedure { get; set; }
        public List<AssetsResponseModel> Assets { get; set; }

        public List<AssetTasksResponseModel> AssetTasks { get; set; }
    }
}
