using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FormIOInsulationResistanceTestRequestModel
    {
        public Guid asset_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int? page_size { get; set; }
        public int? page_index { get; set; }
    }
}
