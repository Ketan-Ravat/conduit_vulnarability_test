using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models {
    public class AssetActivityLogs {
        [Key]
        public int activity_id { get; set; }
        public string activity_header { get; set; }
        public string activity_message { get; set; }
        public int activity_type { get; set; }
        public DateTime created_at { get; set; }
        public string updated_by { get; set; }
        public string data { get; set; }
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        public Guid site_id { get; set; }
        public string ref_id { get; set; }
        public int status { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
