using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class VerifyCognitoTokenResponseModel
    {
        public long success { get; set; }
        public string message { get; set; }
        public CognitoTokenUserDetails data { get; set; }
    }
    public class CognitoTokenUserDetails
    {
        public string ac_default_role_app { get; set; }
        public string ac_default_role_web { get; set; }
        public string ac_default_site { get; set; }
        public string barcode_id { get; set; }
        public string created_at { get; set; }
        public string created_by { get; set; }
        //public Nullable<int> default_app { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string modified_at { get; set; }
        public string modified_by { get; set; }
        public string notification_token { get; set; }
        public string os { get; set; }
        public Nullable<int> status { get; set; }
        //public string ti_default_role { get; set; }
        //public string ti_default_site { get; set; }
        public string username { get; set; }
        public string uuid { get; set; }
    }
}
