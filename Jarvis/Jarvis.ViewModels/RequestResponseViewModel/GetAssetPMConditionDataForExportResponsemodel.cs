using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMConditionDataForExportResponsemodel
    {
        public string asset_class_name { get; set; }
        public string asset_class_code { get; set; }
        public int condition_1_asset_count { get; set; }
        public int condition_2_asset_count { get; set; }
        public int condition_3_asset_count { get; set; }
    }
}
