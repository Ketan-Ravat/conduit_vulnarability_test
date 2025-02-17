using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GenericRequestModel
    {
        //public const string DEVICE_UUID_KEY = "DEVICE_UUID";
        public static Guid requested_by { get; set; }
        public static Guid device_uuid { get; set; }
        public static string mac_address { get; set; }
        public static string device_latitude { get; set; }
        public static string device_longitude { get; set; }
        public static string device_battery_percentage { get; set; }
        public static string request_id { get; set; }
        public static string site_id { get; set; }
        public static string company_id { get; set; }
        public static string role_id { get; set; }
        public static string role_name { get; set; }
        public static string site_name { get; set; }
        public static string token { get; set; }
        public static string domain_name { get; set; }
        public static int site_status { get; set; }
        public static int company_status { get; set; }
        public static string app_version { get; set; }
        public static string store_app_version { get; set; }
        public static string domain_refer { get; set; }
        public static bool is_optional_update { get; set; }

    }
}
