using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class EmailNotificationStatusUpdate
    {
        [Key]
        public Guid notification_id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string status { get; set; }
        public string subject { get; set; }
        public DateTime submitted_on { get; set; }
        public Nullable<DateTime> retry_on { get; set; }
    }
}
