using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class UserSession
    {
        [Key]
        public Guid user_session_id { get; set; }

        [ForeignKey("User")]
        public Guid user_id { get; set; }
        public Guid? device_id { get; set; }
        public Guid role_id { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public virtual User User { get; set; }
    }
}
