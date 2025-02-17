using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class EnableDisableAssetPMsStatusRequestModel
    {
        public Guid asset_pm_id {  get; set; }
        public bool is_assetpm_enabled { get; set; }
    }
}
