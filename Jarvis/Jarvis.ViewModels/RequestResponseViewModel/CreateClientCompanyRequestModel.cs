using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateClientCompanyRequestModel
    {
        public string client_company_name { get; set; }
        public int status { get; set; }
        public string client_company_code { get; set; }
        public string owner { get; set; }
        public string owner_address { get; set; }
        public Guid? client_company_id { get; set; }

    }
}
