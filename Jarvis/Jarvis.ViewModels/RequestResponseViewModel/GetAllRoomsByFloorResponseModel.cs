using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllRoomsByFloorResponseModel
    {
        public List<GetAllRoomsByFloorData> room_list { get; set; }
        public int listsize { get;set; }
    }
    public class GetAllRoomsByFloorData
    {
        public int formioroom_id { get; set; }
        public string formio_room_name { get; set; }
        public int? formiofloor_id { get; set; }
        public int asset_count { get; set; }
    }
}
