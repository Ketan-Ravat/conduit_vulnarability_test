using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFormPropertiesByAssetclassIDResponsemodel
    {
        public string form_name { get; set; }
        public Guid form_id { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_form_properties { get; set; }
        public string form_description { get; set; }
        public string work_procedure { get; set; }
        public int form_type_id { get; set; }
        public string form_type_name { get; set; }
    }
}
