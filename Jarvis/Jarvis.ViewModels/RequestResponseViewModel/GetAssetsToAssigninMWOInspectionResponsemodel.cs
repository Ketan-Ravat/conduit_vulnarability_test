using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsToAssigninMWOInspectionResponsemodel
    {
        public List<GetAssetsToAssignResponsemodel> main_asset_list { get; set; }
        public List<OBWOAssetDetails> ob_wo_asset_list { get; set; }

    }
}
