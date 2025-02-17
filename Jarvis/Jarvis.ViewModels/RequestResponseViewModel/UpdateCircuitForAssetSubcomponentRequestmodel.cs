using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateCircuitForAssetSubcomponentRequestmodel
    {
        public Guid asset_sublevelcomponent_mapping_id { get; set; }
        public string circuit { get; set; }
    }
}
