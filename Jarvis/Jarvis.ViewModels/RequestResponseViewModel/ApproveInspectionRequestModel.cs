using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class ApproveInspectionRequestModel
    {
        public Guid inspection_id { get; set; }

        public Guid asset_id { get; set; }

        public string manager_id { get; set; }

        public int status { get; set; }

        public int meter_hours { get; set; }

        //public AssetsValueJsonObjectViewModel[] attribute_values { get; set; }

        public string manager_notes { get; set; }
    }
}
