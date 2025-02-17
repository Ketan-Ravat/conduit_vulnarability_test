using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOOBAssetFedByMapping
    {
        [Key]
        public Guid wo_ob_asset_fed_by_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }
        [ForeignKey("Sites")]
        public Guid? site_id { get; set; }
        public Guid parent_asset_id { get; set; }
        public bool is_parent_from_ob_wo { get; set; }
        public int? fed_by_usage_type_id { get; set; } // 1- normal , 2- emergency
        public Guid? via_subcomponant_asset_id { get; set; } //this will act as main OCP / current asset
        public bool is_via_subcomponant_asset_from_ob_wo { get; set; }
        public Guid? fed_by_via_subcomponant_asset_id { get; set; } // fed by subcomponent, this will act as OCP
        public bool is_fed_by_via_subcomponant_asset_from_ob_wo { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? updated_at{ get; set; }
        public string updated_by { get; set; }
        public bool is_deleted { get; set; }
        public string length { get; set; }   // length of conductor
        public string style { get; set; }   // size of conductor
        public int? number_of_conductor { get; set; }    // # of conductor
        public int? conductor_type_id { get; set; }    // 1 = Copper , 2 = Aluminum
        public int? raceway_type_id { get; set; }    // 1 = Metallic , 2 = NonMetallic
        public WOOnboardingAssets WOOnboardingAssets { get; set; }
        public Sites Sites { get; set; }
    }
}
