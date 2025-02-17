using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class BulkCreatePMWOlineRequestmodel
    {
        public List<BulkCreatePMWOlineData> pm_list { get; set; }
        public Guid wo_id { get; set; }
    }

    public class BulkCreatePMWOlineData
    {
        public Guid asset_pm_id { get; set; }
    }
}
