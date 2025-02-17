using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateDefaultCompanyRequestModel
    {
        //public Guid user_id { get; set; }

        //public Guid requested_by { get; set; }

        public Guid company_id { get; set; }
        public Guid site_id { get; set; } =  Guid.Empty;
    }
}
