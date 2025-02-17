using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetAttachmentMapping
    {
        [Key]
        public Guid assetatachmentmapping_id { get; set; }
        public string file_name { get; set; }
        public string user_uploaded_file_name { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public string created_by { get; set; }
        public bool is_deleted { get; set; }
        [ForeignKey("Site")]
        public Guid site_id { get; set; }
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        public Asset Asset { get; set; }
        public Sites Site { get; set; }
    }
}
