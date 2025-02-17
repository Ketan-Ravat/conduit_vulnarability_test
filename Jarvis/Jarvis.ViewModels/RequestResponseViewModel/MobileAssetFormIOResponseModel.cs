using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileAssetFormIOResponseModel
    {
        public Nullable<DateTime> created_at { get; set; }
        public string requested_by { get; set; }
        public string form_retrived_workOrderType { get; set; }
        public Guid asset_form_id { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public string timezone { get; set; }
    }
}
