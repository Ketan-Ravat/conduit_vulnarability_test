using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMsListByAssetClassIdRequestModel
    {
        public Guid? asset_id { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string search_string { get; set; }
    }
}
