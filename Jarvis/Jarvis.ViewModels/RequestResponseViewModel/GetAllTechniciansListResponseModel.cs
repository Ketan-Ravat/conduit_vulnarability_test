using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAllTechniciansListResponseModel
    {
        public List<GetAllUserData_Class> list { get; set; }
    }
    public class GetAllUserData_Class
    {
        public Guid uuid { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public bool is_curr_site_user { get; set; }
    }
}
