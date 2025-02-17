using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetNameplateInfoByAssetidResponsemodel
    {
        public string form_retrived_nameplate_info { get; set; }
        public Guid asset_id { get; set; }
    }
}
