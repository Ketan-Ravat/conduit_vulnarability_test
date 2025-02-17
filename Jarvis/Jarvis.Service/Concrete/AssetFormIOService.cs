using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using iTextSharp.text.pdf;
using Jarvis.db.DBResponseModel;
using Jarvis.db.Migrations;
using Jarvis.db.Models;
using Jarvis.db.MongoDB;
using Jarvis.Repo.Concrete;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.Helper;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using MimeKit.Encodings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.WebPages;
using ThirdParty.Json.LitJson;
using TimeZoneConverter;
using static Jarvis.Service.Concrete.WorkOrderService;
using Logger = Jarvis.Shared.Utility.Logger;
using NetaInspectionBulkReportTracking = Jarvis.db.Models.NetaInspectionBulkReportTracking;

namespace Jarvis.Service.Concrete
{
    public class AssetFormIOService : BaseService, IAssetFormIOService
    {
        public readonly IMapper _mapper;
        private Logger _logger;
        private readonly MongoDBContext _Mongodb;
        private readonly IS3BucketService s3BucketService;

        public AssetFormIOService(IMapper mapper, MongoDBContext Mongodb) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<AssetFormIOService>();
            _Mongodb = Mongodb;
            this.s3BucketService = new S3BucketService();
        }

        public async Task<AssetFormIOResponseModel> GetAssetFormIOByAssetId(Guid asset_id)
        {
            AssetFormIOResponseModel formResponse = new AssetFormIOResponseModel();
            try
            {
                var formDetails = await _UoW.AssetFormIORepository.GetAssetFormIOByAssetId(asset_id);
                if (formDetails != null)
                {
                    formResponse = _mapper.Map<AssetFormIOResponseModel>(formDetails);
                }
                else
                {
                    formResponse.response_status = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
            return formResponse;
        }
        class Customer
        {
            //this is list of value objects (value is a dictionary) 
            public List<Dictionary<String, dynamic>> Value { get; set; }

        }
        public async Task<AssetFormIOResponseModel> AddUpdateAssetFormIO(AssetFormIORequestModel formRequest, string LVCB_Form_id)
        {
            AssetFormIOResponseModel formResponse = new AssetFormIOResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    /*   dynamic dynamicform1 = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                       dynamic dynamicform2 = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                       string name = dynamicform1.data.nameplateInformation["manufacturer"];
                       dynamicform1.data.nameplateInformation["manufacturer"] = "ttt";
                       name = dynamicform1.data.nameplateInformation["manufacturer"];
                       dynamicform2.data.nameplateInformation["manufacturer"] = "RRR";
                       dynamicform2.data.nameplateInformation["model"] = "RR22R";
                       List<string> str = new List<string> { "manufacturer", "model" };
                       foreach(var test in str)
                       {
                           dynamicform1.data.nameplateInformation[test] = dynamicform2.data.nameplateInformation[test];
                       }
                       name = dynamicform1.data.nameplateInformation["manufacturer"];
                       name = dynamicform1.data.nameplateInformation["model"];
                    */
                    ////////Updated
                    ///
                    if (formRequest.asset_form_id != null && formRequest.asset_form_id != Guid.Empty)
                    {
                        var formDetails = await _UoW.AssetFormIORepository.GetAssetFormIOById(formRequest.asset_form_id);
                        if (formDetails != null)
                        {
                            formDetails.asset_form_name = formRequest.asset_form_name;
                            formDetails.asset_form_type = formRequest.asset_form_type;
                            // formDetails.asset_form_data = formRequest.asset_form_data;
                            formDetails.status = formRequest.status;
                            formDetails.asset_form_description = formRequest.asset_form_description;
                            formDetails.modified_at = DateTime.UtcNow;
                            formDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            formDetails.requested_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                            formDetails.inspected_at = DateTime.UtcNow;
                            formDetails.WOcategorytoTaskMapping.updated_at = DateTime.UtcNow;
                            //Update AssetPM Status 
                            if (formDetails.AssetPMs != null && formDetails.AssetPMs.Count > 0)
                            {
                                formDetails.AssetPMs.ToList().ForEach(i =>
                                {
                                    if (formDetails.status == (int)Status.InProgress || formDetails.status == (int)Status.Ready_for_review)
                                    {
                                        i.status = (int)Status.InProgress;
                                        i.modified_at = DateTime.UtcNow;
                                        i.modified_by = GenericRequestModel.requested_by.ToString();
                                    }
                                });
                            }

                             
                            //formRequest.asset_form_data = "{\"data\": " + formRequest.asset_form_data + "}"  ;

                            //  if (formDetails.intial_form_filled_date == null)
                            // {
                            try
                            {
                                TimeZoneInfo estTimeZoneInfo = TZConvert.GetTimeZoneInfo(formDetails.Sites.timezone);
                                //LogHelper.Log<TimeZonerror>(LogLevel.Information, " time zone info " + estTimeZoneInfo);
                                dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(formRequest.asset_form_data, new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    DateParseHandling = Newtonsoft.Json.DateParseHandling.None
                                });



                                string initial_date = dynamicform.data.header.date;
                                if (String.IsNullOrEmpty(initial_date))  // if initial date is null or empty then only assign date to it
                                {

                                    formDetails.intial_form_filled_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, estTimeZoneInfo);
                                    dynamicform.data.header.date = formDetails.intial_form_filled_date;
                                    formDetails.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicform);
                                    formRequest.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicform);
                                }
                                else
                                {
                                    formDetails.intial_form_filled_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(initial_date), estTimeZoneInfo);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation("date time convert issue" + ex.Message);
                                formDetails.intial_form_filled_date = DateTime.UtcNow;
                            }
                            // }
                            try
                            {
                                /* var form_obj = JsonSerializer.Deserialize<FormIOObject.Root>(formRequest.asset_form_data);
                                 var dynamic_form_obj = JsonSerializer.Deserialize<FormioDynamicobj.Root>(formRequest.asset_form_data);
                                 var dynamic__data_form_obj = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(formRequest.asset_form_data);
                                 if (dynamic_form_obj != null && dynamic_form_obj.data != null)
                                 {
                                     var nameplate = dynamic_form_obj.data.nameplateInformation;
                                     string nameplateString = JsonSerializer.Serialize(nameplate);
                                     var identification = form_obj.data.header.identification;
                                     var location = form_obj.data.header.parent;
                                     var asset_id = form_obj.data.header.assetId;
                                     formDetails.form_retrived_asset_name = identification;
                                     formDetails.form_retrived_location = location;
                                     formDetails.form_retrived_asset_id = asset_id;
                                     formDetails.form_retrived_nameplate_info = nameplateString;
                                     formDetails.form_retrived_data = JsonSerializer.Serialize(dynamic__data_form_obj.data); ;
                                 }*/

                                //dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                                dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(formRequest.asset_form_data, new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    DateParseHandling = Newtonsoft.Json.DateParseHandling.None
                                });
                                if (dynamicform != null && dynamicform.data != null && dynamicform.data.header != null)
                                {
                                    var identification = dynamicform.data.header.identification;
                                    var location = dynamicform.data.header.parent;
                                    var asset_id = dynamicform.data.header.assetId;
                                    formDetails.form_retrived_asset_name = identification;
                                    formDetails.form_retrived_location = location;
                                    formDetails.form_retrived_asset_id = asset_id;
                                    formDetails.form_retrived_workOrderType = dynamicform.data.header.workOrderType;
                                    formDetails.form_retrived_nameplate_info = dynamicform.data.nameplateString;

                                    string building = dynamicform.data.header.building;
                                    string floor = dynamicform.data.header.floor;
                                    string room = dynamicform.data.header.room;
                                    string section = dynamicform.data.header.section;

                                    formResponse.building = building;
                                    formResponse.floor = floor;
                                    formResponse.room = room;
                                    formResponse.section = section;

                                    //update in requested form
                                    formDetails.building = building;
                                    formDetails.floor = floor;
                                    formDetails.room = room;
                                    formDetails.section = section;
                                }
                                try
                                {
                                    if (dynamicform != null && dynamicform.data != null && dynamicform.data.footer != null)
                                    {
                                        if (dynamicform.data.footer.inspectionVerdict != null)
                                        {
                                            string inspection_verdict = dynamicform.data.footer.inspectionVerdict;
                                            if (!String.IsNullOrEmpty(inspection_verdict))
                                            {
                                                inspection_verdict = inspection_verdict.Trim().ToLower();
                                                if (inspection_verdict == inspectionVerdictname.acceptable || inspection_verdict == inspectionVerdictname_2.acceptable)
                                                {
                                                    formDetails.inspection_verdict = (int)inspectionVerdictnumber.acceptable;
                                                }
                                                else if (inspection_verdict == inspectionVerdictname.alert || inspection_verdict == inspectionVerdictname_2.alert)
                                                {
                                                    formDetails.inspection_verdict = (int)inspectionVerdictnumber.alert;
                                                }
                                                else if (inspection_verdict == inspectionVerdictname.danger || inspection_verdict == inspectionVerdictname_2.danger)
                                                {
                                                    formDetails.inspection_verdict = (int)inspectionVerdictnumber.danger;
                                                }
                                                else if (inspection_verdict == inspectionVerdictname.defective || inspection_verdict == inspectionVerdictname_2.defective)
                                                {
                                                    formDetails.inspection_verdict = (int)inspectionVerdictnumber.defective;
                                                }
                                            }

                                        }
                                        if (dynamicform.data.footer.defects != null)
                                        {
                                            string defects = dynamicform.data.footer.defects;
                                            if (defects.ToLower().Trim() == "yes")
                                            {
                                                formDetails.defects = true;
                                            }
                                            else //if (defects.ToLower().Trim() == "no")
                                            {
                                                formDetails.defects = false;
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {

                                }
                                object _data = dynamicform;
                                formDetails.form_retrived_data = Newtonsoft.Json.JsonConvert.SerializeObject(_data);
                                formDetails.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(_data);
                            }
                            catch (Exception ex)
                            {

                            }
                            result = _UoW.AssetFormIORepository.Update(formDetails).Result;
                            if (result > 0)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();

                                if (formDetails.wo_id != null && formDetails.wo_id.Value != Guid.Empty)
                                {
                                    WorkOrderService WorkOrderService = new WorkOrderService(_mapper);
                                    await WorkOrderService.updateWOCategoryStatusforStatusmanage(formDetails.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id);
                                    await WorkOrderService.updateWOStatusforStatusmanage(formDetails.wo_id.Value);
                                    /*var wo = _UoW.WorkOrderRepository.GetWOByidforDelete(formDetails.wo_id.Value);
                                    var get_all_category_of_WO = _UoW.WorkOrderRepository.GetAllCatagoryOFWO(formDetails.wo_id.Value);
                                    if (get_all_category_of_WO.All(q => q.status_id == (int)Status.Submitted) || get_all_category_of_WO.All(q=>q.status_id == (int)Status.Ready_for_review) || get_all_category_of_WO.All(q => q.status_id == (int)Status.Completed))
                                    {
                                        wo.status = (int)Status.Ready_for_review;
                                        wo.modified_at = DateTime.UtcNow;
                                        var res1ult = await _UoW.BaseGenericRepository<db.Models.WorkOrders>().Update(wo);
                                    }
                                    else
                                    {
                                        wo.status = (int)Status.ReleasedOpenWO;
                                        wo.modified_at = DateTime.UtcNow;
                                        var res1ult = await _UoW.BaseGenericRepository<db.Models.WorkOrders>().Update(wo);
                                    }*/
                                }

                                // insert asset form io insulation resistance test in table
                                var form_list = LVCB_Form_id.Split(',').ToList().Where(x => !String.IsNullOrEmpty(x)).ToList();

                                if (form_list.Contains(formRequest.form_id.ToString()))
                                {
                                    FormIOInsulationResistanceTestMapping FormIOInsulationResistanceTestMapping = null;
                                    FormIOInsulationResistanceTestMapping = _UoW.AssetFormIORepository.GetFormIOInsulationResistanceTestMappingByAssetFormID(formRequest.asset_form_id);
                                    if (FormIOInsulationResistanceTestMapping == null)
                                    {
                                        FormIOInsulationResistanceTestMapping = new FormIOInsulationResistanceTestMapping();
                                    }
                                    try
                                    {
                                        dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                                        if (dynamicform != null && dynamicform.data != null && dynamicform.data.insulationResistancePoleToPole != null)
                                        {
                                            FormIOInsulationResistanceTestMapping.IRPoletoPoleAsFound1 = dynamicform.data.insulationResistancePoleToPole.p1AsFound;
                                            FormIOInsulationResistanceTestMapping.IRPoletoPoleAsFound2 = dynamicform.data.insulationResistancePoleToPole.p2AsFound;
                                            FormIOInsulationResistanceTestMapping.IRPoletoPoleAsFound3 = dynamicform.data.insulationResistancePoleToPole.p3AsFound;
                                        }
                                        if (dynamicform != null && dynamicform.data != null && dynamicform.data.insulationResistanceAcrossPole != null)
                                        {
                                            FormIOInsulationResistanceTestMapping.IRAcrossPoleAsFound1 = dynamicform.data.insulationResistanceAcrossPole.p1AsFound;
                                            FormIOInsulationResistanceTestMapping.IRAcrossPoleAsFound2 = dynamicform.data.insulationResistanceAcrossPole.p2AsFound;
                                            FormIOInsulationResistanceTestMapping.IRAcrossPoleAsFound3 = dynamicform.data.insulationResistanceAcrossPole.p3AsFound;
                                        }
                                        FormIOInsulationResistanceTestMapping.asset_form_id = formRequest.asset_form_id;
                                        if (FormIOInsulationResistanceTestMapping.FormIOInsulationResistanceTestMapping_id == null || FormIOInsulationResistanceTestMapping.FormIOInsulationResistanceTestMapping_id == Guid.Empty)
                                        {
                                            FormIOInsulationResistanceTestMapping.created_at = DateTime.UtcNow;
                                            FormIOInsulationResistanceTestMapping.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            var insert = await _UoW.BaseGenericRepository<FormIOInsulationResistanceTestMapping>().Insert(FormIOInsulationResistanceTestMapping);
                                            _UoW.SaveChanges();
                                        }
                                        else
                                        {
                                            FormIOInsulationResistanceTestMapping.updated_at = DateTime.UtcNow;
                                            FormIOInsulationResistanceTestMapping.updated_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                            var update = await _UoW.BaseGenericRepository<FormIOInsulationResistanceTestMapping>().Update(FormIOInsulationResistanceTestMapping);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                //// add temp issues based on inspection verdict 
                                ///
                                if (formDetails.WorkOrders.wo_type == (int)Status.Maintenance_WO)
                                {
                                    AddUpdateTempIssueFromWORequestmodel tempIssueRequest = InspectionMakeRequestmodelforWOlineIssue(formDetails.asset_form_id);
                                    if (tempIssueRequest.list_temp_issue != null && tempIssueRequest.list_temp_issue.Count > 0)
                                    {
                                        WorkOrderService woservice = new WorkOrderService(_mapper);
                                        await woservice.AddUpdateTempIssueFromWO(tempIssueRequest);
                                    }
                                }
                                formResponse = _mapper.Map<AssetFormIOResponseModel>(formDetails);
                                formResponse.response_status = result;
                            }
                        }
                    }
                    else
                    {
                        var asset_details = _UoW.AssetRepository.GetAssetByAssetID(formRequest.asset_id.ToString());
                        var addForm = _mapper.Map<AssetFormIO>(formRequest);
                        addForm.asset_form_name = formRequest.asset_form_name;
                        addForm.asset_form_type = formRequest.asset_form_type;
                        addForm.asset_form_data = formRequest.asset_form_data;
                        addForm.asset_form_description = formRequest.asset_form_description;
                        addForm.status = (int)Status.Ready_for_review;
                        addForm.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        addForm.created_at = DateTime.UtcNow;
                        addForm.requested_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                        addForm.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);


                        result = await _UoW.AssetFormIORepository.Insert(addForm);
                        if (result > 0)
                        {
                            /// add form data in mongo db 
                            tbl_mongo_form_io tbl_mongo_form_io = new tbl_mongo_form_io();
                            tbl_mongo_form_io.asset_id = formRequest.asset_id.ToString();
                            tbl_mongo_form_io.asset_form_name = formRequest.asset_form_name;
                            tbl_mongo_form_io.asset_form_type = formRequest.asset_form_type;
                            tbl_mongo_form_io.asset_form_description = formRequest.asset_form_description;
                            tbl_mongo_form_io.created_Date = addForm.created_at.Value;
                            tbl_mongo_form_io.status = addForm.status;
                            tbl_mongo_form_io.site_id = asset_details.asset_id.ToString();
                            //convert encode string json to json
                            string structure = HttpUtility.UrlDecode(formRequest.asset_form_data);
                            tbl_mongo_form_io.asset_form_data = BsonSerializer.Deserialize<BsonDocument>(structure);

                            _Mongodb.tbl_mongo_form_io.InsertOne(tbl_mongo_form_io);

                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                            formResponse = _mapper.Map<AssetFormIOResponseModel>(addForm);
                            formResponse.response_status = result;
                        }
                        else
                        {
                            formResponse.response_status = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _dbtransaction.Rollback();
                    formResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }
            return formResponse;
        }

        public AddUpdateTempIssueFromWORequestmodel InspectionMakeRequestmodelforWOlineIssue(Guid asset_form_id)
        {
            AddUpdateTempIssueFromWORequestmodel requestmodel = new AddUpdateTempIssueFromWORequestmodel();
            requestmodel.list_temp_issue = new List<TempIssueListRequest>();

            var get_asset_form = _UoW.AssetFormIORepository.GetAssetFormIOByIdForTempIssue(asset_form_id);
            dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject(get_asset_form.asset_form_data);
            int temp_issue_status = (int)Status.open;
            if (get_asset_form.status != (int)Status.open)
            {
                temp_issue_status = (int)Status.InProgress;
            }
            try
            {
                if (dynamicform != null && dynamicform.data != null && dynamicform.data.footer != null)
                {
                    if (dynamicform.data.footer.inspectionVerdict != null)
                    {
                        string inspection_verdict = dynamicform.data.footer.inspectionVerdict;
                        if (!String.IsNullOrEmpty(inspection_verdict))
                        {
                            inspection_verdict = inspection_verdict.Trim().ToLower();
                            if (inspection_verdict == inspectionVerdictname.acceptable || inspection_verdict == inspectionVerdictname_2.acceptable) //Pass
                            {
                                /// check if any replace issue is exist then delete those Issues
                                /// 
                                var exist_replace_issues = get_asset_form.WOLineIssue.Where(x => x.issue_type == (int)WOLine_Temp_Issue_Type.Replace && !x.is_deleted).ToList();
                                if (exist_replace_issues != null && exist_replace_issues.Count > 0)
                                {
                                    exist_replace_issues.ForEach(x =>
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = x.wo_id;
                                        request.wo_line_issue_id = x.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    });
                                }
                                /// check if any repair issue is exist then delete those Issues
                                /// 
                                var exist_repai_issues = get_asset_form.WOLineIssue.Where(x => x.issue_type == (int)WOLine_Temp_Issue_Type.Repair && !x.is_deleted).ToList();
                                if (exist_repai_issues != null && exist_repai_issues.Count > 0)
                                {
                                    exist_repai_issues.ForEach(x =>
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = x.wo_id;
                                        request.wo_line_issue_id = x.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    });
                                }
                            }
                            else if (inspection_verdict == inspectionVerdictname.alert || inspection_verdict == inspectionVerdictname_2.alert) // Repair
                            {
                                // if issue schedule is for today then link issue to WO(I will repair today)
                                string schedule_timing = dynamicform.data.footer.repairSchedule;
                                schedule_timing = schedule_timing.Trim().ToLower();
                                bool is_issue_linked_for_fix = false;
                                if (schedule_timing == "today")
                                {
                                    is_issue_linked_for_fix = true;
                                }
                                bool visual_flag = dynamicform.data.footer.selectRepairIssues.visual;
                                bool mechanical_flag = dynamicform.data.footer.selectRepairIssues.mechanical;
                                bool electrical_flag = dynamicform.data.footer.selectRepairIssues.electrical;

                                #region Visual
                                if (visual_flag) // if its true
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Repair_Visual && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_title = Issue_Title.Repair_Visual;
                                        request.atmw_first_comment = dynamicform.data.footer.visualIssueDescription;
                                        request.issue_description = Issue_Description.visual_issue;
                                        request.is_deleted = false;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;

                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                    else
                                    {
                                        /// insert New Issue
                                        /// 
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.issue_title = Issue_Title.Repair_Visual;
                                        request.atmw_first_comment = dynamicform.data.footer.visualIssueDescription;
                                        request.issue_description = Issue_Description.visual_issue;
                                        request.is_deleted = false;
                                        request.wo_id = get_asset_form.wo_id;
                                        request.temp_issue_status = temp_issue_status;
                                        request.asset_form_id = get_asset_form.asset_form_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_type = (int)WOLine_Temp_Issue_Type.Repair;
                                        request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.Repair_Visual;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                                        {
                                            request.asset_id = get_asset_form.asset_id;
                                        }
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                else // if its false
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Repair_Visual && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = exist.wo_id;
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                #endregion Visual

                                #region Mechanical
                                if (mechanical_flag) // if its true
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Repair_mechanical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = exist.wo_id;
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_title = Issue_Title.Repair_Mechanical;
                                        request.atmw_first_comment = dynamicform.data.footer.mechanicalIssueDescription;
                                        request.issue_description = Issue_Description.mechanical_issue;
                                        request.is_deleted = false;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;

                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                    else
                                    {
                                        /// insert New Issue
                                        /// 
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.issue_title = Issue_Title.Repair_Mechanical;
                                        request.atmw_first_comment = dynamicform.data.footer.mechanicalIssueDescription;
                                        request.issue_description = Issue_Description.mechanical_issue;
                                        request.is_deleted = false;
                                        request.wo_id = get_asset_form.wo_id;
                                        request.temp_issue_status = temp_issue_status;
                                        request.asset_form_id = get_asset_form.asset_form_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_type = (int)WOLine_Temp_Issue_Type.Repair;
                                        request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.Repair_mechanical;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                                        {
                                            request.asset_id = get_asset_form.asset_id;
                                        }
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                else // if its false
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Repair_mechanical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.is_deleted = true;
                                        request.wo_id = exist.wo_id;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                #endregion Mechanical

                                #region Electrical
                                if (electrical_flag) // if its true
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Repair_electrical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = exist.wo_id;
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_title = Issue_Title.Repair_Electrical;
                                        request.atmw_first_comment = dynamicform.data.footer.electricalIssueDescription;
                                        request.issue_description = Issue_Description.electrical_issue;
                                        request.is_deleted = false;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;

                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                    else
                                    {
                                        /// insert New Issue
                                        /// 
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.issue_title = Issue_Title.Repair_Electrical;
                                        request.issue_description = Issue_Description.electrical_issue;
                                        request.atmw_first_comment = dynamicform.data.footer.electricalIssueDescription;
                                        request.is_deleted = false;
                                        request.wo_id = get_asset_form.wo_id;
                                        request.temp_issue_status = temp_issue_status;
                                        request.asset_form_id = get_asset_form.asset_form_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_type = (int)WOLine_Temp_Issue_Type.Repair;
                                        request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.Repair_electrical;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                                        {
                                            request.asset_id = get_asset_form.asset_id;
                                        }
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                else // if its false
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Repair_electrical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = exist.wo_id;
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                #endregion Electrical


                                /// check if any replace issue is exist then delete those Issues
                                /// 
                                var exist_replace_issues = get_asset_form.WOLineIssue.Where(x => x.issue_type == (int)WOLine_Temp_Issue_Type.Replace && !x.is_deleted).ToList();
                                if (exist_replace_issues != null && exist_replace_issues.Count > 0)
                                {
                                    exist_replace_issues.ForEach(x =>
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = x.wo_id;
                                        request.wo_line_issue_id = x.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    });
                                }

                            }
                            else if (inspection_verdict == inspectionVerdictname.danger || inspection_verdict == inspectionVerdictname_2.danger) // Replace
                            {
                                string schedule_timing = dynamicform.data.footer.replacementSchedule;
                                schedule_timing = schedule_timing.Trim().ToLower();
                                bool is_issue_linked_for_fix = false;
                                if (schedule_timing == "today")
                                {
                                    is_issue_linked_for_fix = true;
                                }
                                bool visual_flag = dynamicform.data.footer.selectReplacementIssues.visual;
                                bool mechanical_flag = dynamicform.data.footer.selectReplacementIssues.mechanical;
                                bool electrical_flag = dynamicform.data.footer.selectReplacementIssues.electrical;

                                #region Visual
                                if (visual_flag) // if its true
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Replace_Visual && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.issue_title = Issue_Title.Replace_Visual;
                                        request.atmw_first_comment = dynamicform.data.footer.ReplacevisualIssueDescription;
                                        request.issue_description = Issue_Description.visual_issue;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.is_deleted = false;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                    else
                                    {
                                        /// insert New Issue
                                        /// 
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.issue_title = Issue_Title.Replace_Visual;
                                        request.atmw_first_comment = dynamicform.data.footer.ReplacevisualIssueDescription;
                                        request.issue_description = Issue_Description.visual_issue;
                                        request.is_deleted = false;
                                        request.wo_id = get_asset_form.wo_id;
                                        request.temp_issue_status = temp_issue_status;
                                        request.asset_form_id = get_asset_form.asset_form_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_type = (int)WOLine_Temp_Issue_Type.Replace;
                                        request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.Replace_Visual;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                                        {
                                            request.asset_id = get_asset_form.asset_id;
                                        }
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                else // if its false
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Replace_Visual && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                #endregion Visual

                                #region Mechanical
                                if (mechanical_flag) // if its true
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Replace_mechanical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.issue_title = Issue_Title.Replace_Mechanical;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.atmw_first_comment = dynamicform.data.footer.ReplacemechanicalIssueDescription;
                                        request.issue_description = Issue_Description.mechanical_issue;
                                        request.is_deleted = false;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;

                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                    else
                                    {
                                        /// insert New Issue
                                        /// 
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.issue_title = Issue_Title.Replace_Mechanical;
                                        request.atmw_first_comment = dynamicform.data.footer.ReplacemechanicalIssueDescription;
                                        request.issue_description = Issue_Description.mechanical_issue;
                                        request.is_deleted = false;
                                        request.wo_id = get_asset_form.wo_id;
                                        request.temp_issue_status = temp_issue_status;
                                        request.asset_form_id = get_asset_form.asset_form_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_type = (int)WOLine_Temp_Issue_Type.Replace;
                                        request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.Replace_mechanical;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                                        {
                                            request.asset_id = get_asset_form.asset_id;
                                        }
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                else // if its false
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Replace_mechanical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                #endregion Mechanical

                                #region Electrical
                                if (electrical_flag) // if its true
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Replace_electrical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_title = Issue_Title.Replace_Electrical;
                                        request.atmw_first_comment = dynamicform.data.footer.ReplaceelectricalIssueDescription;
                                        request.issue_description = Issue_Description.electrical_issue;
                                        request.is_deleted = false;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;

                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                    else
                                    {
                                        /// insert New Issue
                                        /// 
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.issue_title = Issue_Title.Replace_Electrical;
                                        request.atmw_first_comment = dynamicform.data.footer.ReplaceelectricalIssueDescription;
                                        request.issue_description = Issue_Description.electrical_issue;
                                        request.is_deleted = false;
                                        request.wo_id = get_asset_form.wo_id;
                                        request.temp_issue_status = temp_issue_status;
                                        request.asset_form_id = get_asset_form.asset_form_id;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.issue_type = (int)WOLine_Temp_Issue_Type.Replace;
                                        request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.Replace_electrical;
                                        request.form_retrived_asset_name = get_asset_form.form_retrived_asset_name;
                                        request.is_issue_linked_for_fix = is_issue_linked_for_fix;
                                        if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                                        {
                                            request.asset_id = get_asset_form.asset_id;
                                        }
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                else // if its false
                                {
                                    // check if issue is already exist 
                                    var exist = get_asset_form.WOLineIssue.Where(x => x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.Replace_electrical && !x.is_deleted).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_line_issue_id = exist.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    }
                                }
                                #endregion Electrical


                                /// check if any repair issue is exist then delete those Issues
                                /// 
                                var exist_repai_issues = get_asset_form.WOLineIssue.Where(x => x.issue_type == (int)WOLine_Temp_Issue_Type.Repair && !x.is_deleted).ToList();
                                if (exist_repai_issues != null && exist_repai_issues.Count > 0)
                                {
                                    exist_repai_issues.ForEach(x =>
                                    {
                                        /// update Issue
                                        TempIssueListRequest request = new TempIssueListRequest();
                                        request.wo_id = x.wo_id;
                                        request.wo_line_issue_id = x.wo_line_issue_id;
                                        request.is_deleted = true;
                                        requestmodel.list_temp_issue.Add(request);
                                    });
                                }
                            }
                        }

                    }

                    // for NEC violation in form
                    if (dynamicform.data.footer.necViolations != null)
                    {
                        var NEC_violation = dynamicform.data.footer.necViolations;
                        var serealize = Newtonsoft.Json.JsonConvert.SerializeObject(NEC_violation);

                        List<AssetformioNECObject> NEC_violatin_obj = JsonSerializer.Deserialize<List<AssetformioNECObject>>(serealize);
                        var requested_issue_descriptin = NEC_violatin_obj.Select(x => x.selectViolation).ToList();

                        var db_issues = _UoW.AssetFormIORepository.GetNECTempIssueByAssetFormid(asset_form_id);
                        var db_issue_description = db_issues.Select(x => x.issue_description).ToList();
                        // new added issue 
                        var new_issue = requested_issue_descriptin.Where(x => !db_issue_description.Contains(x)).ToList();

                        // deleted issue 
                        var deleted_issue = db_issue_description.Where(x => !requested_issue_descriptin.Contains(x)).ToList();

                        foreach (var nec_vilation in new_issue)
                        {
                            //get all nec issues of this WO line
                            TempIssueListRequest request = new TempIssueListRequest();
                            request.issue_title = Issue_Title.NEC_Violation;
                            request.issue_description = nec_vilation;//
                            request.is_deleted = false;
                            request.wo_id = get_asset_form.wo_id;
                            request.temp_issue_status = temp_issue_status;
                            request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.NEC_Violation;
                            request.issue_type = (int)WOLine_Temp_Issue_Type.Compliance;
                            request.woonboardingassets_id = null;
                            request.asset_form_id = get_asset_form.asset_form_id;
                            if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                            {
                                request.asset_id = get_asset_form.WOcategorytoTaskMapping.assigned_asset;
                            }
                            requestmodel.list_temp_issue.Add(request);
                        }
                        foreach (var nec_vilation in deleted_issue)
                        {
                            var exist_issue = db_issues.Where(x => x.issue_description == nec_vilation).FirstOrDefault();

                            TempIssueListRequest request = new TempIssueListRequest();
                            request.wo_id = exist_issue.wo_id;
                            request.wo_line_issue_id = exist_issue.wo_line_issue_id;
                            request.is_deleted = true;

                            requestmodel.list_temp_issue.Add(request);
                        }
                    }
                    // for OSHA violation in form
                    if (dynamicform.data.footer.oshaViolations != null)
                    {
                        var OSHA_violation = dynamicform.data.footer.oshaViolations;
                        var serealize = Newtonsoft.Json.JsonConvert.SerializeObject(OSHA_violation);

                        List<AssetformioOshaObject> OSHA_violatin_obj = JsonSerializer.Deserialize<List<AssetformioOshaObject>>(serealize);
                        var requested_issue_descriptin = OSHA_violatin_obj.Select(x => x.selectViolation).ToList();

                        var db_issues = _UoW.AssetFormIORepository.GetOSHATempIssueByAssetFormid(asset_form_id);
                        var db_issue_description = db_issues.Select(x => x.issue_description).ToList();
                        // new added issue 
                        var new_issue = requested_issue_descriptin.Where(x => !db_issue_description.Contains(x)).ToList();

                        // deleted issue 
                        var deleted_issue = db_issue_description.Where(x => !requested_issue_descriptin.Contains(x)).ToList();

                        foreach (var osha_vilation in new_issue)
                        {
                            //get all nec issues of this WO line
                            TempIssueListRequest request = new TempIssueListRequest();
                            request.issue_title = Issue_Title.Osha_Violation;
                            request.issue_description = osha_vilation;//
                            request.is_deleted = false;
                            request.wo_id = get_asset_form.wo_id;
                            request.temp_issue_status = temp_issue_status;
                            request.issue_caused_id = (int)WOLine_Temp_Issue_Caused.OSha_Violation;
                            request.issue_type = (int)WOLine_Temp_Issue_Type.Compliance;
                            request.woonboardingassets_id = null;
                            request.asset_form_id = get_asset_form.asset_form_id;
                            if (get_asset_form.WOcategorytoTaskMapping.assigned_asset != null && get_asset_form.WOcategorytoTaskMapping.assigned_asset != Guid.Empty)
                            {
                                request.asset_id = get_asset_form.WOcategorytoTaskMapping.assigned_asset;
                            }
                            requestmodel.list_temp_issue.Add(request);
                        }
                        foreach (var nec_vilation in deleted_issue)
                        {
                            var exist_issue = db_issues.Where(x => x.issue_description == nec_vilation).FirstOrDefault();

                            TempIssueListRequest request = new TempIssueListRequest();
                            request.wo_id = exist_issue.wo_id;
                            request.wo_line_issue_id = exist_issue.wo_line_issue_id;
                            request.is_deleted = true;

                            requestmodel.list_temp_issue.Add(request);
                        }
                    }

                }

            }
            catch (Exception ex)
            {

            }

            return requestmodel;
        }


        public ListViewModel<AssetFormIOResponseModel> GetAllAssetTemplateList(GetAllAssetInspectionListByAssetIdRequestModel request)
        {
            ListViewModel<AssetFormIOResponseModel> templateModel = new ListViewModel<AssetFormIOResponseModel>();
            try
            {
                //var templateRequests = _UoW.AssetFormIORepository.GetAllAssetTemplateList(request);
                var templateRequests = _UoW.AssetFormIORepository.GetAllAssetTemplateListNew(request);

                var list_form_ids = templateRequests.Item1.Select(x => x.form_id.Value).Distinct().ToList();

                var workOrderIds = templateRequests.Item1.Select(c => c.wo_id.Value).Distinct().ToList();
                var forms = _UoW.formIORepository.GetFormsExcludedByIds(list_form_ids);
                var workOrders = _UoW.formIORepository.GetWorkOrdersByIds(workOrderIds);

                if (templateRequests.Item1?.Count > 0)
                {
                    templateModel.list = _mapper.Map<List<AssetFormIOResponseModel>>(templateRequests.Item1);

                    //templateModel.list = Mapping(templateRequests.Item1);

                    foreach (var temp in templateModel.list)
                    {
                        try
                        {
                            var form = forms.Where(x => x.form_id == temp.form_id).FirstOrDefault();
                            if (form != null)
                            {
                                //  var inspection_form_data = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(temp.asset_form_data);
                                //  var master_form_ = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(form.form_data);
                                //  master_form_.data = inspection_form_data.data;
                                //  string str = JsonSerializer.Serialize(master_form_);
                                //  temp.asset_form_data = str;
                                temp.work_procedure = form.work_procedure;
                                temp.form_category_name = form.form_type;
                                temp.asset_form_description = form.form_description;
                                temp.asset_form_name = form.form_name;
                            }

                            // Removed WorkOrder Object from Response model 17-05-23
                            var workOrderResult = workOrders.Where(x => x.wo_id == temp.wo_Id).FirstOrDefault();
                            if (workOrderResult != null)
                            {
                                temp.workOrderStatus = workOrderResult.status;
                                temp.wo_type = workOrderResult.wo_type;

                                if (workOrderResult.wo_type == (int)Status.Acceptance_Test_WO)
                                {
                                    temp.wo_number = "AT" + temp.wo_number;
                                }
                                else if (workOrderResult.wo_type == (int)Status.Maintenance_WO)
                                {
                                    temp.wo_number = "WO" + temp.wo_number;
                                }
                            }

                            //Update code taken into try catch block 17-05-23
                            if (temp.requested_by != null && temp.requested_by != "")
                            {
                                var userDetails = _UoW.UserRepository.GetUserByID(temp.requested_by).Result;
                                if (userDetails != null)
                                {
                                    temp.requested_by = userDetails.firstname + " " + userDetails.lastname;
                                }
                            }

                            //Update code taken into try catch block 17-05-23
                            if (temp.accepted_by != null && temp.accepted_by != "")
                            {
                                var AcceptedbyuserDetails = _UoW.UserRepository.GetUserByID(temp.accepted_by).Result;
                                if (AcceptedbyuserDetails != null)
                                {
                                    temp.accepted_by = AcceptedbyuserDetails.firstname + " " + AcceptedbyuserDetails.lastname;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    templateModel.listsize = templateRequests.Item2;
                    templateModel.pageIndex = request.pageindex;
                    templateModel.pageSize = request.pagesize;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return templateModel;
        }

        public async Task<AssetFormIOReportStatusResponsemodel> GetReportStatus(string asset_form_id)
        {
            AssetFormIOReportStatusResponsemodel response = new AssetFormIOReportStatusResponsemodel();
            try
            {
                AssetFormIO reports = _UoW.AssetFormIORepository.GetAssetFormIOBytId(Guid.Parse(asset_form_id));
                if (reports != null)
                {

                    if (reports.pdf_report_status == (int)Status.ReportInProgress && reports.export_pdf_at.Value.AddMinutes(2) < DateTime.UtcNow)
                    {
                        reports.pdf_report_status = (int)Status.ReportFailed;
                        reports.modified_at = DateTime.UtcNow;
                    }
                    reports.modified_at = DateTime.UtcNow;
                    bool report_added = await _UoW.BaseGenericRepository<AssetFormIO>().Update(reports);
                    if (report_added)
                    {
                        _logger.LogError("Generation Failed Report Timeout");
                    }
                    else
                    {
                        _logger.LogError("Error In Update Generation Failed Report Timeout");
                    }
                    reports = _UoW.AssetFormIORepository.GetAssetFormIOBytId(Guid.Parse(asset_form_id));
                    response.asset_form_id = reports.asset_form_id;
                    response.pdf_report_url = reports.pdf_report_url;
                    response.pdf_report_status = reports.pdf_report_status.Value;
                    response.report_lambda_logs = reports.report_lambda_logs;
                }
                return response;
            }
            catch { throw; }
        }
        public async Task<int> UpdateOnlyAssetFormIO(UpdateOnlyAssetFormIORequestmodel request)
        {
            /*   var get_asset_formios = _UoW.AssetFormIORepository.Getallassetformios();
               //   var assetform = get_asset_formios.FirstOrDefault();

               foreach (var assetform in get_asset_formios)
               {
                   try
                   {

                       dynamic dynamic_data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(assetform.asset_form_data, new Newtonsoft.Json.Converters.ExpandoObjectConverter());

                       string wo_number = dynamic_data.data.header.workOrder;
                       wo_number = wo_number.Replace("AT", "");
                       wo_number = wo_number.Replace("WO", "");
                       dynamic_data.data.header.workOrder = wo_number;

                       object obj = dynamic_data;
                       string str = Newtonsoft.Json.JsonConvert.SerializeObject(dynamic_data);
                       assetform.asset_form_data = str;

                       if (!String.IsNullOrEmpty(assetform.form_retrived_data))
                       {
                           try
                           {
                               dynamic dynamic_data1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(assetform.form_retrived_data, new Newtonsoft.Json.Converters.ExpandoObjectConverter());
                               dynamic_data1.data.header.workOrder = wo_number;
                               string str1 = Newtonsoft.Json.JsonConvert.SerializeObject(dynamic_data1);
                               assetform.form_retrived_data = str1;
                           }
                           catch (Exception ex)
                           {

                           }
                       }
                       assetform.modified_at = DateTime.UtcNow;
                       var updtae = await _UoW.BaseGenericRepository<AssetFormIO>().Update(assetform);
                   }
                   catch(Exception ex)
                   {

                   }
               } */



            var get_form = _UoW.AssetFormIORepository.GetAssetFormIOBytId(request.asset_form_id);
            get_form.asset_form_data = request.asset_form_data;
            get_form.modified_at = DateTime.UtcNow;
            var updaate = await _UoW.BaseGenericRepository<AssetFormIO>().Update(get_form);
            if (updaate)
            {
                return (int)ResponseStatusNumber.Success;
            }
            return (int)ResponseStatusNumber.Error;
        }

        public async Task<AddUpdateAssetFormIOOfflineResponsemodel> AddUpdateAssetFormIOOffline(AddUpdateAssetFormIOOfflineRequestModel offlineformRequest, string offline_sync_bucket, string S3_aws_access_key, string S3_aws_secret_key)
        {
            AddUpdateAssetFormIOOfflineResponsemodel response = new AddUpdateAssetFormIOOfflineResponsemodel();
            response.uploaded_asset_form_id = new List<Guid>();
            // int formResponse = (int)ResponseStatusNumber.Error;
            int result = (int)ResponseStatusNumber.Error;


            // get device info by device uuid
            var get_device_info = _UoW.WorkOrderRepository.GetdeviceInfoById(UpdatedGenericRequestmodel.CurrentUser.device_uuid);

            // add record in track table
            TrackMobileSyncOffline TrackMobileSyncOffline = new TrackMobileSyncOffline();
            TrackMobileSyncOffline.device_uuid = get_device_info.device_uuid;
            TrackMobileSyncOffline.device_code = get_device_info.device_code;
            TrackMobileSyncOffline.sync_time = DateTime.UtcNow;
            TrackMobileSyncOffline.user_id = UpdatedGenericRequestmodel.CurrentUser.requested_by;
            TrackMobileSyncOffline.status = 2;

            var insert_track = await _UoW.BaseGenericRepository<TrackMobileSyncOffline>().Insert(TrackMobileSyncOffline);
            _UoW.SaveChanges();

            TrackMobileSyncOffline.s3_file_name = "Neta - " + TrackMobileSyncOffline.trackmobilesyncoffline_id + "-" + get_device_info.device_code + ".txt";
            var update_track = await _UoW.BaseGenericRepository<TrackMobileSyncOffline>().Update(TrackMobileSyncOffline);

            //requestmodel.trackmobilesyncoffline_id = TrackMobileSyncOffline.trackmobilesyncoffline_id;

            // upload file to S3 bucket
            PrepareModelForOfflineLambda PrepareModelForOfflineLambda = new PrepareModelForOfflineLambda();
            PrepareModelForOfflineLambda.data = offlineformRequest;
            PrepareModelForOfflineLambda.requested_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(PrepareModelForOfflineLambda);

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
            using (var stream = new System.IO.MemoryStream(byteArray))
            {
                var upload_file = await s3BucketService.UploadJsonfileAsStrem(stream, S3_aws_access_key, S3_aws_secret_key, offline_sync_bucket, TrackMobileSyncOffline.s3_file_name);
            }


            foreach (var formRequest in offlineformRequest.submitted_form_list)
            {
                _UoW.BeginTransaction();
                try
                {
                    var formDetails = await _UoW.AssetFormIORepository.GetAssetFormIOById(formRequest.asset_form_id);
                    if (formDetails != null)
                    {
                        formDetails.asset_form_name = formRequest.asset_form_name;
                        formDetails.asset_form_type = formRequest.asset_form_type;

                        formDetails.status = formRequest.status;
                        formDetails.asset_form_description = formRequest.asset_form_description;
                        formDetails.modified_at = DateTime.UtcNow;
                        formDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        formDetails.requested_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                        formDetails.inspected_at = DateTime.UtcNow;
                        formDetails.WOcategorytoTaskMapping.updated_at = DateTime.UtcNow;
                        try
                        {
                            /// append form data to master form
                            //   var master_form = await _UoW.formIORepository.GetFormIOById(formDetails.form_id.Value);
                            //    var dynamic_master_form__data_obj = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(master_form.form_data);
                            //    dynamic_master_form__data_obj.data = JsonSerializer.Deserialize<object>(formRequest.asset_form_data); /// from mobile in offline we will only receive filled data
                            // formRequest.asset_form_data = JsonSerializer.Serialize(dynamic_master_form__data_obj);

                            // formRequest.asset_form_data = "{\"data\": " + formRequest.asset_form_data + "}";
                            // formDetails.asset_form_data = formRequest.asset_form_data;

                            try
                            {
                                TimeZoneInfo estTimeZoneInfo = TZConvert.GetTimeZoneInfo(formDetails.Sites.timezone);
                                //LogHelper.Log<TimeZonerror>(LogLevel.Information, " time zone info " + estTimeZoneInfo);
                                dynamic dynamicform1 = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                                string initial_date = dynamicform1.data.header.date;
                                if (String.IsNullOrEmpty(initial_date))  // if initial date is null or empty then only assign date to it
                                {

                                    formDetails.intial_form_filled_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, estTimeZoneInfo);
                                    dynamicform1.data.header.date = formDetails.intial_form_filled_date;
                                    formDetails.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicform1);
                                    formRequest.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicform1);
                                }
                                else
                                {
                                    formDetails.intial_form_filled_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(initial_date), estTimeZoneInfo);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation("date time convert issue" + ex.Message);
                                formDetails.intial_form_filled_date = DateTime.UtcNow;
                            }

                            dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                            var identification = dynamicform.data.header.identification;
                            var location = dynamicform.data.header.parent;
                            var asset_id = dynamicform.data.header.assetId;
                            formDetails.form_retrived_asset_name = identification;
                            formDetails.form_retrived_location = location;
                            formDetails.form_retrived_asset_id = asset_id;
                            formDetails.form_retrived_workOrderType = dynamicform.data.header.workOrderType;
                            formDetails.form_retrived_nameplate_info = dynamicform.data.nameplateString;
                            string building = dynamicform.data.header.building;
                            string floor = dynamicform.data.header.floor;
                            string room = dynamicform.data.header.room;
                            string section = dynamicform.data.header.section;

                            //update in requested form
                            formDetails.building = building;
                            formDetails.floor = floor;
                            formDetails.room = room;
                            formDetails.section = section;
                            object _data = dynamicform;
                            formDetails.form_retrived_data = Newtonsoft.Json.JsonConvert.SerializeObject(_data);
                            formDetails.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(_data);
                            try
                            {
                                if (dynamicform != null && dynamicform.data != null && dynamicform.data.footer != null && dynamicform.data.footer.inspectionVerdict != null)
                                {
                                    string inspection_verdict = dynamicform.data.footer.inspectionVerdict;
                                    if (!String.IsNullOrEmpty(inspection_verdict))
                                    {
                                        inspection_verdict = inspection_verdict.Trim().ToLower();
                                        if (inspection_verdict == inspectionVerdictname.acceptable || inspection_verdict == inspectionVerdictname_2.acceptable)
                                        {
                                            formDetails.inspection_verdict = (int)inspectionVerdictnumber.acceptable;
                                        }
                                        else if (inspection_verdict == inspectionVerdictname.alert || inspection_verdict == inspectionVerdictname_2.alert)
                                        {
                                            formDetails.inspection_verdict = (int)inspectionVerdictnumber.alert;
                                        }
                                        else if (inspection_verdict == inspectionVerdictname.danger || inspection_verdict == inspectionVerdictname_2.danger)
                                        {
                                            formDetails.inspection_verdict = (int)inspectionVerdictnumber.danger;
                                        }
                                        else if (inspection_verdict == inspectionVerdictname.defective || inspection_verdict == inspectionVerdictname_2.defective)
                                        {
                                            formDetails.inspection_verdict = (int)inspectionVerdictnumber.defective;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            /* var form_obj = JsonSerializer.Deserialize<FormIOObject.Root>(formRequest.asset_form_data);
                             var dynamic_form_obj = JsonSerializer.Deserialize<FormioDynamicobj.Root>(formRequest.asset_form_data);
                             var dynamic__data_form_obj = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(formRequest.asset_form_data);
                             if (dynamic_form_obj != null && dynamic_form_obj.data != null)
                             {
                                 var nameplate = dynamic_form_obj.data.nameplateInformation;
                                 string nameplateString = JsonSerializer.Serialize(nameplate);
                                 var identification = form_obj.data.header.identification;
                                 var location = form_obj.data.header.parent;
                                 var asset_id = form_obj.data.header.assetId;
                                 formDetails.form_retrived_asset_name = identification;
                                 formDetails.form_retrived_location = location;
                                 formDetails.form_retrived_asset_id = asset_id;
                                 formDetails.form_retrived_nameplate_info = nameplateString;
                                 formDetails.form_retrived_data = JsonSerializer.Serialize(dynamic__data_form_obj);
                             }*/
                            // dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(formRequest.asset_form_data);
                            //  stuff.data.header.date = "2022-07-15";
                            //  formDetails.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(stuff);
                        }
                        catch (Exception ex)
                        {

                        }
                        result = _UoW.AssetFormIORepository.Update(formDetails).Result;
                        if (result > 0)
                        {
                            _UoW.SaveChanges();
                            _UoW.CommitTransaction();

                            WorkOrderService WorkOrderService = new WorkOrderService(_mapper);
                            await WorkOrderService.updateWOCategoryStatusforStatusmanage(formDetails.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id);

                            try
                            {
                                //// add temp issues based on inspection verdict 
                                ///
                                if (formDetails.WorkOrders.wo_type == (int)Status.Maintenance_WO)
                                {
                                    AddUpdateTempIssueFromWORequestmodel tempIssueRequest = InspectionMakeRequestmodelforWOlineIssue(formDetails.asset_form_id);
                                    if (tempIssueRequest.list_temp_issue != null && tempIssueRequest.list_temp_issue.Count > 0)
                                    {
                                        WorkOrderService woservice = new WorkOrderService(_mapper);
                                        await woservice.AddUpdateTempIssueFromWO(tempIssueRequest);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            // formResponse = result;
                            response.uploaded_asset_form_id.Add(formDetails.asset_form_id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _UoW.RollbackTransaction();
                    response.success = (int)ResponseStatusNumber.Error;
                }
            }
            if (response.uploaded_asset_form_id.Count == offlineformRequest.submitted_form_list.Count)
            {
                response.success = (int)ResponseStatusNumber.Success;
            }

            return response;
        }

        public async Task<int> ExractandStoreOnlydatafromOldForm()
        {
            await sendemail();
            AssetFormIOReportStatusResponsemodel response = new AssetFormIOReportStatusResponsemodel();
            try
            {/*
                int reportscount = _UoW.AssetFormIORepository.ExractandStoreOnlydatafromOldFormcount();
                var page_size = 100;
                var page_index = 1;
                int total_skips = reportscount / page_size;
                int skip = 0;
                int take = 0;
               // for(int i = 1; i<= total_skips ; i++)
               // {
                    take = page_size;
                    List<AssetFormIO> reports = _UoW.AssetFormIORepository.ExractandStoreOnlydatafromOldForm(0,5);

                    if (reports != null)
                    {
                        foreach (var x in reports)
                        {
                            try
                            {
                                var dynamic_master_form__data_obj = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(x.asset_form_data);
                                var str = JsonSerializer.Serialize(dynamic_master_form__data_obj.data);
                                var test = "{\"data\": " + str + "}";
                                x.asset_form_data = test;
                                x.form_retrived_data = test;

                                var update = await _UoW.BaseGenericRepository<AssetFormIO>().Update(x);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    skip = 100 + skip;
               // }
                */
                return 1;
            }
            catch { throw; }
        }
        public async Task<int> sendemail()
        {
            var templateID = ConfigurationManager.AppSettings["User_Signup_mail"];
            usersignupemail usersignupemail = new usersignupemail();
            usersignupemail.username = "ketan.ravat@sculptsoft.com";
            usersignupemail.temp_code = "02154858";
            usersignupemail.env_domain = "https://democompany.dev.egalvanic.com";

            var response = await SendEmail.SendGridEmailWithTemplate("ketan.ravat@sculptsoft.com", "test", usersignupemail, templateID);
            // await SendEmail.SendGridEmail("ketan.ravat@sculptsoft.com", "test", "test");

            return 1;
        }
        public class usersignupemail
        {
            public string username { get; set; }

            public string temp_code { get; set; }
            public string env_domain { get; set; }
        }
        public async Task<int> UpdateAssetInfo(UpdateAssetInfoRequestmodel request)
        {

            Asset get_asset = _UoW.AssetFormIORepository.GetAssetForUpdate(request.asset_id);
            get_asset.form_retrived_nameplate_info = request.form_retrived_nameplate_info;
            get_asset.criticality_index = request.criticality_index;
            get_asset.condition_index = request.condition_index;
            get_asset.criticality_index = request.criticality_index;
            get_asset.condition_index = request.condition_index;
            get_asset.modified_at = DateTime.UtcNow;
            if (request.formio_id != null)
            {
                get_asset.form_id = request.formio_id.Value;
            }
            var update = await _UoW.BaseGenericRepository<Asset>().Update(get_asset);

            // NamePlate Images
            if (request.asset_namespate_images != null && request.asset_namespate_images.Count > 0)
            {
                var new_image_list = request.asset_namespate_images.Where(x => x.asset_profile_images_id == null).ToList();

                var requested_deleting_images = request.asset_namespate_images.Where(x => x.asset_profile_images_id != null && x.is_deleted == true).ToList();
                if (new_image_list.Count > 0 && new_image_list != null)
                {
                    foreach (var item in new_image_list)
                    {
                        AssetProfileImages assetProfileImages = new AssetProfileImages();
                        assetProfileImages.asset_photo_type = item.asset_photo_type;
                        assetProfileImages.asset_id = request.asset_id;
                        assetProfileImages.asset_photo = item.asset_photo;
                        assetProfileImages.asset_thumbnail_photo = item.asset_thumbnail_photo;
                        assetProfileImages.created_at = DateTime.UtcNow;
                        assetProfileImages.created_by = UpdatedGenericRequestmodel.CurrentUser.request_id;
                        assetProfileImages.is_deleted = false;

                        var insertimage = await _UoW.BaseGenericRepository<AssetProfileImages>().Insert(assetProfileImages);
                        _UoW.SaveChanges();
                    }
                }
                if (requested_deleting_images.Count > 0 && requested_deleting_images != null)
                {
                    var requested_deleting_images_ids = requested_deleting_images.Select(x => x.asset_profile_images_id).ToList();
                    foreach (var id in requested_deleting_images_ids)
                    {
                        var get_asset_image = _UoW.AssetRepository.GetAssetImagebyID(id.Value);

                        get_asset_image.is_deleted = true;
                        get_asset_image.modified_by = UpdatedGenericRequestmodel.CurrentUser.request_id;
                        get_asset_image.modified_at = DateTime.UtcNow;

                        var deleteimage = await _UoW.BaseGenericRepository<AssetProfileImages>().Update(get_asset_image);
                        _UoW.SaveChanges();
                    }
                }
            }

            if (update)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public ListViewModel<FormIOInsulationResistanceTestResponseModel> GetFormIOInsulationResistanceTest(FormIOInsulationResistanceTestRequestModel request)
        {
            ListViewModel<FormIOInsulationResistanceTestResponseModel> response = new ListViewModel<FormIOInsulationResistanceTestResponseModel>();
            var list = _UoW.AssetFormIORepository.GetFormIOInsulationResistanceTest(request);

            if (list.Item1 != null && list.Item1.Count > 0)
            {
                response.list = _mapper.Map<List<FormIOInsulationResistanceTestResponseModel>>(list.Item1);
                response.listsize = list.Item2;
                response.pageIndex = request.page_index.Value;
                response.pageSize = request.page_size.Value;
            }
            return response;
        }

        public async Task<int> changeassetformiostatus(changeassetformiostatusRequestmodel request)
        {
            int response = (int)ResponseStatusNumber.Error;
            var get_asset_formio = _UoW.AssetFormIORepository.GetAssetFormIOByIdForStatusShange(request.asset_form_id);
            if (get_asset_formio.WorkOrders.status == (int)Status.Completed)
            {
                return (int)ResponseStatusNumber.WO_completed;
            }
            if (get_asset_formio.status == (int)Status.Completed || get_asset_formio.status == (int)Status.Submitted || get_asset_formio.status == (int)Status.Ready_for_review)
            {
                get_asset_formio.status = request.status;// (int)Status.Ready_for_review;
                get_asset_formio.modified_at = DateTime.UtcNow;
                var update = await _UoW.BaseGenericRepository<AssetFormIO>().Update(get_asset_formio);

                var get_all_categorytask = _UoW.WorkOrderRepository.GetWOcategoryTaskByCategoryID(get_asset_formio.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id.ToString());
                var get_category = _UoW.WorkOrderRepository.GetWOcategoryID(get_asset_formio.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id);

                List<int> asset_formio_status = get_all_categorytask.Select(x => x.AssetFormIO.status).ToList();
                List<int> get_all_status = new List<int>();
                
                if(!asset_formio_status.All(x=>x == (int)Status.Submitted)) // if all sttaus is submitted then do not consider it as completed as we need to keep category status as submitted if all asset formio status is submitted.
                {
                    foreach (var item in asset_formio_status)
                    {
                        int status = item;
                        if (status == (int)Status.Submitted)
                        {
                            status = (int)Status.Completed;
                        }
                        get_all_status.Add(status);
                    }
                }
                
                if (get_all_status.All(q => q == (int)Status.Completed))
                {
                    get_category.status_id = (int)Status.Completed;
                    // get_category.WorkOrders.status = (int)Status.Submitted;
                    get_category.updated_at = DateTime.UtcNow;
                    var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                }
                else if (get_all_status.All(q => q == (int)Status.Submitted))
                {
                    get_category.status_id = (int)Status.Submitted;
                    // get_category.WorkOrders.status = (int)Status.Submitted;
                    get_category.updated_at = DateTime.UtcNow;
                    var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                }
                else if (get_all_status.Any(q => q== (int)Status.Ready_for_review))
                {
                    get_category.status_id = (int)Status.Ready_for_review;
                    get_category.updated_at = DateTime.UtcNow;
                    var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                }
                else
                {
                    get_category.status_id = (int)Status.open;
                    get_category.updated_at = DateTime.UtcNow;
                    var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                }
                WorkOrderService WorkOrderService = new WorkOrderService(_mapper);
                await WorkOrderService.updateWOStatusforStatusmanage(get_asset_formio.wo_id.Value);

                /* var get_all_category_of_WO = _UoW.WorkOrderRepository.GetAllCatagoryOFWO(get_asset_formio.wo_id.Value);
                 var wo = _UoW.WorkOrderRepository.GetWOByidforDelete(get_asset_formio.wo_id.Value);

                 if (get_all_category_of_WO.All(q => q.status_id == (int)Status.Submitted || q.status_id == (int)Status.Ready_for_review || q.status_id == (int)Status.Completed))
                 {
                     wo.status = (int)Status.Ready_for_review;
                     wo.modified_at = DateTime.UtcNow;
                     var res1ult = await _UoW.BaseGenericRepository<db.Models.WorkOrders>().Update(wo);
                 }
                 else
                 {
                     wo.status = (int)Status.ReleasedOpenWO;
                     wo.modified_at = DateTime.UtcNow;
                     var res1ult = await _UoW.BaseGenericRepository<db.Models.WorkOrders>().Update(wo);
                 }*/
                response = (int)ResponseStatusNumber.Success;
            }
            else
            {
                response = (int)ResponseStatusNumber.form_is_not_completed;
            }
            return response;
        }
        public async Task<int> ChangeAssetFormIOStatusFormultiple(ChangeAssetFormIOStatusFormultipleRequestmodel request)
        {
            int response = (int)ResponseStatusNumber.Error;
            var is_wo_completed = _UoW.AssetFormIORepository.IsWOCompleted(request.asset_form_id);
            if (is_wo_completed)
            {
                return (int)ResponseStatusNumber.WO_completed;
            }
            var is_form_completed = _UoW.AssetFormIORepository.is_form_completed(request.asset_form_id);
            if (!is_form_completed)
            {
                return (int)ResponseStatusNumber.form_is_not_completed;
            }

            var get_asset_formio_list = _UoW.AssetFormIORepository.GetAssetFormIOByIdForStatusShangeFormultiple(request.asset_form_id);
            foreach (var get_asset_formio in get_asset_formio_list)
            {
                get_asset_formio.status = request.status;// (int)Status.Ready_for_review;
                get_asset_formio.modified_at = DateTime.UtcNow;
                var update = await _UoW.BaseGenericRepository<AssetFormIO>().Update(get_asset_formio);

                /*  var get_all_categorytask = _UoW.WorkOrderRepository.GetWOcategoryTaskByCategoryID(get_asset_formio.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id.ToString());
                  var get_category = _UoW.WorkOrderRepository.GetWOcategoryID(get_asset_formio.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id);
                  if (get_all_categorytask.All(q => q.AssetFormIO.status == (int)Status.Submitted))
                  {
                      get_category.status_id = (int)Status.Submitted;
                      // get_category.WorkOrders.status = (int)Status.Submitted;
                      get_category.updated_at = DateTime.UtcNow;
                      var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                  }
                  else if (get_all_categorytask.All(q => q.AssetFormIO.status == (int)Status.Ready_for_review))
                  {
                      get_category.status_id = (int)Status.Ready_for_review;
                      get_category.updated_at = DateTime.UtcNow;
                      var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                  }
                  else
                  {
                      get_category.status_id = (int)Status.open;
                      get_category.updated_at = DateTime.UtcNow;
                      var res1ult = await _UoW.BaseGenericRepository<db.Models.WOInspectionsTemplateFormIOAssignment>().Update(get_category);
                  }*/
                response = (int)ResponseStatusNumber.Success;
            }

            return response;
        }
        public List<GetAssetsForSubmittedFilterOptionsResponsemodel> GetAssetsForSubmittedFilterOptions()
        {
            List<GetAssetsForSubmittedFilterOptionsResponsemodel> response = new List<GetAssetsForSubmittedFilterOptionsResponsemodel>();
            GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel = new GetAssetsForSubmittedFilterOptionsByStatusRequestModel();
            requestmodel.status = null;  requestmodel.wo_type = null;

            var get_asset_forms = _UoW.AssetFormIORepository.GetAssetsForSubmittedFilterOptions(requestmodel);
            get_asset_forms.ForEach(x =>
            {

                GetAssetsForSubmittedFilterOptionsResponsemodel GetAssetsForSubmittedFilterOptionsResponsemodel = new GetAssetsForSubmittedFilterOptionsResponsemodel();
                GetAssetsForSubmittedFilterOptionsResponsemodel.asset_name = x;
                response.Add(GetAssetsForSubmittedFilterOptionsResponsemodel);
            });
            return response;
        }
        public List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel> GetWorkOrdersForSubmittedFilterOptions()
        {
            List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel> response = new List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel>();
            GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel = new GetAssetsForSubmittedFilterOptionsByStatusRequestModel();
            requestmodel.status = null;  requestmodel.wo_type = null;

            var get_WO_forms = _UoW.AssetFormIORepository.GetWorkOrdersForSubmittedFilterOptions(requestmodel);
            get_WO_forms = get_WO_forms.GroupBy(p => p.wo_id)
                                       .Select(grp => grp.First())
                                       .ToList();
            get_WO_forms.ForEach(x =>
            {

                GetWorkOrdersForSubmittedFilterOptionsResponsemodel GetWorkOrdersForSubmittedFilterOptionsResponsemodel = new GetWorkOrdersForSubmittedFilterOptionsResponsemodel();
                GetWorkOrdersForSubmittedFilterOptionsResponsemodel.value = x.wo_id;
                GetWorkOrdersForSubmittedFilterOptionsResponsemodel.label = x.manual_wo_number;
                /* if (x.wo_type == (int)Status.Acceptance_Test_WO)
                 {
                     GetWorkOrdersForSubmittedFilterOptionsResponsemodel.label = "AT" + x.manual_wo_number;
                 }
                 else if (x.wo_type == (int)Status.Maintenance_WO)
                 {
                     GetWorkOrdersForSubmittedFilterOptionsResponsemodel.label = "WO" + x.manual_wo_number;
                 }*/
                response.Add(GetWorkOrdersForSubmittedFilterOptionsResponsemodel);
            });
            return response;
        }
        public List<GetInspectedForSubmittedFilterOptionsResponsemodel> GetInspectedForSubmittedFilterOptions()
        {
            List<GetInspectedForSubmittedFilterOptionsResponsemodel> response = new List<GetInspectedForSubmittedFilterOptionsResponsemodel>();

            var get_users = _UoW.AssetFormIORepository.GetInspectedForSubmittedFilterOptions().Distinct().ToList();
            get_users.ForEach(x =>
            {
                var userDetails = _UoW.UserRepository.GetUserByID(x.ToString()).Result;
                if (userDetails != null)
                {
                    GetInspectedForSubmittedFilterOptionsResponsemodel GeInspectedForSubmittedFilterOptionsResponsemodel = new GetInspectedForSubmittedFilterOptionsResponsemodel();
                    GeInspectedForSubmittedFilterOptionsResponsemodel.value = x;
                    GeInspectedForSubmittedFilterOptionsResponsemodel.label = userDetails.firstname + " " + userDetails.lastname;
                    response.Add(GeInspectedForSubmittedFilterOptionsResponsemodel);
                }
            });

            return response;
        }
        public List<GeApprovedForSubmittedFilterOptionsResponsemodel> GetApprovedForSubmittedFilterOptions()
        {
            List<GeApprovedForSubmittedFilterOptionsResponsemodel> response = new List<GeApprovedForSubmittedFilterOptionsResponsemodel>();

            var get_users = _UoW.AssetFormIORepository.GetApprovedForSubmittedFilterOptions().Distinct().ToList();
            get_users.ForEach(x =>
            {
                var userDetails = _UoW.UserRepository.GetUserByID(x).Result;
                if (userDetails != null)
                {
                    GeApprovedForSubmittedFilterOptionsResponsemodel GeInspectedForSubmittedFilterOptionsResponsemodel = new GeApprovedForSubmittedFilterOptionsResponsemodel();
                    GeInspectedForSubmittedFilterOptionsResponsemodel.value = x;
                    GeInspectedForSubmittedFilterOptionsResponsemodel.label = userDetails.firstname + " " + userDetails.lastname;
                    response.Add(GeInspectedForSubmittedFilterOptionsResponsemodel);
                }
            });

            return response;
        }
        public GetAssetFormJsonbyIdResponsemodel GetAssetFormJsonbyId(GetAssetFormJsonbyIdRequestmodel request)
        {
            GetAssetFormJsonbyIdResponsemodel response = new GetAssetFormJsonbyIdResponsemodel();
            var get_form = _UoW.formIORepository.GetFormIOById(request.form_id).Result;
            string asset_form_data = get_form.form_data;
            if (request.asset_form_id != null)
            {
                var get_asset_form = _UoW.AssetFormIORepository.GetAssetFormIOByIdMobile(request.asset_form_id.Value);

                //isCalibrationDateEnabled flag
                response.isCalibrationDateEnabled = get_asset_form.Sites.Company.isCalibrationDateEnabled;

                // append form with asset form
                var inspection_form_data = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(get_asset_form.asset_form_data);
                //Newtonsoft.Json.JsonConvert.DeserializeObject(temp.asset_form_data);
                var master_form_ = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(asset_form_data);
                //Newtonsoft.Json.JsonConvert.DeserializeObject(form.form_data);
                //master_form_.data = new object();
                master_form_.data = inspection_form_data.data;
                string str = JsonSerializer.Serialize(master_form_);
                asset_form_data = str;
                response.asset_form_id = get_asset_form.asset_form_id;
            }

            response.asset_form_data = asset_form_data;
            response.form_id = get_form.form_id;

            return response;
        }
        public async Task<int> updateformjson()
        {
            var get_forms = _UoW.AssetFormIORepository.GetFormtoUpdate();
            foreach (var form in get_forms)
            {
                if (form.asset_form_id != Guid.Parse("59cca272-412d-4852-8d74-7bb5dc181e22") && form.asset_form_id != Guid.Parse("5495bc63-88f1-4be2-ba97-516a777bec93"))
                {
                    dynamic copy_from_form_data = Newtonsoft.Json.JsonConvert.DeserializeObject(form.asset_form_data);
                    copy_from_form_data.data.pleaseSelectTests = copy_from_form_data.data.nameplateInformation.pleaseSelectTests;
                    string str = Newtonsoft.Json.JsonConvert.SerializeObject(copy_from_form_data);
                    form.asset_form_data = str;
                    form.form_retrived_data = str;
                    form.modified_at = DateTime.UtcNow;
                    var update = await _UoW.BaseGenericRepository<AssetFormIO>().Update(form);
                }
            }
            return 1;
        }


        public async Task<int> ReplaceAssetformIOJson(Guid siteId, Guid companyId)
        {

            ListViewModel<AssetsResponseModel> responseModel = new ListViewModel<AssetsResponseModel>();

            var get_assetformio = _UoW.formIORepository.ReplaceAssetformIOJsonAll(siteId);

            foreach (var form in get_assetformio)
            {
                //  TimeZoneInfo estTimeZoneInfo = TZConvert.GetTimeZoneInfo(form.Sites.timezone);
                //LogHelper.Log<TimeZonerror>(LogLevel.Information, " time zone info " + estTimeZoneInfo);
                ///  var intial_form_filled_date = TimeZoneInfo.ConvertTimeFromUtc(form.intial_form_filled_date, estTimeZoneInfo);
                if (form.asset_form_data != null)
                {
                    try
                    {
                        dynamic dynamicform = Newtonsoft.Json.JsonConvert.DeserializeObject(form.asset_form_data);
                       // string _form_date = dynamicform.data.header.date;
                       // if (form.intial_form_filled_date != null && String.IsNullOrEmpty(_form_date))
                       // {
                            dynamicform.data.header.workOrder = "15718";
                            form.asset_form_data = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicform);
                            form.form_retrived_data = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicform);
                            var update = await _UoW.BaseGenericRepository<AssetFormIO>().Update(form);
                        //}
                    }
                    catch (Exception ex)
                    {

                    }
                }

                /*  var get_assetformio = _UoW.formIORepository.ReplaceAssetformIOJson(requestmodel.asset_form_io);
                  requestmodel.asset_form_json = "{\"data\":" + requestmodel.asset_form_json + "}";

                  get_assetformio.asset_form_data = requestmodel.asset_form_json;
                  get_assetformio.form_retrived_data = requestmodel.asset_form_json;
                  get_assetformio.form_retrived_asset_id = requestmodel.assetId;
                  get_assetformio.form_retrived_location = requestmodel.parent;
                  get_assetformio.form_retrived_asset_name = requestmodel.identification;

                  var update = await _UoW.BaseGenericRepository<AssetFormIO>().Update(get_assetformio);
                  */

            }
            #region Replace Asset location with proper site id
            /* var response = _UoW.formIORepository.ReplaceAssetLocationScript(siteId);
            foreach (var item in response)
            {
                int myBuildingId = 0;
                if (item.AssetFormIOBuildingMappings.FormIOBuildings.site_id == siteId)
                {
                    //do nothing
                    myBuildingId = item.AssetFormIOBuildingMappings.formiobuilding_id.Value;
                }
                else
                {
                    var building_name = _UoW.WorkOrderRepository.GetFormIOBuildingByNameTemp(item.AssetFormIOBuildingMappings.FormIOBuildings.formio_building_name, siteId);
                    if (building_name != null)
                    {
                        myBuildingId = building_name.formiobuilding_id;
                        item.AssetFormIOBuildingMappings.formiobuilding_id = building_name.formiobuilding_id;
                    }
                    else
                    {
                        FormIOBuildings formIOBuildings = new FormIOBuildings();

                        formIOBuildings.formiobuilding_id = building_name.formiobuilding_id;
                        formIOBuildings.formio_building_name = building_name.formio_building_name;
                        formIOBuildings.company_id = companyId;
                        formIOBuildings.site_id = siteId;


                        var insert_formIoBuilding = await _UoW.BaseGenericRepository<FormIOBuildings>().
                            Insert(formIOBuildings);
                        _UoW.SaveChanges();
                        myBuildingId = formIOBuildings.formiobuilding_id;
                        item.AssetFormIOBuildingMappings.formiobuilding_id = formIOBuildings.formiobuilding_id;
                    }
                }

                int myFloorId = 0;
                if (item.AssetFormIOBuildingMappings.FormIOFloors.site_id == siteId)
                {
                    myFloorId = item.AssetFormIOBuildingMappings.FormIOFloors.formiofloor_id;
                }
                else
                {
                    var floor_name = _UoW.WorkOrderRepository.GetFormIOFloorByNameTemp(item.AssetFormIOBuildingMappings.FormIOFloors.formio_floor_name, myBuildingId, siteId);
                    if (floor_name != null)
                    {
                        myFloorId = floor_name.formiofloor_id;
                        item.AssetFormIOBuildingMappings.formiofloor_id = floor_name.formiofloor_id;
                    }
                    else
                    {
                        FormIOFloors formIOFloors = new FormIOFloors();

                        formIOFloors.formiofloor_id = floor_name.formiofloor_id;
                        formIOFloors.formio_floor_name = floor_name.formio_floor_name;
                        formIOFloors.company_id = companyId;
                        formIOFloors.site_id = siteId;
                        formIOFloors.formiobuilding_id = myBuildingId;

                        var insert_formIoFloors = await _UoW.BaseGenericRepository<FormIOFloors>().
                            Insert(formIOFloors);
                        _UoW.SaveChanges();
                        myFloorId = formIOFloors.formiofloor_id;
                        item.AssetFormIOBuildingMappings.formiofloor_id = formIOFloors.formiofloor_id;
                    }
                }

                int myRoomId = 0;
                if (item.AssetFormIOBuildingMappings.FormIORooms.site_id == siteId)
                {
                    myRoomId = item.AssetFormIOBuildingMappings.FormIOFloors.formiofloor_id;
                }
                else
                {
                    var room_name = _UoW.WorkOrderRepository.GetFormIORoomByNameTemp(item.AssetFormIOBuildingMappings.FormIORooms.formio_room_name, myFloorId, siteId);
                    if (room_name != null)
                    {
                        myRoomId = room_name.formioroom_id;
                        item.AssetFormIOBuildingMappings.formioroom_id = room_name.formioroom_id;
                    }
                    else
                    {
                        FormIORooms formIORooms = new FormIORooms();

                        formIORooms.formioroom_id = room_name.formioroom_id;
                        formIORooms.formio_room_name = room_name.formio_room_name;
                        formIORooms.company_id = companyId;
                        formIORooms.site_id = siteId;
                        formIORooms.formiofloor_id = myFloorId;

                        var insert_formIoRooms = await _UoW.BaseGenericRepository<FormIORooms>().
                            Insert(formIORooms);
                        _UoW.SaveChanges();
                        myRoomId = formIORooms.formioroom_id;
                        item.AssetFormIOBuildingMappings.formioroom_id = formIORooms.formioroom_id;
                    }
                }

                int mySectionId = 0;
                if (item.AssetFormIOBuildingMappings.FormIOSections.site_id == siteId)
                {
                    mySectionId = item.AssetFormIOBuildingMappings.FormIOSections.formiosection_id;
                }
                else
                {
                    var section_name = _UoW.WorkOrderRepository.GetFormIOSectionByNameTemp(item.AssetFormIOBuildingMappings.FormIOSections.formio_section_name, myRoomId, siteId);
                    if (section_name != null)
                    {
                        mySectionId = section_name.formiosection_id;
                        item.AssetFormIOBuildingMappings.formiosection_id = section_name.formiosection_id;
                    }
                    else
                    {
                        FormIOSections formIOSection = new FormIOSections();

                        formIOSection.formiosection_id = section_name.formiosection_id;
                        formIOSection.formio_section_name = section_name.formio_section_name;
                        formIOSection.company_id = companyId;
                        formIOSection.site_id = siteId;
                        formIOSection.formioroom_id = myRoomId;

                        var insert_formIoSections = await _UoW.BaseGenericRepository<FormIOSections>().
                            Insert(formIOSection);
                        _UoW.SaveChanges();
                        mySectionId = formIOSection.formiosection_id;
                        item.AssetFormIOBuildingMappings.formiosection_id = formIOSection.formiosection_id;
                    }
                }
            }
            try
            {
                if (response?.Count > 0)
                {
                    responseModel.list = _mapper.Map<List<AssetsResponseModel>>(response);
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error in Get All Assets " + e.Message);
                throw e;
            }
            return responseModel;*/
            #endregion Replace Asset location with proper site id
            return 1;
        }

        public GetAssetformByIDResponsemodel GetAssetformByID(string asset_form_id)
        {
            GetAssetformByIDResponsemodel response = new GetAssetformByIDResponsemodel();
            var get_asset_form = _UoW.AssetFormIORepository.GetAssetFormIOByIdMobile(Guid.Parse(asset_form_id));
            var get_form = _UoW.formIORepository.GetFormIOById(get_asset_form.form_id.Value).Result;

            var asset_form_data = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(get_asset_form.asset_form_data);
            var master_form_ = JsonSerializer.Deserialize<FormioDynamicDataobj.Root>(get_form.form_data);
            master_form_.data = asset_form_data.data;
            response.asset_form_data = JsonSerializer.Serialize(master_form_);
            response.asset_form_id = get_asset_form.asset_form_id;

            return response;
        }
        public GetAssetformByIDForBulkReportResponsemodel GetAssetformByIDForBulkReport(GetAssetformByIDForBulkReportRequestmodel requestmodel)
        {
            GetAssetformByIDForBulkReportResponsemodel response = new GetAssetformByIDForBulkReportResponsemodel();
            var get_asset_form_data = _UoW.AssetFormIORepository.GetAssetformsByIds(requestmodel.asset_form_ids);

            var master_form_ids = get_asset_form_data.Select(x => x.form_id.Value).Distinct().ToList();
            var get_master_form_ids = _UoW.AssetFormIORepository.GetMasterFormDataByIds(master_form_ids);

            response.asset_form_data = _mapper.Map<List<asset_form_data_bulk_report>>(get_asset_form_data);
            response.master_form_data = _mapper.Map<List<master_form_data_bulk_report>>(get_master_form_ids);

            var get_company = _UoW.AssetFormIORepository.GetCompanyById(Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
            response.isCalibrationDateEnabled = get_company.isCalibrationDateEnabled;

            return response;
        }

        public GetAssetformEquipmentListResponsemodel GetAssetformEquipmentList(GetAssetformEquipmentListRequestmodel requestmodel)
        {
            GetAssetformEquipmentListResponsemodel responsemodel = new GetAssetformEquipmentListResponsemodel();
            responsemodel.equipment_list = new List<FormioEquipmentlist>();

            for (int i = 0; i < 5; i++)
            {
                FormioEquipmentlist FormioEquipmentlist = new FormioEquipmentlist();
                FormioEquipmentlist.label = "equipment-label" + i.ToString();
                FormioEquipmentlist.value = "121" + i.ToString();
                FormioEquipmentlist.name = "test1" + i.ToString();
                FormioEquipmentlist.serialNumber = "111-test12" + i.ToString();
                FormioEquipmentlist.calibrationDate = DateTime.UtcNow;
                responsemodel.equipment_list.Add(FormioEquipmentlist);
            }


            return responsemodel;
        }


        public ListViewModel<GetAllEquipmentListResponsemodel> GetAllEquipmentListService(GetAllEquipmentListRequestmodel requestmodel)
        {
            ListViewModel<GetAllEquipmentListResponsemodel> response = new ListViewModel<GetAllEquipmentListResponsemodel>();

            var get_all_equipments = _UoW.AssetFormIORepository.GetAllEquipmentListData(requestmodel);

            var maplist = _mapper.Map<List<GetAllEquipmentListResponsemodel>>(get_all_equipments.Item1);

            response.list = maplist;
            response.listsize = get_all_equipments.Item2;

            response.pageIndex = requestmodel.page_index;
            response.pageSize = requestmodel.page_size;

            return response;
        }

        public async Task<AddUpdateEquipmentResponseModel> AddUpdateEquipmentService(AddUpdateEquipmentRequestModel request)
        {
            AddUpdateEquipmentResponseModel response = null;
            try 
            {
                if (request.equipment_id == null || request.equipment_id == Guid.Empty)
                {
                    // check for equipment number if duplicate then return error
                    var isDuplicate = _UoW.AssetFormIORepository.CheckForDuplicateEquipmentNumber(request.equipment_number);
                    if(!isDuplicate)
                    {
                        Equipment equipment = new Equipment();
                        equipment.site_id = Guid.Parse(GenericRequestModel.site_id);
                        equipment.company_id = Guid.Parse(GenericRequestModel.company_id);
                        equipment.equipment_number = request.equipment_number;
                        equipment.equipment_name = request.equipment_name;
                        equipment.manufacturer = request.manufacturer;
                        equipment.model_number = request.model_number;
                        equipment.serial_number = request.serial_number;
                        equipment.calibration_interval = request.calibration_interval;
                        equipment.calibration_date = request.calibration_date;
                        equipment.calibration_status = request.calibration_status;
                        equipment.created_at = DateTime.UtcNow;
                        equipment.created_by = GenericRequestModel.requested_by.ToString();

                        var insert = await _UoW.BaseGenericRepository<Equipment>().Insert(equipment);
                        _UoW.SaveChanges();
                        response = new AddUpdateEquipmentResponseModel();
                        response.equipment_id = equipment.equipment_id;
                        response.success = (int)ResponseStatusNumber.Success;

                    }
                    else
                    {
                        response = new AddUpdateEquipmentResponseModel();
                        response.success = (int)ResponseStatusNumber.equipment_number_must_be_unique;
                    }
                }
                // update
                else
                {
                    var isDuplicate = _UoW.AssetFormIORepository.CheckForDuplicateEquipmentNumberForUpdate(request.equipment_number, request.equipment_id);
                    if(!isDuplicate)
                    {
                        var get_equipment = _UoW.AssetFormIORepository.GetEquipmentDataByID(request.equipment_id);
                        if (get_equipment != null)
                        {
                            get_equipment.equipment_number = request.equipment_number;
                            get_equipment.equipment_name = request.equipment_name;
                            get_equipment.manufacturer = request.manufacturer;
                            get_equipment.model_number = request.model_number;
                            get_equipment.serial_number = request.serial_number;
                            get_equipment.calibration_interval = request.calibration_interval;
                            get_equipment.calibration_date = request.calibration_date;
                            get_equipment.calibration_status = request.calibration_status;
                            get_equipment.modified_at = DateTime.UtcNow;
                            get_equipment.modified_by = GenericRequestModel.requested_by.ToString();

                            var update = await _UoW.BaseGenericRepository<Equipment>().Update(get_equipment);
                            _UoW.SaveChanges();

                            response = new AddUpdateEquipmentResponseModel();
                            response.equipment_id = get_equipment.equipment_id;
                            response.success = (int)ResponseStatusNumber.Success;
                        }
                    }
                    else
                    {
                        response = new AddUpdateEquipmentResponseModel();
                        response.success = (int)ResponseStatusNumber.equipment_number_must_be_unique;
                    }

                }
            } catch (Exception e) 
            { 
                
            }
            
            return response;
        }

        public async Task<int> DeleteEquipmentService(Guid equipmentId)
        {
            var get_equipment = _UoW.AssetFormIORepository.GetEquipmentDataByID(equipmentId);

            get_equipment.isarchive = true;
            get_equipment.modified_at = DateTime.UtcNow;
            get_equipment.modified_by = GenericRequestModel.requested_by.ToString();

            var update = await _UoW.BaseGenericRepository<Equipment>().Update(get_equipment);
            
            if (update)
            {
                return (int)ResponseStatusNumber.Success;
            }
            return (int)ResponseStatusNumber.Error;
        }

        public async Task<FilterAttributesEquipmentResponseModel> FilterAttributesEquipmentService()
        {
            FilterAttributesEquipmentResponseModel response = new FilterAttributesEquipmentResponseModel();
            var get_equipment_number = _UoW.AssetFormIORepository.GetDropdownEquipmentNumber();
            var get_manufacturer = _UoW.AssetFormIORepository.GetDropdownEquipmentManufacturer();
            var get_model_number = _UoW.AssetFormIORepository.GetDropdownEquipmentModelNumber();
            var get_calibration_status = _UoW.AssetFormIORepository.GetDropdownEquipmentCalibrationStatus();

            response.equipment_number = get_equipment_number;
            response.manufacturer = get_manufacturer;
            response.model_number = get_model_number;
            response.calibration_status = get_calibration_status;

            return response;
        }


        public GetAssetFormIOByAssetIdResponseModel GetAssetFormIOByAssetID(Guid asset_id)
        {
            GetAssetFormIOByAssetIdResponseModel responseModel = new GetAssetFormIOByAssetIdResponseModel();
            try
            {
                var response = _UoW.AssetFormIORepository.GetAssetFormIOByAssetID(asset_id);
                responseModel = _mapper.Map<GetAssetFormIOByAssetIdResponseModel>(response);
            }
            catch (Exception e)
            {
                throw e;
            }
            return responseModel;
        }

        

        public List<GetAssetsForSubmittedFilterOptionsResponsemodel> GetAssetsForSubmittedFilterOptionsByStatus(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel)
        {
            List<GetAssetsForSubmittedFilterOptionsResponsemodel> response = new List<GetAssetsForSubmittedFilterOptionsResponsemodel>();
            var get_asset_forms = _UoW.AssetFormIORepository.GetAssetsForSubmittedFilterOptions(requestmodel);
            get_asset_forms.ForEach(x =>
            {
                if (x != null && x != "")
                {
                    GetAssetsForSubmittedFilterOptionsResponsemodel GetAssetsForSubmittedFilterOptionsResponsemodel = new GetAssetsForSubmittedFilterOptionsResponsemodel();
                    GetAssetsForSubmittedFilterOptionsResponsemodel.asset_name = x;
                    response.Add(GetAssetsForSubmittedFilterOptionsResponsemodel);
                }
            });
            return response;
        }
        public List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel> GetWorkOrdersForSubmittedFilterOptionsByStatus(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel)
        {
            List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel> response = new List<GetWorkOrdersForSubmittedFilterOptionsResponsemodel>();
            var get_WO_forms = _UoW.AssetFormIORepository.GetWorkOrdersForSubmittedFilterOptions(requestmodel);
            get_WO_forms = get_WO_forms.GroupBy(p => p.wo_id)
                                       .Select(grp => grp.First())
                                       .ToList();
            get_WO_forms.ForEach(x =>
            {
                GetWorkOrdersForSubmittedFilterOptionsResponsemodel GetWorkOrdersForSubmittedFilterOptionsResponsemodel = new GetWorkOrdersForSubmittedFilterOptionsResponsemodel();
                GetWorkOrdersForSubmittedFilterOptionsResponsemodel.value = x.wo_id;
                GetWorkOrdersForSubmittedFilterOptionsResponsemodel.label = x.manual_wo_number;

                response.Add(GetWorkOrdersForSubmittedFilterOptionsResponsemodel);
            });
            return response;
        }


        public async Task<int> UpdateEquipmentCalibrationStatusByDateAndInterval()
        {
            int response = (int)ResponseStatusNumber.Error;
            try
            {
                var get_all_equipments = _UoW.AssetFormIORepository.GetAllEquipmentListForUpdateCalStatus();

                foreach (var equipment in get_all_equipments)
                {
                    var cal_date = equipment.calibration_date.Date;
                    int interval = equipment.calibration_interval;

                    if ((DateTime.UtcNow.Date > cal_date || DateTime.UtcNow.Date == cal_date) &&
                        (DateTime.UtcNow.Date < cal_date.AddMonths(interval) || DateTime.UtcNow.Date == cal_date.AddMonths(interval)))
                    {
                        equipment.calibration_status = (int)CalibrationStatus.Calibrated;
                    }
                    else
                    {
                        equipment.calibration_status = (int)CalibrationStatus.NotCalibrated;
                    }

                    var update = await _UoW.BaseGenericRepository<Equipment>().Update(equipment);
                    _UoW.SaveChanges();

                    if (update) { response = (int)ResponseStatusNumber.Success; }
                }
            }
            catch (Exception ex)
            {
            }
            return response;
        }


        public async Task<int> GenerateBulkNetaReport(GenerateBulkNetaReportRequestModel requestModel, string aws_access_key, string aws_secret_key)
        {
            int res = (int)ResponseStatusNumber.Error;
            try
            {
                if (requestModel.asset_form_ids != null && requestModel.asset_form_ids.Count > 0)
                {
                    NetaInspectionBulkReportTracking netaInspectionBulkReportTracking = new NetaInspectionBulkReportTracking();

                    var count = _UoW.AssetFormIORepository.GetNetareportCountBySite();

                    string asset_form_ids_string = String.Join(",", requestModel.asset_form_ids);

                    netaInspectionBulkReportTracking.asset_form_ids = asset_form_ids_string;
                    netaInspectionBulkReportTracking.report_id_number = (count + 1).ToString();
                    netaInspectionBulkReportTracking.report_inspection_type = requestModel.report_inspection_type;
                    netaInspectionBulkReportTracking.report_status = (int)NetaInspectionReportStatusType.InProgress;
                    netaInspectionBulkReportTracking.is_deleted = false;
                    netaInspectionBulkReportTracking.created_at = DateTime.UtcNow;
                    netaInspectionBulkReportTracking.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by;
                    netaInspectionBulkReportTracking.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);

                    var insert = await _UoW.BaseGenericRepository<NetaInspectionBulkReportTracking>().Insert(netaInspectionBulkReportTracking);
                    if (insert)
                    {
                        res = (int)ResponseStatusNumber.Success;
                        _UoW.SaveChanges();

                        LambdaNetaInspectionReportRequestmodel _LambdaNetaInspectionReportRequestmodel = new LambdaNetaInspectionReportRequestmodel();
                        _LambdaNetaInspectionReportRequestmodel.tracking_id = netaInspectionBulkReportTracking.netainspectionbulkreporttracking_id.ToString();
                        _LambdaNetaInspectionReportRequestmodel.asset_list = requestModel.asset_form_ids;

                        // Call Lamda
                        string jsonString = JsonSerializer.Serialize(_LambdaNetaInspectionReportRequestmodel);
                        await AssetFornioInspectionReport.GenerateReportForBulkNetaReport(aws_access_key, aws_secret_key, jsonString, _logger);
                    }
                }
            }
            catch(Exception e)
            {
            }
            return res;
        }

        public async Task<ListViewModel<GetAllNetaInspectionBulkReportTrackingListResponseModel>> GetAllNetaInspectionBulkReportTrackingList(GetAllNetaInspectionBulkReportTrackingListRequestModel requestModel)
        {
            ListViewModel<GetAllNetaInspectionBulkReportTrackingListResponseModel> responseModel = new ListViewModel<GetAllNetaInspectionBulkReportTrackingListResponseModel>();
            try
            {
                var get_all_neta_insp = _UoW.AssetFormIORepository.GetAllNetaInspectionsReportList(requestModel);

                // check timeout of 16 minute if created_at > 16 min. then mark that report as failed
                foreach(var item in get_all_neta_insp.Item1.ToList())
                {
                    if(item.report_status == (int)NetaInspectionReportStatusType.InProgress && item.created_at.Value.AddMinutes(16) < DateTime.UtcNow) // if status is in progress and time elapsed is more than 16 min. then mark that as failed
                    {
                        item.report_status = (int)NetaInspectionReportStatusType.Failed;
                        await _UoW.BaseGenericRepository<NetaInspectionBulkReportTracking>().Update(item);
                        _UoW.SaveChanges();
                    }
                }
                if (get_all_neta_insp.Item1 != null && get_all_neta_insp.Item1.Count > 0)
                {
                    var mapped_list = _mapper.Map<List<GetAllNetaInspectionBulkReportTrackingListResponseModel>>(get_all_neta_insp.Item1);

                    foreach(var item in mapped_list)
                    {
                        if(!String.IsNullOrEmpty(item.asset_form_ids))
                        {
                            item.assets_name_list = new List<string>();
                            string[] stringArray = item.asset_form_ids.Split(',');
                            List<string> asset_form_ids_list = stringArray.ToList();

                            var get_asset_names = _UoW.AssetFormIORepository.GetAssetNamesByAssetFormIds(asset_form_ids_list);
                            item.assets_name_list = get_asset_names;
                        }
                        if (item.created_by != null && item.created_by!=Guid.Empty)
                        {
                            var technician_user = _UoW.WorkOrderRepository.GetUserByID(item.created_by.Value);
                            item.created_by_name = technician_user.firstname + " " + technician_user.lastname;
                        }
                    }

                    if (!String.IsNullOrEmpty(requestModel.search_string))
                    {
                        mapped_list = mapped_list.Where(x=> x.assets_name_list.Contains(requestModel.search_string)).ToList();
                    }

                    responseModel.listsize = get_all_neta_insp.Item2;
                    responseModel.pageIndex = requestModel.pageindex;
                    responseModel.pageSize = requestModel.pagesize;
                    responseModel.list = mapped_list;
                }
            }
            catch (Exception e)
            {
            }
            return responseModel;
        }



        public async Task<int> ScriptForAddNFPA70BToAssets()
        {
            int res = 0;
            int i = 0; int j = 0;
            AssetPMService assetPMService1 = new AssetPMService(_mapper);
            try
            {
                #region Add PMs to AssetPMs Table for Assets (if other than NFPA-70B is Assigned to Asset then remove and Assign NFPA-70B)

                List<Asset> my_assets = new List<Asset>();
                List<string> list = new List<string>();
                list.Add("282a7754-022a-4cef-85da-801cf687174b");

                string asset_ids = "{\"ids\":[\"14ba03d5-dc8e-4cc1-ae57-e05412f11dd8\",\"22547970-58e0-49ac-bed1-de296f71e7b6\",\"16f220a4-efa3-4ec6-9242-75cfd39fefd0\",\"258231f2-0840-4c4a-ae5c-7326b5426048\",\"23331879-b1ee-4206-8b0e-14f1b8164e55\",\"27b755fd-cb31-44a7-97e9-344a2ff3c9cb\",\"1bd3d18c-b526-496f-8624-b725dd42fa3a\",\"16bed0b9-7844-4a2e-88eb-f0da064ef17b\",\"14b5a36f-c7c0-480e-b2a3-8cdf844957e5\",\"36624a61-94ca-448b-8ed1-f79c0b5016d3\",\"255f514c-8170-4307-852b-2d4ff6a8f99a\",\"38c34d23-fcc2-483e-83f9-b8448673838b\",\"35b7da00-c28f-4704-a513-e3bcf601e906\",\"198219d4-67f2-4d9b-9643-408b28a80058\",\"2b381545-0749-4d92-b7a7-a600ca4add5c\",\"31a63b5c-7c68-4d74-b2f2-1dbbe3fa3c2e\",\"1ce03d72-76e4-49f4-9d86-9c49f55b4390\",\"1c762cdf-052c-4b76-bf1a-010521f7bf72\",\"2d07e4e4-1d73-477f-8477-667d8de4353a\",\"37778c99-b186-4986-b879-fe770e6c1f50\",\"2de3e6be-1324-402c-a6ae-200bc0344a89\",\"317f3a6d-45f5-4735-a0a8-7c0b4688c4d7\",\"331b9995-6d6e-4df6-899a-143f373ce778\",\"38f58058-d6ad-4c45-bbe1-d28e8b3b0a73\",\"184f6e22-0e20-477b-a7fa-a73f81f228b6\",\"3f75a509-e886-4a87-a262-2c2add6cc48c\",\"249e1537-ef83-497d-ba44-2d4aa0e88e95\",\"2c048f48-96fc-430b-9bcb-dce82f112d85\",\"382cf39a-d6f9-469b-9c56-470361560069\",\"340c4372-5d83-43ca-b81e-867cc9a8a303\",\"2c0a559c-03e8-4aa3-b9c6-940778d20761\",\"19053f71-9d01-41cc-84ed-87a8fc5ae65f\",\"3db462cf-a135-4df2-9839-73095018aebf\",\"2be8ec59-12f7-44c6-9684-9cb101ba1678\",\"3c1a57d8-7501-44f4-9955-530cda59c5c9\",\"2cef6c21-98fd-4304-8799-789a44b15f4c\",\"1b2542a0-0c3e-4da2-a6d2-dd4580f6ccdc\",\"29d3fca6-0e74-4516-ae6f-99a67244e2d4\",\"2bccd7af-2540-4c34-bd69-38cebb1aa1b0\",\"31fa0fdb-07db-41c3-ba9b-1b28f138a4a3\",\"1d61de8f-a54d-4ea1-9650-9742a9e26034\",\"1f189d13-1566-4034-9a62-eb4132e48d77\",\"40fa9100-adb7-49da-8fea-240648644f88\",\"162ca23a-6aca-4d54-adda-86da776c4a50\",\"406d669e-6339-417c-a930-8ea3b54ef7b1\",\"23dcdaac-83b7-4ff2-820f-4ea0b4b7f636\",\"32fe6ace-0d3a-4cb9-8bb9-73de87bd02a0\",\"26e58a8e-7b6e-4f45-a184-4eb397318bd8\",\"2ac0a59c-51c6-4565-ae94-15b5c2b25ff9\",\"36199497-9b13-4546-9057-8f09ae537ea1\",\"2d8b1833-62e4-47bc-bb7a-8cba0fc82ad3\",\"2889b091-8d77-40a1-b33b-f46f579c54bd\",\"37932460-d2c7-41be-bce3-44829b37f871\",\"3234b532-c3ce-4d63-a8ab-09e19e700425\",\"3aaf93fa-2292-49eb-af56-b7e022174189\",\"395928dd-7d97-4169-a499-2f809c8773ca\",\"27aeb63f-ed41-412a-833a-c071b3c9b323\",\"27f29c9a-4f19-4912-a2ec-c3b0867946db\",\"35cf1305-03de-411c-8d80-6f0b5ab7dc4e\",\"17acb4f4-13b3-41ce-8d20-9328baa16f53\",\"38410f80-8fe3-4a8a-94f2-810da23cfe78\",\"39c73e0f-b100-4e55-b5ff-30ec7bff7cfb\",\"3110d9b2-a428-460c-a420-60a4e8c56b5f\",\"372dbdd6-d101-4a03-91b9-7ab650525521\",\"1bf9216e-be65-4d7e-905e-d2c6b43e2d7b\",\"41ebd6e2-8ddc-4532-b53c-0de8d734fd6d\",\"1c7892eb-24f7-440d-b260-f259c3918efe\",\"2e032180-d6cd-40b5-83bb-acbe1491b289\",\"3c7b686e-cb6b-4efe-a6f4-76223e245723\",\"3c573d6e-fec0-4397-830a-046b5072adc1\",\"208a46e2-e73f-4b3a-baa2-bf9c38ad748a\",\"1a358571-5a2d-459a-ac6b-50b6594edcd4\",\"179b3865-c9b6-4bf3-9a0d-74a44f35a067\",\"32c074a2-07ee-4da2-8f66-5e58cdf01f0b\",\"1c375533-6fe0-41f2-9217-46c0e1f35fd4\",\"21360554-31b5-4cc0-ae74-fb56277565f9\",\"2b4498b5-b546-4e50-9d5b-3488365e928e\",\"2b05dd29-631c-4677-a162-439d854b6a40\",\"3311c1f8-5173-4b91-8319-1ca0ace65b7f\",\"184006da-f510-4a8b-b4c1-564050407724\",\"31ef6cba-bff8-4c05-afa4-1ba3415b7d82\",\"3bd467a0-090f-4bca-9e06-de29bede64a3\",\"1b34e2d8-1415-4b94-a9ee-e24f475912ce\",\"3cf3f8e0-9783-476c-93bf-de2bc2a7ebe5\",\"2040feff-10a8-4c7a-8a57-a5ac1a309431\",\"34bba0ea-288f-456b-b361-3b8d4133ff8e\",\"14c88ade-1f7b-4d22-ad5c-5b6b1c7d5835\",\"312132c0-6480-44a9-9bcb-63839787f48f\",\"14423901-f360-4ca6-9646-5f55c5dfb003\",\"21f9e58a-c997-4f95-b7bd-4e5ab6bae298\",\"150da18f-859c-4615-bf6e-4db8f760bbba\",\"3c35ff7f-79bf-458b-ad98-288109965d8f\",\"34ebdf51-fc38-4ace-af79-acff4e4654da\",\"17be7d65-ea78-4a40-9134-7c117a6e8c5c\",\"31de5b7a-063d-41a7-8352-ae2d0460e940\",\"31f18b2c-4840-4a6a-bdf8-7d947dae2f01\",\"3fcf7d40-aaef-4cc0-bb80-410e8f907342\",\"1a6f85b3-466b-4e79-a17a-50182b0418e7\",\"1a892ce6-9bfc-424d-ac40-2b301d2c8277\",\"38cd1c10-2148-4849-a2b6-515ae4dd6ffb\",\"1d7fa6e4-4d9a-4be0-8cab-9946635181af\",\"372a2175-0b68-459d-b697-33df979a093b\",\"2b5c349e-e435-4b2a-824b-406b400fc714\",\"2cb76794-ca7e-4bc9-a589-dbf6956e1343\",\"1d274c53-cf54-4745-9711-eb9253630461\",\"3e99b1d8-22f8-4c66-86f6-6b71afea7065\",\"147d66f7-a98d-4680-b6cc-8e4d1e30a309\",\"41afcfed-cdbe-4ea1-91ea-3e23598bcb24\",\"3ee666cf-e50a-4ca7-a4c6-0667a99d80e9\",\"2a6d0747-76a9-455d-95b9-361c21db3f3a\",\"3554289c-cda8-4af3-9a0b-7fd3fc3fbecc\",\"31ae0a5a-9859-4db4-a350-3860446c28da\",\"3a71716c-4ba3-47b6-b872-88964689d27d\",\"338a2a65-aa8c-4070-ac60-1e5874683eb0\",\"23c9a75f-b563-492c-a700-0a9699f1a5ea\",\"314be2e7-43ff-4b5c-848b-8b5cbcc79469\",\"14ab939b-422d-4a1c-8f4d-057885d81cfc\",\"40620a43-d99c-474d-b8a9-371e111da4e2\",\"303e565a-5bb1-4532-b6bc-26474d09eedf\",\"200ab9fd-c33a-45da-8af3-4c35796d76a9\",\"314ebb42-36bd-4db6-b729-173f268b44ad\",\"1f4dbca5-402a-4b87-8c54-5bea9c718a6c\",\"23144c5e-ec9d-47ca-a147-fa73cf078b96\",\"304e345b-942d-4102-8e76-c28cd7c8b214\",\"15ba98d5-b265-4d20-b146-c437a3e4c537\",\"2bb28b91-cdb4-4f03-8460-b902b5fc390e\",\"1b6e122d-16d9-4cd9-959e-1f4935f1a1d7\",\"2f6869cc-325b-4f74-8536-43a1464283b9\",\"3183a4af-e8e4-4827-840b-6a666846936f\",\"363da04b-a914-444e-8827-f7c46d12b375\",\"20f936b6-7ece-4b24-b278-46c2e6b68a06\",\"34d2c730-d7fa-4a75-ad26-b08b9c8b5afe\",\"17afba0b-207b-41f7-b75b-2c649c414907\",\"1a4ffe63-0169-484e-87e3-97381043523c\",\"28b942c7-ef2f-4f9a-a693-dbd4138e83bf\",\"1f924876-cd52-45a2-bb18-53c9c47055e0\",\"2c3621fe-dbdd-4225-bfc6-f3b81e4fc45d\",\"3eccb5e9-55cb-475e-993d-e0ce45b2e24b\",\"213bebfa-2fc5-44e6-88fa-7aba7da718c2\",\"1ef7dd75-80eb-44cc-aaec-fcd070e2de27\",\"39d37758-f6cb-47ee-a446-0f0de9430fb9\",\"256aaf23-4ae2-45cf-866b-bab5cf081132\",\"262c10ef-34ec-403a-a21b-1121b56d81b4\",\"28f3a67f-997a-4e14-8851-d97c7e49042f\",\"1eabb7d2-38a7-4afd-8006-d63fa176e560\",\"157438cf-56cc-4040-a546-2857b1918718\",\"3e0bc03f-ff19-4394-9871-f80816853db8\",\"2409f981-36c7-471f-af53-fdaec02cd2cb\",\"14ef27bc-fee4-4af1-b718-5d004b7900c7\",\"1e80dff5-d9e8-4698-881a-0f4caed63fe1\",\"261352b7-c526-42c6-bce9-7b8798af97c8\",\"3a097f0b-85c1-4fbd-bce6-f40eb49bd498\",\"2add42f0-fbba-48dc-bfa6-2a1a896e7e46\",\"3a714680-1a98-458c-8456-3de9de71a9f0\",\"142e86b6-b903-4426-840b-c160e73c5f6d\",\"13e28f7f-6795-4d4a-a22d-3f81066ea1ba\",\"3c701b1d-5b60-4f45-8c29-34707f337ac2\",\"3a2d07b8-441f-4eb5-a479-4f9a00f6495f\",\"2271dcf2-6d12-4214-9e70-5666b232ad54\",\"36bac6be-1fbb-4e21-8074-8cf80b1423bf\",\"16069853-15b9-497f-bea2-223c907fc6aa\",\"34933025-4b98-4a06-888b-87d10ebefaf3\",\"274418b1-0435-4c49-ba4c-1a35394edf1e\",\"2e720adf-723a-40ef-aeb9-5ec3927bf0d8\",\"2d6046c2-34dc-4dd3-aae3-bcf9af01f4bb\",\"1fa6b050-aaa6-4d23-a748-f15ce3bd6ade\",\"1c802a4f-bf27-4a7c-903b-9cd9cacbcada\",\"39dec9b4-a2f5-44f2-b1b0-304b0eb34727\",\"24a137ac-618f-421d-a330-989f8f843450\",\"13ec47d8-1af8-49b8-90d5-6315fee5f5b0\",\"1dad88fd-2847-40b8-931a-1b476f700f85\",\"2959cec0-90d8-48ce-b550-eced6b85e287\",\"367d6def-5e97-4c6b-8cec-e2c3ee0d1e8d\",\"281824d6-c703-43eb-8760-6efdbc570d3a\",\"39638183-0afa-4089-9af4-06ee5d2a874a\",\"348ce746-6f27-40a9-ac8b-7df4a22d924a\",\"31a7c474-b87d-44c0-a720-853a998e2671\",\"1fa5bcad-4796-45cb-b1aa-0b61e572f513\",\"23e4f8a7-5508-4fc5-944e-0661b4f6415b\",\"1bbb7f31-5c03-4a43-b13c-2863eeb84995\",\"1e85add8-b3ac-4aa1-91da-8b182afd854f\",\"172cb3c6-0a63-40cc-961a-db58727d89a8\",\"32af29d3-c94c-40fb-babd-5e8b397c0f39\",\"3b3d5785-d7af-4c65-b98c-40cdadc4fdf3\",\"3b1beae4-1a6a-4f34-9640-ebb8276e29f5\",\"25c0fe85-1f56-457b-a07c-2815bb331284\",\"149fc3b7-1bba-4186-a840-3a4009760b50\",\"1de325e9-ba5d-47b6-827a-d9af29fb81ef\",\"12f49991-c55d-463d-b0bc-f7f3d5cd59d5\",\"28a049d9-029d-4a78-84d5-0f400d2fa4ef\",\"33ebc986-a311-4a18-8abb-461bca642c3c\",\"27e3fb37-3d71-4a0e-ae3e-294f7d162da8\",\"3708a130-166d-482e-8117-0b856288bd01\",\"1efa7568-208c-4dda-ab81-936e4607de65\",\"1b7012c7-1d79-4adf-943f-578d36f5ae47\",\"1b0e6e07-4350-4813-bbf6-7fa566a57707\",\"22f00295-4a9e-4df7-af41-db83dece3286\",\"13cebcec-4571-4d18-bd57-44654187059f\",\"3960cfdf-87cd-473a-8e04-b403732f8e28\",\"39ce0c56-639b-4f28-ba4c-df4a35018f73\",\"14895413-880d-4510-8563-2da743929ede\",\"182e319a-a8eb-49d6-be14-c351c496cb54\",\"2afcf9bc-82bf-4e3c-8a74-dc821678f08a\",\"2e3096d3-b6be-454c-9a54-2a3b9549cc84\",\"28747139-2420-484a-aa9c-3ad7eb102c53\",\"3f53c37b-0648-4388-88f9-066748450019\",\"36749c12-7304-450d-a93c-9c65f4f23989\",\"3cd0264f-af38-4f7a-9498-af1ba587f387\",\"294c5874-10c2-43e3-858a-8b9298eea869\",\"1ceed213-3b28-4bf9-aac1-2765567e9221\",\"3fa5414e-7282-49bf-b305-e6874bafd929\",\"25a5b35f-278e-4cdb-9eda-e5abe2a0588d\",\"2f4bc93a-e40a-4e85-b7f5-76cb81a08e68\",\"3c69ace6-9ba5-489f-9056-133b7eec1766\",\"3c136110-67e6-4ec6-8734-f38cd700b19e\",\"303d48ae-a5a3-4ff4-9329-331494a6cc3a\",\"2ab2774e-7677-422c-aeb3-f56e6674042c\",\"1b362557-8b44-4100-bba6-02a8e1602a42\",\"17e67e0b-ee57-4b49-9e5e-78761663e870\",\"205c2008-de60-4b4c-9164-f167cd7b0a9d\",\"3ed719f5-37c7-4ca7-be2b-90e3757fbbaf\",\"2bfddf53-27f5-470f-986f-a4830f182d6f\",\"18ec401d-efa6-49af-a371-fa06ccde9640\",\"15cfdb1d-952b-40da-aedd-71733965b1e3\",\"1ca7d1e4-d3ee-46aa-b551-ffd3b5a7863e\",\"2e916dbd-78f8-410f-a1cc-13728633a2c5\",\"2fb5b866-41a9-4f2f-8f87-d3b4f7602a9c\",\"18297fa7-5c19-4db5-a8ba-242bf3ad7576\",\"2f767ad1-c0ba-4047-a921-e4680d63e343\",\"2ec7bef2-e3aa-4ada-9860-16c85b7b3e73\",\"30941f6c-1182-43ea-95db-1fca859562f2\",\"2d66212e-c9c4-4aa7-b666-f78da605ab53\",\"27c8feaf-f438-4610-8fbb-0bb283291096\",\"34e50c25-2bcd-4e0f-9adb-31647637bba2\",\"38d8731a-fe4c-4b55-8e6d-d5ca77d6b5c5\",\"22910a59-59e3-4da6-b6ab-e7950d6aa2a7\",\"3b0b3306-0fca-4167-a91b-cfb68ff21704\",\"3c1f66cd-f665-46e6-9e01-fbdd31dd3a15\",\"1985dbc1-ea10-455c-b90e-efeef28c87a9\",\"20206772-9b26-4b60-9be8-b08ed101ecdd\",\"1e586675-54e3-4ed0-8ccf-82392a9c2486\",\"33ef68ef-6889-4a87-b8cf-c39dbbdb88c7\",\"39395d11-f8bf-4fc7-aa12-ad5b49271515\",\"303d1b38-571a-4b5f-b19f-efd557defd73\",\"37c5047d-2fb3-44f2-a4a4-83a6fda942e8\",\"3fa7b545-3e45-412e-964f-af53df8b4d3b\",\"2e674810-4eff-449d-9251-59fb4976ba52\",\"17d8aa47-5575-4453-9fe0-e2d4afbab024\",\"39f4fed9-5673-40b9-ab2e-258ed4125c44\",\"340fafd0-80b9-4f29-b2f8-9828696df7c6\",\"3ee0680e-5d1e-41eb-b2a5-78fd72f189ba\",\"1faff73c-e14b-4b2c-b074-d0e35aacf7e4\",\"179614f6-4bee-4d3b-8239-edf55da5e7c9\",\"20778209-879b-44cd-8e81-45cc678da9fa\",\"155e9f64-f338-4459-9690-a5022b81f6d1\",\"14a55075-f133-457d-9c24-5666d5494371\",\"268cf6e3-b616-4158-9098-1a4ae186f616\",\"1abc9c81-946f-462c-8c0a-0540fc19c94a\",\"250d5cdd-6fdf-4ff2-a24c-707ae2449f52\",\"15dd656e-8c38-49ef-a2ac-b78c87b92e53\",\"305b461f-0645-4ced-abfb-0d302f21a23b\",\"339f5b03-eb06-4983-bdfb-a9e81ecc4d86\",\"391e0b30-f029-4eda-a9bc-a4a03dc3326d\",\"3edebad6-0913-4550-a64a-0252a458748d\",\"33c4821a-f90e-48a3-a3ac-dff508375fd0\",\"2e1835da-dd81-4ac6-9901-c11519213643\",\"28f07ce7-4ed7-4a1b-b036-5a8c5ba745fe\",\"1a2a3a26-d052-4811-a3a9-aec57f0ec1b4\",\"21f9c808-163e-4894-91fd-921b520fa76b\",\"40ce4e4f-78b6-4c07-8a37-41663b638d03\",\"1ae52624-2ded-4903-84ee-14890bfc0893\",\"27b336cc-b1dd-4d43-b575-d11189f88a7e\",\"4185cd90-c38a-4c54-870a-ab174e5a8a2b\",\"2d23ee97-e63b-4e0d-a58a-6430b851d29f\",\"1e587a0f-a5e8-4f6e-b2f2-ab37d0eb0804\",\"23eb43d6-5184-4188-a24b-5863a9182bae\",\"326e9f44-5488-4d38-aa4a-b71d4c467506\",\"1c53db75-b103-477b-a780-20b88c20007d\",\"296c59ea-77e9-49ba-ab13-2331830776b2\",\"375e49ba-11ab-4dda-b425-0840a5e59394\",\"23e7b53f-a291-42dc-8664-643d75690e0b\",\"207817c0-f76a-4bc4-8747-87ce6cede9b3\",\"3a4359a6-9790-4294-bfad-dc86cd5a3e73\",\"3b65a99a-6ae4-4f93-a6d8-807fc9c02b74\",\"37a710fa-f5c3-48d5-a741-0fa38ca4fdff\",\"33f568d4-ae94-44e3-83fd-ab4feec70dc4\",\"1c36b8c1-381d-4312-b0bd-36817183f3d1\",\"1f1abe9a-8687-4562-a4c3-bb5fd84a2466\",\"4051a7d1-0730-4e45-8986-87645f2dedf9\",\"32f8a733-4faa-4857-85eb-c79c565f2736\",\"14330519-ee02-4754-b959-976827dc58eb\",\"376223f8-371d-482e-b56f-ac3c3236b78e\",\"33f63b4a-c528-4088-a5c0-770925613ef6\",\"2ac1006f-a42c-4a05-a871-530623cea2dd\",\"4044e0dd-5596-4de9-b821-76153f3cc9dc\",\"3b80baa7-2cb2-446a-86a9-cddbf0318733\",\"3949c4e6-1fd5-4da6-85ba-e33a3afe9e7a\",\"26261e98-a276-4930-94f5-2d67a3667112\",\"154fd4dd-eb33-499b-9ab3-4b1d8d94b28f\",\"160d2cab-4c89-4d42-ad56-52411bb4be93\",\"1e1d88d3-569d-4c93-844e-e9cb45ee9363\",\"140ef147-abf7-49c0-9265-2ec26fdf9025\",\"34e60482-9926-47ea-9356-95130775a08a\",\"27a4b6ff-a508-4087-b117-dd8134743c76\",\"229e3e8b-6233-4ccd-bc31-3bc93faba37a\",\"2d992ff0-a07a-4865-b806-82f4281eb8de\",\"1ccb0b60-7330-46bf-8a8a-d6be5e8cefcf\",\"21ae58a9-b777-4287-b2b4-0a04c40218c0\",\"401e2308-fe9b-4018-a7d6-40b746a5d9d8\",\"39d15016-2ff7-4e57-9459-8139cde06c90\",\"15919783-7455-4947-8476-51e3f63f7f35\",\"380ccd80-0ebd-41f8-88c0-38f9c613f73a\",\"2c8999ed-4568-453a-9721-04e3ce7b3ad1\",\"3f3de775-65b1-440c-b2ca-28aa5ff56199\",\"40d091bc-1fef-4d31-b4ff-4bb7bf75cdc7\",\"1d4e6e99-adeb-43c5-be7b-9b22452eb54f\",\"133de445-ae89-440a-bf9e-d4bbd6fd4db8\",\"1b8a5781-df58-4a80-a183-cb86f1229ece\",\"3d9836f9-784b-4d6c-901c-584de1053506\",\"2ba9885d-4faf-4302-955b-0c3af373dcec\",\"3be24ac1-4660-4995-a4c3-321b42741084\",\"12aa73b1-6a21-4b2e-9b48-53e037a1ac05\",\"221f4c71-5efd-4cd0-a331-2693810f2e5f\",\"41c46784-6cc5-44fb-97c8-86fb8ca3c966\",\"1cbb2131-e3b8-43e9-ad0e-d1c8079eb65a\",\"1397e7e6-f8cb-440e-b80e-26cac114aa79\",\"205835ca-02b0-4a2c-a1da-71aba47e3322\",\"23fdf230-3128-4c95-b23b-6479bf7c6c2f\",\"132a00f4-f8eb-47b7-a5c5-5671ef66e856\",\"3b34bd15-75d5-416c-aad8-ad23278d648e\",\"32b09e11-35bf-49a8-8dd8-6f7ef4999973\",\"1b1510c4-2f1b-41b0-a14d-1aa37637e91a\",\"18de4c52-236a-4c26-a5b3-774a824496fe\",\"1f8ad608-5c10-4583-8410-f3b6dbbe2b0e\",\"146a70a2-1395-4f33-9bf0-cb958c3f9bb9\",\"1e697e07-bd8d-4a83-a9ec-4d10a58b9ce7\",\"26670f08-c8df-45e3-b694-644004b867bf\",\"30b27afb-dc84-407f-9d86-615f02e195fc\",\"2be42619-229b-44ee-9ac1-5a26d135ae83\",\"37718d34-a33a-443a-9497-046e179dd091\",\"15f77001-5c35-4d59-a229-d07cd30c9b78\",\"2da985bf-a0aa-4bdf-ace5-8f71183aa7c0\",\"13501438-8731-428f-8fca-b49281c4ad4d\",\"2b871d75-9190-485b-ad19-e7d0ff8eb216\",\"29c667cb-5319-4f62-b824-6efe692936b9\",\"1ebb5f14-a510-4b2a-b40f-fc6099342be9\",\"1c574950-56c5-49f8-ae48-95108722ff72\",\"291badf2-a468-44bf-bf00-32556d60f75e\",\"2907d595-ada6-40d5-8232-03f87dc4b47f\",\"27666025-a8b9-40a1-ae07-31d6a0430cac\",\"28a811a2-a367-4cb4-90d4-0265c1d8cd9d\",\"133557ff-0c4a-4233-b144-c71b627efb22\",\"2bbd2049-dfe4-4077-9258-9360d55e3b2c\",\"17f3f6db-438f-4eb0-84ee-73d5221705fb\",\"2293f6de-c8cf-4d5b-88fc-111fa0e0a611\",\"35c13559-4cbb-4549-a0cf-c7f5fe3710a1\",\"27e36922-7ff8-45ac-8889-985a86d30bf6\",\"160c2d92-ae9c-410a-9a28-7fda59fc3418\",\"13687358-5e0d-4270-9db2-f927b1993cc3\",\"3b915520-6267-4133-a82f-134fd64037a4\",\"3e508512-9d55-49bf-a3de-fc57ceb67150\",\"2070d7f7-0911-4695-8680-c3b8e924f95a\",\"21e229db-63d8-418c-8ef9-355237c2a0c7\",\"322c225c-40fa-47cc-8de8-146e757729e1\",\"32b4b1d1-209d-4be5-8a12-a44368f6be2c\",\"3e9b9e04-ff57-4040-aec3-aa43929f3a93\",\"190ca46a-7152-4ecc-939b-4f77dbad2572\",\"41d80196-9a8d-4cbb-a690-f420a851d922\",\"2c827966-95b8-463a-9ddc-39dd2cd2a102\",\"1a8d3bc4-0521-4e93-9296-701c1938774d\",\"33a590ef-59bf-4880-baa2-b804aaa05dea\",\"3c22afe5-2329-45d6-ab08-a90e56ae85f0\",\"2f8dfd0f-d261-4207-8942-365094dafeb9\",\"1f16ee60-e3c2-4e35-a557-72f37f41ce87\",\"3b52fbcd-9f6f-459c-885c-0c480086a746\",\"39fe6656-b711-4376-b7a7-7b3f2fb3ab73\",\"2d7e7379-8d7a-43d6-9cb4-169d92641ab3\",\"3d00edab-4f61-4201-8382-6834b08c0546\",\"1b1a3651-179a-4d3f-bf70-02863a31a0cc\",\"140ba9ec-4b75-4499-99fc-6c4f1c8eeda6\",\"2a5e4d1e-3b69-423b-a949-940cb871be11\",\"3586da01-c32b-42b6-828d-5100d6db8569\",\"2f460d8e-12b9-44a7-b155-1a44b88f26db\",\"170c34fe-c4ea-43be-973d-a01617e2812a\",\"1ae00fee-5c7d-42ce-8998-19d88fb30c07\",\"1710a0f9-6e57-4fbc-a323-4e24e47f39a7\",\"34f00b36-e17d-4ba6-b2fb-68f8af048b42\",\"26d0ca2d-23f2-494a-95d6-8630ff65b519\",\"3f24ad38-6f4a-4612-a230-a06e45a87375\",\"39f7e958-8b3c-4feb-8010-8eb999feb073\",\"1eacc24d-a6fd-454a-9d91-2adab32d3e93\",\"2546c2f1-560c-4a95-afcf-b2f3893b4728\"]}";
                YourClassName yourObject = Newtonsoft.Json.JsonConvert.DeserializeObject<YourClassName>(asset_ids);

                //list.Add("");

                AssetPMService assetPMService = new AssetPMService(_mapper);
                var get_assets = _UoW.AssetFormIORepository.GetAllAssetsToAddPMs2(yourObject.ids);

                foreach (var asset in get_assets)
                {

                    try
                    {
                        var assetplans = asset.AssetPMPlans.Where(x => x.status == 1 && x.plan_name == "NFPA-70B").ToList();
                        if (assetplans.Count > 1) //if duplicate
                        {
                            await assetPMService.RemoveAssetPMPlan2_forScript(assetplans.FirstOrDefault().asset_pm_plan_id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("AssetMain-" + asset.asset_id.ToString() + ex.Message);
                    }

                }
                #endregion Add PMs to AssetPMs Table for Assets


            }
            catch (Exception e)
            {
                res = -1;
                _logger.LogError("AssetMain-" + i + e.Message);
            }

            _logger.LogError("Script Completed Successfully");
            return 1;
        }
        public async Task<int> scriptforgehealtcare()
        {
            /*var get_class = _UoW.AssetFormIORepository.GetceccoClasses();

            foreach(var item in get_class)
            {

                var class_ = _mapper.Map<InspectionTemplateAssetClass>(item);
                class_.company_id = Guid.Parse("16e4e286-fe9a-44c6-b5ca-96f0bdf24b76");
                class_.PMCategory.company_id = Guid.Parse("16e4e286-fe9a-44c6-b5ca-96f0bdf24b76");
                var insert = await _UoW.BaseGenericRepository<InspectionTemplateAssetClass>().Insert(class_);
                _UoW.SaveChanges();
            }*/

            //to copy forms uncommment below function after adding class
            //await CopyFormstoBrucefromCeccoScript();

            // to map forms and class uncomment below function after adding forms and class
            //await ScriptForAddAssetClassFormIOMappings();

            //await AddAssetfromSheettoCecco();

            await ScriptToMigrateNewTempMasterLocations();

            return 1;
        }

        public async Task<int> ScriptToMigrateNewTempMasterLocations()
        {
            WorkOrderService workOrderService = new WorkOrderService(_mapper);
            try
            {
                var getsites = _UoW.AssetFormIORepository.GetAllSitesByCompany();

                foreach (var site_id in getsites)
                {
                    try
                    {
                        var get_tempassets = _UoW.AssetFormIORepository.GetAllTempAssetsForScript(site_id);
                        foreach (var tempasset in get_tempassets)
                        {
                            try
                            {
                                var building = tempasset.TempFormIOBuildings.temp_formio_building_name;
                                var floor = tempasset.TempFormIOFloors.temp_formio_floor_name;
                                var room = tempasset.TempFormIORooms.temp_formio_room_name;
                                var section = tempasset.TempFormIOSections!=null?tempasset.TempFormIOSections.temp_formio_section_name:"Default";

                                AddTempMasterLocationDataMainFunctionRequestModel req2 = new AddTempMasterLocationDataMainFunctionRequestModel();
                                req2.temp_building = building;
                                req2.temp_floor = floor;
                                req2.temp_room = room;
                                req2.wo_id = tempasset.wo_id;

                                var res_location = await workOrderService.AddTempMasterLocationDataMainFunction_V2Script(req2,site_id);

                                req2.temp_master_building_id = res_location.temp_master_building_id;
                                req2.temp_master_floor_id = res_location.temp_master_floor_id;
                                req2.temp_master_room_id = res_location.temp_master_room_id;

                                tempasset.temp_master_building_id = res_location.temp_master_building_id;
                                tempasset.temp_master_floor_id = res_location.temp_master_floor_id;
                                tempasset.temp_master_room_id = res_location.temp_master_room_id;
                                tempasset.temp_master_section = !String.IsNullOrEmpty(section) ? section : "Default";
                                tempasset.modified_at = DateTime.UtcNow;

                                var update = await _UoW.BaseGenericRepository<TempAsset>().Update(tempasset);
                                _UoW.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                _logger.LogInformation("================BUG IN TEMPASSET : " + tempasset.tempasset_id + " =======ScriptToMigrateNewTempMasterLocationsV2=========================");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("================BUG IN SITE ID : " + site_id + " =======ScriptToMigrateNewTempMasterLocationsV2=========================");
                    }
                }
            }
            catch (Exception e)
            {
            }
            _logger.LogInformation("=======================ScriptToMigrateNewTempMasterLocationsV2=========================");
            _logger.LogInformation("=======================COMPLETED SUCCESSFULLY=========================");
            return 1;
        }

        public async Task<int> AddAssetfromSheettoCecco()
        {
            //string carpenter_210 = "[{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":2,\"Room\":\"10th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":4,\"Room\":\"10th Floor\",\"Asset Name\":\"MCH-10\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":3,\"Room\":\"10th Floor\",\"Asset Name\":\"ERL -10\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":5,\"Room\":\"10th Floor\",\"Asset Name\":\"PH 10-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":6,\"Room\":\"10th Floor\",\"Asset Name\":\"RL 10-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":7,\"Room\":\"10th Floor\",\"Asset Name\":\"RL 10-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":8,\"Room\":\"10th Floor\",\"Asset Name\":\"RL-10--3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":9,\"Room\":\"10th Floor\",\"Asset Name\":\"RL-10--4\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":10,\"Room\":\"10th Floor\",\"Asset Name\":\"T-10-1\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":11,\"Room\":\"10th Floor\",\"Asset Name\":\"T-10-3\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":13,\"Room\":\"11th Floor\",\"Asset Name\":\"MCL-11\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":12,\"Room\":\"11th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":14,\"Room\":\"11th Floor\",\"Asset Name\":\"T-11\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":17,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1A ( Bottom Left )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":18,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1A ( Bottom Right\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":19,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1A ( Top Left )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":20,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1A ( Top Right )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":21,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1B ( Bottom Left )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":22,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1B ( Bottom Right\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":23,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1B ( Main )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":24,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1B ( Top Left )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":25,\"Room\":\"12th Floor\",\"Asset Name\":\"PH 12-1B ( Top Right )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":16,\"Room\":\"12th Floor\",\"Asset Name\":\"MCL-12\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":15,\"Room\":\"12th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":26,\"Room\":\"12th Floor\",\"Asset Name\":\"RL 12-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":2,\"Room\":\"12th Floor\",\"Asset Name\":\"T-12-1\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"8132f471-5fae-4512-a283-e480ffffab45\",\"row_index\":27,\"Room\":\"12th Floor\",\"Asset Name\":\"T-12\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":3,\"Room\":\"3rd Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"BUSW-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":4,\"Room\":\"3rd Floor\",\"Asset Name\":\"MCL-3\",\"Asset Class Code\":\"BUSW-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":5,\"Room\":\"3rd Floor\",\"Asset Name\":\"RL-3-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":6,\"Room\":\"3rd Floor\",\"Asset Name\":\"RL-3-2-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":7,\"Room\":\"3rd Floor\",\"Asset Name\":\"RL-3-2-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":8,\"Room\":\"3rd Floor\",\"Asset Name\":\"RL-3-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":9,\"Room\":\"3rd Floor\",\"Asset Name\":\"T3\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":11,\"Room\":\"4th Floor\",\"Asset Name\":\"MCL-4\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":10,\"Room\":\"4th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":12,\"Room\":\"4th Floor\",\"Asset Name\":\"RL-4\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":13,\"Room\":\"4th Floor\",\"Asset Name\":\"T-4\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":15,\"Room\":\"5th Floor\",\"Asset Name\":\"MCL 5\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":14,\"Room\":\"5th Floor\",\"Asset Name\":\"Buss Duct Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":16,\"Room\":\"5th Floor\",\"Asset Name\":\"T-5\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":19,\"Room\":\"6th Floor\",\"Asset Name\":\"MCL-6\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":17,\"Room\":\"6th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":18,\"Room\":\"6th Floor\",\"Asset Name\":\"ERL-6\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":20,\"Room\":\"6th Floor\",\"Asset Name\":\"T-6\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":22,\"Room\":\"7th Floor\",\"Asset Name\":\"MCL-7\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":21,\"Room\":\"7th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":23,\"Room\":\"7th Floor\",\"Asset Name\":\"RL-7\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":24,\"Room\":\"7th Floor\",\"Asset Name\":\"T-7\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":26,\"Room\":\"8th Floor\",\"Asset Name\":\"MCL-8\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":25,\"Room\":\"8th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"27ed018c-82d5-4c45-8cda-3d3e7605072e\",\"row_index\":27,\"Room\":\"8th Floor\",\"Asset Name\":\"RL 8-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":2,\"Room\":\"8th Floor\",\"Asset Name\":\"RL 8-1-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":3,\"Room\":\"8th Floor\",\"Asset Name\":\"RL 8-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":4,\"Room\":\"8th Floor\",\"Asset Name\":\"T-8\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":6,\"Room\":\"9th Floor\",\"Asset Name\":\"MCL-9\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":5,\"Room\":\"9th Floor\",\"Asset Name\":\"Buss Switch\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":7,\"Room\":\"9th Floor\",\"Asset Name\":\"RL 9-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":8,\"Room\":\"9th Floor\",\"Asset Name\":\"RL 9-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":9,\"Room\":\"9th Floor\",\"Asset Name\":\"RL 9-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":10,\"Room\":\"9th Floor\",\"Asset Name\":\"RL 9-3A\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":11,\"Room\":\"9th Floor\",\"Asset Name\":\"RL 9-4\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":12,\"Room\":\"9th Floor\",\"Asset Name\":\"T-9\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":13,\"Room\":\"ATS Room\",\"Asset Name\":\"ATS-1\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":14,\"Room\":\"ATS Room\",\"Asset Name\":\"ATS-2\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":15,\"Room\":\"ATS Room\",\"Asset Name\":\"ATS-3\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":16,\"Room\":\"ATS Room\",\"Asset Name\":\"EPH-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":17,\"Room\":\"ATS Room\",\"Asset Name\":\"EPL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":18,\"Room\":\"ATS Room\",\"Asset Name\":\"ERL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":19,\"Room\":\"ATS Room\",\"Asset Name\":\"ET-1-1\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":20,\"Room\":\"ATS Room\",\"Asset Name\":\"ET-1-2\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":21,\"Room\":\"DAS Room\",\"Asset Name\":\"RL 2-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":23,\"Room\":\"Generator Room\",\"Asset Name\":\"CB ATS FPC (E) BUS (\",\"Asset Class Code\":\"BUSW-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":24,\"Room\":\"Generator Room\",\"Asset Name\":\"GDH -1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":22,\"Room\":\"Generator Room\",\"Asset Name\":\"CB ATS FPC (E) BUS (\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":4,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"MCH-1\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":14,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"SSH-1\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":27,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"DL-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":5,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"Meter Bank ( Bottom )\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":6,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"Meter Bank ( Top )\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":7,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"PG-PH\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":8,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"PG-RL1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":10,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"PH 1-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":11,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"RL 1-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":12,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"RL 1-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":13,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"RL 1-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":25,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"DH 1-1A\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"dbc383ce-f5f8-43fb-aae3-fa6137db2040\",\"row_index\":26,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"DH 1-1B\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":2,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"EF1-2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":3,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"EF1-3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":9,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"PG-T1\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":15,\"Room\":\"Main Electrical Rm.\",\"Asset Name\":\"T-1\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":16,\"Room\":\"Parking Garage\",\"Asset Name\":\"Bus Duct Riser Tap Box\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":17,\"Room\":\"SH-1 Distribution\",\"Asset Name\":\"Incoming Utility Buss\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":18,\"Room\":\"SH-1 Distribution\",\"Asset Name\":\"Incoming Utility Buss\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":19,\"Room\":\"SH-1 Distribution\",\"Asset Name\":\"Incoming Utility Buss\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":20,\"Room\":\"SH-1 Distribution\",\"Asset Name\":\"Load Side Buss\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":21,\"Room\":\"SH-1 Distribution\",\"Asset Name\":\"SH-1 DIST ( Bottom )\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":22,\"Room\":\"SH-1 Distribution\",\"Asset Name\":\"SH-1 DIST Breakers (\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":23,\"Room\":\"Water Heater Room\",\"Asset Name\":\"CU-1\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"},{\"table_id\":\"30549342-5951-4225-9fb9-22dc0d94216e\",\"row_index\":24,\"Room\":\"Water Heater Room\",\"Asset Name\":\"CU-1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"High\",\"address_1\":\"210 N Carpenter\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"210 N Carpenter\",\"inspection_date\":\"2/28/19\"}]";
            //string W_Fulton = "[{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":10,\"Room\":\"1st Floor\",\"Asset Name\":\"DS-AHU-1\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"ABB\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":3,\"Room\":\"1st Floor\",\"Asset Name\":\"1-HDP-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":5,\"Room\":\"1st Floor\",\"Asset Name\":\"1-LDP-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":2,\"Room\":\"1st Floor\",\"Asset Name\":\"1-EM-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":4,\"Room\":\"1st Floor\",\"Asset Name\":\"1-HP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":6,\"Room\":\"1st Floor\",\"Asset Name\":\"1-LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":7,\"Room\":\"1st Floor\",\"Asset Name\":\"1-LP-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":8,\"Room\":\"1st Floor\",\"Asset Name\":\"1-LP-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":9,\"Room\":\"1st Floor\",\"Asset Name\":\"1-LP-4\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":14,\"Room\":\"2nd Floor\",\"Asset Name\":\"DS-AHU-2\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"ABB\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":12,\"Room\":\"2nd Floor\",\"Asset Name\":\"BP-AHU-2\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":13,\"Room\":\"2nd Floor\",\"Asset Name\":\"BP-T-MC-2\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":15,\"Room\":\"2nd Floor\",\"Asset Name\":\"MC-2\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":11,\"Room\":\"2nd Floor\",\"Asset Name\":\"2-EL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":18,\"Room\":\"3rd Floor\",\"Asset Name\":\"DS-AHU-3\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"ABB\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":16,\"Room\":\"3rd Floor\",\"Asset Name\":\"BP-AHU-3\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":17,\"Room\":\"3rd Floor\",\"Asset Name\":\"BP-T-MC-3\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":19,\"Room\":\"3rd Floor\",\"Asset Name\":\"MC-3\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":23,\"Room\":\"4th Floor\",\"Asset Name\":\"DS-AHU-4\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"ABB\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":21,\"Room\":\"4th Floor\",\"Asset Name\":\"BP-AHU-4\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":22,\"Room\":\"4th Floor\",\"Asset Name\":\"BP-T-MC-4\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":24,\"Room\":\"4th Floor\",\"Asset Name\":\"MC-4\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":20,\"Room\":\"4th Floor\",\"Asset Name\":\"4-LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":2,\"Room\":\"5th Floor\",\"Asset Name\":\"DS-AHU-5\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"ABB\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":3,\"Room\":\"5th Floor\",\"Asset Name\":\"MC-5\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":26,\"Room\":\"5th Floor\",\"Asset Name\":\"BP-AHU-5\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":27,\"Room\":\"5th Floor\",\"Asset Name\":\"BP-T-MC-5\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"cefc1159-3644-4a7c-8a2f-1f6d4b94ee44\",\"row_index\":25,\"Room\":\"5th Floor\",\"Asset Name\":\"5-EL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":7,\"Room\":\"6th Floor\",\"Asset Name\":\"MC-6\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":6,\"Room\":\"6th Floor\",\"Asset Name\":\"AHU-6\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":4,\"Room\":\"6th Floor\",\"Asset Name\":\"BP-AHU-6\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":5,\"Room\":\"6th Floor\",\"Asset Name\":\"BP-T-MC-6\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":9,\"Room\":\"7th Floor\",\"Asset Name\":\"BP-AHU-7\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":10,\"Room\":\"7th Floor\",\"Asset Name\":\"BP-T-MC-7\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":11,\"Room\":\"7th Floor\",\"Asset Name\":\"MC-7\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":8,\"Room\":\"7th Floor\",\"Asset Name\":\"7-LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":13,\"Room\":\"8th Floor\",\"Asset Name\":\"BP-AHU-8\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":14,\"Room\":\"8th Floor\",\"Asset Name\":\"BP-HDP-1\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":15,\"Room\":\"8th Floor\",\"Asset Name\":\"BP-T-MC-8\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":16,\"Room\":\"8th Floor\",\"Asset Name\":\"CT-MC-8\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":12,\"Room\":\"8th Floor\",\"Asset Name\":\"8-EL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":18,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH-HDP-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":17,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH EHDP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":19,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH-HP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":20,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH-HP-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":21,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH-HP-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":22,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH-LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":23,\"Room\":\"Boiler Room\",\"Asset Name\":\"PH-LP-1A\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":24,\"Room\":\"Boiler Room\",\"Asset Name\":\"VFD-BP-9.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":25,\"Room\":\"Boiler Room\",\"Asset Name\":\"VFD-BP-9.2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":26,\"Room\":\"Boiler Room\",\"Asset Name\":\"VFD-BP-9.3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"2207e7b3-e446-4869-92d1-558bd3415a9c\",\"row_index\":27,\"Room\":\"Boiler Room\",\"Asset Name\":\"VFD-BP-9.4\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":2,\"Room\":\"Boiler Room\",\"Asset Name\":\"VFD-HWP-9.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":3,\"Room\":\"Boiler Room\",\"Asset Name\":\"VFD-HWP-9.2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":4,\"Room\":\"Dock Security Office\",\"Asset Name\":\"LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":5,\"Room\":\"Dock Security Office\",\"Asset Name\":\"P1-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":6,\"Room\":\"Dock Security Office\",\"Asset Name\":\"P1-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":7,\"Room\":\"Electrical Room\",\"Asset Name\":\"ATS -1\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Caterpillar\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":8,\"Room\":\"Electrical Room\",\"Asset Name\":\"ATS-2\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Caterpillar\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":9,\"Room\":\"Electrical Room\",\"Asset Name\":\"ATS-3\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Caterpillar\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":10,\"Room\":\"Electrical Room\",\"Asset Name\":\"ATS-4\",\"Asset Class Code\":\"ATSW-LV\",\"manufacturer\":\"Caterpillar\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":11,\"Room\":\"Electrical Room\",\"Asset Name\":\"Disconnect T-SB-ELDP-1Disconnect\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":12,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-EHDP-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":15,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-GEN-DP\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":16,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-HDP-2\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":17,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-OPT-DP-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":13,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-ELDP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":14,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-ESLP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":18,\"Room\":\"Electrical Room\",\"Asset Name\":\"SB-OPT-LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":22,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"MDP-1 (Left)\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":23,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"MDP-1 (Main)\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":24,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"MDP-1 (Right)\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":26,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"SB HDP-1\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":27,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"SWBD-1 (Sec. 3)\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":2,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"SWBD-1 Main\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":3,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"SWBD-2 (Left)\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":4,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"SWBD-2 Main\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":19,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"Buss Duct 1 Breaker\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":20,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"Buss Duct 2 Breaker\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":21,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"LB-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"7b7dd516-3301-4813-a0dd-b52dbcfa9a24\",\"row_index\":25,\"Room\":\"Electrical Switchgear\",\"Asset Name\":\"PB-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":5,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"DS ECP-A\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":6,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"DS ECP-B\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":7,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"DS ECP-C\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":8,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"DS ECP-D\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":9,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"DS ECP-E\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":10,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"ECP-A\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Other\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":11,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"ECP-B\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Other\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":12,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"ECP-C\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Other\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":13,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"ECP-D\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Other\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":14,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"ECP-E\",\"Asset Class Code\":\"Other\",\"manufacturer\":\"Other\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":15,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"PH ELEVDP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":16,\"Room\":\"Elevator Control Room\",\"Asset Name\":\"PH ELEV-ELP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":18,\"Room\":\"Garage 2nd Floor\",\"Asset Name\":\"T-LP-PARK Disconnect\",\"Asset Class Code\":\"DISC-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":17,\"Room\":\"Garage 2nd Floor\",\"Asset Name\":\"LP Park\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":19,\"Room\":\"Garage 3rd Floor\",\"Asset Name\":\"3-EL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":20,\"Room\":\"Garage 3rd Floor\",\"Asset Name\":\"L3-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":21,\"Room\":\"Garage 3rd Floor\",\"Asset Name\":\"P3-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":22,\"Room\":\"Garage 6th Floor\",\"Asset Name\":\"6-EL-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":23,\"Room\":\"Garage 6th Floor\",\"Asset Name\":\"L6-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":24,\"Room\":\"Garage 6th Floor\",\"Asset Name\":\"P6-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":25,\"Room\":\"Garage 7th Floor\",\"Asset Name\":\"7 ELEVLP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":26,\"Room\":\"Garage 7th Floor\",\"Asset Name\":\"7-ELEVDP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":2,\"Room\":\"Garage 8th Floor\",\"Asset Name\":\"DS ECP-G2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":3,\"Room\":\"Garage 8th Floor\",\"Asset Name\":\"DS-ECP-G3\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"f2ab53c0-1fe6-4493-8411-a2c28b9a1bbe\",\"row_index\":27,\"Room\":\"Garage 8th Floor\",\"Asset Name\":\"DS ECP-G1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":4,\"Room\":\"Garage 8th Floor\",\"Asset Name\":\"ECP-G1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Mitsu Seiki\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":5,\"Room\":\"Garage 8th Floor\",\"Asset Name\":\"ECP-G2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Mitsu Seiki\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":6,\"Room\":\"Garage 8th Floor\",\"Asset Name\":\"ECP-G3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Mitsu Seiki\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":7,\"Room\":\"Garage Office\",\"Asset Name\":\"1-OPT-LP-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":8,\"Room\":\"Garage Office\",\"Asset Name\":\"L1-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":9,\"Room\":\"Garage Office\",\"Asset Name\":\"P1-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":10,\"Room\":\"Mechanical Fan Room\",\"Asset Name\":\"VFD-EF-10.3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":11,\"Room\":\"Mechanical Fan Room\",\"Asset Name\":\"VFD-GEF-10.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":12,\"Room\":\"Mechanical Fan Room\",\"Asset Name\":\"VFD-TEF-10.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":13,\"Room\":\"Mechanical Rm.\",\"Asset Name\":\"PB-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":14,\"Room\":\"Mechanical Rm.\",\"Asset Name\":\"PB-3\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":15,\"Room\":\"Netpop Rm.\",\"Asset Name\":\"1-DASV-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":16,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"EF 10.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"General Electric\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"3a7ef10a-84f2-4d6e-9f89-e82521480790\",\"row_index\":2,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFFD-CTFAN-9.1.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":17,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"EHU-9.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"ABB\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":18,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CTFAN-9.1.2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":19,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CTFAN-9.1.3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":20,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CTFAN-9.1.4\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":21,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CTWP-9.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":22,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CTWP-9.2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":23,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CTWP-9.3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":24,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CWP-9.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":25,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CWP-9.2\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":26,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-CWP-9.3\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"},{\"table_id\":\"df1acd68-848e-4964-b780-45f4affbce85\",\"row_index\":27,\"Room\":\"Roof Mechanical Room\",\"Asset Name\":\"VFD-DWE-9.1\",\"Asset Class Code\":\"MCEQ-LV\",\"manufacturer\":\"Danfoss\",\"Criticality\":\"High\",\"address_1\":\"1330 W Fulton / 323 N Ada\",\"address_2\":\"Chicago IL 60607 US\",\"client_company\":\"1330 W Fulton / 323 N Ada\",\"inspection_date\":\"5/31/19\"}]";
            string Edgewater_Plaza = "[{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":3,\"Room\":\"BASEMENT\",\"Asset Name\":\"COM ED\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"ITE\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":3,\"Room\":\"BASEMENT\",\"Asset Name\":\"COM ED\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"ITE\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":5,\"Room\":\"BASEMENT\",\"Asset Name\":\"LB EM.1 PANEL\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Refer to Notes\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":6,\"Room\":\"BASEMENT\",\"Asset Name\":\"LB-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":7,\"Room\":\"BASEMENT\",\"Asset Name\":\"LB-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":5,\"Room\":\"BASEMENT\",\"Asset Name\":\"LB EM.1 PANEL\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Refer to Notes\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":6,\"Room\":\"BASEMENT\",\"Asset Name\":\"LB-1\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":7,\"Room\":\"BASEMENT\",\"Asset Name\":\"LB-2\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":2,\"Room\":\"BASEMENT\",\"Asset Name\":\"COM ED\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"McGraw Edison\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":4,\"Room\":\"BASEMENT\",\"Asset Name\":\"EM TRANSFORMER\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":2,\"Room\":\"BASEMENT\",\"Asset Name\":\"COM ED\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"McGraw Edison\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":4,\"Room\":\"BASEMENT\",\"Asset Name\":\"EM TRANSFORMER\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":8,\"Room\":\"CHILLER\",\"Asset Name\":\"MAIN FEED 1200 AMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Refer to Notes\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":8,\"Room\":\"CHILLER\",\"Asset Name\":\"MAIN FEED 1200 AMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Refer to Notes\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":9,\"Room\":\"CHILLER\",\"Asset Name\":\"MAIN LUGS\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":9,\"Room\":\"CHILLER\",\"Asset Name\":\"MAIN LUGS\",\"Asset Class Code\":\"DPNL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":12,\"Room\":\"CHILLER\",\"Asset Name\":\"UNMARKED BUCKET\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":12,\"Room\":\"CHILLER\",\"Asset Name\":\"UNMARKED BUCKET\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":10,\"Room\":\"CHILLER\",\"Asset Name\":\"NORTH CHILLER\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":11,\"Room\":\"CHILLER\",\"Asset Name\":\"SOUTH CHILLER\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":10,\"Room\":\"CHILLER\",\"Asset Name\":\"NORTH CHILLER\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":11,\"Room\":\"CHILLER\",\"Asset Name\":\"SOUTH CHILLER\",\"Asset Class Code\":\"MCCB-S-LV\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":13,\"Room\":\"COM ED\",\"Asset Name\":\"COMPACTOR NORTH\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":14,\"Room\":\"COM ED\",\"Asset Name\":\"COMPACTOR SOUTH\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":15,\"Room\":\"COM ED\",\"Asset Name\":\"GARAGE LIGHTS\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":16,\"Room\":\"COM ED\",\"Asset Name\":\"L1-1 & L1-2 1ST FL.\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":17,\"Room\":\"COM ED\",\"Asset Name\":\"L11-1 11TH FLR. N.\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":18,\"Room\":\"COM ED\",\"Asset Name\":\"L2-2 2ND FL. LAUNDRY\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":19,\"Room\":\"COM ED\",\"Asset Name\":\"L3-1 3RD FL. S. CHUTE\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":20,\"Room\":\"COM ED\",\"Asset Name\":\"LB-1 & LB-2 BASEMENT\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":13,\"Room\":\"COM ED\",\"Asset Name\":\"COMPACTOR NORTH\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":14,\"Room\":\"COM ED\",\"Asset Name\":\"COMPACTOR SOUTH\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":15,\"Room\":\"COM ED\",\"Asset Name\":\"GARAGE LIGHTS\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":16,\"Room\":\"COM ED\",\"Asset Name\":\"L1-1 & L1-2 1ST FL.\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":17,\"Room\":\"COM ED\",\"Asset Name\":\"L11-1 11TH FLR. N.\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":18,\"Room\":\"COM ED\",\"Asset Name\":\"L2-2 2ND FL. LAUNDRY\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":19,\"Room\":\"COM ED\",\"Asset Name\":\"L3-1 3RD FL. S. CHUTE\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":20,\"Room\":\"COM ED\",\"Asset Name\":\"LB-1 & LB-2 BASEMENT\",\"Asset Class Code\":\"ENCL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":21,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"AIR COMPRESSOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":22,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"BASEMENT EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":23,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"BOILER #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":24,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"BOILER RM EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":25,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"CIRC. PUMP #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":26,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"COND. PUMP #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"35fc1f33-fd91-4410-937b-60467c8661b0\",\"row_index\":27,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"COND. PUMP #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":21,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"AIR COMPRESSOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":22,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"BASEMENT EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":23,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"BOILER #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":24,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"BOILER RM EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":25,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"CIRC. PUMP #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":26,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"COND. PUMP #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"a21fdc9f-7270-4978-aaf6-e933f6cd7134\",\"row_index\":27,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"COND. PUMP #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":2,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"COOLING TOWER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":3,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"EAST BOILER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":4,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"GARBAGE HOIST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":5,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"HOUSE PUMP #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":6,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"HOUSE PUMP #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":7,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"HOUSE PUMP #3\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":8,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"JOCKEY PUMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":10,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"P.7\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":11,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"P.8\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":12,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"P.9\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":13,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"PP2- 2ND FLOOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":14,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"PPG-LOCKER RM.\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":15,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"SEWAGE EJECTOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":16,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"SEWAGE EJECTOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":17,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"SOUTH FAN\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":18,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"STAND BY PUMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":19,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"UNIT #7\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":20,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"UNIT HEATER-BOILER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":2,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"COOLING TOWER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":3,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"EAST BOILER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":4,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"GARBAGE HOIST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":5,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"HOUSE PUMP #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":6,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"HOUSE PUMP #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":7,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"HOUSE PUMP #3\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":8,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"JOCKEY PUMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":10,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"P.7\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":11,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"P.8\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":12,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"P.9\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":13,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"PP2- 2ND FLOOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":14,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"PPG-LOCKER RM.\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":15,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"SEWAGE EJECTOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":16,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"SEWAGE EJECTOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":17,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"SOUTH FAN\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":18,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"STAND BY PUMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":19,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"UNIT #7\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":20,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"UNIT HEATER-BOILER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":9,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"MAIN LUGS\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":9,\"Room\":\"MAIN BUCKET\",\"Asset Name\":\"MAIN LUGS\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":21,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"E.M. TRANSFORMER\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":22,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"HOUSE T- FORMER\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":23,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"MAIN LUG FEED\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":24,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"NORTH ELECTRIC RM.\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":25,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"P.P.B. BASEMENT\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":26,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"PENTHOUSE\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"bb6a70ef-4f91-4ce1-8edb-80a74250b32b\",\"row_index\":27,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"SOUTH ELECTRIC RM.\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":21,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"E.M. TRANSFORMER\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":22,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"HOUSE T- FORMER\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":23,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"MAIN LUG FEED\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":24,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"NORTH ELECTRIC RM.\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":25,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"P.P.B. BASEMENT\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":26,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"PENTHOUSE\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"d02d11cb-128f-4c20-aa45-1022224e8c60\",\"row_index\":27,\"Room\":\"MAIN SWITCH GEAR\",\"Asset Name\":\"SOUTH ELECTRIC RM.\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":2,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"BATH EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":3,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"CIRC PUMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":4,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":5,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":6,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #3\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":7,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #4\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":8,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #5\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":9,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR LIGHTS\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":10,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"KITCHEN EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":12,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"NORTH-2E FAN\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":13,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"PASSENGER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":14,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"S-5 CORR AIR SUPPLY\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":15,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"SERVICE ELEVATOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":16,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"SOUTH- 3E FAN\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":17,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"TRANSFORMER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":2,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"BATH EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":3,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"CIRC PUMP\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":4,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #1\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":5,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #2\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":6,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #3\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":7,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #4\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":8,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR #5\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":9,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"ELEVATOR LIGHTS\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":10,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"KITCHEN EXHAUST\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":12,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"NORTH-2E FAN\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":13,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"PASSENGER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":14,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"S-5 CORR AIR SUPPLY\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":15,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"SERVICE ELEVATOR\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":16,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"SOUTH- 3E FAN\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":17,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"TRANSFORMER\",\"Asset Class Code\":\"DISC-F-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":11,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"MAIN LUGS\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":11,\"Room\":\"POWER PANEL PP.PH\",\"Asset Name\":\"MAIN LUGS\",\"Asset Class Code\":\"SWBD\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":18,\"Room\":\"ROOF TOP\",\"Asset Name\":\"ELEVATOR PANEL\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Challenger\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":20,\"Room\":\"ROOF TOP\",\"Asset Name\":\"PANEL L.PH\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":22,\"Room\":\"ROOF TOP\",\"Asset Name\":\"PENTHOUSE A/C\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":18,\"Room\":\"ROOF TOP\",\"Asset Name\":\"ELEVATOR PANEL\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Challenger\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":20,\"Room\":\"ROOF TOP\",\"Asset Name\":\"PANEL L.PH\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":22,\"Room\":\"ROOF TOP\",\"Asset Name\":\"PENTHOUSE A/C\",\"Asset Class Code\":\"PANL\",\"manufacturer\":\"Siemens\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":19,\"Room\":\"ROOF TOP\",\"Asset Name\":\"ELEVATOR\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Challenger\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"247d11d9-d274-43cd-be4f-24f750082a86\",\"row_index\":21,\"Room\":\"ROOF TOP\",\"Asset Name\":\"PANEL L.PH\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":19,\"Room\":\"ROOF TOP\",\"Asset Name\":\"ELEVATOR\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Challenger\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"},{\"table_id\":\"2b984d71-6c84-416d-bd6d-f6d0e243a05a\",\"row_index\":21,\"Room\":\"ROOF TOP\",\"Asset Name\":\"PANEL L.PH\",\"Asset Class Code\":\"DTTR-LV\",\"manufacturer\":\"Square D\",\"Criticality\":\"Medium\",\"address_1\":\"5445 Edgewater Plaza\",\"address_2\":\"Chicago IL 60640 US\",\"client_company\":\"5445 Edgewater Plaza\",\"inspection_date\":\"2/23/22\"}]";

            alljson alljsondata = new alljson();
            alljsondata.json_data = new List<string>();


            foreach (var json_ in alljsondata.json_data)
            {
                var myDeserializedClass = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Root1>>(json_);

                // add site and CC 
                var first_record = myDeserializedClass.FirstOrDefault();
                var client_compnay = first_record.client_company;
                var site = first_record.address_1;
                var address = first_record.address_1 + " " + first_record.address_2;

                // check if CC is exist or not in db
                var clientCompany = _UoW.AssetFormIORepository.GetCCbyname(client_compnay);
                if (clientCompany == null)
                {
                    clientCompany = new ClientCompany();
                    clientCompany.client_company_name = client_compnay;
                    clientCompany.clientcompany_code = client_compnay;
                    clientCompany.owner = client_compnay;
                    clientCompany.owner_address = address;
                    clientCompany.status = 1;
                    clientCompany.created_at = DateTime.UtcNow;
                    clientCompany.parent_company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");
                    var insert = await _UoW.BaseGenericRepository<ClientCompany>().Insert(clientCompany);
                    _UoW.SaveChanges();
                }


                // check site
                var sites = _UoW.AssetFormIORepository.Getsitebyname(site, clientCompany.client_company_id);
                if (sites == null)
                {
                    sites = new Sites();
                    sites.site_name = site;
                    sites.site_code = site;
                    sites.isAddAssetClassEnabled = true;
                    sites.customer = site;
                    sites.customer_address = address;
                    sites.status = 1;
                    sites.timezone = "America/Los_Angeles";
                    sites.company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");
                    sites.client_company_id = clientCompany.client_company_id;
                    sites.created_at = DateTime.UtcNow;

                    var insert = await _UoW.BaseGenericRepository<Sites>().Insert(sites);
                    _UoW.SaveChanges();

                    var all_company_admin = _UoW.UserRepository.GetAllCompanyAdmins(Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d"));
                    foreach (var admin in all_company_admin)
                    {
                        // Add Site Access to all company admin user by adding it to UserSites Table
                        UserSites userSites = new UserSites();

                        userSites.user_id = admin.uuid;
                        userSites.company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");
                        userSites.site_id = sites.site_id;
                        userSites.status = (int)Status.Active;
                        userSites.created_at = DateTime.UtcNow;

                        var insert_user_site = await _UoW.BaseGenericRepository<UserSites>().Insert(userSites);
                        _UoW.SaveChanges();
                    }
                }

                Guid site_id = sites.site_id;

                // get building and floor 
                string building = "Default";
                string floor = "Default";
                string section = "Default";

                var FormIOBuildings = _UoW.AssetFormIORepository.GetFormIOBuildingByName(building, site_id);
                if (FormIOBuildings == null)
                {
                    FormIOBuildings = new FormIOBuildings();
                    FormIOBuildings.formio_building_name = building;
                    FormIOBuildings.created_at = DateTime.UtcNow;
                    FormIOBuildings.site_id = site_id;
                    FormIOBuildings.company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");

                    var insertbuilding = await _UoW.BaseGenericRepository<FormIOBuildings>().Insert(FormIOBuildings);
                    _UoW.SaveChanges();
                }
                var FormIOFloors = _UoW.AssetFormIORepository.GetFormIOFloorByName(floor, FormIOBuildings.formiobuilding_id, site_id);
                if (FormIOFloors == null)
                {
                    FormIOFloors = new FormIOFloors();
                    FormIOFloors.formio_floor_name = floor;
                    FormIOFloors.formiobuilding_id = FormIOBuildings.formiobuilding_id;
                    FormIOFloors.created_at = DateTime.UtcNow;
                    FormIOFloors.site_id = site_id;
                    FormIOFloors.company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");

                    var insertfloor = await _UoW.BaseGenericRepository<FormIOFloors>().Insert(FormIOFloors);
                    _UoW.SaveChanges();
                }

                foreach (var item in myDeserializedClass)
                {
                    try
                    {
                        Asset assset = new Asset();
                        assset.name = item.AssetName;

                        // check class code 
                        var get_class = _UoW.AssetFormIORepository.GetassetclassbyClassCode(item.AssetClassCode);
                        assset.name = item.AssetName;

                        if (get_class != Guid.Empty)
                        {
                            assset.inspectiontemplate_asset_class_id = get_class;
                        }


                        // check criticality
                        if (!String.IsNullOrEmpty(item.Criticality))
                        {
                            if (item.Criticality.ToLower().Trim() == "high")
                                assset.criticality_index_type = 3;
                            if (item.Criticality.ToLower().Trim() == "medium")
                                assset.criticality_index_type = 2;
                            if (item.Criticality.ToLower().Trim() == "low")
                                assset.criticality_index_type = 1;
                        }

                        nameplate nameplate = new nameplate();
                        nameplate.manufacturer = item.manufacturer;
                        assset.form_retrived_nameplate_info = Newtonsoft.Json.JsonConvert.SerializeObject(nameplate);

                        // check location details 
                        var FormIORooms = _UoW.AssetFormIORepository.GetFormIORoomByName(item.Room, FormIOFloors.formiofloor_id, site_id);
                        if (FormIORooms == null)
                        {
                            FormIORooms = new FormIORooms();
                            FormIORooms.formio_room_name = item.Room;
                            FormIORooms.formiofloor_id = FormIOFloors.formiofloor_id;
                            FormIORooms.created_at = DateTime.UtcNow;
                            FormIORooms.site_id = site_id;
                            FormIORooms.company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");

                            var insertroom = await _UoW.BaseGenericRepository<FormIORooms>().Insert(FormIORooms);
                            _UoW.SaveChanges();
                        }

                        // check for section 
                        var FormIOSections = _UoW.AssetFormIORepository.GetFormIOSectionByName(section, FormIORooms.formioroom_id, site_id);
                        if (FormIOSections == null)
                        {
                            FormIOSections = new FormIOSections();
                            FormIOSections.formio_section_name = section;
                            FormIOSections.formioroom_id = FormIORooms.formioroom_id;
                            FormIOSections.created_at = DateTime.UtcNow;
                            FormIOSections.site_id = site_id;
                            FormIOSections.company_id = Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d");

                            var insertroom = await _UoW.BaseGenericRepository<FormIOSections>().Insert(FormIOSections);
                            _UoW.SaveChanges();
                        }
                        assset.AssetFormIOBuildingMappings = new AssetFormIOBuildingMappings();
                        assset.AssetFormIOBuildingMappings.formiobuilding_id = FormIOBuildings.formiobuilding_id;
                        assset.AssetFormIOBuildingMappings.formiofloor_id = FormIOFloors.formiofloor_id;
                        assset.AssetFormIOBuildingMappings.formioroom_id = FormIORooms.formioroom_id;
                        assset.AssetFormIOBuildingMappings.formiosection_id = FormIOSections.formiosection_id;


                        assset.site_id = site_id;
                        assset.company_id = "47aa8c4d-684a-4da2-9aad-cfbb851d3f6d";
                        assset.status = 3;
                        assset.component_level_type_id = 1;
                        assset.asset_operating_condition_state = 1;
                        int asset_count = _UoW.WorkOrderRepository.GetAssetscountBySite(site_id.ToString());
                        assset.internal_asset_id = site + (asset_count + 1).ToString();
                        assset.QR_code = site_id + (asset_count + 1).ToString();
                        assset.code_compliance = 1;
                        assset.created_at = DateTime.UtcNow;
                        var insert_ = await _UoW.BaseGenericRepository<Asset>().Insert(assset);
                        _UoW.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("asset scirpt issue with asset " + item.AssetName + item.client_company, item.Room);
                    }

                }
            }

            return 1;
        }

        public class alljson
        {
            public List<string> json_data { get; set; }
        }

        public class nameplate
        {
            public string manufacturer { get; set; }
        }
        public class Root1
        {
            public string table_id { get; set; }
            public int row_index { get; set; }
            public string Room { get; set; }

            [Newtonsoft.Json.JsonProperty("Asset Name")]
            public string AssetName { get; set; }

            [Newtonsoft.Json.JsonProperty("Asset Class Code")]
            public string AssetClassCode { get; set; }
            public string manufacturer { get; set; }
            public string Criticality { get; set; }
            public string address_1 { get; set; }
            public string address_2 { get; set; }
            public string client_company { get; set; }
            public string inspection_date { get; set; }
        }
        public class YourClassName
        {
            public List<string> ids { get; set; }
        }

        /*
        //public static List<AssetFormIOResponseModel> Mapping(List<AssetFormIO> assetFormIOs)
        //{
        //    var result = new List<AssetFormIOResponseModel>();

        //    foreach (var item in assetFormIOs)
        //    {
        //        var newItem = Mapping(item);
        //        result.Add(newItem);
        //    }

        //    return result;
        //}

        //public static AssetFormIOResponseModel Mapping(AssetFormIO assetFormIO)
        //{
        //    return new AssetFormIOResponseModel()
        //    {
        //        asset_form_id = assetFormIO.asset_form_id,
        //        asset_id = assetFormIO.asset_id.Value,
        //        form_id = assetFormIO.form_id.Value,
        //        asset_form_name = assetFormIO.asset_form_name,
        //        asset_form_type = assetFormIO.asset_form_type,
        //        asset_form_description = assetFormIO.asset_form_description,
        //        asset_form_data = assetFormIO.asset_form_data,
        //        requested_by = assetFormIO.requested_by.ToString(),
        //        status = assetFormIO.status,
        //        //status_name = 
        //        //timezone = 
        //        created_at = assetFormIO.created_at,
        //        created_by = assetFormIO.created_by,
        //        modified_at = assetFormIO.modified_at,
        //        //asset_name = 
        //        modified_by = assetFormIO.modified_by,
        //        accepted_by = assetFormIO.accepted_by.ToString(),
        //        inspected_at = assetFormIO.inspected_at,
        //        accepted_at = assetFormIO.accepted_at,
        //        //pdf_report_status_id = 
        //        //pdf_report_status_name = 
        //        pdf_report_url = assetFormIO.pdf_report_url,
        //        //wo_number = 
        //        //manual_wo_number = 
        //        WOcategorytoTaskMapping_id = assetFormIO.WOcategorytoTaskMapping_id.Value,
        //        inspection_verdict = assetFormIO.inspection_verdict,
        //        //inspection_verdict_name = assetFormIO.inspection_verdict
        //        form_retrived_workOrderType = assetFormIO.form_retrived_workOrderType,
        //        //work_procedure =
        //        //form_category_name = 
        //        intial_form_filled_date = assetFormIO.intial_form_filled_date,
        //        workOrderStatus = assetFormIO.WorkOrders.status
        //    };
        //}*/

        public class AssetformioNECObject
        {
            public string selectViolation { get; set; }

        }
        public class AssetformioOshaObject
        {
            public string selectViolation { get; set; }

        }
    }
}
