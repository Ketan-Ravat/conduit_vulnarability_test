using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetPMNotificationRequestModel
    {
        public Guid asset_pm_notification_configuration { get; set; }
        public Guid asset_id { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int first_reminder_before_on_status { get; set; }
        public int first_reminder_before_on { get; set; }
        /// <summary>
        /// 29 = Month
        /// 39 = Week
        /// 40 = Day
        /// </summary>
        public int first_reminder_before_on_type { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int second_reminder_before_on_status { get; set; }
        public int second_reminder_before_on { get; set; }
        /// <summary>
        /// 29 = Month
        /// 39 = Week
        /// 40 = Day
        /// </summary>
        public int second_reminder_before_on_type { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int due_at_reminder_status { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int first_reminder_before_on_meter_hours_status { get; set; }
        public int first_reminder_before_on_meter_hours { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int second_reminder_before_on_meter_hours_status { get; set; }
        public int second_reminder_before_on_meter_hours { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int due_at_reminder_meter_hours_status { get; set; }
        /// <summary>
        /// 1 = Active
        /// 2 = Inactive
        /// </summary>
        public int status { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
    }
}
