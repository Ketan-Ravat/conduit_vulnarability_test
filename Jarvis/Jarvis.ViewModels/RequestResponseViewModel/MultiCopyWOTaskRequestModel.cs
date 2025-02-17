using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MultiCopyWOTaskRequestModel
    {
        public Guid WOcategorytoTaskMapping_id { get; set; }
        public bool keep_visual_inspection { get; set; }
        public bool keep_nameplate { get; set; }
        public bool keep_parent_assset { get; set; }
        public bool keep_trip_test { get; set; }
        public bool keep_technician_user { get; set; } 
        public bool keep_all_data { get; set; }
        
        public int number_of_copies { get; set; }
        public Guid? technician_user_id { get; set; }
    }
}
