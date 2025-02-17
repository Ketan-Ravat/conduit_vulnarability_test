using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetFeedingCircuitForReportResponsemodel
    {
        public List<AssetFeedingCircuitList> asset_feeding_circuit_list { get; set;}
        public int success{ get; set;}
        public string message { get; set;}
    }

    public class AssetFeedingCircuitList
    {
        public Guid feeding_asset_id { get; set;}
        public string feeding_asset_name { get; set;}
        public Guid? fed_by_asset_id { get; set;}
        public string fed_by_asset_name { get; set;}
        public bool is_script_done { get; set; }
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
    }

}
