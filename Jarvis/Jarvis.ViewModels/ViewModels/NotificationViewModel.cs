using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class NotificationViewModel
    {
        public Guid notification_id { get; set; }

        public Guid user_id { get; set; }

        public int target_type { get; set; }

        public string device_key { get; set; }

        public string ref_id { get; set; }
        public int? wo_type { get; set; }

        public string heading { get; set; }

        public string message { get; set; }
        public string data { get; set; }

        public int notification_type { get; set; }
        public string notification_user_role { get; set; }

        public int notification_status { get; set; }

        public DateTime createdDate { get; set; }

        public DateTime sendDate { get; set; }

        public int status { get; set; }

        public string OS { get; set; }

        public string target_role { get; set; }
        public bool is_visible { get; set; }
    }
}
