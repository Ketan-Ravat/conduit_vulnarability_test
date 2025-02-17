using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileViewWorkOrderDetailsByIdResponsemodel
    {
        public Guid wo_id { get; set; }
        public string wo_number { get; set; }
        public string manual_wo_number { get; set; }
        public int wo_type { get; set; }
        public string wo_type_name { get; set; }
        public string description { get; set; }
   //     public Guid site_id { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public int wo_status_id { get; set; }
        public string wo_status { get; set; }
       
       // public Guid? client_company_id { get; set; }
        public string client_company_name { get; set; }
        public List<mobile_form_categoty_list> form_category_list { get; set; }
        public List<MobileWorkOrderAttachmentsResponseModel> WorkOrderAttachments { get; set; }
        public List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> wo_all_tasks { get; set; }

    }
    public class mobile_form_categoty_list
    {
        public Guid wo_inspectionsTemplateFormIOAssignment_id { get; set; }
        public string form_category_name { get; set; }
     //   public string form_description { get; set; }
        public string WP { get; set; }
        public string form_data { get; set; }
        public string parent_asset_name { get; set; }
      //  public Guid? parent_asset_id { get; set; }
        public int progress_total { get; set; }
        public int progress_completed { get; set; }
       
        public int status_id { get; set; }
        public string status_name { get; set; }
        public string form_name { get; set; }
       // public Guid wo_id { get; set; }
        public bool is_archived { get; set; }

    }
}
