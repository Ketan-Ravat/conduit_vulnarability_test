using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class MobileGetWOsForOfflineResponsemodel
    {
        public List<MobileNewFlowWorkorderListResponseModel> workorders { get; set; }
    //    public List<GetAllHierarchyAssetsResponseModel> asset_hierarchy { get; set; }
        public List<AssetsResponseModel> asset_list { get; set; }
        public List<mobile_form_categoty_list> form_category_list { get; set; }
        public List<MobileGetWOcategoryTaskByCategoryIDListResponsemodel> wo_category_task_list { get; set; }
        public List<MobileGetFormByWOTaskIDResponsemodel> wo_task_forms_list { get; set; }
        public List<MobileFormIOResponseModel> formio_master_forms { get; set; }

        public List<MobileTaskResponseModel> MasterTasks { get; set; }
        public bool force_to_reset { get; set; }
    }
}
