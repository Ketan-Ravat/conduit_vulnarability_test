using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsToAssignOBWOResponsemodel
    {
        public Guid asset_id { get; set; }
        public string asset_name { get; set;}
        public string building { get; set;}
        public string floor { get; set;}
        public string room { get; set;}
        public string section { get;set;}
        public Guid inspectiontemplate_asset_class_id { get; set; }
        public string asset_class_code { get; set; }
        public string asset_class_name { get; set; }
    }
}
