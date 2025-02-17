using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetCircuitDetailsResponsemodel
    {
        public List<FedByAssetsResponse> fedby_asset_list {  get; set; }
        public List<FeedingAssetsResponse> feeding_asset_list {  get; set; }

    }
    public class FedByAssetsResponse
    {
        public Guid asset_parent_hierrachy_id { get; set; }
        public Guid? parent_asset_id { get; set; }
        public string parent_asset_name { get; set; }
        public string parent_asset_class_name { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }
        public string via_subcomponent_asset_name { get; set; }
        public string amps { get; set; }
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public string length { get; set; }
        public string style { get; set; }
        public int? number_of_conductor { get; set; }
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
        public Guid? fed_by_via_subcomponant_asset_id { get; set; }
        public string fed_by_via_subcomponent_asset_name { get; set; } 
    }
    public class FeedingAssetsResponse
    {
        public Guid asset_children_hierrachy_id { get; set; }
        public Guid? children_asset_id { get; set; }
        public string children_asset_name { get; set; }
        public string children_asset_clss_name { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }
        public string via_subcomponent_asset_name { get; set; }
        public string circuit { get; set; }
        public string amps { get; set; }
    }
}
