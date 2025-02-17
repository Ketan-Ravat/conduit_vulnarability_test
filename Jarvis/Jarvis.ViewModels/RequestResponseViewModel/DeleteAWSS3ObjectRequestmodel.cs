using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class DeleteAWSS3ObjectRequestmodel
    {
        public List<string> file_name { get; set; }
        public int bucket_type { get; set; }

    }
}
