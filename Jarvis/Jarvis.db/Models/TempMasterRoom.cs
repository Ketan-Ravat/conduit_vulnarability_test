using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempMasterRoom
    {
        [Key]
        public Guid temp_master_room_id { get; set; }
        public string temp_master_room_name { get; set; }

        [ForeignKey("TempMasterFloor")]
        public Guid temp_master_floor_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("FormIORooms")]
        public int? formioroom_id { get; set; }

        public int? room_conditions { get; set; }

        public string access_notes { get; set; }

        public string issue { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }

        public virtual Sites Sites { get; set; }
        public virtual TempMasterFloor TempMasterFloor { get; set; }
        public virtual ICollection<TempMasterRoomWOMapping> TempMasterRoomWOMapping { get; set; }

        public virtual ICollection<TempMasterRoomImagesMapping> TempMasterRoomImagesMapping { get; set; }
    }
}
