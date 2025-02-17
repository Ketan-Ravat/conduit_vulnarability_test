using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetClassCodesResponsemodel
    {
        public string label { get; set; }
        public string value { get; set; }
        public string className { get; set; }
        public string classType { get; set; }

        public Guid? plan_id { get; set; }
        public string plan_name { get; set; }
    }
}
