using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetUploadedOneLinePdfResponseModel
    {
        public int cluster_diagram_pdf_id { get; set; }
        public Guid site_id { get; set; }
        public string file_name { get; set; }

        public string file_url { get; set;}
    }
}
