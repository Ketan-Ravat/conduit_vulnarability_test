using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempAsset  // this table is to store asset related content for wolines
    {
        [Key]
        public Guid tempasset_id { get; set; }
        
        public string asset_name { get; set; }
        public string QR_code { get; set; }
        public int? condition_index_type { get; set; }
        public int? criticality_index_type { get; set; }
        public DateTime? commisiion_date { get; set; }
        public string form_nameplate_info { get; set; }
        public int? maintenance_index_type { get; set; } // 1 = Serviceable, 2 = Limited Service , 3 = Nonserviceable
        public int? component_level_type_id { get; set; } // 1 for top_level , 2- subcomponant
        public int? asset_operating_condition_state { get; set; } // Operating Normally,Repair Needed,Replacement Needed,Repair Scheduled,Replacement Scheduled,Decomissioned,Spare
        public int? code_compliance { get; set; } // compliant  , non-compliant 
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public int? panel_schedule { get; set; }
        public int? arc_flash_label_valid { get; set; } // 1 = Yes, 2 = No

        [ForeignKey("Asset")]
        public Guid? asset_id { get; set; }// if WOline is added from main asset

        public Guid? new_created_asset_id { get; set; } // update main asset_id to this coloumn when complete WO 

        [ForeignKey("InspectionTemplateAssetClass")]
        public Guid? inspectiontemplate_asset_class_id { get; set; }

        [ForeignKey("TempFormIOBuildings")]
        public Guid? temp_formiobuilding_id { get; set; }

        [ForeignKey("TempFormIOFloors")]
        public Guid? temp_formiofloor_id { get; set; }

        [ForeignKey("TempFormIORooms")]
        public Guid? temp_formioroom_id { get; set; }

        [ForeignKey("TempFormIOSections")]
        public Guid? temp_formiosection_id { get; set; }

        // new flow for TempMasterLocations
        [ForeignKey("TempMasterBuilding")]
        public Guid? temp_master_building_id { get; set; }

        [ForeignKey("TempMasterFloor")]
        public Guid? temp_master_floor_id { get; set; }

        [ForeignKey("TempMasterRoom")]
        public Guid? temp_master_room_id { get; set; }
        public string temp_master_section { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        
        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("AssetGroup")]
        public Guid? asset_group_id { get; set; }

        public InspectionTemplateAssetClass InspectionTemplateAssetClass { get; set; }
        public virtual TempFormIOBuildings TempFormIOBuildings { get; set; }
        public virtual TempFormIOFloors TempFormIOFloors { get; set; }
        public virtual TempFormIORooms TempFormIORooms { get; set; }
        public virtual TempFormIOSections TempFormIOSections { get; set; }
        public virtual TempMasterBuilding TempMasterBuilding { get; set; }
        public virtual TempMasterFloor TempMasterFloor { get; set; }
        public virtual TempMasterRoom TempMasterRoom { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual Sites Sites { get; set; }
        public virtual AssetGroup AssetGroup { get; set; }
        public virtual ICollection<SitewalkthroughTempPmEstimation> SitewalkthroughTempPmEstimation { get; set; }
        public ICollection<WOOnboardingAssets> WOOnboardingAssets { get; set; }
    }
}
