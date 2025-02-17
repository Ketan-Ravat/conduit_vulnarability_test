using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class FilterBaseRequestModel
    {
        /// <summary>
        /// Page Size Default 20
        /// </summary>
        public int pagesize { get; set; }
        /// <summary>
        /// Enter Page Index
        /// </summary>
        public int pageindex { get; set; }
        /// <summary>
        /// Enter Site Id or Null
        /// </summary>
        public List<string> site_id { get; set; }
        /// <summary>
        /// Enter Company Id or Null
        /// </summary>
        public List<string> company_id { get; set; }
    }
}
