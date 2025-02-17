using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Jarvis.db.Models
{
    public class WorkOrderAttachments
    {
        [Key]
        public Guid wo_attachment_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        public string user_uploaded_name { get; set; }

        public string filename { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public virtual WorkOrders WorkOrders { get; set; }
    }
}
