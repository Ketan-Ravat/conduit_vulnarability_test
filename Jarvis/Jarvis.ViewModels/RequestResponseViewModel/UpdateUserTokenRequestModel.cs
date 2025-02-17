using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UpdateUserTokenRequestModel
    {
        public string userid { get; set; }

        public string notification_token { get; set; }
    }
}
