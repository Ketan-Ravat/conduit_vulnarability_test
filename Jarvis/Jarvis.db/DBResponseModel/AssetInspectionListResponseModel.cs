using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.DBResponseModel
{
    public class AssetInspectionListResponseModel
    {
        public AssetInspectionListResponseModel()
        {
            AssetInspectionReport = new List<AssetInspectionReport>();
        }

        public List<AssetInspectionReport> AssetInspectionReport { get; set; }

        public int list_size { get; set; }
    }
}
