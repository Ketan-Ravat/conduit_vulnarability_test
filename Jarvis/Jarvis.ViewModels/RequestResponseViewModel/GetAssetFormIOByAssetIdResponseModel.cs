using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetFormIOByAssetIdResponseModel
    {
        public Guid asset_form_id { get; set; }
        public Nullable<Guid> form_id { get; set; }
        public string asset_form_name { get; set; }
        public string asset_form_type { get; set; }
        public string asset_form_description { get; set; }
        public string asset_form_data { get; set; }
        public int status { get; set; }
        public Guid? wo_id { get; set; }
        public int wo_type { get; set; }
        public Guid? WOcategorytoTaskMapping_id { get; set; }

        public string form_retrived_asset_name { get; set; }
        public string form_retrived_asset_id { get; set; }
        public string form_retrived_location { get; set; }
        public string form_retrived_data { get; set; }
    }
}
