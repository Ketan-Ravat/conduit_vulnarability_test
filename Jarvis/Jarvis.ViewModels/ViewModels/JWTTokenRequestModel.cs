using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.ViewModels
{
    public class JWTTokenRequestModel
    {
        public string privateKey { get; set; }

        public string Issuer { get; set; }

        public string Audiance { get; set; }
    }
}
