using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOlineIssueImagesMapping
    {
        [Key]
        public Guid woline_issue_image_mapping_id { get; set; }

        [ForeignKey("WOLineIssue")]
        public Guid wo_line_issue_id { get; set; }
        [ForeignKey("Sites")]
        public Guid site_id { get; set; }
        public string image_file_name { get; set; } 
        public string image_thumbnail_file_name { get; set; }

        [ForeignKey("IRWOImagesLabelMapping")]
        public Guid? irwoimagelabelmapping_id { get; set; }

        //public string visual_imgage_name { get; set; }// if duration_type = 3 then we use image_file_name field as ir_img & saperate visual_img field
        //public string visual_thumbnail_imgage_name { get; set; }
        //public string s3_image_folder_name_irvisual { get; set; } // only using for ir_visual image 
        public int? image_duration_type_id { get; set; }  // 1 - before , 2 - after  
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public Sites Sites { get; set; }
        public IRWOImagesLabelMapping IRWOImagesLabelMapping { get; set; }
        public WOLineIssue WOLineIssue { get; set; }
    }
}
