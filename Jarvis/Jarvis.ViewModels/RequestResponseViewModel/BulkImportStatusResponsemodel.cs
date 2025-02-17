using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class BulkImportStatusResponsemodel
    {
        public int? bulk_data_import_status { get; set; } // Completed = 1, Inprogress = 2,  Failed = 3
        public string bulk_data_import_failed_logs { get; set; } // identification array if bulk import is failed to insert in db 
    }
}
