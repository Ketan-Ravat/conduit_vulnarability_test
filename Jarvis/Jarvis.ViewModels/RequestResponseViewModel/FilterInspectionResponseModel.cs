using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterInspectionResponseModel : BaseViewModel
    {
        public ListViewModel<InspectionResponseModel> filterData { get; set; }
        public List<AssetListResponseModel> assetsName { get; set; }
        public List<int> status { get; set; }
        public List<int> shift { get; set; }
        public List<OperatorsListResponseModel> oprators { get; set; }
        public List<SitesViewModel> sites { get; set; }
        public FilterInspectionResponseModel()
        {
            filterData = new ListViewModel<InspectionResponseModel>();
        }
    }
    public class OperatorsListResponseModel
    {
        public Guid uuid { get; set; }

        public Guid barcode_id { get; set; }

        public string email { get; set; }

        public string username { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }
    }
}
