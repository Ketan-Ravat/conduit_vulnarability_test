using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class IssueStatus
    {
        [Key]
        public Guid issue_status_id { get; set; }

        [ForeignKey("Issue")]
        public Guid issue_id { get; set; }

        public int status { get; set; }

        public string modified_by { get; set; }

        public DateTime modified_at { get; set; }

        public Issue Issue { get; set; }
    }
}