using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterIssueResponseModel : BaseViewModel
    {
        public ListViewModel<IssueResponseModel> filterData { get; set; }
        public List<IssuesNameResponseModel> issueNames { get; set; }
        public List<AssetListResponseModel> assets { get; set; }
        public List<int> status { get; set; }
        public List<int> priorities { get; set; }
        public List<SitesViewModel> sites { get; set; }
        public FilterIssueResponseModel()
        {
            filterData = new ListViewModel<IssueResponseModel>();
        }
    }
}
