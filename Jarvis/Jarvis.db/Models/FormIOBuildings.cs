using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class FormIOBuildings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int formiobuilding_id { get; set; }
        public string formio_building_name { get; set; }
        public Guid  site_id { get; set; }
        public Guid company_id { get; set; }
        public DateTime? created_at { get; set; }
        public virtual ICollection<FormIOFloors> FormIOFloors { get; set; }
        public virtual ICollection<TempFormIOBuildings> TempFormIOBuildings { get; set; }
    }
}
