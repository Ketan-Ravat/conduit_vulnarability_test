using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOlineTopLevelcomponentMapping
    {
        [Key]
        public Guid woline_toplevelcomponent_mapping_id { get; set; }
        [ForeignKey("WOOnboardingAssets")]
        public Guid woonboardingassets_id { get; set; }
        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        public Guid toplevelcomponent_asset_id { get; set; }
        public bool is_toplevelcomponent_from_ob_wo { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public Sites Sites { get; set; }
    }
}
