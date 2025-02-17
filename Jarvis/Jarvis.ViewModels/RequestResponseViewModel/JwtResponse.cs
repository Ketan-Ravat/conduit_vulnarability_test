using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class JwtResponse
    {
        public int response_status { get; set; }
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
    }
}
