using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FormIOResponseModel
    {
        public int response_status { get; set; }

        public Guid form_id { get; set; }

        public string form_name { get; set; }

        public string form_type { get; set; }

        public string form_data { get; set; }

        public Guid? asset_id { get; set; }

        public int status { get; set; }
        public string status_name { get; set; }

        public string form_description { get; set; }

        public Guid company_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

      //  public virtual AssetsResponseModel Asset { get; set; }

        public SitesViewModel Sites { get; set; }

        public string work_procedure { get; set; }
        public int? form_type_id { get; set; }
    }
}
