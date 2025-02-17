using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddNewSubComponentRequestmodel
    {
        public Guid asset_id { get; set; }
        public bool is_subcomponent_for_existing { get; set; }
        public Guid? sublevelcomponent_asset_id { get; set; }
        public string sublevelcomponent_asset_name { get; set; }
        public Guid? inspectiontemplate_asset_class_id { get; set; }
        public string circuit { get; set; }
        public List<subcomponentasset_image_list_class>? subcomponentasset_image_list { get; set; }
    }
}
