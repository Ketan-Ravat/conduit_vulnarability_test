using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetFormIORequestModel
    {
        public Guid asset_form_id { get; set; }

        public Guid asset_id { get; set; }

        public Nullable<Guid> form_id { get; set; }

        public string asset_form_name { get; set; }

        public string asset_form_type { get; set; }

        public string asset_form_description { get; set; }

        public string asset_form_data { get; set; }

        public int status { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }
    }
}
