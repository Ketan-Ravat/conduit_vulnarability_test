using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class SiteProjectManagerMapping
    {
        [Key]
        public Guid site_projectmanager_mapping_id { get; set; }

        [ForeignKey("Sites")]
        public Guid site_id { get; set; }

        [ForeignKey("User")]
        public Guid user_id { get; set; }

        public DateTime? created_at {  get; set; }
        public DateTime? modified_at {  get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }
        public virtual User User { get; set; }
        public virtual Sites Sites { get; set; }
    }
}
