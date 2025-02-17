using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetFeedingCircuitRequestmodel
    {
        public Guid asset_children_hierrachy_id { get; set; }
        public Guid? via_subcomponent_asset_id { get; set; }
        public string circuit { get; set; }
    }
}
