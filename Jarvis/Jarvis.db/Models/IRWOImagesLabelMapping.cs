using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class IRWOImagesLabelMapping
    {
        [Key]
        public Guid irwoimagelabelmapping_id { get; set; }
        public string ir_image_label { get; set; }
        public string visual_image_label { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public string s3_image_folder_name { get; set; }
        public bool is_deleted { get; set; }

        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }

        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public Sites Sites { get; set; }
        public ICollection<WOlineIssueImagesMapping> WOlineIssueImagesMapping { get; set; }
    }
}
