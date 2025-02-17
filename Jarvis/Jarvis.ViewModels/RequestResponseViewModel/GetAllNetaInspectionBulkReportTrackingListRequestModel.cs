using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllNetaInspectionBulkReportTrackingListRequestModel
    {
        public int pagesize { get; set;}
        public int pageindex { get; set;}
        public string search_string { get; set;}
        public int? status { get; set;}
        public int? report_inspection_type { get; set; }
    }
}
