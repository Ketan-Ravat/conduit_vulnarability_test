using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class CommentJsonObjectViewModel
    {
        public string comment { get; set; }

        public string created_by { get; set; }
        public string created_by_name { get; set; }

        public DateTime created_at { get; set; }
    }
}
