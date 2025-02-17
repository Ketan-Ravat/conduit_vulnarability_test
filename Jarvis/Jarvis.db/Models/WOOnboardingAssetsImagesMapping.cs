using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOOnboardingAssetsImagesMapping
    {
        [Key]
        public Guid woonboardingassetsimagesmapping_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public string image_extracted_json { get; set; }
        public string image_actual_json { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }// 1 for profile and 2 for nameplate images , Thermal_Anomly_Photo = 3, NEC_Violation_Photo = 4, OSHA_Violation_Photo = 5,
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }


        [ForeignKey("WOlineSubLevelcomponentMapping")] // For Adding Existing Asset' SubComponent(main asset) in IR/OB WO
        public Guid? woline_sublevelcomponent_mapping_id { get; set; }
        
        public WOlineSubLevelcomponentMapping WOlineSubLevelcomponentMapping { get; set; }
        public WOOnboardingAssets WOOnboardingAssets { get; set; }

    }
}
