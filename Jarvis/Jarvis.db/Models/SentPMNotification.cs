using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models {
    public class SentPMNotification {
        [Key]
        public long id { get; set; }
        public Guid manager_id { get; set; }
        public Guid trigger_id { get; set; }
        public int notification_type { get; set; }
    }
}
