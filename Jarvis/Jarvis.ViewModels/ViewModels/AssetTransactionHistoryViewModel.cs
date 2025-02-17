using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetTransactionHistoryViewModel
    {
        public Guid asseet_txn_id { get; set; }
        public Guid asset_id { get; set; }
        public string inspection_id { get; set; }
        public string operator_id { get; set; }
        public string manager_id { get; set; }
        public Nullable<int> status { get; set; }
        public string comapny_id { get; set; }
        public string site_id { get; set; }
        public AttributesJsonObjectViewModel attributeValues { get; set; }
        public string inspection_form_id { get; set; }
        public Nullable<long> meter_hours { get; set; }
        public Nullable<int> shift { get; set; }
        public Nullable<DateTime> created_at { get; set; }
    }
}
