using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels
{
    public class PendingInspectionCheckoutAssetsResponseModel
    {
        //public List<InspectionResponseModel> PendingInspection { get; set; }
        //public List<AssetsResponseModel> CheckOutAssets { get; set; }

        public Guid asset_id { get; set; }

        public string asset_photo { get; set; }

        public string internal_asset_id { get; set; }

        public Nullable<int> status { get; set; }

        public Nullable<int> inspection_status { get; set; }

        public string status_name { get; set; }

        public Guid inspectionform_id { get; set; }

        public string notes { get; set; }

        public Nullable<int> usage { get; set; }

        public Nullable<long> meter_hours { get; set; }

        public string name { get; set; }

        public string asset_type { get; set; }

        public string product_name { get; set; }

        public string model_name { get; set; }

        public string asset_serial_number { get; set; }

        public string model_year { get; set; }

        public string site_location { get; set; }

        public string current_stage { get; set; }

        public string parent { get; set; }

        public string children { get; set; }

        public Guid site_id { get; set; }

        public string site_name { get; set; }

        public string site_code { get; set; }

        public Guid company_id { get; set; }

        public string company_name { get; set; }

        public string company_code { get; set; }

        public List<InspectionStatus> inspection { get; set; }

        public string timezone { get; set; }

        //public virtual SitesViewModel Sites { get; set; }

        //public virtual AssetInspectionFormResponseModel InspectionForms { get; set; }

        //public virtual List<AssetInspectionViewModel> Inspections { get; set; }

        //public virtual List<WorkOrderResponseModel> WorkOrders { get; set; }

    }

    public class InspectionStatus
    {
        public int status { get; set; }

        public DateTime created_at { get; set; }

        public DateTime modified_at { get; set; }

        public string Duration { get; set; }
    }
}