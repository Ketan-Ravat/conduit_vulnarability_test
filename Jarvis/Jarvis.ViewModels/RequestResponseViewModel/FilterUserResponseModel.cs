using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterUserResponseModel : BaseViewModel
    {
        public ListViewModel<GetUserResponseModel> filterData { get; set; }
        public List<int> status { get; set; }
        public List<GetRolesResponseModel> roles { get; set; }
        public List<SitesViewModel> sites { get; set; }
        public FilterUserResponseModel()
        {
            filterData = new ListViewModel<GetUserResponseModel>();
        }
    }
}
