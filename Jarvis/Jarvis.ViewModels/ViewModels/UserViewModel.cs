using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UserViewModel
    {
            public Guid uuid { get; set; }

            public string email { get; set; }

            public string username { get; set; }

            public string status { get; set; }

            public RoleViewModel Role { get; set; }
    }
}
