using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class SiteDocuments
    {
        [Key]
        public Guid sitedocument_id { get; set; }
        [ForeignKey("Company")]
        public Guid company_id { get; set; }
        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        public string file_name { get; set; }
        public string s3_folder_name { get; set; }
        public bool is_archive { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }

    }
}
