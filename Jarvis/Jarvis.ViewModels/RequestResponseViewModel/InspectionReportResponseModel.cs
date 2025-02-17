using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class InspectionReportResponseModel
    {
        public InspectionReportResponseModel()
        {
            inspections = new ListViewModel<AssetInspectionViewModel>();
        }
        public AssetViewModel asset { get; set; }

        public ListViewModel<AssetInspectionViewModel> inspections { get; set; }
    }
}
