using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jarvis.ViewModels.ViewModels
{
    public class FilterAssetResponseModel : BaseViewModel
    {
        public ListViewModel<AssetsResponseModel> filterData { get; set; }
        public List<AssetListResponseModel> assetsName { get; set; }
        public List<string> modelNames { get; set; }
        public List<string> modelYears { get; set; }
        public List<int> status { get; set; }
        public List<SitesViewModel> sites { get; set; }
        public FilterAssetResponseModel()
        {
            filterData = new ListViewModel<AssetsResponseModel>();
        }
    }
}