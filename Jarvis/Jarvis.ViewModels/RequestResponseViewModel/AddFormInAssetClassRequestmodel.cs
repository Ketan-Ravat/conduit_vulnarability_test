using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddFormInAssetClassRequestmodel
    {
        public Guid? form_id { get; set; }
        public Guid? asset_class_formio_mapping_id { get; set; }
    }
}
