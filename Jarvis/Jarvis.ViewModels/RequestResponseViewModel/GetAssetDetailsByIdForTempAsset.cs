using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetDetailsByIdForTempAsset
    {
        public Guid? asset_id { get; set; }
        public Guid? wo_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
    }
}
