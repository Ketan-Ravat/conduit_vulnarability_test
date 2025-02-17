using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class WorkOrderAttachmentsResponseModel
    {
        public Guid wo_attachment_id { get; set; }

        public Guid wo_id { get; set; }

        public string user_uploaded_name { get; set; }

        public string filename { get; set; }

        public string file_url { get; set; }
        public string thumbnail_filename { get; set; }

        public string thumbnail_file_url { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public List<MultipleImageUpload_ResponseModel_class> image_list { get; set; }
    }

    public class MultipleImageUpload_ResponseModel_class
    {
        public string user_uploaded_name { get; set; }
        public string filename { get; set; }
        public int asset_photo_type { get; set; }
        public string file_url { get; set; }
        public string thumbnail_filename { get; set; }
        public string thumbnail_file_url { get; set; }
    }
}
