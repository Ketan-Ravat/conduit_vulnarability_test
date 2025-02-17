using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.Utility
{
    public class RequestHeaderUtils
    {
        public const string IP_ADDRESS_KEY = "IP_ADDRESS";
        public const string REQUESTED_BY_KEY = "REQUESTED_BY";
        public const string REQUEST_ID_KEY = "REQUEST_ID";
        public const string DEVICE_UUID_KEY = "DEVICE_UUID";
        public const string MAC_ADDRESS_KEY = "MAC_ADDRESS";
        public const string DEVICE_LATITUDE_KEY = "DEVICE_LATITUDE";
        public const string DEVICE_LONGITUDE_KEY = "DEVICE_LONGITUDE";
        public const string DEVICE_BATTERY_PERCENTAGE_KEY = "DEVICE_BATTERY_PERCENTAGE";
        public const string SITE_ID_KEY = "SITE_ID";
        public const string ROLE_ID_KEY = "ROLE_ID";
        public const string TOKEN_KEY = "Token";
        public const string DoMAIN_NAME_KEY = "Domain_Name";
        public const string APP_VERSION_KEY = "app_version";
        public const string AUTH_TYPE = "x-sensaii-auth-type";
        public const string App_Brand = "appbrand";
        public const string Device_Brand = "devicebrand"; //android / Ios
        public const string PLATFORM_TYPE = "x-sensaii-platform-type";
        public const string Referer = "Referer";
        public const string USER_SESSION_ID = "USER_SESSION_ID";


        public static Guid requested_by { get; set; }
        public static Guid request_id { get; set; }
        public static string ip_address { get; set; }
        public static Guid device_uuid { get; set; }
        public static string mac_address { get; set; }
        public static string refer { get; set; }
        public static string device_latitude { get; set; }
        public static string device_longitude { get; set; }
        public static string device_battery_percentage { get; set; }
        public static string site_id { get; set; }
        public static string role_id { get; set; }
        public static string token { get; set; }
        public static string domain_name { get; set; }
        public static string auth_type { get; set; }
        public static string platform_type { get; set; }
        public static string app_brand { get; set; }
        public static string device_brand { get; set; }
        public static string user_session_id { get; set; }
    }
}
