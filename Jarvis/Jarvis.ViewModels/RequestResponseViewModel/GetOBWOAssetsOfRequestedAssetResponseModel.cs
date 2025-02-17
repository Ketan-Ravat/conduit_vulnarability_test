using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetOBWOAssetsOfRequestedAssetResponseModel
    {
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public int status { get; set; }
        public Guid wo_id { get; set; }
        public int inspection_type { get; set; }
        public Guid? asset_pm_id { get; set; }
        public Guid? pm_id { get; set; }
        public string asset_pm_title { get; set; }
        public DateTime? inspected_at { get; set; }
        public DateTime? completed_at { get; set; }
        public string modified_by { get; set; }
        public string modified_by_name { get; set; }
        public Nullable<int> pm_inspection_type_id { get; set; } // 1 = Infrared ThermoGraphy
        public int wo_type { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string manual_wo_number { get; set; }
        public int? workOrderStatus { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
    }
}
