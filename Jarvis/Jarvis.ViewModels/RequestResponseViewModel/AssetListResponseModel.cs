using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetListResponseModel
    {
        public Guid asset_id { get; set; }
        public string name { get; set; }
        public string internal_asset_id { get; set; }
    }
}
