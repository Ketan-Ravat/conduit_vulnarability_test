using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetCompanyLogosResponsemodel
    {
        public string company_logo { get; set; }
        public string company_thumbnail_logo { get; set; }
        public string company_favicon_logo { get; set; }
        public string company_logo_base64 { get; set;}
    }
}
