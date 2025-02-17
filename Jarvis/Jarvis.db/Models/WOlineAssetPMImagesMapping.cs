using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOlineAssetPMImagesMapping
    {
        [Key]
        public Guid woline_assetpm_images_mapping_id { get; set; }
        public string image_name { get; set; }
        public bool is_deleted { get; set; }
        public int image_type { get; set; } //PM_Additional_General_photo = 6 , PM_Additional_Nameplate_photo = 7 , PM_Additional_Before_photo = 8 , PM_Additional_After_photo = 9 , PM_Additional_Environment_photo = 10
        public string pm_image_caption { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        [ForeignKey("ActiveAssetPMWOlineMapping")]
        public Guid? active_asset_pm_woline_mapping_id { get; set; } // this is for main asset pms

        [ForeignKey("TempActiveAssetPMWOlineMapping")]
        public Guid? temp_active_asset_pm_woline_mapping_id { get; set; } // this is for temp asset pms

        public ActiveAssetPMWOlineMapping ActiveAssetPMWOlineMapping { get; set; }
        public TempActiveAssetPMWOlineMapping TempActiveAssetPMWOlineMapping { get; set; }
    }
}
