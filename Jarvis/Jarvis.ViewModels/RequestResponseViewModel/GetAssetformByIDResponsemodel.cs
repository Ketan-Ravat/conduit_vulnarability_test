using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetformByIDResponsemodel
    {
        public string asset_form_data { get;set; }
        public Guid asset_form_id { get;set; }
        public Guid form_id { get; set; }
    }
}
