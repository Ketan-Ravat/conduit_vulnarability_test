using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetFormListtoAddByAssetclassIDRequestModel
    {
        public Guid? inspectiontemplate_asset_class_id {  get; set; }

        public int inpsection_form_type { get; set; }   
    }
}
