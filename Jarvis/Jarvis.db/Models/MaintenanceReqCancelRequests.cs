using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class MaintenanceReqCancelRequests
    {
        [Key]
        public Guid mr_cancel_id { get; set; }

        [ForeignKey("CancelReasonMaster")]
        public int reason_id { get; set; }

        [ForeignKey("MaintenanceRequests")]
        public Guid mr_id { get; set; }

        public string notes { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual CancelReasonMaster CancelReasonMaster { get; set; }

        public virtual MaintenanceRequests MaintenanceRequests { get; set; }
    }
}
