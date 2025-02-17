using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models {
    public class UserEmailNotificationConfigurationSettings {
        [Key]
        public long email_config_id { get; set; }
        [ForeignKey("User")]
        public Guid user_id { get; set; }
        public bool executive_pm_due_not_resolved_email_notification { get; set; }
        public Nullable<DateTime> disabled_till_date { get; set; }
        public Nullable<DateTime> setup_on { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public Nullable<int> disable_till { get; set; }
        [ForeignKey("StatusMaster")]
        public Nullable<int> disable_till_by { get; set; }
        public int status { get; set; }
        public virtual User User { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
    }
}
