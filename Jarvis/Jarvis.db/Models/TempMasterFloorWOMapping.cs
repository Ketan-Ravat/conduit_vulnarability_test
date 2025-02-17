using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempMasterFloorWOMapping
    {
        [Key]
        public Guid temp_master_floor_wo_mapping_id { get; set; }

        [ForeignKey("TempMasterFloor")]
        public Guid temp_master_floor_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public virtual TempMasterFloor TempMasterFloor { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual ICollection<TempMasterRoomWOMapping> TempMasterRoomWOMapping { get; set; }
    }
}
