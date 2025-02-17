using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignAssettoWOcategoryRequestModel
    {
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public Guid parent_asset_id { get; set; }
    }
}
