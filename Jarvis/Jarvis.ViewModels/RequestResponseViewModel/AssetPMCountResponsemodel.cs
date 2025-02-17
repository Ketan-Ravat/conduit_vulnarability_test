using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssetPMCountResponsemodel
    {
        public int open_asset_pm_count { get; set; }    
        public int due_asset_pm_count { get; set; }
        public int overdue_asset_pm_count { get; set; }
        public int completed_asset_pm_count { get; set; }
        public int scheduled_asset_pm_count { get; set; }
    }
}
