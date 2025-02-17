using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetFormIOByAssetFormIdResponsemodel
    {

        public string asset_form_description { get; set; }
        public string asset_form_data { get; set; }
        public Guid asset_form_id { get; set; }
        public string wo_number { get; set; }
        public string manual_wo_number { get; set; }
        public string task_rejected_notes { get; set; }
        public int wo_type { get; set; }
     //   public string wo_type_name { get; set; }
        public int wo_status_id { get; set; }
      //  public string wo_status { get; set; }
    }
}
