﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllAssetGroupsListRequestModel
    {
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public double asset_health { get; set; }
        public string search_string { get; set; }
    }
}
