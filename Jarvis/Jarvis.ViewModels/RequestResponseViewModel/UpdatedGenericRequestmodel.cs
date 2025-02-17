using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public static class UpdatedGenericRequestmodel
    {
        private static readonly AsyncLocal<RequestInfo> _currentUser = new AsyncLocal<RequestInfo>();

        public static RequestInfo CurrentUser
        {
            get => _currentUser.Value;
            set => _currentUser.Value = value;
        }
    }
    public class RequestInfo
    {
        public Guid requested_by { get; set; }
        public Guid device_uuid { get; set; }
        public string mac_address { get; set; }
        public string device_latitude { get; set; }
        public string device_longitude { get; set; }
        public string device_battery_percentage { get; set; }
        public string request_id { get; set; }
        public string site_id { get; set; }
        public string company_id { get; set; }
        public string role_id { get; set; }
        public string role_name { get; set; }
        public string site_name { get; set; }
        public string token { get; set; }
        public string domain_name { get; set; }
        public int site_status { get; set; }
        public int company_status { get; set; }
        public string app_version { get; set; }
        public string store_app_version { get; set; }
        public string platform_type { get; set; }
        public bool is_optional_update { get; set; }
    }
}
