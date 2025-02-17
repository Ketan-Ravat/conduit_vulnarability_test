using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddTempMasterLocationDataMainFunctionRequestModel
    {
        public string temp_building { get; set; }
        public string temp_floor { get; set; }
        public string temp_room { get; set; }
        public Guid? temp_master_building_id { get; set; }
        public Guid? temp_master_floor_id { get; set; }
        public Guid? temp_master_room_id { get; set; }
        public Guid wo_id { get; set; }

    }
}
