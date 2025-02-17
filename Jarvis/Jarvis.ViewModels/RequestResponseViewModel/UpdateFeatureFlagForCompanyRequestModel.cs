using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateFeatureFlagForCompanyRequestModel
    {
        public Guid company_feature_id { get; set; }
        public bool is_required { get; set; }
    }
}
