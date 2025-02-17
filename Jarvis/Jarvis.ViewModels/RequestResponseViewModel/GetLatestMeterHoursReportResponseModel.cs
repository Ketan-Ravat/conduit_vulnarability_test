using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class GetLatestMeterHoursReportResponseModel
    {
        public Guid asset_id { get; set; }

        public string internal_asset_id { get; set; }

        public string asset_name { get; set; }

        public string current_meter_hours { get; set; }

        public DateTime latest_inspection_date { get; set; }

        public string timezone { get; set; }
    }
}
