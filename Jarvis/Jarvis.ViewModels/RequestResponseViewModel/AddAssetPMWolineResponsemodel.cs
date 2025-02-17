using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddAssetPMWolineResponsemodel
    {
        public Guid asset_pm_id { get; set; }
        public Guid woonboardingassets_id { get; set; }
        public int? pm_inspection_type_id { get; set; } // 1 = Infrared ThermoGraphy

    }
}
