using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetInspectionFormResponseModel
    {
        public string inspection_form_id { get; set; }
        public string name { get; set; }
        public string company_id { get; set; }
        public string category_name { get; set; }
        public string site_id { get; set; }
        //public string inspection_form_type_id { get; set; }
        public Nullable<int> status { get; set; }
        public string status_name { get; set; }
        public List<FormAttributesJsonObjectViewModel> form_attributes { get; set; }
        public List<CategoryWiseAttributes> categoryAttributeList { get; set; }
    }

    public class CategoryWiseAttributes
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public string spanish_name { get; set; }
        public List<FormAttributesJsonObjectViewModel> form_attributes { get; set; }
    }
}
