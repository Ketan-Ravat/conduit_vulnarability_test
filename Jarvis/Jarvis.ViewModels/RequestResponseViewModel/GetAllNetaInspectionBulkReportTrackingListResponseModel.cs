using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllNetaInspectionBulkReportTrackingListResponseModel
    {
        public Guid netainspectionbulkreporttracking_id { get; set; }
        public string asset_form_ids { get; set; }
        public int report_status { get; set; }
        public string report_id_number { get; set; }
        public string report_url { get; set; }
        public string report_lambda_logs { get; set; }
        public Guid? site_id { get; set; }
        public int? report_inspection_type { get; set; }
        public Guid? created_by { get; set; }
        public string created_by_name { get; set; }
        public Nullable<DateTime> reprt_completed_date { get; set; }
        public List<string> assets_name_list { get; set;}
    }
}
