using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileNewFlowWorkorderListResponseModel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public string manual_wo_number { get; set; }
        public int wo_type { get; set; }
        public string wo_type_name { get; set; }
      //  public Guid site_id { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public int wo_status_id { get; set; }
        public string wo_status { get; set; }
        public string description { get; set; }
        public string timezone { get; set; }
       // public string client_company_name { get; set; }
      //  public Guid client_company_id { get; set; }
        public bool is_archive { get; set; }
        public List<WorkOrderAttachmentsResponseModel> WorkOrderAttachments { get; set; }

    }
}
