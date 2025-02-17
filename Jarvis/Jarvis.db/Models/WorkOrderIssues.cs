using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrderIssues
    {
        [Key]
        public Guid wo_issue_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        [ForeignKey("Issue")]
        public Guid issue_id { get; set; }

        public bool is_archive{ get; set; }

        public virtual WorkOrders WorkOrders { get; set; }

        public virtual Issue Issue { get; set; }


    }
}
