using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempMasterBuilding
    {
        [Key]
        public Guid temp_master_building_id {  get; set; }
        public string temp_master_building_name { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("FormIOBuildings")]
        public int? formiobuilding_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }

        public virtual Sites Sites { get; set; }
        public virtual FormIOBuildings FormIOBuildings { get; set; }
        public virtual ICollection<TempMasterFloor> TempMasterFloor { get; set; }
        public virtual ICollection<TempMasterBuildingWOMapping> TempMasterBuildingWOMapping { get; set; }
    }
}
