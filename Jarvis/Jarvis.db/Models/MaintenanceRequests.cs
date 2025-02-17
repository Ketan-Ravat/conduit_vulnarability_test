using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class MaintenanceRequests
    {
        [Key]
        public Guid mr_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        [ForeignKey("PriorityStatusMaster")]
        public int priority { get; set; }

        [ForeignKey("MRTypeStatusMaster")]
        public int mr_type { get; set; }

        public Guid? mr_type_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid? wo_id { get; set; }

        public Guid requested_by { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        public bool is_archive { get; set; }
        public string resolve_reason { get; set; }
        public Nullable<DateTime> resolved_at { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual Sites Sites { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }

        public virtual StatusMaster PriorityStatusMaster { get; set; }

        public virtual StatusMaster MRTypeStatusMaster { get; set; }

        public virtual WorkOrders WorkOrders { get; set; }

        public virtual ICollection<Issue> Issue { get; set; }
    }
}
