using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class AuthenticateTokenRequestModel : GenericRequestModel
    {
        public string barcodeId { get; set; }
    }
}
