using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class GetAssetsByIdRequestModel
    {
        /// <summary>
        /// Enter Login UserID
        /// </summary>
        //public string userid { get; set; }

        /// <summary>
        /// Enter Internal Asset ID
        /// </summary>
        public string internal_asset_id { get; set; }

        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public string barcode_id { get; set; }

        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public string asset_id { get; set; }

        /// <summary>
        /// for BE use only
        /// </summary>
        public string form_io_form_id { get; set; }

        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public string qr_code { get; set; }
    }
}
