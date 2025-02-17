using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetComponantLevelAssetsRequestmodel
    {
        //public int pagesize { get; set; } = 0;
        //public int pageindex { get; set; } = 0;
        public int? component_level_type_id { get; set; }
        //public string search_string { get; set; }
        public Guid? wo_id { get; set; }
    }
}
