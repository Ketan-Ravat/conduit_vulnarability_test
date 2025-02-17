using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class StatusTypes
    {
        public StatusTypes()
        {
           Status  = new HashSet<StatusMaster>();
        }

        [Key]
        public int status_type_id { get; set; }
        public string status_type_name { get; set; }
        public virtual ICollection<StatusMaster> Status { get; set; }
    }
}
