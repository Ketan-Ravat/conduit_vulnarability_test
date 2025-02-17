using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetPMListMobileRequestmodel
    {
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public Guid? asset_form_id { get; set; }
    }
}
