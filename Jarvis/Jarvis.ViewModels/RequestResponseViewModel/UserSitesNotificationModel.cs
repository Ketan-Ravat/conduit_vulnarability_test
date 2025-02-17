using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UserSitesNotificationModel
    {
        public virtual UsersData User { get; set; }
        //public Users User { get; set; }
    }

    public class UsersData
    {
        public Guid uuid { get; set; }

        public Guid role_id { get; set; }

        public string email { get; set; }

        public string username { get; set; }

        public string notification_token { get; set; }

        public string OS { get; set; }
    }
}
