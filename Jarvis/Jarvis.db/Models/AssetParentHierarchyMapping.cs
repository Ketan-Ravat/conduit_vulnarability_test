using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class AssetParentHierarchyMapping
    {
        [Key]
        public Guid asset_parent_hierrachy_id { get; set; }
        
        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }
        public Guid? parent_asset_id { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }        //this will act as  Main-OCP / current asset
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent this will act as OCP 
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public string length { get; set; }  // length of conductor
        public string style { get; set; }   // size of conductor
        public int? number_of_conductor { get; set; }    
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
        public string label { get; set; } // edge label 

        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        public Sites Sites { get; set; }
        public Asset Asset { get; set; }

    }
}
