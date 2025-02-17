using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetIRScanImagesFilesResponsemodel
    {
        public Guid irscanwoimagefilemapping_id { get; set; }
        public string img_file_name { get; set; }
        public string img_file_url { get; set; }
        public string manual_wo_number { get; set; }
        public bool is_img_attached { get; set; }
        public Guid wo_id { get; set; }
        public bool is_deleted { get; set; }
    }
}
