using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOBacklogCardListResponsemodel
    {
        public List<NewFlowWorkorderListResponseModel> planned { get; set; }
        public List<NewFlowWorkorderListResponseModel> released_open { get; set; }
        public List<NewFlowWorkorderListResponseModel> in_progress { get; set; }
        public List<NewFlowWorkorderListResponseModel> on_hold { get; set; }
        public List<NewFlowWorkorderListResponseModel> complete { get; set; }
    }
}
