using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetsListForReactFlowResponseModel
    {
        public List<asset_node_main_class> list { get; set; }
        public List<initialEdges_class> initialEdges { get; set; }
    }
    public class initialEdges_class
    {
        public string id { get; set;}
        public string source { get; set;}
        public string target { get; set;}
        public Guid? via_subcomponent_asset_id { get; set; }        //this will act as  Main-OCP / current asset
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent this will act as OCP 
        public string label { get; set; }
        public style_class style { get; set; }
    }
    public class asset_node_main_class
    {
        public string asset_id { get; set; } 
        public string type { get; set; }
        public string asset_node_data_json { get; set; }
        public asset_node_position position { get; set; }
        public asset_node_data_class data { get; set; }
        public string parent_id { get; set; } // top level component id
        public bool selected { get; set; }  = false;
        public bool dragging { get; set; } = false;
        public string extent { get; set; }

    }
    public class asset_node_position
    {
        public int? x { get; set; }
        public int? y { get; set; }
    }
    public class asset_node_data_class
    {
        public string code { get; set; }
        public string label { get; set; }
        public string background { get; set; }
        public string border { get; set; }
    }
    public class style_class
    {
        public string stroke { get; set; }
    }
}
