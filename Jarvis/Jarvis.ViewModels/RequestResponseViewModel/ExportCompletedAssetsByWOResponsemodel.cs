using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ExportCompletedAssetsByWOResponsemodel
    {
        public List<ExportCompletedAssets> asset_list { get; set; }
    }
    public class ExportCompletedAssets
    {
        public string form_retrived_asset_name { get; set; }
        public DateTime? intial_form_filled_date { get; set; }
    }
}
