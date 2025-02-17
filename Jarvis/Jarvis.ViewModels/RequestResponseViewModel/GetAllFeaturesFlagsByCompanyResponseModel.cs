using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllFeaturesFlagsByCompanyResponseModel
    {
        public List<GetAllFeaturesFlagsByCompany_Class> list {  get; set; }
    }
    public class GetAllFeaturesFlagsByCompany_Class
    {
        public Guid company_feature_id { get; set; }
        public Guid? feature_id { get; set; }
        public string feature_name { get; set; }
        public string feature_description { get; set; }
        public bool is_required { get; set; }
    }
}
