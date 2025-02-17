using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetInspectionReportRequestModel
    {
        //public string requested_by { get; set; }
        public string internal_asset_id { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }
    }
}
