using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.ViewModels
{
    public class CreateUpdateSiteResponseModel
    {
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public int status { get; set; }
    }
}
