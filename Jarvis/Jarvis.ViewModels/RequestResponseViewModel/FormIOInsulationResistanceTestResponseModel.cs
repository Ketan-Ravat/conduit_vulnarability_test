using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FormIOInsulationResistanceTestResponseModel
    {
        public Guid FormIOInsulationResistanceTestMapping_id { get; set; }

        public string IRPoletoPoleAsFound1 { get; set; }
        public string IRPoletoPoleAsFound2 { get; set; }
        public string IRPoletoPoleAsFound3 { get; set; }

        public string IRAcrossPoleAsFound1 { get; set; }
        public string IRAcrossPoleAsFound2 { get; set; }
        public string IRAcrossPoleAsFound3 { get; set; }
        public DateTime? created_at { get; set; }
        public Nullable<Guid> asset_form_id { get; set; }
    }
}
