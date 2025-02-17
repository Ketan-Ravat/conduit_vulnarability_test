using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class ManagerPMNotificationConfiguration
    {
        [Key]
        public Guid manager_notification_conf_id { get; set; }
        public Guid user_id { get; set; }
        public bool is_disabled { get; set; }
        public Guid pm_trigger_id { get; set; }
    }
}
