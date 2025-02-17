using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DeleteFormFromAssetClassRequestmodel
    {
        public Guid form_id { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }
    }
}
