using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetAttachmentsRequestmodel
    {
        public Guid asset_id { get; set; }
        public int pagesize { get; set; } = 0;
        public int pageindex { get; set; } = 0;
    }
}
