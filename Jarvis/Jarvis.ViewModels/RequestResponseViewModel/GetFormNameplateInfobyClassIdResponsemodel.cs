using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFormNameplateInfobyClassIdResponsemodel
    {
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string form_nameplate_info { get; set; }
        public string pdf_report_template_url { get; set; }
    }
}
