using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetFedByCircuitRequestmodel
    {
        public Guid asset_parent_hierrachy_id { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } 
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public string length { get; set; }
        public string style { get; set; }
        public int? number_of_conductor { get;set; }
        public int? conductor_type_id { get;set; }
        public int? raceway_type_id { get;set; }
    }
}
