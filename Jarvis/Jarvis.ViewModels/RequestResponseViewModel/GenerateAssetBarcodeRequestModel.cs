using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class GenerateAssetBarcodeRequestModel
    {
        /// <summary>
        /// Enter List Of Asset ID
        /// </summary>
        public List<string> assetList { get; set; }
    }
}
