using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetSubcomponentAssetsToAddinAssetResponsemodel
    {
        public string asset_name { get; set; }
        public Guid asset_id { get; set; }
        public string internal_asset_id { get; set; }
        public string QR_code { get; set; }
    }
}
