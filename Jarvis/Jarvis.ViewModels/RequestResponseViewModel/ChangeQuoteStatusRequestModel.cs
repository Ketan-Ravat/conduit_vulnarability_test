using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ChangeQuoteStatusRequestModel
    {
        public Guid wo_id { get; set; }
        public int quote_status { get; set;}
    }
}
