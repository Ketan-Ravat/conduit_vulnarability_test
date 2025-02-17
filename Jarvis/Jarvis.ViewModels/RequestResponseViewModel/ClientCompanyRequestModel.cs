using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ClientCompanyRequestModel
    {
        public Guid? client_company_id { get; set; }
        public int status { get; set; }
        public List<Guid> site_ids { get; set; }
        public string client_company_name { get; set; }
        public string owner { get; set; }
        public string owner_address { get; set; }
        public Guid parent_company_id { get; set; }
    }
}
