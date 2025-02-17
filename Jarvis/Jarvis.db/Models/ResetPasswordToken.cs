using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class ResetPasswordToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int token_id { get; set; }

        public string? email { get; set; }

        public string? token { get; set; }

        [ForeignKey("user")]
        public Guid user_id { get; set; }

        public bool is_used { get; set; }

        public DateTime? used_at { get; set; }

        public Nullable<DateTime> created_at { get; set; }

        public Nullable<DateTime> modified_at { get; set; }

        public User user { get; set; }
    }
}
