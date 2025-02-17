using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetQuoteListStatusWiseResponsemodel
    {
        public List<GetAllWorkOrdersNewflowOptimizedResponsemodel> open { get; set; }
        public List<GetAllWorkOrdersNewflowOptimizedResponsemodel> submitted { get; set; }
        public List<GetAllWorkOrdersNewflowOptimizedResponsemodel> deferred { get; set; }
        public List<GetAllWorkOrdersNewflowOptimizedResponsemodel> rejected { get; set; }
        public List<GetAllWorkOrdersNewflowOptimizedResponsemodel> accepted { get; set; }
    }
}
