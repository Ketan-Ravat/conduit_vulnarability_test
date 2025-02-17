using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DeviceInfoViewModel
    {
        public long device_info_id { get; set; }
        public Guid device_uuid { get; set; }
        public string device_code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string version { get; set; }
        public string os { get; set; }
        public string mac_address { get; set; }
        public bool is_approved { get; set; }
        public Guid? approved_by { get; set; }
        public Guid? company_id { get; set; }
        public string company_name { get; set; }
        public DateTime? last_sync_time { get; set; }
        public Guid? last_sync_site_id { get; set; }
        public string last_sync_site_name { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }
}
