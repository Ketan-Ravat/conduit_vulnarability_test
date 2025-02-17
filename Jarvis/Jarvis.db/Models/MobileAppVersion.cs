using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jarvis.db.Models
{
    public class MobileAppVersion
    {
        [Key]
        public int mobileappversion_id { get; set; }
        public string force_to_update_app_version { get; set; }
        public string store_app_version { get; set; }
        public int device_brand { get; set; } // 1 for android and 2 for Ios
    }
}
