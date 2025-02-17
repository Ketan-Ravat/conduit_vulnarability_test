using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsbyLocationHierarchyRequestmodel
    {
        public int pagesize { get; set; } = 0;
        public int pageindex { get; set; } = 0;
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
        public string search_string { get; set; }
    }
}
