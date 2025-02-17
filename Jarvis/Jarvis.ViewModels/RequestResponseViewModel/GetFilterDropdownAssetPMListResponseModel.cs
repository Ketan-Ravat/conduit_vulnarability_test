using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFilterDropdownAssetPMListResponseModel
    {
        public List<FilterDropdownAssetPMListClassCodeList> asset_class_list { get; set; }
        public List<string> pm_title_list { get; set; }

    }
    public class FilterDropdownAssetPMListClassCodeList
    {
        public string asset_class_code { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_name { get; set; }
    }
}
