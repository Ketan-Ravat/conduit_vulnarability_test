using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class AssetValueJsonObjectRequestModel
    {
        /// <summary>
        /// Enter Attribute ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Enter Attribute Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Enter Attribute Type
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Enter Attribute Value
        /// </summary>
        public string value { get; set; }
    }
}
