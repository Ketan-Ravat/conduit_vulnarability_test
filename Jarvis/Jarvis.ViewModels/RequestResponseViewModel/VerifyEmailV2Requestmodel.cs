using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class VerifyEmailV2Requestmodel
    {
        public string email { get; set; }
        public string company_id { get; set; }
        public string domain_name { get; set; }
    }
}
