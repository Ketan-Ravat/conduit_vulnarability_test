using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateAssetStatusRequestModel
    {
        public string asset_id { get; set; }
        public int status { get; set; }
        //public string updatedby { get; set; }
    }
}
