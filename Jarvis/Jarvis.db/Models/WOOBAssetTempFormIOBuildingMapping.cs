using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class WOOBAssetTempFormIOBuildingMapping
    {
        [Key]
        public Guid wo_ob_asset_temp_formiobuilding_id { get; set; }
        public DateTime? created_at { get; set; }

        [ForeignKey("TempFormIOBuildings")]
        public Guid? temp_formiobuilding_id { get; set; }

        [ForeignKey("TempFormIOFloors")]
        public Guid? temp_formiofloor_id { get; set; }

        [ForeignKey("TempFormIORooms")]
        public Guid? temp_formioroom_id { get; set; }

        [ForeignKey("TempFormIOSections")]
        public Guid? temp_formiosection_id { get; set; }

        [ForeignKey("WOOnboardingAssets")]
        public Guid? woonboardingassets_id { get; set; }

        public virtual WOOnboardingAssets WOOnboardingAssets { get; set; }
        public virtual TempFormIOBuildings TempFormIOBuildings { get; set; }
        public virtual TempFormIOFloors TempFormIOFloors { get; set; }
        public virtual TempFormIORooms TempFormIORooms { get; set; }
        public virtual TempFormIOSections TempFormIOSections { get; set; }
    }
}
