using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddFormIORequestModel
    {
        public Guid form_id { get; set; }

        public string form_name { get; set; }

        public string form_type { get; set; }

        public string form_data { get; set; }

        public Guid? asset_id { get; set; }

        public int status { get; set; }

        //public Guid site_id { get; set; }
        public string form_description { get; set; }


        public Guid company_id { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        public string work_procedure { get; set; }
        public int? form_type_id { get; set; }
        public List<form_dynamic_fields> dynamic_fields { get; set; }
        public string dynamic_nameplate_fields { get; set; }
        public string asset_class_form_properties { get; set; }

        public int? inpsection_form_type { get; set; }
    }

    public class form_dynamic_fields
    {
        public string key { get; set;  }
        public string value { get; set;  }
    }
}
