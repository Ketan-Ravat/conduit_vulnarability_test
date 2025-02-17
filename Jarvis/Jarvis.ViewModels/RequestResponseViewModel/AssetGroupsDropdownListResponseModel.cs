using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetGroupsDropdownListResponseModel
    {
        public List<AssetGroupClass_Obj> list { get; set; }
    }
    public class AssetGroupClass_Obj
    {
        public Guid? asset_group_id { get; set; }
        public string asset_group_name { get; set; }
        public string asset_group_description { get; set; }
        public int criticality_index_type { get; set; }
    }
}
