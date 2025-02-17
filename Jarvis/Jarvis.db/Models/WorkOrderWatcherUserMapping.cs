using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class WorkOrderWatcherUserMapping
    {
        [Key]
        public Guid wo_watcher_user_mapping_id { get; set; }
        public Guid ref_id { get; set; } 
        public int ref_type { get; set; }  // 1 = Workorder , 2 = ?
        public Guid user_id { get; set; }
        public int user_role_type { get; set; } // 1 = Back-Office , 2 = Technician
        public Guid? site_id { get; set; }
        public bool is_deleted { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_at { get; set; }
        public string modified_by { get; set; }

    }
}
