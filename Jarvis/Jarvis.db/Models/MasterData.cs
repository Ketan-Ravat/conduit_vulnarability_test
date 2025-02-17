using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class MasterData
    {
        [Key]
        public int master_data_id { get; set; }

        public string meter_hours { get; set; }
    }
}
