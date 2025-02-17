using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AddPMtoNewWolineRequestmodel
    {
        public Guid woonboardingassets_id { get; set; }
        public List<Guid> pm_ids { get; set; }
        public Guid wo_id { get; set; }
    }
}
