using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetOfflineSyncLambdaStatusResponsemodel
    {
        public Guid trackmobilesyncoffline_id { get; set; }
        public int status { get; set; } //1 - completed , 2 - open , 3 - inprogress , 4 - failed

    }
}
