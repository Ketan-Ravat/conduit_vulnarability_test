using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class DashboardOutstandingIssues
    {
     
        [Key]
        public Guid site_id { get; set; }

        [Column(TypeName = "jsonb")]
        public List<ReportJsonData> data { get; set; }

        public DateTime created_at { get; set; }

        public DateTime modified_at { get; set; }
    }
}
