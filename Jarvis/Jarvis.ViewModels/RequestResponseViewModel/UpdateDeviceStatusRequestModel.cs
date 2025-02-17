using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateDeviceStatusRequestModel
    {
        public bool status { get; set; }
        public string requested_by { get; set; }
    }
}
