using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class CancelReasonMaster
    {
        [Key]
        public int reason_id { get; set; }
       
        public string name { get; set; }

        public bool is_archive { get; set; }
    }
}
