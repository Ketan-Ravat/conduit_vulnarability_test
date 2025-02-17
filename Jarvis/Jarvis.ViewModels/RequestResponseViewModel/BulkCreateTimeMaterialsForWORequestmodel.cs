using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class BulkCreateTimeMaterialsForWORequestmodel
    {
        public List<AddUpdateTimeMaterialRequestModel> time_materials_list {  get; set; }
    }
    /*
    public class BulkCreateTimeMaterialsForWO_Data
    {
        public Guid wo_id { get; set; }
        public int time_material_category_type { get; set; } // 1 = labor , 2 = materials , 3 = miscellaneous
        public string description { get; set; }
        public bool no_sub_flag { get; set; }
        public double quantity { get; set; }
        public int quantity_unit_type { get; set; } // 1 = feet , 2 = ?
        public double rate { get; set; }
        public double amount { get; set; } // quantity * rate
        public double markup { get; set; }
        public double total_of_markup { get; set; }
    }
    */
}
