using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetsForSubmittedFilterOptionsByStatusRequestModel
    {
        public List<int>? status { get; set; }
        public List<int>? wo_type { get; set; }
    }
}
