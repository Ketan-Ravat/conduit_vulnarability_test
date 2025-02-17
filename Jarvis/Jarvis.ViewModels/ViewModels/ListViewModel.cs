using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jarvis.ViewModels.ViewModels
{
    public class ListViewModel<T> : BaseViewModel
    {
        public List<T> list { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public int listsize { get; set; }
        public ListViewModel()
        {
            list = new List<T>();
        }
    }
}