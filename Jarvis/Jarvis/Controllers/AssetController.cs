using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Service.Concrete;
using Jarvis.Shared;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.Filter;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class AssetController : ControllerBase
    {
        public AWSConfigurations _options { get; set; }
        private readonly IAssetService assetService;
        private readonly IS3BucketService s3BucketService;
        public AssetController(IAssetService _assetService, IOptions<AWSConfigurations> options)
        {
            this.assetService = _assetService;
            this._options = options.Value;
            this.s3BucketService = new S3BucketService();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadExcel()
        {
            Response_Message responcemodel = new Response_Message();
            //try
            //{
            List<IFormFile> filesToUpload = new List<IFormFile>();
            foreach (var file1 in Request.Form.Files)
            {
                filesToUpload.Add(file1);
            }

            var list = await s3BucketService.UploadImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.inspection_bucket_name);

            if (list.Count > 0)
            {
                responcemodel.success = 1;
                responcemodel.data = "uploaded successfully.";
                return Ok(responcemodel);
            }
            else
            {
                responcemodel.success = 0;
                Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                responcemodel.data = mess;
                return Ok(responcemodel);
            }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log("Error in UploadExcel " + ex.ToString());
            //    return StatusCode(500, "Internal server error");
            //}
        }

        /// <summary>
        /// Check Valid Internal Asset ID
        /// </summary>
        /// <param name="internal_asset_id" example="100300"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpGet("{internal_asset_id}")]
        public IActionResult ValidateInternalAssetID(string internal_asset_id)
        {
            Response_Message response = new Response_Message();
            //bool response = false;
            //try
            //{
            int isvalidate = assetService.ValidateInternalAssetID(internal_asset_id);
            if (isvalidate > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                //response = true;
            }
            else if (isvalidate == (int)ResponseStatusNumber.NotFound)
            {
                response.success = isvalidate;
                response.message = ResponseMessages.AssetNotFound;
            }
            else if (isvalidate == (int)ResponseStatusNumber.False)
            {
                response.success = isvalidate;
                response.message = ResponseMessages.AssetInMaintenanace;
            }
            else
            {
                response.success = isvalidate;
                response.message = ResponseMessages.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in ValidateInternalAssetID " + e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        /// <summary>
        /// Add Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> AddAssets([FromBody] AddAssetRequestModel request)
        {
            //bool result = false;
            Response_Message responcemodel = new Response_Message();
            List<string> result = new List<string>();
            //try
            //{
            result = await assetService.AddAssetsFromExcelAsync(request);
            if (result.Count == 0)
            {
                responcemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responcemodel.data = result;
            }
            else
            {
                responcemodel.data = result;
                responcemodel.message = ResponseMessages.Error;
                responcemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
            }
            //}
            //catch (Exception e)
            //{
            //    responcemodel.data = result;
            //    Logger.Log("Error in controller ", e.Message);
            //    responcemodel.success = (int)ResponseStatusNumber.Error;
            //    //responcemodel.message = e.Message.ToString();
            //    responcemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responcemodel);
        }

        /// <summary>
        /// Get All Asset 
        /// </summary>
        /// <param name="status" example="0"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet("{status}/{pagesize?}/{pageindex?}")]
        public IActionResult GetAllAssets(int status, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            response = assetService.GetAllAssets(status, pagesize, pageindex);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get sub assets(child assets) by parent asset id
        /// </summary>
        /// <param name="status" example="0"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet("{asset_id}/{pagesize?}/{pageindex?}")]
        public IActionResult GetSubAssetsByAssetID(string asset_id, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            response = assetService.GetSubAssetsByAssetID(asset_id, pagesize, pageindex);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get sub assets(child assets) by parent asset id
        /// </summary>
        /// <param name="status" example="0"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Technician" })]
        [HttpGet("{asset_id}/{pagesize?}/{pageindex?}")]
        public IActionResult GetChildrenByAssetID(string asset_id, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<GetChildrenByAssetIDResponsemodel> response = new ListViewModel<GetChildrenByAssetIDResponsemodel>();
            response = assetService.GetChildrenByAssetID(asset_id, pagesize, pageindex);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 1 && response.list.Count == 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllAssets ", e.ToString());
            //    responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //    //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //}
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Filtered Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive , Technician" })]
        [HttpPost]
        public IActionResult FilterAsset([FromBody] FilterAssetsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            FilterAssetsResponseview<AssetsResponseModel> response = new FilterAssetsResponseview<AssetsResponseModel>();
            requestModel.form_io_form_id = _options.LVCB_Form_id;
            response = assetService.FilterAssets(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Filtered Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive , Technician" })]
        [HttpPost]
        public IActionResult FilterAssetOptimized([FromBody] FilterAssetsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            requestModel.form_io_form_id = _options.LVCB_Form_id;
            var response = assetService.FilterAssetOptimized(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Filtered Asset Name Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterAssetNameOptions([FromBody] FilterAssetsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetListResponseModel> response = new ListViewModel<AssetListResponseModel>();
            response = assetService.FilterAssetsNameOptions(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Filtered Asset Model Options
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterAssetModelOptions([FromBody] FilterAssetsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<string> response = new ListViewModel<string>();
            response = assetService.FilterAssetsModelOptions(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterAssetModelYearOptions([FromBody] FilterAssetsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<string> response = new ListViewModel<string>();
            response = assetService.FilterAssetsModelYearOptions(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterAssetStatusOptions([FromBody] FilterAssetsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<int> response = new ListViewModel<int>();
            response = assetService.FilterAssetsStatusOptions(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterAssetSitesOptions([FromBody] FilterAssetsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<SitesViewModel> response = new ListViewModel<SitesViewModel>();
            response = assetService.FilterAssetsSitesOptions(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public IActionResult FilterAssetCompanyOptions([FromBody] FilterAssetsOptionsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<CompanyViewModel> response = new ListViewModel<CompanyViewModel>();
            response = assetService.FilterAssetsCompanyOptions(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Asset By ID
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive,Operator,Maintenance staff,Technician" })]
        [HttpPost("{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> GetAssetsById(int pagesize, int pageindex, [FromBody] GetAssetsByIdRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            AssetsResponseModel response = new AssetsResponseModel();
            requestModel.form_io_form_id = _options.LVCB_Form_id;
            response = await assetService.GetAsset(pagesize, pageindex, requestModel);
            if (response != null && response.role == 1)
            {
                responsemodel.message = ResponseMessages.Noinspectionform;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else if (response.message != null && response.message != string.Empty)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.AlreadyExists;
                responsemodel.message = response.message;
            }
            //else if (response.site_id != Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id))
            //{
            //    responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Forbidden;
            //    responsemodel.message = "Access Forbidden";

            //}
            else if (response != null && response.asset_id != null && response.asset_id != Guid.Empty)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (response != null && response.response_status == (int)ResponseStatusNumber.asset_in_different_location)
            {
                responsemodel.message = "QR code not linked to an Asset at " + UpdatedGenericRequestmodel.CurrentUser.site_name;
                responsemodel.success = (int)ResponseStatusNumber.asset_in_different_location;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAssetsById ", e.ToString());
            //    responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Error;
            //    responsemodel.message = ResponseMessages.Error;
            //}
            return Ok(responsemodel);
        }



        /// <summary>
        /// Get All Checkout Asset
        /// </summary>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> GetAllCheckedOutAssets(int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            ListViewModel<PendingAndCheckoutInspViewModel> responseModel = new ListViewModel<PendingAndCheckoutInspViewModel>();
            responseModel.pageIndex = pageindex;
            responseModel.pageSize = pagesize;
            responseModel = await assetService.GetAllCheckedOutAssets(pagesize, pageindex);
            if (responseModel.list != null && responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (pageindex > 1 && responseModel.list.Count == 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                //response.data = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAllCheckedOutAssets : ", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        //[HttpGet]
        //public IActionResult GetAllInspections()
        //{
        //    Response_Message responsemodel = new Response_Message();
        //    try
        //    {
        //        List<InspectionResponseModel> response = new List<InspectionResponseModel>();
        //        response = assetService.GetAllInspections();
        //        if (response.Count > 0)
        //        {
        //            responsemodel.success = (int)ReponseStatusNumber.Success;
        //            responsemodel.data = response;
        //        }
        //        else
        //        {
        //            responsemodel.message = "no data found";
        //            responsemodel.success = (int)ReponseStatusNumber.NotFound;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log("Error in GetAllAssets ", e.Message);
        //        responsemodel.success = (int)ReponseStatusNumber.Error;
        //        //HTTPRES = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //    }
        //    return Ok(responsemodel);
        //}

        /// <summary>
        /// Generate Asset Barcode
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager" })]
        [HttpPost]
        public async Task<ActionResult> GenerateAssetBarcode(GenerateAssetBarcodeRequestModel requestdata)
        {
            //try
            //{
            AssetBarcodeGenerator assetBarcodelists = new AssetBarcodeGenerator();
            //List<string> imageList = new List<string>();
            List<AssetBarcodeGeneratorList> assetBarcodelist = new List<AssetBarcodeGeneratorList>();
            var count = 0;
            foreach (var i in requestdata.assetList)
            {
                var assets = assetService.GetAssetDetailsByID(i);
                var qr_code = assets.QR_code;
                if (!String.IsNullOrEmpty(qr_code))
                {
                    AssetBarcodeGeneratorList asset = new AssetBarcodeGeneratorList();
                    var isExist = await s3BucketService.Exists(qr_code + ".png", _options.barcodes_bucket_name, _options.aws_access_key, _options.aws_secret_key);

                    if (isExist)
                    {
                        asset.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(qr_code + ".png");
                    }
                    else
                    {
                        var imagename = await CreateBarcode.GetQRCode(qr_code, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);
                        asset.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(imagename);
                    }


                    // var assets = assetService.GetAssetDetailsByQRcode(i);
                    if (assets != null && assets.asset_id != null && assets.asset_id != Guid.Empty)
                    {
                        asset.asset_id = assets.asset_id.ToString();
                        asset.asset_name = assets.name;
                        asset.internal_asset_id = "#" + assets.internal_asset_id;
                        asset.site_name = assets.Sites.site_name;
                    }
                    //assetlist.Add(asset);
                    assetBarcodelist.Add(asset);
                    count++;

                }

            }
            assetBarcodelists.asset = assetBarcodelist;
            var pdfBytes = CreateBarcode.CreatePDF(assetBarcodelists, CreatePDFTypes.Asset);
            var fileName = DateTime.Now.Ticks.ToString() + ".pdf";
            var filePathLocal = fileName;
            var done = await CreateBarcode.ByteArrayToFile(filePathLocal, pdfBytes, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);

            if (done)
            {
                //response.data = "https://s3-us-west-2.amazonaws.com/jarvis.barcodesimages/637116799754807863.pdf";
                //response.success = (int)ResponseStatusNumber.Success;
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(filePathLocal));
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filePathLocal
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");
                var FileStream = new FileStream(filePathLocal, FileMode.Open);
                return File(FileStream, "application/pdf", "Test.pdf");
                //return result;
            }
            else
            {
                return NotFound();
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GenerateAssetBarcode ", e.ToString());
            //    return NotFound();
            //}
        }




        /// <summary>
        /// Generate Asset Barcode
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager" })]
        [HttpPost]
        public async Task<ActionResult> GenerateRandomBarcodes(GenerateAssetBarcodeRequestModel requestdata)
        {

            AssetBarcodeGenerator assetBarcodelists = new AssetBarcodeGenerator();

            List<AssetBarcodeGeneratorList> assetBarcodelist = new List<AssetBarcodeGeneratorList>();
            var count = 0;
            for (int i = 1; i <= 350; i++)
            {
                requestdata.assetList.Add("cvsck-" + i.ToString());
            }

            var t = 1;
            foreach (var i in requestdata.assetList)
            {
                AssetBarcodeGeneratorList asset = new AssetBarcodeGeneratorList();

                var isExist = await s3BucketService.Exists(i + ".png", _options.barcodes_bucket_name, _options.aws_access_key, _options.aws_secret_key);

                if (isExist)
                {
                    asset.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(i + ".png");
                }
                else
                {
                    var imagename = await CreateBarcode.GetQRCode(i, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);
                    asset.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(imagename);
                }
                asset.internal_asset_id = i;

                assetBarcodelist.Add(asset);
                count++;
            }
            assetBarcodelists.asset = assetBarcodelist;
            var pdfBytes = CreateBarcode.CreateRandomBarcodePDF(assetBarcodelists, CreatePDFTypes.Asset);
            var fileName = DateTime.Now.Ticks.ToString() + ".pdf";
            var filePathLocal = fileName;
            var done = await CreateBarcode.ByteArrayToFile(filePathLocal, pdfBytes, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);

            if (done)
            {

                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(filePathLocal));
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filePathLocal
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");
                var FileStream = new FileStream(filePathLocal, FileMode.Open);
                return File(FileStream, "application/pdf", "Test.pdf");
                //return result;
            }
            else
            {
                return NotFound();
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GenerateAssetBarcode ", e.ToString());
            //    return NotFound();
            //}
        }

        /// <summary>
        /// Generate Asset Barcode
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager" })]
        [HttpPost]
        public async Task<ActionResult> GetBarcodesIds(GenerateAssetBarcodeRequestModel requestdata)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            AssetBarcodeGenerator assetBarcodelists = new AssetBarcodeGenerator();
            //List<string> imageList = new List<string>();
            List<AssetBarcodeGeneratorList> assetBarcodelist = new List<AssetBarcodeGeneratorList>();
            var count = 0;

            var t = 1;
            foreach (var i in requestdata.assetList)
            {
                AssetBarcodeGeneratorList asset = new AssetBarcodeGeneratorList();

                var isExist = await s3BucketService.Exists(i + ".png", _options.barcodes_bucket_name, _options.aws_access_key, _options.aws_secret_key);

                if (isExist)
                {
                    asset.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(i + ".png");
                }
                else
                {
                    var imagename = await CreateBarcode.GetQRCode(i, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);
                    asset.asset_barcode_image = UrlGenerator.GetAssetBarcodeImagesURL(imagename);
                }

                // asset = new AssetBarcodeGeneratorList();
                asset.internal_asset_id = i;
                //assetlist.Add(asset);
                assetBarcodelist.Add(asset);

                count++;
            }
            response.data = assetBarcodelist;
            return Ok(response);
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GenerateAssetBarcode ", e.ToString());
            //    return NotFound();
            //}
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            Response_Message responcemodel = new Response_Message();
            HttpResponseMessage HTTPRES;

            //try
            //{
            id = "637116799754807863.pdf";
            var FileStream = new FileStream(id, FileMode.Open);
            //var stream = new MemoryStream(System.IO.File.ReadAllBytes(id));

            //var result = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new ByteArrayContent(stream.ToArray())
            //};
            //result.Content.Headers.ContentDisposition =
            //    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            //    {
            //        FileName = id
            //    };
            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");
            return File(FileStream, "application/octet-stream", "Test.pdf"); // returns a FileStreamResult


            //}
            //catch (Exception e)
            //{
            //    Logger.Log(e.ToString());
            //    var result = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            //    {

            //    };
            //    return NotFound();
            //}


        }

        /// <summary>
        /// Search Asset
        /// </summary>
        /// <param name="searchstring" example=""></param>
        /// <param name="status" example="0"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager" })]
        [HttpGet("{searchstring}/{status}/{pagesize?}/{pageindex?}")]
        public async Task<IActionResult> SearchAssets(string searchstring, int status, int pagesize, int pageindex)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            searchstring = WebUtility.UrlDecode(searchstring);
            ListViewModel<AssetsResponseModel> responseModel = new ListViewModel<AssetsResponseModel>();
            responseModel = await assetService.SearchAssets(searchstring.Trim(), status, pagesize, pageindex);
            if (responseModel.list.Count > 0)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModel.list.Count == 0 && pageindex > 1)
            {
                response.data = responseModel;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in SearchAssets :", e.ToString());
            //    response.message = ResponseMessages.Error;
            //    response.success = (int)ResponseStatusNumber.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Upload Asset Photots
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager,Technician" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAssetPhoto()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            List<IFormFile> filesToUpload = new List<IFormFile>();
            foreach (var file1 in Request.Form.Files)
            {
                //Logger.Log("Images ", file1.FileName.ToString());
                filesToUpload.Add(file1);
            }

            var list = await s3BucketService.UploadImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.asset_bucket_name);
            if (list.Count > 0)
            {
                string stringassetGuid = Request.Form["asset_id"];
                ImagesListObjectViewModel images = new ImagesListObjectViewModel();
                List<string> images_names = new List<string>();
                foreach (var item in list)
                {
                    images_names.Add(item);
                }
                images.image_names = images_names;

                UploadAssetPhotoRequestModel requestModel = new UploadAssetPhotoRequestModel();
                requestModel.asset_id = stringassetGuid;
                requestModel.asset_photo = images_names.FirstOrDefault();
                var result = await assetService.UploadAssetPhoto(requestModel);
                if (result > 0)
                {
                    response.success = (int)ResponseStatusNumber.Success;
                }
                else if (result == (int)ResponseStatusNumber.NotFound)
                {
                    response.success = (int)ResponseStatusNumber.NotFound;
                    response.message = ResponseMessages.nodatafound;
                }
                else
                {
                    response.success = (int)ResponseStatusNumber.Error;
                    response.message = ResponseMessages.Error;
                }
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in UploadAssetPhoto ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get All Asset With InspectionForm
        /// </summary>
        /// <param name="timestamp" example="yyyy-MM-dd HH:mm:ss"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Operator,Company Admin" })]
        [HttpGet("{timestamp?}")]
        public async Task<IActionResult> GetAllAssetsWithInspectionForm(string timestamp)
        {
            Response_Message response = new Response_Message();
            var responseModels = await assetService.GetAllAssetsWithInspectionForm(timestamp);
            if (responseModels.Count > 0)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{i}")]
        public async Task<IActionResult> GetQRCode(string i)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var response1 = await CreateBarcode.GetQRCode(i, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);
            var barcodeimages = UrlGenerator.GetAssetBarcodeImagesURL(response1);

            AssetBarcodeGeneratorList asset = new AssetBarcodeGeneratorList();
            List<AssetBarcodeGeneratorList> assetBarcodelist = new List<AssetBarcodeGeneratorList>();
            asset.asset_barcode_image = barcodeimages;
            AssetBarcodeGenerator assetBarcode = new AssetBarcodeGenerator();
            asset.asset_id = "1234556";
            asset.asset_name = "ABC";
            asset.internal_asset_id = "# 123";
            asset.site_name = "Site";
            assetBarcodelist.Add(asset);
            assetBarcode.asset = assetBarcodelist;
            var pdfBytes = CreateBarcode.CreatePDF(assetBarcode, CreatePDFTypes.Asset);
            var fileName = DateTime.Now.Ticks.ToString() + ".pdf";
            var filePathLocal = fileName;
            var done = await CreateBarcode.ByteArrayToFile(filePathLocal, pdfBytes, _options.aws_access_key, _options.aws_secret_key, _options.barcodes_bucket_name);

            if (done)
            {
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(filePathLocal));
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filePathLocal
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");
                var FileStream = new FileStream(filePathLocal, FileMode.Open);
                return File(FileStream, "application/pdf", "Test.pdf");
            }
            //}
            //catch(Exception e)
            //{
            //    Logger.Log("Error in GetQRCode ", e.ToString());
            //    response.success = -2;
            //    response.message = e.ToString();
            //}
            return Ok(response);
        }

        //[HttpGet]
        //public IActionResult GetUTCDateTime()
        //{
        //    DateTime dt = DateTime.UtcNow;
        //    return Ok(dt);

        //}

        /// <summary>
        /// Get Asset Detailts By Id
        /// </summary>
        /// <param name="assetid" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager,Operator,Maintenance staff,Technician" })]
        [HttpGet("{assetid}")]
        public IActionResult GetAssetDetails(string assetid)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            var asset = assetService.GetAssetByAssetID(assetid);

            if (asset != null && asset.asset_id != null && asset.asset_id != string.Empty)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = asset;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.data = ResponseMessages.nodatafound;
            }
            //}catch(Exception e)
            //{
            //    Logger.Log("Error in GetAssetDetails ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Generate Asset Inspection Monthly Report
        /// </summary>
        /// <param name="fromdate" example="yyyy-MM-dd"></param>
        /// <param name="todate" example="yyyy-MM-dd"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Executive,Manager,Company Admin" })]
        [HttpGet("{fromdate}/{todate}")]
        public async Task<IActionResult> GetAssetInspectionReportMonthly(string fromdate, string todate)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            DateTime start = DateTime.ParseExact(fromdate, "yyyy-MM-dd", null);
            DateTime end = DateTime.ParseExact(todate, "yyyy-MM-dd", null);
            var report = await assetService.GetAssetInspectionReportMonthly(start, end);
            if (report.list.Count > 0)
            {
                response.data = report;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAssetInspectionReportMonthly ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Asset Inspection Report Weekly
        /// </summary>
        /// <param name="fromdate" example="yyyy-MM-dd"></param>
        /// <param name="todate" example="yyyy-MM-dd"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Executive,Manager,Company Admin" })]
        [HttpGet("{fromdate}/{todate}")]
        public async Task<IActionResult> GetAssetInspectionReportWeekly(string fromdate, string todate)
        {
            Response_Message response = new Response_Message();
            //try
            //{
            DateTime start = DateTime.ParseExact(fromdate, "yyyy-MM-dd", null);
            DateTime end = DateTime.ParseExact(todate, "yyyy-MM-dd", null);

            var report = await assetService.GetAssetInspectionReportWeekly(start, end);
            if (report.list.Count > 0)
            {
                response.data = report;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAssetInspectionReportWeekly ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Asset Report Weekly
        /// </summary>
        /// <param name="fromdate" example="yyyy-MM-dd"></param>
        /// <param name="todate" example="yyyy-MM-dd"></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Executive,Manager,Company Admin" })]
        [HttpGet("{fromdate}/{todate}")]
        public async Task<IActionResult> GetAssetReportWeekly(string fromdate, string todate)
        {
            Response_Message response = new Response_Message();
            //try
            //{

            DateTime start = DateTime.ParseExact(fromdate, "yyyy-MM-dd", null);
            DateTime end = DateTime.ParseExact(todate, "yyyy-MM-dd", null);

            var report = await assetService.GetAssetReportWeekly(start, end);
            if (report.list.Count > 0)
            {
                response.data = report;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in GetAssetReportWeekly ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Dashboard outstanding issue
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin" })]
        [HttpGet]
        public async Task<IActionResult> DashboardOutstandingIssues()
        {
            Response_Message response = new Response_Message();
            var report = await assetService.DashboardOutstandingIssues();
            response.data = report;
            if (report.reports != null && report.reports.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get Asset Latest Meterhours
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpGet]
        public async Task<IActionResult> GetLatestMeterHoursReport()
        {
            Response_Message response = new Response_Message();
            var report = await assetService.GetLatestMeterHoursReport();
            if (report.list.Count > 0)
            {
                response.data = report;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Update Asset Meterhours
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Company Admin,Executive" })]
        [HttpPut]
        public async Task<IActionResult> UpdateMeterHours([FromBody] UpdateMeterHoursRequestModel request)
        {
            Response_Message response = new Response_Message();
            int result = await assetService.UpdateMeterHours(request);
            if (result > 0)
            {
                response.success = result;
            }
            else if (result == (int)ResponseStatusNumber.NotFound)
            {
                response.success = result;
                response.message = ResponseMessages.nodatafound;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get Sync Data
        /// </summary>
        /// <param name="userid" example="a343180f-6474-42fd-a486-9f9491d91a43"></param>
        [TypeFilter(typeof(ValidationFilterAttribute))]
        [HttpGet("{userid}")]
        public async Task<IActionResult> SyncData(string userid)
        {
            Response_Message response = new Response_Message();
            SyncDataResponseModel responseModels = await assetService.GetSyncData(userid);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Generate Asset Inspection Report
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GenerateAssetInspectionReport([FromBody] AssetInspectionReportRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            DateTime from_date = DateTime.ParseExact(requestModel.from_date, "yyyy-MM-dd", null);
            DateTime to_date = DateTime.ParseExact(requestModel.to_date, "yyyy-MM-dd", null);
            var dateSpan = DateTime.Compare(from_date, to_date);
            if (from_date.AddMonths(3) > to_date && from_date <= to_date)
            {
                string aws_access_key = _options.aws_access_key;
                string aws_secret_key = _options.aws_secret_key;
                AssetInspectionReportResponseModel responseModels = await assetService.GenerateAssetInspectionReport(requestModel, from_date, to_date, aws_access_key, aws_secret_key);
                if (responseModels.success > 0)
                {
                    response.data = responseModels;
                    response.success = (int)ResponseStatusNumber.Success;
                }
                else if (responseModels.success == (int)ResponseStatusNumber.NotFound)
                {
                    response.success = (int)ResponseStatusNumber.NotFound;
                    response.message = ResponseMessages.nodatafound;
                }
                else if (responseModels.success == (int)ResponseStatusNumber.NotFoundInspectionForReport)
                {
                    response.success = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                    response.message = responseModels.message;
                }
                else if (responseModels.success == (int)ResponseStatusNumber.AlreadyExists)
                {
                    response.success = (int)ResponseStatusNumber.AlreadyExists;
                    response.message = ResponseMessages.AlreadyInspectionExists;
                }
                else
                {
                    response.success = (int)ResponseStatusNumber.Error;
                    response.message = ResponseMessages.Error;
                }
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.SelectValidDate;
            }
            return Ok(response);
        }

        /// <summary>
        /// Generate Asset Inspection Report Without Date Range To View in WEB
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Manager,Executive" })]
        [HttpPost]
        public async Task<IActionResult> GenerateAssetInspectionReportToView([FromBody] AssetInspectionReportRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            ListViewModel<InspectionResponseModel> responseModels = await assetService.GetAssetInspectionForReportView(requestModel);
            if (responseModels.result > 0)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else if (responseModels.result == (int)ResponseStatusNumber.NotFound)
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            else if (responseModels.result == (int)ResponseStatusNumber.NotFoundInspectionForReport)
            {
                response.success = (int)ResponseStatusNumber.NotFoundInspectionForReport;
                response.message = ResponseMessages.InspectionNotFound;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.Error;
            }

            return Ok(response);
        }

        /// <summary>
        /// Get Asset Inspection Report List
        /// </summary>
        /// <param name="internal_asset_id"></param>
        /// <param name="pagesize" example="10"></param>
        /// <param name="pageindex" example="1"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpGet]
        public IActionResult GetAssetInspectionReport(string internal_asset_id, int pagesize = 0, int pageindex = 0)
        {
            Response_Message response = new Response_Message();
            ListViewModel<AssetInspectionReportResponseModel> responseModels = assetService.GetAssetInspectionReport(internal_asset_id, pagesize, pageindex);
            if (responseModels != null)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Check Asset Inspection Report Status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> ReportStatus([FromBody] ReportStatusRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            List<AssetInspectionReportResponseModel> responseModels = await assetService.GetReportStatus(requestModel);
            if (responseModels != null && responseModels.Count() > 0)
            {
                response.data = responseModels;
                response.success = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /// <summary>
        /// Get All Asset List
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllAssets()
        {
            Response_Message response = new Response_Message();
            List<AssetListResponseModel> responseModels = assetService.GetAllAssetsList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }

        /// <summary>
        /// Get All Asset Model Name For Filter Column
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllAssetsModels()
        {
            Response_Message response = new Response_Message();
            List<string> responseModels = assetService.GetAllAssetsModelsList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }

        /// <summary>
        /// Get All Asset Model Year For Filter COlumn
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,Executive,SuperAdmin" })]
        [HttpGet]
        public IActionResult AllAssetsModelYear()
        {
            Response_Message response = new Response_Message();
            List<string> responseModels = assetService.GetAllAssetsModelYearsList();
            response.data = responseModels;
            response.success = (int)ResponseStatusNumber.Success;
            return Ok(response);
        }

        /// <summary>
        /// Update Asset Status
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,Manager,SuperAdmin,Executive" })]
        [HttpPost]
        public async Task<IActionResult> UpdateAssetStatus([FromBody] UpdateAssetStatusRequestModel requestModel)
        {
            Response_Message response = new Response_Message();
            int responseModel = await assetService.UpdateAssetStatus(requestModel);
            if (responseModel > 0)
            {
                response.success = responseModel;
            }
            else if (responseModel == (int)ResponseStatusNumber.NotFound)
            {
                response.success = responseModel;
                response.message = ResponseMessages.nodatafound;
            }
            else if (responseModel == (int)ResponseStatusNumber.InvalidStatus)
            {
                response.success = responseModel;
                response.message = ResponseMessages.InvalidStatus;
            }
            else
            {
                response.success = responseModel;
                response.message = ResponseMessages.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// Add Or Update Asset
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        public async Task<IActionResult> AddUpdateAsset([FromBody] AssetRequestModel asset)
        {
            Response_Message response_Message = new Response_Message();

            if (ModelState.IsValid)
            {
                var response = await assetService.InsertUpdateAssetDetails(asset);
                if (response.response_status > 0)
                {
                    response_Message.success = (int)ResponseStatusNumber.Success;
                    response_Message.message = ResponseMessages.RecordAdded;
                }
                else if (response.response_status == (int)ResponseStatusNumber.AlreadyExists)
                {
                    response_Message.message = ResponseMessages.RecordExist;
                    response_Message.success = (int)ResponseStatusNumber.AlreadyExists;
                }
                else if (!string.IsNullOrEmpty(response.message))
                {
                    response_Message.message = response.message;
                    response_Message.success = (int)ResponseStatusNumber.Error;
                }
                else
                {
                    response_Message.message = ResponseMessages.Error;
                    response_Message.success = (int)ResponseStatusNumber.Error;
                }
            }
            else
            {
                BadRequest(ModelState);
                response_Message.message = ResponseMessages.NotValidModel;
                response_Message.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Add or Update Asset Type
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        public async Task<IActionResult> AddUpdateAssetType([FromBody] AssetTypeRequestModel request)
        {
            Response_Message response_Message = new Response_Message();
            if (ModelState.IsValid)
            {
                AssetTypeResponseModel responseModel = new AssetTypeResponseModel();
                responseModel = await assetService.AddUpdateAssetType(request);
                if (responseModel.response_status > 0)
                {
                    response_Message.data = responseModel;
                    response_Message.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response_Message.data = responseModel;
                    response_Message.message = ResponseMessages.Error;
                    response_Message.success = (int)ResponseStatusNumber.False;
                }
            }
            return Ok(response_Message);
        }

        /// <summary>
        /// Get All Asset Types
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin" })]
        [HttpGet("{pageindex?}/{pagesize?}/{searchstring?}")]
        public async Task<IActionResult> GetAllAssetTypes(int pageindex = 0, int pagesize = 0, string searchstring = "")
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetTypeResponseModel> response = new ListViewModel<AssetTypeResponseModel>();
            response = await assetService.GetAllAssetTypes(pageindex, pagesize, searchstring);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
                responsemodel.message = ResponseMessages.nodatafound;
            }

            return Ok(responsemodel);
        }

        /// <summary>
        /// Get User Notification
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Company Admin,Technician" })]
        public async Task<IActionResult> GetAssetActivityLogs([FromQuery] int pageindex, [FromQuery] int pagesize, [FromQuery] string asset_id, [FromQuery] int filter_type)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<AssetActivityLogsViewModel> response = new ListViewModel<AssetActivityLogsViewModel>();
            response = await assetService.GetActivityLogs(GenericRequestModel.requested_by.ToString(), pagesize, pageindex, asset_id, filter_type);
            response.pageIndex = pageindex;
            response.pageSize = pagesize;
            if (response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (pageindex > 0 && response.list.Count == 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get PArent Child Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAllHierarchyAssets([FromBody] FilterAssetsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            response = assetService.GetAllHierarchyAssets(requestModel);
            if (response?.list?.Count > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get User Notification
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> GetAllRawHierarchyAssets()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            List<GetAllHierarchyAssetsResponseModel> response = new List<GetAllHierarchyAssetsResponseModel>();
            response = assetService.GetAllRawHierarchyAssets();

            if (response != null && response.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }
        /// <summary>
        /// Get User Notification
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public IActionResult GetAllAssetsForCluster([FromQuery] string wo_id)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            List<GetAllAssetsForClusterResponsemodel> response = new List<GetAllAssetsForClusterResponsemodel>();
            response = assetService.GetAllAssetsForCluster(wo_id);

            if (response != null && response.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Get User Notification
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> GetAssetLevelOptions()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            ListViewModel<string> response = new ListViewModel<string>();
            response = assetService.GetAssetLevelOptions();

            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// View Work Order By ID 
        /// </summary>
        /// <param name="wo_inspectionsTemplateFormIOAssignment_id" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{asset_id}")]
        public IActionResult GetNameplateInfoByAssetid(string asset_id)
        {
            Response_Message responsemodel = new Response_Message();
            //GetWOcategoryTaskByCategoryIDResponsemodel response = new GetWOcategoryTaskByCategoryIDResponsemodel();
            var response = assetService.GetNameplateInfoByAssetid(asset_id);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get PArent Child Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> ChangeAssetHierarchy([FromBody] ChangeAssetHierarchyRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            // ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            int response = await assetService.ChangeAssetHierarchy(requestModel);
            if (response > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        //  [ApiExplorerSettings(IgnoreApi = true)]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAssetExcelTest()
        {
            Response_Message responcemodel = new Response_Message();
            //try
            //{
            //  List<IFormFile> filesToUpload = new List<IFormFile>();
            //   foreach (var file1 in Request.Form.Files)
            //   {
            //       filesToUpload.Add(file1);
            //    }
            var t = await assetService.UploadAssetExcelTest();
            return Ok(responcemodel);

        }

        /// <summary>
        /// Upload Asset image from Web for multiple images
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager,Technician" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAssetImage()  /// Upload Asset image from Web for multiple images
        {
            Response_Message response = new Response_Message();
            //try
            //{ 
            List<IFormFile> filesToUpload = new List<IFormFile>();
            foreach (var file1 in Request.Form.Files)
            {
                //Logger.Log("Images ", file1.FileName.ToString());
                filesToUpload.Add(file1);
            }

            var list = await s3BucketService.UploadAssetMultipleImage(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.asset_bucket_name);
            List<MultipleImageUpload_ResponseModel_class> res = new List<MultipleImageUpload_ResponseModel_class>();
            WorkOrderAttachmentsResponseModel WorkOrderAttachmentsResponseModel = new WorkOrderAttachmentsResponseModel();
            foreach (var item in list)
            {
                if (item != null && item.original_imege_file != null)
                {
                    string stringassetGuid = Request.Form["asset_id"];
                    int asset_photo_type = 1;
                    if (!String.IsNullOrEmpty(Request.Form["asset_photo_type"]))
                    {
                        asset_photo_type = int.Parse(Request.Form["asset_photo_type"]);
                    }
                    UploadAssetPhotoRequestModel requestModel = new UploadAssetPhotoRequestModel();
                    requestModel.asset_id = stringassetGuid;
                    requestModel.asset_photo_type = asset_photo_type;
                    requestModel.asset_photo = item.original_imege_file;
                    requestModel.thumbnail_photo = item.thumbnail_image_file;
                    var result = await assetService.UploadAssetimage(requestModel);

                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = item.original_imege_file,
                        file_url = UrlGenerator.GetAssetNameplateImageURL(item.original_imege_file),
                        thumbnail_filename = item.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetAssetNameplateImageURL(item.thumbnail_image_file),
                        user_uploaded_name = item.user_uploaded_filename
                    };
                    var listitem = new MultipleImageUpload_ResponseModel_class
                    {
                        filename = item.original_imege_file,
                        asset_photo_type = asset_photo_type,
                        file_url = UrlGenerator.GetAssetNameplateImageURL(item.original_imege_file),
                        thumbnail_filename = item.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetAssetNameplateImageURL(item.thumbnail_image_file),
                        user_uploaded_name = item.user_uploaded_filename
                    };

                    response.success = 1;
                    res.Add(listitem);
                    WorkOrderAttachmentsResponseModel = items;
                }
            }
            response.data = res;
            response.success = 1;
            //if (result > 0)
            //{
            //    response.success = (int)ResponseStatusNumber.Success;
            //}
            //else if (result == (int)ResponseStatusNumber.NotFound)
            //{
            //    response.success = (int)ResponseStatusNumber.NotFound;
            //    response.message = ResponseMessages.nodatafound;
            //}
            //else
            //{
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in UploadAssetPhoto ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get PArent Child Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteAssetImage([FromBody] DeleteAssetImageRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            // ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            int response = await assetService.DeleteAssetImage(requestModel);
            if (response > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get PArent Child Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> EditAssetDetails([FromBody] EditAssetDetailsRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            // ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            var response = await assetService.EditAssetDetails(requestModel);
            if (response != null && response.status == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else if (response != null && response.status == (int)ResponseStatusNumber.qr_code_must_be_unique)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.qr_code_must_be_unique;
                responsemodel.message = "QR-Code must be unique!";
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get PArent Child Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> AddUpdateAssetNotes([FromBody] AddUpdateAssetNotesRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            // ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            int response = await assetService.AddUpdateAssetNotes(requestModel);
            if (response > 0)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get PArent Child Asset 
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> GetAssetNotes([FromBody] GetAssetNotesRequestmodel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            // ListViewModel<AssetsResponseModel> response = new ListViewModel<AssetsResponseModel>();
            var response = assetService.GetAssetNotes(requestModel);
            if (response != null)
            {
                responsemodel.success = (int)Shared.StatusEnums.ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> FilterAssetBuildingLocationOptions()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            FilterAssetBuildingLocationOptions response = new FilterAssetBuildingLocationOptions();
            response = assetService.FilterAssetBuildingLocationOptions();

            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> FilterAssetHierarchyLevelOptions()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{
            FilterAssetHierarchyLevelOptionsResponsemodel response = new FilterAssetHierarchyLevelOptionsResponsemodel();
            response = assetService.FilterAssetHierarchyLevelOptions();

            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Upload Asset Photots
        /// </summary>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin,SuperAdmin,Executive,Manager,Technician,Company Admin" })]
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAssetAttachments()
        {
            Response_Message response = new Response_Message();
            //try
            //{
            List<IFormFile> filesToUpload = new List<IFormFile>();
            foreach (var file1 in Request.Form.Files)
            {
                //Logger.Log("Images ", file1.FileName.ToString());
                filesToUpload.Add(file1);
            }

            var list = await s3BucketService.UploadAssetAttachment(filesToUpload, _options.aws_access_key, _options.aws_secret_key, _options.asset_attachment);
            if (list.Count > 0)
            {
                string stringassetGuid = Request.Form["asset_id"];
                string stringsite = Request.Form["site_id"];
                ImagesListObjectViewModel images = new ImagesListObjectViewModel();
                List<s3uploaded_files> images_names = new List<s3uploaded_files>();
                foreach (var item in list)
                {
                    s3uploaded_files s3uploaded_files = new s3uploaded_files();
                    s3uploaded_files.user_uploaded_file_name = item.user_uploaded_file_name;
                    s3uploaded_files.s3_file_name = item.s3_file_name;
                    images_names.Add(s3uploaded_files);
                }
                //images.image_names = images_names;

                UploadAssetAttachmentsRequestmodel requestModel = new UploadAssetAttachmentsRequestmodel();
                requestModel.asset_id = stringassetGuid;
                requestModel.site_id = stringsite;
                requestModel.S3_files = images_names;
                var result = await assetService.UploadAssetAttachments(requestModel);
                if (result > 0)
                {
                    response.success = (int)ResponseStatusNumber.Success;
                    response.message = ResponseMessages.RecordAdded;
                }
                else if (result == (int)ResponseStatusNumber.NotFound)
                {
                    response.success = (int)ResponseStatusNumber.NotFound;
                    response.message = ResponseMessages.nodatafound;
                }
                else
                {
                    response.success = (int)ResponseStatusNumber.Error;
                    response.message = ResponseMessages.Error;
                }
            }
            //}
            //catch (Exception e)
            //{
            //    Logger.Log("Error in UploadAssetPhoto ", e.ToString());
            //    response.success = (int)ResponseStatusNumber.Error;
            //    response.message = ResponseMessages.Error;
            //}
            return Ok(response);
        }

        /// <summary>
        /// Get Asset Attchments
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetAttachments(GetAssetAttachmentsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = assetService.GetAssetAttachments(requestmodel);
            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Asset Attchments
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteAssetAttachments(DeleteAssetAttachmentsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await assetService.DeleteAssetAttachments(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordDeleted;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Get Asset subcomponents
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetSubcomponentsByAssetId(GetSubcomponentsByAssetIdRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = assetService.GetSubcomponentsByAssetId(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }
        /// <summary>
        /// Edit Circuit for subcomponent
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateCircuitForAssetSubcomponent(UpdateCircuitForAssetSubcomponentRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await assetService.UpdateCircuitForAssetSubcomponent(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordUpdated;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Get Asset Attchments
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteAssetSubcomponent(DeleteAssetSubcomponentRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await assetService.DeleteAssetSubcomponent(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordDeleted;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Asset subcomponent to add
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public IActionResult GetSubcomponentAssetsToAddinAsset()
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{

            var response = assetService.GetSubcomponentAssetsToAddinAsset();

            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }


        /// <summary>
        /// Get Asset Attchments
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AddNewSubComponent(AddNewSubComponentRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await assetService.AddNewSubComponent(requestmodel);
            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.message = ResponseMessages.RecordAdded;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Get Asset Circuit
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public IActionResult GetAssetCircuitDetails(GetAssetCircuitDetailsRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{

            var response = assetService.GetAssetCircuitDetails(requestmodel);

            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Edit Asset Circuit
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> UpdateAssetFedByCircuit(UpdateAssetFedByCircuitRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{

            var response = await assetService.UpdateAssetFedByCircuit(requestmodel);

            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// <summary>
        /// Edit Asset Circuit
        /// </summary>
        /// <param name="requestmodel" example=""></param>
        [HttpPost]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public async Task<IActionResult> UpdateAssetFeedingCircuit(UpdateAssetFeedingCircuitRequestmodel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            //try
            //{

            var response = await assetService.UpdateAssetFeedingCircuit(requestmodel);

            if (response > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        /// Upload Pdf
        //[HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        //[RequestSizeLimit(209715200)]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UploadClusterOneLinePdf()
        {
            Response_Message responsemodel = new Response_Message();

            IFormFile filesToUpload = Request.Form.Files[0];
            var user_file_name = filesToUpload.FileName;

            if (Request.Form.Files.Count > 0)
            {
                var list = await s3BucketService.UploadClusterOneLinePdfService(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.conduit_dev_digital_pdf_bucket, UpdatedGenericRequestmodel.CurrentUser.site_id);

                UpdateDigitalOneLineRequestModel request = new UpdateDigitalOneLineRequestModel();
                request.upload_status = (int)ClusterOneLinePdf.Completed;
                request.file_name = UpdatedGenericRequestmodel.CurrentUser.site_id + Path.GetExtension(user_file_name);
                request.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);

                int response = await assetService.UpdateDigitalOneLine(request);

                UploadOnelinePdfResponseModel uploadOnelinePdfResponseModel = new UploadOnelinePdfResponseModel();
                uploadOnelinePdfResponseModel.file_name = request.file_name;
                uploadOnelinePdfResponseModel.file_url = UrlGenerator.GetClusterOnelinePdfURL(request.file_name);

                if (list != null && response == (int)ResponseStatusNumber.Success)
                {
                    responsemodel.success = 1;
                    responsemodel.data = uploadOnelinePdfResponseModel;
                    responsemodel.message = "File Uploaded Successfully!";
                    return Ok(responsemodel);
                }
                else
                {
                    responsemodel.success = 0;
                    Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                    responsemodel.data = mess;
                    return Ok(responsemodel);
                }
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }

        [HttpGet]
        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        public IActionResult GetUploadedOneLinePdfData(Guid siteId)
        {
            Response_Message responsemodel = new Response_Message();

            var response = assetService.GetUploadedOneLinePdfDataBySiteIdService(siteId);

            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }

            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult ExportAssetsLocationDetails(GetAssetsLocationDetailsRequestModel request)
        {
            Response_Message response = new Response_Message();

            ListViewModel<GetAssetsLocationDetailsResponseModel> responseModel = new ListViewModel<GetAssetsLocationDetailsResponseModel>();


            responseModel = assetService.GetAssetsLocationDetailsService(request);

            if (responseModel.list != null && responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetTopLevelAssets(GetTopLevelAssetsRequestModel request)
        {
            Response_Message response = new Response_Message();

            ListViewModel<GetTopLevelAssetsResponseModel> responseModel = new ListViewModel<GetTopLevelAssetsResponseModel>();


            responseModel = assetService.GetTopLevelAssetsService(request);

            if (responseModel.list != null && responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> ChangeSelectedAssetsLocation(ChangeSelectedAssetsLocationRequestModel request)
        {
            Response_Message response = new Response_Message();

            int res = await assetService.ChangeSelectedAssetsLocation(request);

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> InsertFormIOTemplate()
        {
            Response_Message response = new Response_Message();
            var res = await assetService.InsertFormIOTemplate();

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }



        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> DeleteLocationDetails(DeleteLocationDetailsRequestModel request)
        {
            Response_Message response = new Response_Message();

            int res = await assetService.DeleteLocationDetails(request);

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = res;
            }
            else if (res == (int)ResponseStatusNumber.AlreadyExists)
            {
                response.success = (int)ResponseStatusNumber.AlreadyExists;
                response.message = "Unable to delete location. Assets are still present in this location.";
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateLocationDetails(UpdateLocationDetailsRequestModel request)
        {
            Response_Message response = new Response_Message();

            int res = await assetService.UpdateLocationDetails(request);

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.message = ResponseMessages.RecordUpdated;
                response.data = res;
            }
            else if (res == (int)ResponseStatusNumber.AlreadyExists)
            {
                response.success = (int)ResponseStatusNumber.AlreadyExists;
                response.message = "Update error: Location name must be unique!";
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetOBWOCompletedWOlinesOfRequestedAsset(GetOBWOAssetsOfRequestedAssetRequestModel request)
        {
            Response_Message response = new Response_Message();

            ListViewModel<GetOBWOAssetsOfRequestedAssetResponseModel> responseModel = new ListViewModel<GetOBWOAssetsOfRequestedAssetResponseModel>();

            responseModel = assetService.GetOBWOAssetsOfRequestedAsset(request);

            if (responseModel.list != null && responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAllWOOBAssetsByAssetId(GetOBWOAssetsOfRequestedAssetRequestModel request)
        {
            Response_Message response = new Response_Message();

            ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel> responseModel = new ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel>();

            responseModel = assetService.GetAllWOOBAssetsByAssetId(request);

            if (responseModel.list != null && responseModel.list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpPost]
        public IActionResult GetAssetFeedingCircuitForReport(GetAssetFeedingCircuitForReportRequestmdel request)
        {
            Response_Message response = new Response_Message();

            // ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel> responseModel = new ListViewModel<GetAllWOOBAssetsByAssetIdResponseModel>();

            var responseModel = assetService.GetAssetFeedingCircuitForReport(request);

            if (responseModel.success == (int)ResponseStatusNumber.Success)// &&  responseModel.asset_feeding_circuit_list != null && responseModel.asset_feeding_circuit_list.Count > 0)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else if (responseModel.success == (int)ResponseStatusNumber.Error)
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = responseModel.message;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet]
        public async Task<IActionResult> ScriptForAddTempLocations()
        {
            Response_Message response = new Response_Message();

            int res = await assetService.ScriptForAddTempLocations();

            if (res == (int)ResponseStatusNumber.Success)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.message = ResponseMessages.RecordUpdated;
                response.data = res;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }
            return Ok(response);
        }

        /*
        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAssetNameplateImage()
        {
            Response_Message responsemodel = new Response_Message();

            List<IFormFile> filesToUpload = new List<IFormFile>();
            var user_file_name = String.Empty;

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    user_file_name = file.FileName;
                    filesToUpload.Add(file);
                }

                var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.asset_bucket_name);

                if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetAssetNameplateImageURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetAssetNameplateImageURL(list.thumbnail_image_file),
                        user_uploaded_name = user_file_name
                    };
                    responsemodel.success = 1;
                    responsemodel.data = items;
                    return Ok(responsemodel);
                }
                else
                {
                    responsemodel.success = 0;
                    Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                    responsemodel.data = mess;
                    return Ok(responsemodel);
                }
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }*/

        [HttpPost, RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadAssetNameplateImage()
        {
            Response_Message responsemodel = new Response_Message();

            List<IFormFile> filesToUpload = new List<IFormFile>();
            var user_file_name = String.Empty;

            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    user_file_name = file.FileName;
                    filesToUpload.Add(file);
                }

                //var list = await s3BucketService.UploadAssetImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.asset_bucket_name);
                var images_list = await s3BucketService.UploadAssetMultipleImage(filesToUpload, _options.S3_aws_access_key, _options.S3_aws_secret_key, _options.asset_bucket_name);
                List<MultipleImageUpload_ResponseModel_class> res = new List<MultipleImageUpload_ResponseModel_class>();
                WorkOrderAttachmentsResponseModel WorkOrderAttachmentsResponseModel = new WorkOrderAttachmentsResponseModel();
                foreach (var img in images_list)
                {
                    if (img != null && img.original_imege_file != null)
                    {
                        var items = new WorkOrderAttachmentsResponseModel
                        {
                            filename = img.original_imege_file,
                            file_url = UrlGenerator.GetAssetNameplateImageURL(img.original_imege_file),
                            thumbnail_filename = img.thumbnail_image_file,
                            thumbnail_file_url = UrlGenerator.GetAssetNameplateImageURL(img.thumbnail_image_file),
                            user_uploaded_name = img.user_uploaded_filename
                        };
                        var listitem = new MultipleImageUpload_ResponseModel_class
                        {
                            filename = img.original_imege_file,
                            file_url = UrlGenerator.GetAssetNameplateImageURL(img.original_imege_file),
                            thumbnail_filename = img.thumbnail_image_file,
                            thumbnail_file_url = UrlGenerator.GetAssetNameplateImageURL(img.thumbnail_image_file),
                            user_uploaded_name = img.user_uploaded_filename
                        };

                        responsemodel.success = 1;
                        res.Add(listitem);
                        WorkOrderAttachmentsResponseModel = items;
                        //return Ok(responsemodel);
                    }
                    /*else
                    {
                        responsemodel.success = 0;
                        Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                        responsemodel.data = mess;
                        return Ok(responsemodel);
                    }*/
                }
                WorkOrderAttachmentsResponseModel.image_list = res;
                responsemodel.data = WorkOrderAttachmentsResponseModel;
                return Ok(responsemodel);
                /*if (list != null && list.original_imege_file != null)
                {
                    var items = new WorkOrderAttachmentsResponseModel
                    {
                        filename = list.original_imege_file,
                        file_url = UrlGenerator.GetAssetNameplateImageURL(list.original_imege_file),
                        thumbnail_filename = list.thumbnail_image_file,
                        thumbnail_file_url = UrlGenerator.GetAssetNameplateImageURL(list.thumbnail_image_file),
                        user_uploaded_name = user_file_name
                    };
                    responsemodel.success = 1;
                    responsemodel.data = items;
                    return Ok(responsemodel);
                }
                else
                {
                    responsemodel.success = 0;
                    Res_ErrorMessage mess = new Res_ErrorMessage(Resource.ResponseMessages.UPLOADFILE_INVALIDFORMAT, Resource.ResponseMessages.UPLOADFILE_TITLE);
                    responsemodel.data = mess;
                    return Ok(responsemodel);
                }*/
            }
            else
            {
                responsemodel.success = 0;
                responsemodel.data = "No file selected!!!";
                return Ok(responsemodel);
            }
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult GetAllImagesForAsset(Guid asset_id)
        {
            Response_Message response = new Response_Message();

            GetAllImagesForAssetResponseModel responseModel = new GetAllImagesForAssetResponseModel();

            responseModel = assetService.GetAllImagesForAsset(asset_id);

            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin,SuperAdmin" })]
        [HttpGet]
        public IActionResult GetLocationDataByAssetId(Guid asset_id)
        {
            Response_Message response = new Response_Message();

            GetLocationDataByAssetIdResponseModel responseModel = new GetLocationDataByAssetIdResponseModel();

            responseModel = assetService.GetLocationDataByAssetId(asset_id);

            if (responseModel != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.data = responseModel;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.NotFound;
                response.message = ResponseMessages.nodatafound;
            }

            return Ok(response);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public IActionResult GetAssetDetailsByIdForTempAsset(GetAssetDetailsByIdForTempAsset requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAssetDetailsByIdForTempAsset(requestModel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GetOBTopLevelFedByAssetList(GetOBTopLevelFedByAssetListRequestModel requestmodel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetOBTopLevelFedByAssetList(requestmodel);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = null;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{wo_id?}/{woonboaardingasset_id?}")]
        public IActionResult GetTopLevelSubLevlComponentHiararchy(Guid? wo_id, string woonboaardingasset_id)
        {
            Response_Message responsemodel = new Response_Message();

            var response = assetService.GetTopLevelSubLevlComponentHiararchy(wo_id, woonboaardingasset_id);
            if (response != null)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UploadBulkMainAssets(UploadBulkMainAssetsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetService.UploadBulkMainAssets(requestModel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (response == (int)ResponseStatusNumber.InvalidData)
            {
                responsemodel.message = "No data found!";
                responsemodel.success = (int)ResponseStatusNumber.InvalidData;
            }
            else if (response == (int)ResponseStatusNumber.asset_class_not_found)
            {
                responsemodel.message = "Asset-Class not found!";
                responsemodel.success = (int)ResponseStatusNumber.asset_class_not_found;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> AssetImageDeleteOrSetAsProfile(AssetImageDeleteOrSetAsProfileRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await assetService.AssetImageDeleteOrSetAsProfile(requestModel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{pagesize?}/{pageindex?}")]
        public IActionResult GetAllBuildingLocations(int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllBuildingLocations(pagesize, pageindex);
            if (response != null && response.building_list != null && response.building_list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else if (response.listsize == 0)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{formiobuilding_id}/{pagesize?}/{pageindex?}")]
        public IActionResult GetAllFloorsByBuilding(int formiobuilding_id, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllFloorsByBuilding(formiobuilding_id, pagesize, pageindex);
            if (response != null && response.floor_list != null && response.floor_list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else if (response.listsize == 0)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{formiobuilding_id}")]
        public IActionResult GetAllFloorsByBuilding_Dropdown(int formiobuilding_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllFloorsByBuilding_Dropdown(formiobuilding_id);
            if (response != null && response.floor_list != null && response.floor_list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else if (response.listsize == 0)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpGet("{formiofloor_id}/{pagesize?}/{pageindex?}")]
        public IActionResult GetAllRoomsByFloor(int formiofloor_id, int pagesize, int pageindex)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllRoomsByFloor(formiofloor_id, pagesize, pageindex);
            if (response != null && response.room_list != null && response.room_list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else if (response.listsize == 0)
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> GetNameplateJsonFromImages(GetNameplateJsonFromImagesRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();

            var response = await assetService.GetNameplateJsonFromImages(requestModel);
            if (response.status == (int)Status.Completed)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else if (response.status == (int)ResponseStatusNumber.TimedOut)
            {
                responsemodel.success = (int)ResponseStatusNumber.TimedOut;
                responsemodel.data = response;
                responsemodel.message = "Your request has been timed out, please try again!";
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> EditBulkMainAssets(EditBulkMainAssetsRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetService.EditBulkMainAssets(requestModel);
            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
            }
            else
            {
                responsemodel.message = ResponseMessages.Error;
                responsemodel.success = (int)ResponseStatusNumber.Error;
            }

            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateAssetGroup(CreateUpdateAssetGroupRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = await assetService.CreateUpdateAssetGroup(requestModel);

            if (response != null && response.status == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet]
        public IActionResult AssetGroupsDropdownList()
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.AssetGroupsDropdownList();

            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet("{asset_group_id}")]
        public IActionResult AssetListDropdownForAssetGroup(Guid asset_group_id)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.AssetListDropdownForAssetGroup(asset_group_id);

            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }


        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllAssetGroupsList(GetAllAssetGroupsListRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllAssetGroupsList(requestModel);

            if (response != null && response.list!=null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public IActionResult GetAllOnboardingAssetsByWOId(GetAllOnboardingAssetsByWOIdRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllOnboardingAssetsByWOId(requestModel);

            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpGet]
        public IActionResult GetAllAssetsListForReactFlow()
        {
            Response_Message responsemodel = new Response_Message();
            var response = assetService.GetAllAssetsListForReactFlow();

            if (response != null && response.list != null && response.list.Count > 0)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.SuccessMessage;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }
            return Ok(responsemodel);
        }

        [TypeFilter(typeof(AuthenticationFilter), Arguments = new object[] { "Manager,Executive,Maintenance staff,Technician, Company Admin" })]
        [HttpPost]
        public async Task<IActionResult> UpdateAssetsPositionForReactFlow(UpdateAssetsPositionForReactFlowRequestModel requestModel)
        {
            Response_Message responsemodel = new Response_Message();
            int response = await assetService.UpdateAssetsPositionForReactFlow(requestModel);

            if (response == (int)ResponseStatusNumber.Success)
            {
                responsemodel.success = (int)ResponseStatusNumber.Success;
                responsemodel.data = response;
                responsemodel.message = ResponseMessages.RecordUpdated;
            }
            else
            {
                responsemodel.message = ResponseMessages.nodatafound;
                responsemodel.success = (int)ResponseStatusNumber.NotFound;
            }

            return Ok(responsemodel);
        }
    }
}
