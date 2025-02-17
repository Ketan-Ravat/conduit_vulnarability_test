using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrderTasks
    {
        [Key]
        public Guid wo_task_id { get; set; }

        [ForeignKey("Tasks")]
        public Guid task_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        public int? time_spent_minutes { get; set; }

        public int? time_spent_hours { get; set; }

        public string time_remaining_display { get; set; }

        //public double? hourly_rate { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual Tasks Tasks { get; set; }

        public virtual WorkOrders WorkOrders { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
    }
}
