using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.ViewModels
{
    public class AddAssetRequestModel
    {
        /// <summary>
        /// Enter Company ID
        /// </summary>
        public Guid company_id { get; set; }

        public List<AssetRequestModel> AssetRequestModel { get; set; }

        public AddAssetRequestModel()
        {
            AssetRequestModel = new List<AssetRequestModel>();
        }
    }

    public class AssetRequestModel
    {
        /// <summary>
        /// Enter Asset ID
        /// </summary>
        public Guid asset_id { get; set; }

        /// <summary>
        /// Enter Internal Asset ID
        /// </summary>
        public string internal_asset_id { get; set; }

        /// <summary>
        /// Enter Company ID
        /// </summary>
        public string company_id { get; set; }

        /// <summary>
        /// Enter Site ID
        /// </summary>
        public string site_id { get; set; }

        /// <summary>
        /// Enter Status
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Enter Inspection Form ID
        /// </summary>
        public string inspectionform_id { get; set; }

        [JsonIgnore]
        public DateTime created_at { get; set; }

        [JsonIgnore]
        public DateTime modified_at { get; set; }

        public AssetValueJsonObjectRequestModel[] lastinspection_attribute_values { get; set; }

        /// <summary>
        /// Enter Usage
        /// </summary>
        public int usage { get; set; }

        /// <summary>
        /// Enter Meter Hours
        /// </summary>
        public long meter_hours { get; set; }

        /// <summary>
        /// Enter Asset Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Enter Asset Type
        /// </summary>
        public string asset_type { get; set; }

        /// <summary>
        /// Enter Product Name
        /// </summary>
        public string product_name { get; set; }

        /// <summary>
        /// Enter Model Name
        /// </summary>
        public string model_name { get; set; }

        /// <summary>
        /// Enter Asset Serial Number
        /// </summary>
        public string asset_serial_number { get; set; }

        /// <summary>
        /// Enter Model Year
        /// </summary>
        public string model_year { get; set; }

        /// <summary>
        /// Enter Site Location
        /// </summary>
        public string site_location { get; set; }

        /// <summary>
        /// Enter Current Stage
        /// </summary>
        public string current_stage { get; set; }

        /// <summary>
        /// Enter Asset Parent
        /// </summary>
        public string parent { get; set; }

        /// <summary>
        /// Enter Asset Children
        /// </summary>
        public string children { get; set; }

        /// <summary>
        /// Enter Asset Levels
        /// </summary>
        public string levels { get; set; }

        /// <summary>
        /// Enter Asset Condition Index
        /// </summary>
        public double? condition_index { get; set; }

        /// <summary>
        /// Enter Asset Criticality Index
        /// </summary>
        public int? criticality_index { get; set; }

        /// <summary>
        /// Enter Asset Condition State
        /// </summary>
        public int? condition_state { get; set; }

        /// <summary>
        /// Enter Barcode Id
        /// </summary>
        public string asset_barcode_id { get; set; }

        /// <summary>
        /// client_internal_id
        /// </summary>
        public string client_internal_id { get; set; }

    }

}
