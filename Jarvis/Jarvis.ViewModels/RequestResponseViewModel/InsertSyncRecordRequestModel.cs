using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class InsertSyncRecordRequestModel
    {
        public string request_model { get; set; }
        public string request_id { get; set; }
        public long device_info_id { get; set; }
        public Guid device_uuid { get; set; }
        public string device_battery_percentage { get; set; }
        public string mac_address { get; set; }
        public string device_latitude { get; set; }
        public string device_longitude { get; set; }
        public bool is_inspection { get; set; }
        public bool is_workorder { get; set; }
        public Guid? requested_by { get; set; }
    }
}
