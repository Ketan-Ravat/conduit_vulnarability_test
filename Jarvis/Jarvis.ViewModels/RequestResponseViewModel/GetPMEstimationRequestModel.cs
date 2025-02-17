using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetPMEstimationRequestModel
    {
        public Guid class_id { get; set; }

        public Guid? woonboardingassets_id { get; set; }
    }
}
