using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class CreateInspectionOfflineRequestModel
    {
        public string asset_id { get; set; }

        public string internal_asset_id { get; set; }

        public DateTime request_datetime { get; set; }

        public float meter { get; set; }

        public float swift { get; set; }

        public string notes { get; set; }

        public string Inpection_photos { get; set; }
    }
}
