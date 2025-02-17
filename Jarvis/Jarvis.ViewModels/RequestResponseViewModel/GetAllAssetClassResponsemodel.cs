using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetClassResponsemodel
    {
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
        public int form_type_id { get; set; }
        public int? asset_expected_usefull_life { get; set; }
        public string form_type_name { get; set; }
        public Guid pm_category_id { get; set; }
        public string pdf_report_template_url { get; set; }
        public List<PMPlans_for_Class_Obj> pmplans_list { get; set; }
    }
    public class PMPlans_for_Class_Obj
    {
        public Guid pm_plan_id { get; set; }
        public string plan_name { get; set; }
        public bool is_default_pm_plan { get; set; }
    }
}
