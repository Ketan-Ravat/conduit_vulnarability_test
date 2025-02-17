using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class SubmitPMFormJsonRequestmodel
    {
        public Guid? asset_pm_id { get; set; }
        public Guid? temp_asset_pm_id { get; set; }
        public string pm_form_output_data { get; set; }
        public int status { get; set; } = 13; // default value inprogress
        public Guid woonboardingassets_id { get; set; }
    }
}
