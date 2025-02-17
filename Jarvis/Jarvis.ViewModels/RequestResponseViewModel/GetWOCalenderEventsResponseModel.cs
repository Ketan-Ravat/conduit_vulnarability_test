using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOCalenderEventsResponseModel
    {
        public DateTime WO_Start_Date { get; set; }
        public List<NewFlowWorkorderListResponseModel> wo_list { get; set; }
    }
}
