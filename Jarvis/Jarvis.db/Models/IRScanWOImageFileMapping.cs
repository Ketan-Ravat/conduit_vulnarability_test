using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class IRScanWOImageFileMapping
    {
        [Key]
        public Guid irscanwoimagefilemapping_id { get; set; }
        public string img_file_name { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        public string manual_wo_number { get; set; }
        public bool is_img_attached { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
        public WorkOrders WorkOrders { get; set; }
    }
}
