using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DeleteLocationDetailsRequestModel
    {
        public int formiobuilding_id { get; set; }
        public int formiofloor_id { get; set; }

        public int formioroom_id { get; set; } 
    }
}
