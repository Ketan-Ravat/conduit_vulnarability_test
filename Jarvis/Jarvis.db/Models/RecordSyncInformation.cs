using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class RecordSyncInformation
    {
        [Key]
        public long sync_id { get; set; }
        public string request_model { get; set; }
        public string request_id { get; set; }
        [ForeignKey("DeviceInfo")]
        public long device_info_id { get; set; }
        public Guid device_uuid { get; set; }
        public string device_battery_percentage { get; set; }
        public string mac_address { get; set; }
        public string device_latitude { get; set; }
        public string device_longitude { get; set; }
        public bool is_inspection { get; set; }
        public bool is_workorder { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
}
