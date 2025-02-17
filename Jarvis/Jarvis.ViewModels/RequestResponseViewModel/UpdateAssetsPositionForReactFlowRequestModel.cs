using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetsPositionForReactFlowRequestModel
    {
        public List<update_asset_node_data_class> assets { get; set; }
        public List<update_edges_obj> edges { get; set; }
    }
    public class update_asset_node_data_class
    {
        public string asset_id { get; set; }
        public int x_axis { get; set; }
        public int y_axis { get; set; }
        //public string asset_name { get; set; }
        //public string asset_class_code { get; set; }
        public string asset_node_data_json { get; set; }
    }
    public class update_edges_obj
    {
        public string id { get; set; }//asset_parent_hierrachy_id
        public Guid? source { get; set; }//parent_asset_id
        public Guid? target { get; set; }//asset_id
        public Guid? via_subcomponent_asset_id { get; set; }        //this will act as  Main-OCP / current asset
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent this will act as OCP 
        public bool is_deleted { get; set; }
        public string label { get; set; }
        public string style { get; set; }
    }
}
