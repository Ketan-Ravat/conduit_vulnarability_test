using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAssetsOptimizedResponseview<T> : BaseViewModel
    {
        public List<T> list { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public int listsize { get; set; }
        public List<FilterAssetRoomFloorBuildingLocationOptionsmapping> rooms_with_floor_building { get; set; }
        public FilterAssetsOptimizedResponseview()
        {
            list = new List<T>();
        }
    }
}
