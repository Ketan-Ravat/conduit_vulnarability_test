using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllOBAssetsWithQRCodeByWOIdResponseModel
    {
        public List<GetOBAssetWithQRCode_Class> assets_list { get; set; }
    }
    public class GetOBAssetWithQRCode_Class
    {
        public string asset_name { get; set; }
        public string QR_code { get; set; }
    }
}
