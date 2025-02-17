using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIORooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int formioroom_id { get; set; }
        public string formio_room_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        [ForeignKey("FormIOFloors")]
        public int? formiofloor_id { get; set; }

        public int? room_conditions { get; set; }

        public string access_notes { get; set; }

        public string issue { get; set; }
        public virtual ICollection<FormIOSections> FormIOSections { get; set; }
        public virtual ICollection<TempFormIORooms> TempFormIORooms { get; set; }
        public virtual FormIOFloors FormIOFloors { get; set; }

        public virtual ICollection<FormIORoomsImagesMapping> FormIORoomsImagesMapping { get; set; }
    }
}
