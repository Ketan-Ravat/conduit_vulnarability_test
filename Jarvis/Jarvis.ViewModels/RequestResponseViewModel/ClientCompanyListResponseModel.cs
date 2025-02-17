using System;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ClientCompanyListResponseModel
    {
        public Guid client_company_id { get; set; }
        public string client_company_name { get; set; }
        public Nullable<int> status { get; set; }
        public string owner { get; set; }
        public string owner_address { get; set; }
    }
}
