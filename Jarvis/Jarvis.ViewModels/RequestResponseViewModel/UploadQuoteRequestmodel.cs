using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class UploadQuoteRequestmodel
    {
        public Guid wo_id { get; set; }
        public List<WOCategoryList>? category_list { get; set; }
    }
    public class WOCategoryList
    {
        public string category_type { get; set; }
        public string form_name { get; set; }
        public string asset_class_code { get; set; }
        public string group_string { get; set; }
        public List<WOCategoryTaskList>? category_task_list { get; set; }
    }
    public class WOCategoryTaskList
    {
        public string identification { get; set; }
        public string location { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string section { get; set; }
        public string note { get; set; }
    }
}
