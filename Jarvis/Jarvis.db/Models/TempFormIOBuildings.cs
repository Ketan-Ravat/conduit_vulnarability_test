using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempFormIOBuildings
    {
        [Key]
        public Guid temp_formiobuilding_id { get; set; }
        public string temp_formio_building_name { get; set; }
        public Guid site_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        [ForeignKey("FormIOBuildings")]
        public int? formiobuilding_id { get; set; }
        public bool is_deleted { get; set; }

        public Guid company_id { get; set; }
        public DateTime? created_at { get; set; }
        public virtual ICollection<TempFormIOFloors> TempFormIOFloors { get; set; }

        public virtual WorkOrders WorkOrders { get; set; }
        public virtual FormIOBuildings FormIOBuildings { get; set; }

    }
}
