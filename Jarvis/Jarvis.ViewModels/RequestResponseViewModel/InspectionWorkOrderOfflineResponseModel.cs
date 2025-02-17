using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class InspectionIssueOfflineResponseModel
    {
        public List<Guid> issues { get; set; }
        public List<Guid> inspections { get; set; }
    }
}
