using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UserPoolResponseModel
    {
        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }
        public string domain_name { get; set; }

        public string identity_pool_id { get; set; }

        public string region { get; set; }

        public string user_pool_id { get; set; }

        public string user_pool_web_client_id { get; set; }
        public int? cognito_mfa_timer { get; set; }
    }
}
