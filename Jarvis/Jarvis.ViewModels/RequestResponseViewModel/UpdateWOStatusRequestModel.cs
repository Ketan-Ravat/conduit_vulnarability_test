﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UpdateWOStatusRequestModel
    {
        public Guid wo_id { get; set; }
        public int status { get; set; }
    }
}
