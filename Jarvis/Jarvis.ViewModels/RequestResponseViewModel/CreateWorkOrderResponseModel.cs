using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class CreateIssueResponseModel
    {
        public bool success { get; set; }

        public int status { get; set; }

        public string message { get; set; }

        public string issueguid { get; set; }
    }
}
