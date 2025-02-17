using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class ClusterDiagramPDFSiteMapping
    {
        [Key]
        public int cluster_diagram_pdf_id { get; set; }
        public string file_name { get; set; }
        public int? upload_status { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }

        public virtual Sites Sites { get; set; }
    }
}
