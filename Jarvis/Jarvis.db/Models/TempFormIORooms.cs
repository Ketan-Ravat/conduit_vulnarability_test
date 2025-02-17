using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempFormIORooms
    {
        [Key]
        public Guid temp_formioroom_id { get; set; }
        public string temp_formio_room_name { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }

        [ForeignKey("FormIORooms")]
        public int? formioroom_id { get; set; }

        public bool is_deleted { get; set; }

        public Guid company_id { get; set; }

        [ForeignKey("TempFormIOFloors")]
        public Guid? temp_formiofloor_id { get; set; }
        public virtual ICollection<TempFormIOSections> TempFormIOSections { get; set; }
        public virtual TempFormIOFloors TempFormIOFloors { get; set; }
        public virtual FormIORooms FormIORooms { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }

    }
}
