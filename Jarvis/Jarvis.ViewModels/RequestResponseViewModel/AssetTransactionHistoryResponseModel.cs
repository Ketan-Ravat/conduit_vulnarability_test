using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetMeterHourHistoryResponseModel
    {
        public Guid asset_meter_hour_id { get; set; }
        public Guid asset_id { get; set; }
        public string requested_by { get; set; }
        public Nullable<int> status { get; set; }
        public string company_id { get; set; }
        public string site_id { get; set; }
        public Nullable<long> meter_hours { get; set; }
        public Nullable<DateTime> updated_at { get; set; }
        public string timezone { get; set; }
    }
}
