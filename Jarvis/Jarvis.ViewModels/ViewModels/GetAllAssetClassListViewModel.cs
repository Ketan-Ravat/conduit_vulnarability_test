using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.ViewModels
{
    public class GetAllAssetClassListViewModel<T>:BaseViewModel
    {
        public List<T> list { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public int listsize { get; set; }
        public bool isAddAssetClassEnabled { get; set; }


        public GetAllAssetClassListViewModel()
        {
            list = new List<T>();
        }
    }
}
