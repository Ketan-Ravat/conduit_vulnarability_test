using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UploadSiteDocumentRequestmodel
    {
        public List<string> file_name { get; set; }
        public string folder_path { get; set; }
    }
}
