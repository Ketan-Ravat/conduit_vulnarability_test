using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class PMPlans
    {
        [Key]
        public Guid pm_plan_id { get; set; }

        public string plan_name { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
        public bool is_default_pm_plan { get; set; }             

        [ForeignKey("PMCategory")]
        public Guid pm_category_id { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public virtual PMCategory PMCategory { get; set; }
        public virtual ICollection<PMs> PMs { get; set; }
    }
}
