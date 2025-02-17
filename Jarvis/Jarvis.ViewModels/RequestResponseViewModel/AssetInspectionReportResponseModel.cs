using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetInspectionReportResponseModel
    {
        public int success { get; set; }
        public string message { get; set; }
        public Guid report_id { get; set; }
        public int report_number { get; set; }
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public string download_link { get; set; }
    }
}
