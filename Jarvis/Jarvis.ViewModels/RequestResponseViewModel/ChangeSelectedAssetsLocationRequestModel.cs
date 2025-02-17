using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ChangeSelectedAssetsLocationRequestModel
    {
        public int formiobuilding_id { get; set; }
        public int formiofloor_id { get; set; }
        public int formioroom_id { get; set; }

        public List<Guid> asset_id { get; set; }

        public Guid? toplevelcomponent_asset_id { get; set; }
    }
}
