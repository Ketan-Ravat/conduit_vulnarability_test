using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempActiveAssetPMWOlineMapping
    {

        [Key]
        public Guid temp_active_asset_pm_woline_mapping_id { get; set; }
        public bool is_active { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid woonboardingassets_id { get; set; } // this is for PM woline

        [ForeignKey("TempAssetPMs")]
        public Guid temp_asset_pm_id { get; set; } // this is temp asset pm

        public string pm_form_output_data { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }

        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public TempAssetPMs TempAssetPMs { get; set; }
        public ICollection<WOlineAssetPMImagesMapping> WOlineAssetPMImagesMapping { get; set; }
    }
}
