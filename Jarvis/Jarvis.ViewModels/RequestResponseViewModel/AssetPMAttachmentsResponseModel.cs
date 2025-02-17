using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetPMAttachmentsResponseModel
    {
        public Guid asset_pm_attachment_id { get; set; }

        public Guid asset_id { get; set; }

        public Guid asset_pm_id { get; set; }

        public Guid asset_pm_plan_id { get; set; }

        public string user_uploaded_name { get; set; }

        public string filename { get; set; }

        public string file_url { get; set; }

        public bool is_archive { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
    }
}
