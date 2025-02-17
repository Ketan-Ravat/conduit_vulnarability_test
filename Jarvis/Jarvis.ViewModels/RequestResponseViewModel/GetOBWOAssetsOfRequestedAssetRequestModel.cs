using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetOBWOAssetsOfRequestedAssetRequestModel
    {
        public Guid? asset_id { get; set; }
        public int page_index { get; set; }
        public int page_size { get; set; }
        public string search_string { get; set; }
        public string qr_code { get; set; }
        public List<int>? wo_type { get; set; }
        public List<int>? status { get; set; }
        public bool is_requested_for_not_completed_wos_woline { get; set; } = false;
    }
}
