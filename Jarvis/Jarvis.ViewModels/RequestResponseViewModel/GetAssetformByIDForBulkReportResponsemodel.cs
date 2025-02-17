using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class GetAssetformByIDForBulkReportResponsemodel
    {
        public List<asset_form_data_bulk_report> asset_form_data { get;set; }
        public List<master_form_data_bulk_report> master_form_data { get;set; }
        public bool isCalibrationDateEnabled { get; set; }

    }
    public class asset_form_data_bulk_report
    {
        public Guid asset_form_id { get; set; } 
        public Guid form_id { get; set; }
        public string asset_form_data { get; set; }
    }
    public class master_form_data_bulk_report
    {
        public Guid form_id { get; set; }
        public string form_data { get; set; }
    }
}
