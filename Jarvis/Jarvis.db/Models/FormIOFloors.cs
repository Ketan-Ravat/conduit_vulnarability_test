using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIOFloors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int formiofloor_id { get; set; }
        public string formio_floor_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }
        [ForeignKey("FormIOBuildings")]
        public int? formiobuilding_id { get; set; }
        public virtual ICollection<FormIORooms> FormIORooms { get; set; }
        public virtual ICollection<TempFormIOFloors> TempFormIOFloors { get; set; }
        public  virtual FormIOBuildings FormIOBuildings { get; set; }
    }
}
