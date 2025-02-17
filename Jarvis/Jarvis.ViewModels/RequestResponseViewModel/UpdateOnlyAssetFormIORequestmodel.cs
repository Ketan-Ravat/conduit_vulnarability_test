using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateOnlyAssetFormIORequestmodel
    {
        public Guid asset_form_id { get; set; }
        public string asset_form_data { get; set; }
    }
}
