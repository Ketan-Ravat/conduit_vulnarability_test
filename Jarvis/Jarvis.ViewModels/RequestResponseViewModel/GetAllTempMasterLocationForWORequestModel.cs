using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllTempMasterLocationForWORequestModel
    {
        public Guid? wo_id { get; set; }
        public string search_string { get; set; }
        public bool is_100_floors_required { get; set; }
    }
}
