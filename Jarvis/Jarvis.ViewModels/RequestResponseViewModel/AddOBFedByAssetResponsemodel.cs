using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddOBFedByAssetResponsemodel
    {
        public string asset_name { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public List<subcomponent_ocp_class> subcomponent_ocp_list { get; set; }
    }
    public class subcomponent_ocp_class
    {
        public Guid? subcomponent_woonboardingassets_id { get; set; }
        public string subcomponent_name { get; set; }
    }
}
