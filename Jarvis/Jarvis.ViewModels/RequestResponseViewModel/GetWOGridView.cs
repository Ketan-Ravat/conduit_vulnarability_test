using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetWOGridView
    {
        public List<GetWOGridViewTaskResponsemodel> task_list { get; set; }
        //public List<TaskDynamicFieldResponsemodel> DynamicFields { get; set; }
        public List<form_dynamic_fields> dynamic_fields { get; set; }
    }
}
