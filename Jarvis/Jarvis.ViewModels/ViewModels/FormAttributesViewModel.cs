using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class FormAttributesViewModel
    {
        public Guid attributes_id { get; set; }
        public string name { get; set; }
        public int values_type { get; set; }
        public string company_id { get; set; }
        public string site_id { get; set; }
        public int category_id { get; set; }

        public string categoryname { get; set; }
        public AttributesJsonObjectViewModel[] value_parameters { get; set; }
        //public InspectionAttributeCategoryViewModel InspectionAttributeCategory { get; set; }
    }
}
