using AutoMapper;
using Jarvis.db.DBResponseModel;
using Jarvis.db.Migrations;
using Jarvis.db.Models;
using Jarvis.db.MongoDB;
using Jarvis.Repo.Concrete;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using MimeKit.Encodings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages;
using Logger = Jarvis.Shared.Utility.Logger;
//using Aspose.Words;

namespace Jarvis.Service.Concrete
{
    public class FormIOService : BaseService, IFormIOService
    {
        public readonly IMapper _mapper;
        private Logger _logger;
        private readonly MongoDBContext _db;
        public FormIOService(IMapper mapper , MongoDBContext db) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<FormIOService>();
            _db = db;
        }

        public ListViewModel<GetAllFormIOFormResponsemodel> GetAllForms(int page_size, int page_index , string search_string)
        {
            ListViewModel<GetAllFormIOFormResponsemodel> response = new ListViewModel<GetAllFormIOFormResponsemodel>();

            var formios = _UoW.formIORepository.GetAllForms(page_size, page_index,search_string);
          /*  var t =  _db.tbl_mongo_form_io.Find(a => a.Id == "624c134df37a0791d1a6e777").SingleOrDefaultAsync().Result;

            tbl_mongo_form_io tbl_mongo_form_io = new tbl_mongo_form_io();
            tbl_mongo_form_io.asset_id = "123";
            tbl_mongo_form_io.asset_form_name = "form_name12";
            tbl_mongo_form_io.asset_form_type = "form_type";
            tbl_mongo_form_io.asset_form_description = "form_description";
            //convert encode string json to json
            string structure = HttpUtility.UrlDecode("{\"display1\":\"form\",\"components\":[{\"label\":\"FirstNameEdited\",\"labelPosition\":\"top\",\"labelWidth\":\"\",\"labelMargin\":\"\",\"placeholder\":\"\",\"description\":\"\",\"tooltip\":\"\",\"prefix\":\"\",\"suffix\":\"\",\"widget\":{\"type\":\"input\"},\"inputMask\":\"\",\"displayMask\":\"\",\"allowMultipleMasks\":false,\"customClass\":\"\",\"tabindex\":\"\",\"autocomplete\":\"\",\"hidden\":false,\"hideLabel\":false,\"showWordCount\":false,\"showCharCount\":false,\"mask\":false,\"autofocus\":false,\"spellcheck\":true,\"disabled\":false,\"tableView\":true,\"modalEdit\":false,\"multiple\":false,\"persistent\":true,\"inputFormat\":\"plain\",\"protected\":false,\"dbIndex\":false,\"case\":\"\",\"truncateMultipleSpaces\":false,\"encrypted\":false,\"redrawOn\":\"\",\"clearOnHide\":true,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"allowCalculateOverride\":false,\"validateOn\":\"change\",\"validate\":{\"required\":false,\"minLength\":\"\",\"maxLength\":\"\",\"minWords\":\"\",\"maxWords\":\"\",\"pattern\":\"\",\"customMessage\":\"\",\"custom\":\"\",\"customPrivate\":false,\"json\":\"\",\"strictDateValidation\":false,\"multiple\":false,\"unique\":false},\"unique\":false,\"errorLabel\":\"\",\"errors\":\"\",\"key\":\"firstName\",\"tags\":[],\"properties\":{},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\",\"json\":\"\"},\"customConditional\":\"\",\"logic\":[],\"attributes\":{},\"overlay\":{\"style\":\"\",\"page\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"type\":\"textfield\",\"input\":true,\"refreshOn\":\"\",\"dataGridLabel\":false,\"addons\":[],\"inputType\":\"text\",\"id\":\"ehmdie\",\"defaultValue\":\"\"},{\"label\":\"Submit\",\"labelWidth\":\"\",\"labelMargin\":\"\",\"action\":\"submit\",\"showValidations\":false,\"theme\":\"primary\",\"size\":\"md\",\"block\":false,\"leftIcon\":\"\",\"rightIcon\":\"\",\"shortcut\":\"\",\"description\":\"\",\"tooltip\":\"\",\"customClass\":\"\",\"tabindex\":\"\",\"disableOnInvalid\":false,\"hidden\":false,\"autofocus\":false,\"disabled\":false,\"tableView\":false,\"modalEdit\":false,\"key\":\"submit\",\"tags\":[],\"properties\":{},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\",\"json\":\"\"},\"customConditional\":\"\",\"logic\":[],\"attributes\":{},\"overlay\":{\"style\":\"\",\"page\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"type\":\"button\",\"input\":true,\"placeholder\":\"\",\"prefix\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"dataGridLabel\":true,\"labelPosition\":\"top\",\"errorLabel\":\"\",\"hideLabel\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"widget\":{\"type\":\"input\"},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"allowMultipleMasks\":false,\"addons\":[],\"id\":\"eqo7drd\"}],\"data\":{\"firstName\":\"Anmoldwdwd\",\"submit\":true,\"data\":{\"firstName\":\"Anmol\",\"submit\":true},\"metadata\":{\"timezone\":\"Asia/Calcutta\",\"offset\":330,\"origin\":\"http://localhost:3005\",\"referrer\":\"http://localhost:3005/assets/details/02e46dd2-0b31-4650-838c-aa1c586782e0\",\"browserName\":\"Netscape\",\"userAgent\":\"Mozilla/5.0(WindowsNT10.0;Win64;x64)AppleWebKit/537.36(KHTML,likeGecko)Chrome/99.0.4844.84Safari/537.36\",\"pathName\":\"/assets/details/02e46dd2-0b31-4650-838c-aa1c586782e0\",\"onLine\":true},\"state\":\"submitted\"}}");
            tbl_mongo_form_io.asset_form_data = BsonSerializer.Deserialize<BsonDocument>(structure);
           // _db.tbl_mongo_form_io.InsertOne(tbl_mongo_form_io);
            // tbl_mongo_form_io tbl_mongo_form_io = new tbl_mongo_form_io();
            // tbl_mongo_form_io.Name = "from code";
            // t.Name = "update-done";
             var res =  _db.tbl_mongo_form_io.ReplaceOneAsync(Builders<tbl_mongo_form_io>.Filter.Eq("asset_id", tbl_mongo_form_io.asset_id), tbl_mongo_form_io).Result;
          */
            response.list = _mapper.Map<List<GetAllFormIOFormResponsemodel>>(formios.Item1);
            response.listsize = formios.Item2;  
            
            return response;
        }
        public async Task<FormIOResponseModel> AddUpdateFormIO(AddFormIORequestModel formRequest)
        {
            FormIOResponseModel formResponse = new FormIOResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            // check for requested work_procedure if it is unique or not
            if (!String.IsNullOrEmpty(formRequest.work_procedure))
            {
                var work_procedure = formRequest.work_procedure.Trim().ToLower();
                var is_work_procedure_unique = _UoW.formIORepository.IsValidWorkProcedure(work_procedure, formRequest.form_id);
                if (!is_work_procedure_unique)
                {
                    formResponse.response_status = (int)ResponseStatusNumber.invalidworkprocedure;
                    return formResponse;
                }
            }
            if (!String.IsNullOrEmpty(formRequest.form_name))
            {
                var form_name = formRequest.form_name.Trim().ToLower();
                var is_work_procedure_unique = _UoW.formIORepository.IsValidFormName(form_name, formRequest.form_id);
                if (!is_work_procedure_unique)
                {
                    formResponse.response_status = (int)ResponseStatusNumber.invalidworkprocedure;
                    return formResponse;
                }
            }
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (formRequest.form_id != null && formRequest.form_id != Guid.Empty)
                    {
                        var formDetails = await _UoW.formIORepository.GetFormIOById(formRequest.form_id);
                        if (formDetails != null)
                        {
                            formDetails.form_name = formRequest.form_name;
                            formDetails.form_type = formRequest.form_type;
                            formDetails.form_data = formRequest.form_data;
                            if (formRequest.status == 0)
                            {
                                formDetails.status = (int)Status.Active;
                            }
                            else
                            {
                                formDetails.status = formRequest.status;
                            }
                            //formDetails.status = (int)Status.Active;
                            formDetails.form_description = formRequest.form_description;
                            //formDetails.asset_id = formRequest.asset_id;
                            formDetails.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                            //formDetails.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                            formDetails.modified_at = DateTime.UtcNow;
                            formDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            formDetails.work_procedure = formRequest.work_procedure;
                            formDetails.form_type_id = formRequest.form_type_id;
                            formDetails.modified_at = DateTime.UtcNow;
                            var dynmamic_fields = JsonSerializer.Serialize(formRequest.dynamic_fields);
                           // var dynmamic__nameplate_fields = JsonSerializer.Serialize(formRequest.dynamic_nameplate_fields);
                            formDetails.dynamic_fields = dynmamic_fields;
                            formDetails.dynamic_nameplate_fields = formRequest.dynamic_nameplate_fields;
                            formDetails.asset_class_form_properties = formRequest.asset_class_form_properties;
                            if(formDetails.Tasks!=null && formDetails.Tasks.Count > 0)
                            {
                                formDetails.Tasks.ForEach(x =>
                                {
                                    x.task_title = formRequest.form_name;
                                    x.description = formRequest.form_description;
                                    x.modified_at = DateTime.UtcNow;
                                });
                            }
                            result = _UoW.formIORepository.Update(formDetails).Result;
                            if (result > 0)
                            {
                                
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                formResponse = _mapper.Map<FormIOResponseModel>(formDetails);
                                formResponse.response_status = result;
                            }
                        }
                    }
                    else
                    {
                        var addForm = _mapper.Map<InspectionsTemplateFormIO>(formRequest);
                        if (formRequest.asset_id != null && formRequest.asset_id != Guid.Empty)
                        {
                            var assetDetails = _UoW.AssetRepository.GetAssetByAssetID(formRequest.asset_id.ToString());
                           // addForm.site_id = assetDetails.site_id;
                        }
                        else
                        {
                           // addForm.site_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id);
                        }
                        
                        addForm.form_name = formRequest.form_name;
                        addForm.form_type = formRequest.form_type;
                        addForm.form_data = formRequest.form_data;
                       // addForm.form_description = formRequest.form_description;
                        addForm.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                        // addForm.asset_id = formRequest.asset_id;
                        if (formRequest.status == 0)
                        {
                            addForm.status = (int)Status.Active;
                        }
                        else
                        {
                            addForm.status = formRequest.status;
                        }
                        addForm.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        addForm.created_at = DateTime.UtcNow;
                        addForm.work_procedure = formRequest.work_procedure;

                        var dynmamic_fields = JsonSerializer.Serialize(formRequest.dynamic_fields);
                       // var dynmamic__nameplate_fields = JsonSerializer.Serialize(formRequest.dynamic_nameplate_fields);
                        addForm.dynamic_fields = dynmamic_fields;
                        addForm.dynamic_nameplate_fields = formRequest.dynamic_nameplate_fields;
                        addForm.asset_class_form_properties = formRequest.asset_class_form_properties;
                        result = await _UoW.formIORepository.Insert(addForm);
                        if (result > 0)
                        {

                            // create Task with this form
                            Tasks task = new Tasks();
                            task.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                            task.description = addForm.form_description;
                            task.task_title = addForm.form_name;
                            task.form_id = addForm.form_id;
                            task.created_at = DateTime.UtcNow;
                            task.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();

                            var insert_task = await _UoW.BaseGenericRepository<Tasks>().Insert(task);
                            if (insert_task)
                            {
                                _UoW.SaveChanges();
                                _dbtransaction.Commit();
                                formResponse = _mapper.Map<FormIOResponseModel>(addForm);
                                formResponse.response_status = result;
                            }
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

        public FormIOPIChartCountResponseModel DashboardPIchartcount()
        {
            FormIOPIChartCountResponseModel response = new FormIOPIChartCountResponseModel();

            var assets = _UoW.formIORepository.GetAssetsByCompanyID(UpdatedGenericRequestmodel.CurrentUser.company_id , UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
            if (assets.Count > 0)
            {
                response.total_assets_count = assets.Count();   
                //response.normal_condition_asset_count = assets.Where(x => x.condition_index != null ? x.condition_index.Value > 0.75 : false).Count();
                response.good_condition_asset_count = assets.Where(x => x.condition_index_type != null ? x.condition_index_type.Value == (int)condition_index_type.Good : false).Count();
                response.average_condition_asset_count = assets.Where(x => x.condition_index_type != null ? x.condition_index_type.Value == (int)condition_index_type.Average : false).Count();
                response.poor_condition_asset_count = assets.Where(x => x.condition_index_type != null ? x.condition_index_type.Value == (int)condition_index_type.Poor_Corrosive : false).Count();
                response.poor_dusty_condition_asset_count = assets.Where(x => x.condition_index_type != null ? x.condition_index_type.Value == (int)condition_index_type.Poor_Dusty : false).Count();
                response.zero_value_asset_count = assets.Where(x => x.condition_index_type == null || x.condition_index_type == 0).Count();

                /// new values based on assets operating condition
                /// 
                response.good_condition_asset_count = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Operating_Normally).Count();
                response.Repair_Needed = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Repair_Needed).Count();
                response.Replacement_Needed = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Replacement_Needed).Count();
                response.Repair_Scheduled = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Repair_Scheduled).Count();
                response.Replacement_Scheduled = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Replacement_Scheduled).Count();
                response.Decomissioned = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Decomissioned).Count();
                response.Spare = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Spare).Count();
                response.Repair_Inprogress = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Repair_Inprogress).Count();
                response.Replacement_Inprogress = assets.Where(x => x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Replace_Inprogress).Count();
                response.zero_value_asset_count = assets.Where(x => x.asset_operating_condition_state == null || x.asset_operating_condition_state == 0 || x.asset_operating_condition_state == (int)AssetOperatingConduitionState.Defective).Count();

                // for issue counts
                var asset_issues = _UoW.formIORepository.GetAssetIssuesForDashboardCount();
                response.open_issue_count = asset_issues.Where(x => x.issue_status == (int)Status.open).Count();
                response.inprogress_issue_count = asset_issues.Where(x => x.issue_status == (int)Status.InProgress).Count();
                response.schedule_issue_count = asset_issues.Where(x => x.issue_status == (int)Status.Schedule).Count();
               
            }
            return response;
        }
        public DashboardPropertiescountsResponseModel DashboardPropertiescounts()
        {
            DashboardPropertiescountsResponseModel response = new DashboardPropertiescountsResponseModel();

            _logger.LogInformation("company_id ", UpdatedGenericRequestmodel.CurrentUser.company_id);
            _logger.LogInformation("requested_by ", UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
            _logger.LogInformation("site status ", UpdatedGenericRequestmodel.CurrentUser.site_status.ToString());

            var assets = _UoW.formIORepository.GetAssetsByCompanyID(UpdatedGenericRequestmodel.CurrentUser.company_id, UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());
            var maintainance_request = _UoW.formIORepository.GetMRsByCompanyID(UpdatedGenericRequestmodel.CurrentUser.company_id, UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString());


            

            response.Asset_count = assets.Count();
            response.inspection_ready_for_review_count = _UoW.formIORepository.InspectionRFRCount();
            response.inspection_completed_count = _UoW.formIORepository.InspectionCompletedCount();
            response.annual_maintainance_schedule_count = 0;
            response.test_report_count = 0;
            response.maintainance_request_count = maintainance_request.Count();

            AssetPMService assetPMService = new AssetPMService(_mapper);
            AssetPMCountResponsemodel assetPMCountResponsemodel = new AssetPMCountResponsemodel();
            assetPMCountResponsemodel = assetPMService.AssetPMCount();

            response.open_asset_issue_count = _UoW.formIORepository.OpenAssetIssuesCount();
            response.overdue_asset_pm_count = assetPMCountResponsemodel.overdue_asset_pm_count;

            return response;
        }

        public ListViewModel<FormIOResponseModel> GetAllFormNames(int page_size, int page_index)
        {
            ListViewModel<FormIOResponseModel> response = new ListViewModel<FormIOResponseModel>();

            var formios = _UoW.formIORepository.GetAllFormNames(page_size, page_index);
            response.list = _mapper.Map<List<FormIOResponseModel>>(formios.Item1);
            response.listsize = formios.Item2;
            return response;
        }

        public async Task<ListViewModel<FormTypeResponseModel>> GetAllFormTypes(int pageindex, int pagesize, string searchstring)
        {
         /*  var fileNames = new List<string> { "C:\\Users\\Main\\Downloads\\ketan_HL_provisional_certificate.pdf", "C:\\Users\\Main\\Downloads\\Ketan_Ravat_Rent_Receipt.pdf" };
            var output = new Document();
            // Remove all content from the destination document before appending.
            output.RemoveAllChildren();

            foreach (string fileName in fileNames)
            {
                var input = new Document(fileName);
                // Append the source document to the end of the destination document.
                output.AppendDocument(input, ImportFormatMode.KeepSourceFormatting);
            }

            output.Save("D:\\Egalvanic\\Jarvis\\Jarvis\\Uploads\\Output.pdf");
            */

            ListViewModel<FormTypeResponseModel> typeResponse = new ListViewModel<FormTypeResponseModel>();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var typeDetails = await _UoW.formIORepository.GetAllFormTypes(searchstring);
                    if (typeDetails?.Count > 0)
                    {
                        if (pageindex > 0 && pagesize > 0)
                        {
                            typeResponse.listsize = typeDetails.Count;
                            typeDetails = typeDetails.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                        }
                        typeResponse.list = _mapper.Map<List<FormTypeResponseModel>>(typeDetails);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return typeResponse;
        }
        public async Task<int> DeleteForm(DeleteFormRequestmodel requestmodel)
        {
            var get_form = await _UoW.formIORepository.GetFormIOByIdForDelete(requestmodel.form_id);
            if (get_form != null)
            {
                if ((get_form.Tasks == null || get_form.Tasks.Count == 0) || (get_form.Tasks != null && get_form.Tasks.Count > 0 && get_form.Tasks.All(x => x.WOcategorytoTaskMapping.Count == 0)))
                {
                    get_form.status = (int)Status.Deactive;
                    if (get_form.Tasks != null && get_form.Tasks.Count > 0)
                    {
                        get_form.Tasks.ToList().ForEach(x =>
                        {
                            x.isArchive = true;
                        });
                    }
                    var update_form = await _UoW.BaseGenericRepository<InspectionsTemplateFormIO>().Update(get_form);
                    if (update_form)
                    {
                        return (int)ResponseStatusNumber.Success;
                    }
                }
                else
                {
                    return (int)ResponseStatusNumber.form_is_used;
                }
            }
            else
            {
                return (int)ResponseStatusNumber.NotFound;
            }
            return (int)ResponseStatusNumber.Error;
        }

        public GetFormDataTemplateByFormIdResponsemodel GetFormDataTemplateByFormId(Guid form_id)
        {
            GetFormDataTemplateByFormIdResponsemodel response = new GetFormDataTemplateByFormIdResponsemodel();

            var get_form = _UoW.formIORepository.GetFormDataTemplateByFormId(form_id);
            response.form_id = get_form.form_id;
            response.form_name = get_form.form_name;
            response.form_output_data_template = get_form.form_output_data_template;

            return response;
        }
    }
}
