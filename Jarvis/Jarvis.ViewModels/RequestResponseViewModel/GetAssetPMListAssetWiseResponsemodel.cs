using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMListAssetWiseResponsemodel
    {
        public Guid asset_id { get; set; }
        public string name { get; set; }

        public List<GetAssetPMListOptimizedResponsemodel> asset_pms_list { get; set; }

    }
    public class asset_pm_of_asset
    {
        public Guid asset_pm_id { get; set; }
        public Nullable<Guid> pm_id { get; set; }
        public int status { get; set; }
        public string title { get; set; }
        public Guid asset_pm_plan_id { get; set; }

    }
}
