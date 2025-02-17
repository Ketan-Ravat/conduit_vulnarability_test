using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateOBWOAssetStatusRequestmodel
    {
        public Guid woonboardingassets_id { get; set; }
        public int status { get; set; }
        public string task_rejected_notes { get; set; }

    }
}
