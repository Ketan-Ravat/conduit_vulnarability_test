using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class InspectionFormRequestViewModel
    {

        public string inspection_form_id { get; set; }
        public string name { get; set; }
        public string company_id { get; set; }
        public string site_id { get; set; }
        //public string inspection_form_type_id { get; set; }
        public Nullable<int> status { get; set; }
        public FormAttributesJsonObjectViewModel[] form_attributes { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }
        public string uid { get; set; }
    }
}
