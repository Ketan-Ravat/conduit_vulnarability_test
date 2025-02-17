using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIORoomsImagesMapping
    {
        [Key]
        public Guid formioroomsimagesmapping_id { get; set; }

        [ForeignKey("FormIORooms")]

        public int formioroom_id { get; set; }

        public string image_file_name { get; set; }
        public string image_thumbnail_file_name { get; set; }

        public string s3_image_folder_name { get; set; }

        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }

        public virtual FormIORooms FormIORooms { get; set; }
    }
}
