using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddExistingtoTempLocationRequestmodel
    {
        public int? formiobuilding_id { get; set; }
        public int? formiofloor_id { get; set; }
        public int? formioroom_id { get; set; }
        public string formio_building_name { get; set; }
        public string formio_floor_name { get; set; }
        public string formio_room_name { get; set; }

        public Guid wo_id { get; set; }
    }
}
