using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Shared
{
    public class AssetBarcodeGenerator
    {
        public List<AssetBarcodeGeneratorList> asset { get; set; }
    }

    public class AssetBarcodeGeneratorList
    {
        public string asset_id { get; set; }

        public string asset_name { get; set; }

        public string internal_asset_id { get; set; }

        public string site_name { get; set; }
        public string site_logo_img { get; set; }

        public string asset_barcode_image { get; set; }
    }
}
