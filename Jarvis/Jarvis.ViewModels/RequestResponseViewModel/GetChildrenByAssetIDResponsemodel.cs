using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetChildrenByAssetIDResponsemodel
    {
        public Guid asset_id { get; set; }
        public string asset_name { get; set; }
        public string site_name { get; set; }
        public Guid site_id { get; set; }
    }
}
