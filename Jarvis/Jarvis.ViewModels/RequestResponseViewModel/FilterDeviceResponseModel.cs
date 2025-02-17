using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterDeviceResponseModel : BaseViewModel
    {
        public ListViewModel<DeviceInfoViewModel> filterData { get; set; }
        public List<string> types { get; set; }
        public List<string> brands { get; set; }
        public List<string> model { get; set; }
        public List<string> os { get; set; }
        public List<SitesViewModel> sites { get; set; }
        public FilterDeviceResponseModel()
        {
            filterData = new ListViewModel<DeviceInfoViewModel>();
            sites = new List<SitesViewModel>();
        }
    }
}
