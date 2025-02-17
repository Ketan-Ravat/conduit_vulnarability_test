using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetsForClusterResponsemodel
    {
        public Guid asset_id { get; set; }
        public Guid? temprory_asset_id { get; set; }// this key is only to check if asset is added from existing and fed by is added when adding existing asset to wo
        public string internal_asset_id { get; set; }
        public string name { get; set; }
        public bool is_child_available { get; set; }
        public int? asset_operating_condition_state { get; set; }
        public bool is_asset_temp { get; set; } // this is for if asset is main or temp
        public int? temp_asset_woline_status { get; set; } // this is for if asset is main or temp
        public bool is_main_asset_assigned { get; set; }// this is for if asset is main but it is assigned to WO
        public int? formiofloor_id { get; set; }
        public int? formiobuilding_id { get; set; }
        public int? formioroom_id { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string asset_class_name { get; set; }
        public string asset_class_code { get; set; }
        public int? criticality_index_type { get; set; }
        public string asset_class_type { get; set; }

        public List<AssetChildrenMappingForCluster> children_list { get; set; }
        public List<ClusterSubcomponents> subcomponent_list { get; set; }

    }
    public class ClusterSubcomponents
    {
        public string asset_name { get; set; }
        public Guid asset_id { get; set; }
        public bool is_asset_temp { get; set; }
        public int? asset_operating_condition_state { get; set; }
    }
    public class AssetChildrenMappingForCluster
    {
        public Guid asset_children_hierrachy_id { get; set; }
        public Guid? children_asset_id { get; set; }
        public bool is_asset_temp { get; set; }
        public bool is_deleted { get; set; }// for BE use only
        public Guid? ocp_main_subcomponent_asset { get; set; } // fedby's subcomponent asset id
        public Guid? ocp_subcomponent_asset { get; set; } // current asset's subcomponent asset
    }
}
