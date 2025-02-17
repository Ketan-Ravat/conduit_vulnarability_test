using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetGroup
    {
        [Key]
        public Guid asset_group_id { get; set; }

        public string asset_group_name { get; set; }
        public string asset_group_description { get; set; }
        public int criticality_index_type { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual Sites Sites { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<TempAsset> TempAsset { get; set; }
    }
}
