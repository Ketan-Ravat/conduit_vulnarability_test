using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class IRWOAssetReportStatusResponsemodel
    {
        public string pdf_report_url { get; set; }
        public int? pdf_report_status { get; set; }
        public Guid wo_id { get; set; }
    }
}
