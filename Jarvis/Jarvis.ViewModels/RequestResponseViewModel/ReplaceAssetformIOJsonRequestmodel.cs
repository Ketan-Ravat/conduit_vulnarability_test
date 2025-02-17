using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ReplaceAssetformIOJsonRequestmodel
    {
        public Guid asset_form_io { get; set; }

        public string asset_form_json { get; set; }
        public string identification { get; set; }
        public string parent { get; set; }
        public string assetId { get; set; }
    }
}
