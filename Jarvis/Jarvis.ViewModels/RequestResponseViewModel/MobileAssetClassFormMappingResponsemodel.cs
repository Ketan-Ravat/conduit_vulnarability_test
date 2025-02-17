using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileAssetClassFormMappingResponsemodel
    {
        public Guid asset_class_formio_mapping_id { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public Guid? form_id { get; set; }
        public bool isarchive { get; set; }
        public int? wo_type { get; set; }

    }
}
