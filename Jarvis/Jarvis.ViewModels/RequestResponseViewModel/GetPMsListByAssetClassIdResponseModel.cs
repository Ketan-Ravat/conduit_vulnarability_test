using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMsListByAssetClassIdResponseModel
    {
        public Guid pm_id { get; set; }
        public string title { get; set; }
        public Guid pm_plan_id { get; set; }
        public string plan_name { get; set; }
        public Guid pm_category_id { get; set; }
        public string category_name { get; set; }
        public string form_name { get; set; }
        public Nullable<int> pm_inspection_type_id { get; set; } // 1 = Infrared ThermoGraphy

    }
}
