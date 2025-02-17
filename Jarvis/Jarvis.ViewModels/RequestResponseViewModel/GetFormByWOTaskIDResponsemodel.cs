using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFormByWOTaskIDResponsemodel
    {
        public int response_status { get; set; }

        public Guid asset_form_id { get; set; }

        public Guid asset_id { get; set; }

        public Nullable<Guid> form_id { get; set; }

        public string asset_form_name { get; set; }

        public string asset_form_type { get; set; }

        public string asset_form_description { get; set; }

        public string asset_form_data { get; set; }

        public string requested_by { get; set; }
        public string requested_technician_name { get; set; }

        public int status { get; set; }

        public string timezone { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }
        public string asset_name { get; set; }
        public string modified_by { get; set; }
        public string accepted_by { get; set; }
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public DateTime? intial_form_filled_date { get; set; }
        public Guid wo_id { get; set; }
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string form_retrived_workOrderType { get; set; }
    }
}
