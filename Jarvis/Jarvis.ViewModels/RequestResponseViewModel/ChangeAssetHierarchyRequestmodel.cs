using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public  class ChangeAssetHierarchyRequestmodel
    {
        public Guid changing_asset_id { get; set; }
        public bool is_changing_asset_id_temp { get; set; } // true if changing_asset_id is temp
        public Guid destination_asset_id { get; set; }
        public bool is_destination_asset_id_temp { get; set; } // true if is_destination_asset_id_temp is temp
    }
}
