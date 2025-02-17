using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllOnboardingAssetsByWOIdRequestModel
    {
        public Guid wo_id { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public string search_string { get; set; }
        public List<string> asset_class_code { get; set; }
        public List<string> asset_class_name { get; set; }
        public List<string> buildings { get; set; }
        public List<string> floors { get; set; }
        public List<string> rooms { get; set; }
        public List<string> sections { get; set; }

    }
}
