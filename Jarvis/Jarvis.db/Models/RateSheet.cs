using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jarvis.db.Models
{
    public class RateSheet
    {
        [Key]
        public Guid ratesheet_id { get; set; }
        public int time_material_category_type { get; set; } // 1 = labor , 2 = materials , 3 = miscellaneous
        public string description { get; set; }
        public bool no_sub_flag { get; set; }
        public double rate { get; set; }
        public double markup { get; set; }
        public string item_code { get; set; }
        public double? burden { get; set; }
        public int? burden_type { get; set; } // $ = 1 , % = 2
        public bool is_burden_enabled { get; set; }
        public bool is_markup_enabled { get; set; }

        [ForeignKey("Company")]
        public Guid company_id { get; set; }
        public DateTime? created_at { get; set; }
        public string created_by { get; set; }
        public DateTime? modified_at { get; set; }
        public string modified_by { get; set; }
        public bool is_deleted { get; set; }

        public Company Company { get; set; }

        //public double amount { get; set; } // quantity * rate
        // public double? total_of_burden { get; set; }
        //public double quantity { get; set; }
        //public int quantity_unit_type { get; set; } // 1 = feet , 2 = ?
        //public double total_of_markup { get; set; }
    }
}
