using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddNotificationDataForUsersRequestModel
    {
        public int notification_type { get; set; }
        public string ref_id { get; set; }
        public string heading { get; set; }
        public string message { get; set; }
        public List<UserDetailsWithRole>? users_list { get; set; }
        public UserDetailsWithRole? single_user { get; set; }
    }

    public class UserDetailsWithRole
    {
        public Guid user_id { get; set; }
        public string notification_user_role_type { get; set; }
    }
}
