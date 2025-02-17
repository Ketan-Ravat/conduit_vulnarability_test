using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOOBAssetsbyLocationHierarchyRequestModel
    {
        public int pagesize { get; set; } = 0;
        public int pageindex { get; set; } = 0;
        public Guid wo_id { get; set; }
        public Guid? temp_formiobuilding_id { get; set; }
        public Guid? temp_formiofloor_id { get; set; }
        public Guid? temp_formioroom_id { get; set; }
        public Guid? temp_master_room_id { get; set; }
        public string search_string { get; set; }
        public string buildig_name { get; set; }
        public string floor_name { get; set; }
        public string room_name { get; set; }
    }
}
