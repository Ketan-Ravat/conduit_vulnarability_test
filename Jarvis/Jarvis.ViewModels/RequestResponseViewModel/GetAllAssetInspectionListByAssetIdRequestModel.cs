using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetInspectionListByAssetIdRequestModel
    {
        public string assetid { get; set; }
        public bool is_requested_for_otherthan_completed { get; set; } = false;
        public int pagesize { get; set; } = 10;
        public int pageindex { get; set; } = 1;
        public DateTime? initial_start_date_time { get; set; }
        public DateTime? initial_end_date_time { get; set; }
        public List<string>? filter_asset_name { get; set; }
        public List<Guid>? WO_ids { get; set; }
        public List<int>? wo_type { get; set; }
        public List<Guid>? inspected_by { get; set; }
        public List<string>? accepted_by { get; set; }
        public string search_string { get; set; }
        public string qr_code { get; set; }
        public List<int>? status { get; set; }
        public List<int>? service_type { get; set; }
        public bool is_wo_completed { get; set; } = true;
        public bool is_for_asset_inspection_tab { get; set; } = false;
    }
}
