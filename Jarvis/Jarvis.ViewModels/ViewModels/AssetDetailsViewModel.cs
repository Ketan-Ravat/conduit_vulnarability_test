using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.ViewModels
{
    public class AssetDetailsViewModel
    {
        public Guid asset_id { get; set; }

        public string asset_photo { get; set; }

        public string internal_asset_id { get; set; }

        public Nullable<int> status { get; set; }

        public string status_name { get; set; }

        public Guid inspectionform_id { get; set; }

        public string notes { get; set; }

        public Nullable<int> usage { get; set; }

        public Nullable<long> meter_hours { get; set; }

        public string name { get; set; }

        public string asset_type { get; set; }

        public string product_name { get; set; }

        public string model_name { get; set; }

        public string asset_serial_number { get; set; }

        public string model_year { get; set; }

        public string site_location { get; set; }

        public string current_stage { get; set; }

        public string parent { get; set; }

        public string children { get; set; }

        public Guid site_id { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }

        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }

        public string timeelapsed { get; set; }

        public string timezone { get; set; }
    }
}