using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CompanyPMNotificationResponseModel
    {
        public int response_status { get; set; }
        public Guid company_pm_notification_configuration { get; set; }
        public Guid company_id { get; set; }
        public int first_reminder_before_on { get; set; }
        public int first_reminder_before_on_type { get; set; }
        public int first_reminder_before_on_status { get; set; }
        public int second_reminder_before_on { get; set; }
        public int second_reminder_before_on_type { get; set; }
        public int second_reminder_before_on_status { get; set; }
        public int due_at_reminder_status { get; set; }
        public int first_reminder_before_on_meter_hours_status { get; set; }
        public int first_reminder_before_on_meter_hours { get; set; }
        public int second_reminder_before_on_meter_hours_status { get; set; }
        public int second_reminder_before_on_meter_hours { get; set; }
        public int due_at_reminder_meter_hours_status { get; set; }
        public int status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual CompanyViewModel Company { get; set; }
    }
}
