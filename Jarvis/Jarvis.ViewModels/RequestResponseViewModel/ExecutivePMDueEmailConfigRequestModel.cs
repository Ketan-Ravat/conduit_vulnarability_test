using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel {
    public class ExecutivePMDueEmailConfigRequestModel {
        public Nullable<long> email_config_id { get; set; }
        //public Guid user_id { get; set; }
        public bool executive_pm_due_not_resolved_email_notification { get; set; }
        public Nullable<DateTime> disabled_till_date { get; set; }
        //public Nullable<DateTime> setup_on { get; set; }
        public Nullable<int> disable_till { get; set; }
        public Nullable<int> disable_till_by { get; set; }
        //public int status { get; set; }
    }

    public class ExecutivePMDueEmailConfigResponseModel {
        public Nullable<long> email_config_id { get; set; }
        public Guid user_id { get; set; }
        public bool executive_pm_due_not_resolved_email_notification { get; set; }
        public Nullable<DateTime> disabled_till_date { get; set; }
        public Nullable<DateTime> setup_on { get; set; }
        public Nullable<int> disable_till { get; set; }
        public Nullable<int> disable_till_by { get; set; }
        public int status { get; set; }
    }
}
