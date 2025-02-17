using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMFormByIdRequestmodel
    {
        public Guid? asset_pm_id{get; set; }
        public Guid? temp_asset_pm_id { get; set; }
        public Guid woonboardingassets_id { get; set; }

    }
}
