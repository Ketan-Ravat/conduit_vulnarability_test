using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class LambdaGenerateIRWOAssetReportRequestmodel
    {
        public Guid wo_id { get; set; }
        public string manual_wo_number { get; set;}
        public DateTime? wo_start_date { get; set;}
        public Guid? user_id { get; set; }
    }
}
