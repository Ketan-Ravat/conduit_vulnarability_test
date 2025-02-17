using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class LanguageMaster
    {
        [Key]
        public int language_id { get; set; }
        public string language_name { get; set; }
        [ForeignKey("StatusMaster")]
        public int is_active { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
    }
}
