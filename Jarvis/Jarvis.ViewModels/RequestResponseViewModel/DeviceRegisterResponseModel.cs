using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DeviceRegisterResponseModel
    {
        public int status { get; set; }
        public string company_name { get; set; }
        public string company_code { get; set; }
        public string domain_name { get; set; }
        public string company_id { get; set; }
        public string device_code { get; set; }
        public string device_uuid { get; set; }
        public string identity_pool_id { get; set; }
        public string region { get; set; }
        public string user_pool_id { get; set; }
        public string user_pool_web_client_id { get; set; }
        public string company_logo { get; set; }
        public string company_thumbnail_logo { get; set; }
        public int? cognito_mfa_timer { get; set; }
    }
}
