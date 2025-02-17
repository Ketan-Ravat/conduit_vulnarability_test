using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddUpdateAssetFormIOOfflineResponsemodel
    {
        public List<Guid> uploaded_asset_form_id { get; set; }

        public int success { get; set; }
    }
}
