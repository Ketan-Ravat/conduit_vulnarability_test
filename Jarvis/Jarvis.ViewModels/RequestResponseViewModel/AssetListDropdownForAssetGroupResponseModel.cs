using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetListDropdownForAssetGroupResponseModel
    {      
        public List<Asset_Obj> list { get; set; }
    }
    public class Asset_Obj
    {
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }

    }
}
