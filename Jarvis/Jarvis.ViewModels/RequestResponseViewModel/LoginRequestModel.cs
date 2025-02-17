using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class LoginRequestModel : GenericRequestModel
    {
        public string username { get; set; }

        public string password { get; set; }

        public string barcodeId { get; set; }

        public string notification_token { get; set; }

        public string os { get; set; }
    }
}
