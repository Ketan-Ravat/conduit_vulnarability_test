using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetReplacementMapping
    {
        [Key]
        public Guid asset_replacement_mapping_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; } // main asset id
        public Guid replaced_asset_id { get;set; }// asset is replaced by 

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Sites Sites { get; set; }
        public Asset Asset { get; set; }

    }
}
