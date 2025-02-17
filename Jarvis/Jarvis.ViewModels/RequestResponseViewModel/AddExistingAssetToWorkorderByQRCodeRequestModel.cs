using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddExistingAssetToWorkorderByQRCodeRequestModel
    {
        public string qr_code { get; set; }
        public Guid wo_id { get; set; }
        public string temp_formio_building_name { get; set; }
        public string temp_formio_floor_name { get; set; }
        public string temp_formio_room_name { get; set; }
    }
}
