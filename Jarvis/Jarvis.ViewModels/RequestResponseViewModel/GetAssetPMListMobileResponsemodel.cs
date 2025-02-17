using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMListMobileResponsemodel
    {
        public Guid asset_pm_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public bool is_Asset_PM_fixed { get; set; }//bool use in mobile app as PM is done or not
    }
}
