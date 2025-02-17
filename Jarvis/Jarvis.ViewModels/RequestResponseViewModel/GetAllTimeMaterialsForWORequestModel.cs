using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllTimeMaterialsForWORequestModel
    {
        public Guid wo_id { get; set; }
        public int? time_material_category_type { get; set; } // 1 = labor , 2 = materials , 3 = miscellaneous
        public string search_string { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }

    }
}
