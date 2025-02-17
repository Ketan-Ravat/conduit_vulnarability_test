using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AssignAssetToMWORequestmodel
    {
        public Guid asset_id { get; set; }
        public Guid form_id { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public Guid wo_id { get; set; }
    }
}
