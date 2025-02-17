using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddPMtoNewWolineResponsemodel
    {
        public List<WOlinePMList> WolinePMlist { get; set; }

    }

    public class WOlinePMList
    {
        public Guid temp_asset_pm_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public int? pm_inspection_type_id { get; set; }
    }
}
