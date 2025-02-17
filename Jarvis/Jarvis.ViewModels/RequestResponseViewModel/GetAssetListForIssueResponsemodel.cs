using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetListForIssueResponsemodel
    {
        public List<MainAssetListtoAddIssue> main_assets { get; set; }
        public List<TempAssetListtoAddIssue> temp_assets { get; set; }
    }
    public class MainAssetListtoAddIssue
    {
        public string asset_name { get; set; }
        public Guid asset_id { get; set; }
        public string QR_code { get; set; }

    }
    public class TempAssetListtoAddIssue
    {
        public Guid woonboardingassets_id { get; set; }
        public string asset_name { get; set; }
        public string QR_code { get; set; }
    }
}
