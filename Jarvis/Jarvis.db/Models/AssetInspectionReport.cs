using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetInspectionReport
    {
        [Key]
        public Guid report_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int report_number { get; set; }
        
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }

        public DateTime from_date { get; set; }

        public DateTime to_date { get; set; }

        [ForeignKey("StatusMaster")]
        public int status { get; set; }

        public string download_link { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string created_by { get; set; }

        public string modified_by { get; set; }

        public Asset Asset { get; set; }

        public StatusMaster StatusMaster { get; set; }
    }
}
