using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateWOlinePDFurlfromLambdaRequestmodel
    {
        public Guid asset_form_id { get; set; }
        public string pdf_url { get; set; }
        public string manual_wo_number { get; set; }
    }
}
