﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetclassFormToAddcategoryResponsemodel
    {
        public string asset_class_name { get; set; }
        public string form_name { get; set; }
        public Guid form_id { get; set; }
        public Guid inspectiontemplate_asset_class_id { get; set; }

        public string asset_class_code { get; set; }
    }
}
