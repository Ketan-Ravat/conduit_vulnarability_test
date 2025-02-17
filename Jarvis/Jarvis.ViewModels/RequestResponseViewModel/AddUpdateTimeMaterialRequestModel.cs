using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateTimeMaterialRequestModel
    {
        public Guid? time_material_id { get; set; }
        public Guid wo_id { get; set; }
        public int time_material_category_type { get; set; } // 1 = labor , 2 = materials , 3 = miscellaneous
        public string description { get; set; }
        public bool no_sub_flag { get; set; }
        public bool is_deleted { get; set; } 
        public double quantity { get; set; }
        public int quantity_unit_type { get; set; } // 1 = feet , 2 = ?
        public double rate { get; set; }
       // public double amount { get; set; } // quantity * rate
        public double markup { get; set; }
       // public double total_of_markup { get; set; }
        public string item_code { get; set; }
        public int? burden_type { get; set; } // $ = 1 , % = 2
        public double? burden { get; set; }
        public bool is_burden_enabled { get; set; }
        public bool is_markup_enabled { get; set; }
    }
}
