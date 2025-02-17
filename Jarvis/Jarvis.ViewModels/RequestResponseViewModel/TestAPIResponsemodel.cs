using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class TestAPIResponsemodel
    {
        public string access_key { get; set; }
        public string secret_key { get; set; }
        public string cognito_mfa_access_key { get; set; }
        public string cognito_mfa_secret_key { get; set; }
    }
}
