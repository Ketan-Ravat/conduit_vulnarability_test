using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetPMFixStatusRequestmodel
    {
        public List<MobilePMListupdate> asset_pm_list { get; set; }
    }

    public class MobilePMListupdate
    {
        public Guid asset_pm_id { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
    }
}
