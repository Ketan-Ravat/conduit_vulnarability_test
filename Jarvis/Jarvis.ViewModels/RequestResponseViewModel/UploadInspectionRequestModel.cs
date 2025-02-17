using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class UploadInspectionRequestModel
    {
        public string company_id { get; set; }

        public List<InspectionModel> inspection { get; set; }
    }

    public class InspectionModel
    {
        public string internal_asset_id { get; set; }

        public string name { get; set; }

        public string inspection_date { get; set; }

        public int meter_hours { get; set; }

        public int shift { get; set; }

        public string operator_name { get; set; }

        public List<InspectionAttributes> inspection_form { get; set; }

        public string notes { get; set; }
    }

    public class InspectionAttributes
    {
        public string name { get; set; }

        public string value { get; set; }
    }
}
