using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempMasterFloor
    {
        [Key]
        public Guid temp_master_floor_id { get; set; }
        public string temp_master_floor_name { get; set; }

        [ForeignKey("TempMasterBuilding")]
        public Guid temp_master_building_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("FormIOFloors")]
        public int? formiofloor_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }

        public virtual Sites Sites { get; set; }
        public virtual TempMasterBuilding TempMasterBuilding { get; set; }
        public virtual FormIOFloors FormIOFloors { get; set; }
        public virtual ICollection<TempMasterRoom> TempMasterRoom { get; set; }
        public virtual ICollection<TempMasterFloorWOMapping> TempMasterFloorWOMapping { get; set; }
    }
}
