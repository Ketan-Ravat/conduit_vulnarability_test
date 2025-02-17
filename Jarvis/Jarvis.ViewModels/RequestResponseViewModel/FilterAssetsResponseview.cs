using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterAssetsResponseview<T> : BaseViewModel
    {
            public List<T> list { get; set; }
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            public int listsize { get; set; }
            public FilterAssetBuildingLocationOptions filterassetbuildingbocationoptions { get; set; }
            public FilterAssetsResponseview()
            {
                list = new List<T>();
            }
    }
}
