using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class TempFormIOSections
    {
        [Key]
        public Guid temp_formiosection_id { get; set; }
        public string temp_formio_section_name { get; set; }

        [ForeignKey("WorkOrders")]
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
        public DateTime? created_at { get; set; }
        public Guid site_id { get; set; }
        public Guid company_id { get; set; }

        [ForeignKey("TempFormIORooms")]
        public Guid? temp_formioroom_id { get; set; }
        [ForeignKey("FormIOSections")]
        public int? formiosection_id { get; set; }
        public virtual TempFormIORooms TempFormIORooms { get; set; }
        public virtual WorkOrders WorkOrders { get; set; }
        public virtual FormIOSections FormIOSections { get; set; }

    }
}
