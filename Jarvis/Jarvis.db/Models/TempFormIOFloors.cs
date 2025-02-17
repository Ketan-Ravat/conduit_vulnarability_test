using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempFormIOFloors
    {
        [Key]
        public Guid temp_formiofloor_id { get; set; }
        public string temp_formio_floor_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        [ForeignKey("FormIOFloors")]
        public int? formiofloor_id { get; set; }

        public bool is_deleted { get; set; }
        public Guid company_id { get; set; }

        [ForeignKey("TempFormIOBuildings")]
        public Guid? temp_formiobuilding_id { get; set; }
        public virtual ICollection<TempFormIORooms> TempFormIORooms { get; set; }
        public virtual TempFormIOBuildings TempFormIOBuildings { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual FormIOFloors FormIOFloors { get; set; }

    }
}
