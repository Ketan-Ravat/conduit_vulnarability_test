using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetPMNotificationConfigurations {
        [Key]
        public Guid asset_pm_notification_configuration { get; set; }
        [ForeignKey("Asset")]
        public Guid asset_id { get; set; }
        public int first_reminder_before_on { get; set; }
        [ForeignKey("FirstReminderType")]
        public int first_reminder_before_on_type { get; set; }
        [ForeignKey("FirstReminderStatus")]
        public int first_reminder_before_on_status { get; set; }
        public int second_reminder_before_on { get; set; }
        [ForeignKey("SecondReminderType")]
        public int second_reminder_before_on_type { get; set; }
        [ForeignKey("SecondReminderStatus")]
        public int second_reminder_before_on_status { get; set; }
        [ForeignKey("DueAtStatus")]
        public int due_at_reminder_status { get; set; }
        public int first_reminder_before_on_meter_hours { get; set; }
        [ForeignKey("FirstMeterHoursReminderStatus")]
        public int first_reminder_before_on_meter_hours_status { get; set; }
        public int second_reminder_before_on_meter_hours { get; set; }
        [ForeignKey("SecondMeterHoursReminderStatus")]
        public int second_reminder_before_on_meter_hours_status { get; set; }
        [ForeignKey("DueAtMeterHoursStatus")]
        public int due_at_reminder_meter_hours_status { get; set; }
        [ForeignKey("StatusMaster")]
        public int status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual StatusMaster FirstReminderType { get; set; }
        public virtual StatusMaster FirstReminderStatus { get; set; }
        public virtual StatusMaster SecondReminderType { get; set; }
        public virtual StatusMaster SecondReminderStatus { get; set; }
        public virtual StatusMaster DueAtStatus { get; set; }
        public virtual StatusMaster FirstMeterHoursReminderStatus { get; set; }
        public virtual StatusMaster SecondMeterHoursReminderStatus { get; set; }
        public virtual StatusMaster DueAtMeterHoursStatus { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
    }
}
