using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class SyncDataResponseModel
    {
        public ListViewModel<Guid> active_assets { get; set; }

        public ListViewModel<AssetDetailsViewModel> assets { get; set; }

        public ListViewModel<InspectionFormDataViewModel> inspection_forms { get; set; }

        public ListViewModel<GetUserDetailsResponseModel> users { get; set; }

        public ListViewModel<AssetInspectionViewModel> inspections { get; set; }

        public ListViewModel<IssueViewModel> issues { get; set; }
        public ListViewModel<IssueViewModel> workorders { get; set; }

        public MasterDataResponseModel master_data { get; set; }

    }
}