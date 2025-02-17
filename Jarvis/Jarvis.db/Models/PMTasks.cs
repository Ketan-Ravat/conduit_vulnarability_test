using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMTasks
    {
        [Key]
        public Guid pm_task_id { get; set; }

        [ForeignKey("Tasks")]
        public Guid task_id { get; set; }

        [ForeignKey("PMs")]
        public Guid pm_id { get; set; }

        [ForeignKey("PMPlans")]
        public Guid pm_plan_id { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual Tasks Tasks { get; set; }

        public virtual PMs PMs { get; set; }
        
        public virtual PMPlans PMPlans { get; set; }
    }
}
