using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class OfflineInspectionRequestModel
    {
        public Guid uuid { get; set; }

        public string requested_datetime { get; set; }

        public Guid asset_id { get; set; }

        public string user_id { get; set; }

        public Guid site_id { get; set; }

        public string operator_notes { get; set; }

        public string company_id { get; set; }

        public string created_at { get; set; }

        public long meter_hours { get; set; }

        public int shift { get; set; }

        public int status { get; set; }
        public bool is_comment_important { get; set; }

        public string attribute_value { get; set; }

        public AssetsValueJsonObjectViewModel[] attribute_values { get; set; }

        public ImagesListObjectViewModel image_list { get; set; }
    }
}
