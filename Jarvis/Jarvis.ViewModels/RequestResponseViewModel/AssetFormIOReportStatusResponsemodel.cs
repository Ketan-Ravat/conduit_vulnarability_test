using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetFormIOReportStatusResponsemodel
    {
        public string report_lambda_logs { get; set; }
        public string pdf_report_url { get; set; }
        public int pdf_report_status { get; set; }
        public Guid asset_form_id { get; set; }
    }
}
