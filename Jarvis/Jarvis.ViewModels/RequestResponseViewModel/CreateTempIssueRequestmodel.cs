using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class CreateTempIssueRequestmodel
    {
        public int issue_type { get; set; } //Repair , Replacement , Compliance(NEC,Osha), Thermal
        public int? inspection_type { get; set; } // reapir / replace / general issue resolution
        public Guid? asset_id { get; set; }
        public Guid? woonboardingassets_id { get; set; }
        public string issue_title { get; set; }
        public string problem_description { get; set; }
        public int? priority { get; set; }// low , medium , high
        public Guid wo_id { get; set; }
        public List<CreatetempIssueImages> issue_images { get; set; }
    }

    public class CreatetempIssueImages
    {
        public string url { get; set; }
        public string image_file_name_url { get; set; }
        public string image_file_name { get; set; }
        public bool is_deleted { get; set; }
        public string image_thumbnail_file_name_url { get; set; }
        public string image_thumbnail_file_name { get; set; }
        public Guid? asset_issue_image_mapping_id { get; set; }
        public int image_duration_type_id { get; set; }
    }
}
