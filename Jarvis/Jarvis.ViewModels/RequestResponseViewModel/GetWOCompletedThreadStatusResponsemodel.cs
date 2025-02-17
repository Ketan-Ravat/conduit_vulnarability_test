using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOCompletedThreadStatusResponsemodel
    {
        public int? complete_wo_thread_status { get; set; } // Inprogress = 1, Completed = 2,  Failed = 3
        public int total_wo_line { get; set; }
        public int total_processed_wo_line { get; set; }
    }
}
