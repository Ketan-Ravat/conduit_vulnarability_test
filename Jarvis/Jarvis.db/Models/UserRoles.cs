using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class UserRoles
    {
        [Key]
        public Guid userrole_id { get; set; }

        [ForeignKey("User")]
        public Guid user_id { get; set; }

        [ForeignKey("Role")]
        public Guid role_id { get; set; }
        
        public Nullable<DateTime> created_at { get; set; }

        public string created_by { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public string modified_by { get; set; }

        [ForeignKey("StatusMaster")]
        public Nullable<int> status { get; set; }

        public virtual Role Role { get; set; }

        public virtual User User { get; set; }

        public virtual StatusMaster StatusMaster { get; set; }
    }
}