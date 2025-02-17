using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetGroupsListResponseModel
    {
        public List<GetAllAssetGroupsList_Class> list { get; set; }

        public int count { get; set; }

    }
    public class GetAllAssetGroupsList_Class
    {
        public Guid? asset_group_id { get; set; }
        public string asset_group_name { get; set; }
        public string asset_group_description { get; set; }
        public int criticality_index_type { get; set; }
        public List<AssetGroup_AssetData_Class> asset_list { get; set; }
    }
    public class AssetGroup_AssetData_Class
    {
        public Guid asset_id { get; set; }
        public string name { get; set; }
        public int? criticality_index_type { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public double asset_health_index { get; set; }
        public string asset_health_json { get; set; }
    }
}
