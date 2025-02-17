using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetProfileImages
    {
        [Key]
        public Guid asset_profile_images_id { get; set; }

        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        public string asset_photo { get; set; }
        public string asset_thumbnail_photo { get; set; }
        public string image_extracted_json { get; set; }
        public string image_actual_json { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int asset_photo_type { get; set; }
        public string pm_photo_caption { get; set; }
        
        public virtual Asset Asset { get; set; }

    }
}
