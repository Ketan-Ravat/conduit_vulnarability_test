using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class LinkPMToWOLineRequestmodel
    {
        public Guid asset_form_id { get; set; }
        public List<Guid> asset_pm_id { get; set; }
        public Guid wo_id { get; set; }
        public int? status { get; set; }
    }
}
