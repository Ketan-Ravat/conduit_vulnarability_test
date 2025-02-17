using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIOLocationNotes
    {
        [Key]
        public int FormIOLocationNotes_id { get; set; }
        public DateTime? created_at { get; set; }

        [ForeignKey("FormIOBuildings")]
        public int? formiobuilding_id { get; set; }

        [ForeignKey("FormIOFloors")]
        public int? formiofloor_id { get; set; }

        [ForeignKey("FormIORooms")]
        public int? formioroom_id { get; set; }

        [ForeignKey("FormIOSections")]
        public int? formiosection_id { get; set; }

       public string notes { get; set; }
        public virtual FormIOBuildings FormIOBuildings { get; set; }
        public virtual FormIOFloors FormIOFloors { get; set; }
        public virtual FormIORooms FormIORooms { get; set; }
        public virtual FormIOSections FormIOSections { get; set; }
    }
}
