using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempMasterBuildingWOMapping
    {
        [Key]
        public Guid temp_master_building_wo_mapping_id { get; set; }

        [ForeignKey("TempMasterBuilding")]
        public Guid temp_master_building_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public virtual TempMasterBuilding TempMasterBuilding { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual ICollection<TempMasterFloorWOMapping> TempMasterFloorWOMapping { get; set; }
    }
}
