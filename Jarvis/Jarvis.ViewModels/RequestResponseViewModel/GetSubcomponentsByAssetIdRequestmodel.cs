using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetSubcomponentsByAssetIdRequestmodel
    {
        public Guid asset_id { get; set; }
        public int pagesize { get; set; } = 0;
        public int pageindex { get; set; } = 0;
        public Guid? asset_children_hierrachy_id { get; set;}

    }
}
