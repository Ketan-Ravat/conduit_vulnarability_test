using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFormsByAssetclassIDResponsemodel
    {
        public Guid form_id { get; set; }
        public string form_description { get; set; }
        public string form_name { get; set; }
        public string work_procedure { get; set; }
        public int form_type_id { get; set; }
        public string form_type_name { get; set; }
        public string asset_class_form_properties { get; set; }
        public Guid asset_class_formio_mapping_id { get; set; }
        public int? wo_type { get; set;}
    }
}
