using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public partial class Role
    {

        public Role()
        {
            //User = new HashSet<User>();
        }

        [Key]
        public Guid role_id { get; set; }
        public string name { get; set; }

        //public virtual ICollection<User> User { get; set; }
    }
}
