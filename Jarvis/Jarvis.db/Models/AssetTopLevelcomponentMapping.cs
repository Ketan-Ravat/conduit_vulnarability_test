using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetTopLevelcomponentMapping
    {
        [Key]
        public Guid asset_toplevelcomponent_mapping_id { get; set; }
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
        public Asset Asset { get; set; }
        public Sites Sites { get; set; }
    }
}
