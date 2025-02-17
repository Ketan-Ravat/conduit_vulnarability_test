using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdatePMItemsMasterDataRequestModel
    {
        public Guid company_id { get; set; }
        public string form_json { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public string plan_name { get; set; }
        public string pm_title { get; set; }
    }
}
