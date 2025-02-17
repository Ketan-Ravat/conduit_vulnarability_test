using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrderStatus
    {
        [Key]
        public Guid work_order_status_id { get; set; }

        [ForeignKey("WorkOrder")]
        public Guid work_order_id { get; set; }

        public int status { get; set; }

        public string modified_by { get; set; }

        public DateTime modified_at { get; set; }

        public WorkOrder WorkOrder { get; set; }
    }
}