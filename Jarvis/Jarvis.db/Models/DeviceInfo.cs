using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class DeviceInfo
    {
        [Key]
        public long device_info_id { get; set; }
        [Required]
        public Guid device_uuid { get; set; }
        [Required]
        public string device_code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string version { get; set; }
        public string os { get; set; }
        public string mac_address { get; set; }
        public bool is_approved { get; set; }
        public string app_version { get; set; }
        public Guid? approved_by { get; set; }
        public Guid? company_id { get; set; }
        public DateTime? last_sync_time { get; set; }
        public Guid? last_sync_site_id { get; set; }
        public bool is_location_enabled { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual ICollection<RecordSyncInformation> RecordSyncInformation { get; set; }
    }
}
