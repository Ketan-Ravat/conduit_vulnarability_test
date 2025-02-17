using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetInProgressWorkordersListResponseModel
    {
        public string manual_wo_number { get; set; }
        public Guid? wo_id { get; set; }
        public string asset_name { get; set; }
        public string asset_form_name { get; set; }
        public string asset_form_type { get; set; }
        public Guid site_id { get; set; }
        public string site_name { get; set; }
        public string time_elapsed { get; set; }
        public string time_elapsed_for_overdue_wo { get; set; }
        public int status { get; set; }
    }
}
