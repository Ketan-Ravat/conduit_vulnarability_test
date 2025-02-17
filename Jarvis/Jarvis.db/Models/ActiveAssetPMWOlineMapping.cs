using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class ActiveAssetPMWOlineMapping
    {
        [Key]
        public Guid active_asset_pm_woline_mapping_id { get; set; }

        public bool is_active { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid woonboardingassets_id { get; set; }

        [ForeignKey("AssetPMs")]
        public Guid asset_pm_id { get; set; }
        public string pm_form_output_data { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }

        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public AssetPMs AssetPMs { get; set; }
        public ICollection<WOlineAssetPMImagesMapping> WOlineAssetPMImagesMapping { get; set; }
    }
}
