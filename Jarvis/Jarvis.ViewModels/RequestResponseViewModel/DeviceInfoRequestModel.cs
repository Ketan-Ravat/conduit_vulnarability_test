using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DeviceInfoRequestModel
    {
        /// <summary>
        /// Enter Login User UserID
        /// </summary>
        public Guid? requested_by { get; set; }
        
        /// <summary>
        /// Enter Device UUID
        /// </summary>
        public Guid device_uuid { get; set; }

        /// <summary>
        /// Enter Divice Code
        /// </summary>
        public string device_code { get; set; }

        /// <summary>
        /// Enter Company Code
        /// </summary>
        public string company_code { get; set; }

        /// <summary>
        /// Enter Divice Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Enter Divice Type
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Enter Divice Brand
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// Enter Divice Model
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// Enter Divice Version
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// Enter Divice OS
        /// </summary>
        public string os { get; set; }

        /// <summary>
        /// Enter Mac Address
        /// </summary>
        public string mac_address { get; set; }

        /// <summary>
        /// Enter Device Associated Company ID
        /// </summary>
        public Guid? company_id { get; set; }

        /// <summary>
        /// Enter Installed App Version for this Device
        /// </summary>
        public string app_version { get; set; }
    }
}
