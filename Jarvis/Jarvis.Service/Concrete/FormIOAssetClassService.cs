using AutoMapper;
using Jarvis.db.DBResponseModel;
using Jarvis.db.ExcludePropertiesfromDBHelper;
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
using TimeZoneConverter;
using Logger = Jarvis.Shared.Utility.Logger;

namespace Jarvis.Service.Concrete
{
    public class FormIOAssetClassService : BaseService, IFormIOAssetClassService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public FormIOAssetClassService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<AssetFormIOService>();
        }
        public GetAllAssetClassListViewModel<GetAllAssetClassResponsemodel> GetAllAssetClass(GetAllAssetClassRequestmodel request)
        {
            GetAllAssetClassListViewModel<GetAllAssetClassResponsemodel> response = new GetAllAssetClassListViewModel<GetAllAssetClassResponsemodel>();

            var get_all_asset_class = _UoW.FormIOAssetClassRepository.GetAllAssetClass(request);

            var getIsAssetClassEnableFlagBySiteId = _UoW.FormIOAssetClassRepository.GetIsAssetClassEnableFlagBySiteId();

            var maplist = _mapper.Map<List<GetAllAssetClassResponsemodel>>(get_all_asset_class.Item1);

            response.list = maplist;
            response.listsize = get_all_asset_class.Item2;

            response.pageIndex = request.page_index;
            response.pageSize = request.page_size;

            response.isAddAssetClassEnabled = getIsAssetClassEnableFlagBySiteId;

            return response;
        }

        public async Task<AddAssetClassResponsemodel> AddAssetClass(AddAssetClassRequestmodel request)
        {
            PMPlansService pMPlansService = new PMPlansService(_mapper);
            AddAssetClassResponsemodel response = null;

            string asset_class_code = request.asset_class_code.ToLower();
            // insert
            if(request.inspectiontemplate_asset_class_id == null)
            {
                var get_asset_class = _UoW.WorkOrderRepository.GetAssetclassByCodeForDuplicate(asset_class_code , null);
                if (get_asset_class == null)
                {
                    InspectionTemplateAssetClass InspectionTemplateAssetClass = new InspectionTemplateAssetClass();
                    InspectionTemplateAssetClass.asset_class_code = request.asset_class_code;
                    InspectionTemplateAssetClass.asset_class_name = request.asset_class_name;
                    InspectionTemplateAssetClass.form_type_id = request.form_type_id;
                    InspectionTemplateAssetClass.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                    InspectionTemplateAssetClass.isarchive = false;
                    InspectionTemplateAssetClass.created_at = DateTime.UtcNow;
                    InspectionTemplateAssetClass.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    InspectionTemplateAssetClass.asset_expected_usefull_life = request.asset_expected_usefull_life;

                    InspectionTemplateAssetClass.AssetClassFormIOMapping = new List<AssetClassFormIOMapping>();

                    AssetClassFormIOMapping AssetClassFormIOMapping1 = new AssetClassFormIOMapping();
                    AssetClassFormIOMapping1.wo_type = (int)Status.Acceptance_Test_WO;
                    AssetClassFormIOMapping1.asset_class_formio_mapping_id = InspectionTemplateAssetClass.inspectiontemplate_asset_class_id;
                    AssetClassFormIOMapping1.created_at = DateTime.UtcNow;
                    AssetClassFormIOMapping1.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    InspectionTemplateAssetClass.AssetClassFormIOMapping.Add(AssetClassFormIOMapping1);

                    AssetClassFormIOMapping AssetClassFormIOMapping2 = new AssetClassFormIOMapping();
                    AssetClassFormIOMapping2.wo_type = (int)Status.Maintenance_WO;
                    AssetClassFormIOMapping2.asset_class_formio_mapping_id = InspectionTemplateAssetClass.inspectiontemplate_asset_class_id;
                    AssetClassFormIOMapping2.created_at = DateTime.UtcNow;
                    AssetClassFormIOMapping2.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    InspectionTemplateAssetClass.AssetClassFormIOMapping.Add(AssetClassFormIOMapping2);

                    AssetClassFormIOMapping AssetClassFormIOMapping3 = new AssetClassFormIOMapping();
                    AssetClassFormIOMapping3.wo_type = (int)Status.Onboarding_WO;
                    AssetClassFormIOMapping3.asset_class_formio_mapping_id = InspectionTemplateAssetClass.inspectiontemplate_asset_class_id;
                    AssetClassFormIOMapping3.created_at = DateTime.UtcNow;
                    AssetClassFormIOMapping3.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    InspectionTemplateAssetClass.AssetClassFormIOMapping.Add(AssetClassFormIOMapping3);

                    AssetClassFormIOMapping AssetClassFormIOMapping4 = new AssetClassFormIOMapping();
                    AssetClassFormIOMapping4.wo_type = (int)Status.TroubleShoot_WO;
                    AssetClassFormIOMapping4.asset_class_formio_mapping_id = InspectionTemplateAssetClass.inspectiontemplate_asset_class_id;
                    AssetClassFormIOMapping4.created_at = DateTime.UtcNow;
                    AssetClassFormIOMapping4.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                    InspectionTemplateAssetClass.AssetClassFormIOMapping.Add(AssetClassFormIOMapping4);

                    var insert = await _UoW.BaseGenericRepository<InspectionTemplateAssetClass>().Insert(InspectionTemplateAssetClass);
                    if (insert)
                    {
                        // Add PM category for this Class
                        PMCategory PMCategory = new PMCategory();
                        PMCategory.company_id = Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id);
                        PMCategory.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        PMCategory.created_at = DateTime.UtcNow;
                        var category_code = GenerateRandomString.RandomString(8);
                        PMCategory.category_code = category_code;
                        PMCategory.status = (int)Status.Active;
                        PMCategory.category_name = InspectionTemplateAssetClass.asset_class_name;
                        PMCategory.inspectiontemplate_asset_class_id = InspectionTemplateAssetClass.inspectiontemplate_asset_class_id;
                        var insert_pm = await _UoW.BaseGenericRepository<PMCategory>().Insert(PMCategory);

                        _UoW.SaveChanges();
                        response = new AddAssetClassResponsemodel();
                        response.inspectiontemplate_asset_class_id = InspectionTemplateAssetClass.inspectiontemplate_asset_class_id;
                    }
                }
                else
                {
                    response = new AddAssetClassResponsemodel();
                    response.inspectiontemplate_asset_class_id = Guid.Empty;
                }
            }
            // update
            else
            {
                var get_asset_class = _UoW.WorkOrderRepository.GetAssetclassByID(request.inspectiontemplate_asset_class_id.Value);
                if (get_asset_class != null)
                {
                    // check for duplicatate clas code
                    var is_duplicate_asset_class = _UoW.WorkOrderRepository.GetAssetclassByCodeForDuplicate(asset_class_code, request.inspectiontemplate_asset_class_id);
                    if (is_duplicate_asset_class == null)
                    {
                        get_asset_class.asset_class_name = request.asset_class_name;
                        get_asset_class.asset_class_code = request.asset_class_code;
                        get_asset_class.asset_expected_usefull_life = request.asset_expected_usefull_life;
                        get_asset_class.form_type_id = request.form_type_id;

                        var update = await _UoW.BaseGenericRepository<InspectionTemplateAssetClass>().Update(get_asset_class);
                        _UoW.SaveChanges();

                        if (request.set_default_pm_plan_id != null)
                        {
                            MarkDefaultPMPlanRequestmodel markDefaultPMPlanRequestmodel = new MarkDefaultPMPlanRequestmodel();
                            markDefaultPMPlanRequestmodel.pm_plan_id = request.set_default_pm_plan_id.Value;

                            var set_default_pmplan = await pMPlansService.MarkDefaultPMPlan(markDefaultPMPlanRequestmodel);
                        }
                        if (request.want_to_remove_default_pmplan && get_asset_class.PMCategory != null)
                        {
                            var get_default_plan = _UoW.PMPlansRepository.GetPMPlanByIdtoRemoveDefault(get_asset_class.PMCategory.pm_category_id);
                            if (get_default_plan != null)
                            {
                                get_default_plan.is_default_pm_plan = false;
                                get_default_plan.modified_at = DateTime.Now;
                                get_default_plan.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                                await _UoW.BaseGenericRepository<PMPlans>().Update(get_default_plan);
                                _UoW.SaveChanges();
                            }
                        }
                        
                        response = new AddAssetClassResponsemodel();
                        response.inspectiontemplate_asset_class_id = get_asset_class.inspectiontemplate_asset_class_id;
                    }
                    else
                    {
                        response = new AddAssetClassResponsemodel();
                        response.inspectiontemplate_asset_class_id = Guid.Empty;
                    }
                }
            }
            return response;
        }
        public List<GetFormsByAssetclassIDResponsemodel> GetFormsByAssetclassID(string inspectiontemplate_asset_class_id)
        {
            List<GetFormsByAssetclassIDResponsemodel> response = new List<GetFormsByAssetclassIDResponsemodel>();
            var get_forms_by_asset_class = _UoW.FormIOAssetClassRepository.GetFormsByAssetclassID(Guid.Parse(inspectiontemplate_asset_class_id));
            response = _mapper.Map<List<GetFormsByAssetclassIDResponsemodel>>(get_forms_by_asset_class);
            return response;
        }
        public GetFormIOFormByIdResponsemodel GetFormIOFormById(Guid form_id)
        {
            var get_form_by_id = _UoW.FormIOAssetClassRepository.GetFormIOFormById(form_id);
            var response = _mapper.Map<GetFormIOFormByIdResponsemodel>(get_form_by_id);
            return response;
        }

        public List<GetFormPropertiesByAssetclassIDResponsemodel> GetFormPropertiesByAssetclassID(Guid inspectiontemplate_asset_class_id)
        {
            List<GetFormPropertiesByAssetclassIDResponsemodel> response = new List<GetFormPropertiesByAssetclassIDResponsemodel>();
            var get_form_properties = _UoW.FormIOAssetClassRepository.GetFormPropertiesByAssetclassID(inspectiontemplate_asset_class_id);
            response = _mapper.Map<List<GetFormPropertiesByAssetclassIDResponsemodel>>(get_form_properties);
            return response;
        }
        public List<GetFormIOFormByIdResponsemodel> GetFormListtoAddByAssetclassID(Guid inspectiontemplate_asset_class_id)
        {
            List<GetFormIOFormByIdResponsemodel> response = new List<GetFormIOFormByIdResponsemodel>();
            var get_form_properties = _UoW.FormIOAssetClassRepository.GetFormListtoAddByAssetclassID(inspectiontemplate_asset_class_id);
            response = _mapper.Map<List<GetFormIOFormByIdResponsemodel>>(get_form_properties);
            return response;
        }

        public List<GetFormIOFormByIdResponsemodel> GetFormListtoAddByAssetclassID_V2(GetFormListtoAddByAssetclassIDRequestModel requestModel)
        {
            List<GetFormIOFormByIdResponsemodel> response = new List<GetFormIOFormByIdResponsemodel>();
            var get_form_properties = _UoW.FormIOAssetClassRepository.GetFormListtoAddByAssetclassID_V2(requestModel);
            response = _mapper.Map<List<GetFormIOFormByIdResponsemodel>>(get_form_properties);
            return response;
        }
        public async  Task<int> AddFormInAssetClass(AddFormInAssetClassRequestmodel request)
        {
            var get_class_formio_mapping =  _UoW.formIORepository.GetAssetclassFormIOmappingByID(request.asset_class_formio_mapping_id.Value);
            get_class_formio_mapping.form_id = request.form_id;
            get_class_formio_mapping.modified_at = DateTime.UtcNow;
            get_class_formio_mapping.modified_by =UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            var insert = await _UoW.BaseGenericRepository<AssetClassFormIOMapping>().Update(get_class_formio_mapping);
            if (insert)
            {
                return (int)ResponseStatusNumber.Success;
            }
            return (int)ResponseStatusNumber.Error;
        }
        public async Task<int> DeleteFormFromAssetClass(DeleteFormFromAssetClassRequestmodel request)
        {
            var get_form_mapping =  _UoW.formIORepository.GetFormMappedWithAssetclass(request.form_id , request.inspectiontemplate_asset_class_id);
            get_form_mapping.isarchive = true;
            get_form_mapping.modified_at = DateTime.UtcNow;
            var update = await _UoW.BaseGenericRepository<AssetClassFormIOMapping>().Update(get_form_mapping);
            if (update)
            {
                return (int)ResponseStatusNumber.Success;
            }
            return (int)ResponseStatusNumber.Error;
        }
        public async Task<int> DeleteAssetClass(DeleteAssetClassRequestmodel request)
        {
            int response = (int)ResponseStatusNumber.Error;
            var get_asset_class = _UoW.FormIOAssetClassRepository.GetAssetclassbyID(request);
            var wo_category = get_asset_class.WOInspectionsTemplateFormIOAssignment.Where(x => !x.is_archived).ToList();
            if(wo_category!=null && wo_category.Count > 0)
            {
                response = (int)ResponseStatusNumber.asset_class_already_used;
                return response;
            }
            // check for woline 
            var get_woline = _UoW.FormIOAssetClassRepository.GetWolinebyAssetclassCode(get_asset_class.asset_class_code.ToLower().Trim());
            if (get_woline != null)
            {
                response = (int)ResponseStatusNumber.asset_class_already_used;
                return response;
            }
            get_asset_class.isarchive = true;
            get_asset_class.modified_at = DateTime.UtcNow;
            get_asset_class.modified_by =UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            var update = await _UoW.BaseGenericRepository<InspectionTemplateAssetClass>().Update(get_asset_class);
            if (update)
            {
                response = (int)ResponseStatusNumber.Success;
            }
            return response;
        }
        public List<GetAllAssetClassCodesResponsemodel> GetAllAssetClassCodes()
        {
            List<GetAllAssetClassCodesResponsemodel> response = new List<GetAllAssetClassCodesResponsemodel>();

            var get_asset_class = _UoW.FormIOAssetClassRepository.GetAllAssetClassForList();

            response = _mapper.Map<List<GetAllAssetClassCodesResponsemodel>>(get_asset_class);

            foreach (var assetClass in response)
            {
                // Get the default PM plan for the asset class
                var defaultPmPlan = _UoW.FormIOAssetClassRepository.GetDefaultPmPlan(Guid.Parse(assetClass.value)); // value is the class_id

                if (defaultPmPlan != null)
                {
                    assetClass.plan_id = defaultPmPlan.pm_plan_id;
                    assetClass.plan_name = defaultPmPlan.plan_name;
                }
            }


            return response;
        }

        public async Task<int> UpdateNamePlateinfo(UpdateNamePlateinfoRequestmodel request)
        {
            int response = (int)ResponseStatusNumber.Success;
            /* // Commenting this code as per requirement so Nameplate does not change every time 
            var get_asset_class = _UoW.FormIOAssetClassRepository.GetAssetclassbyIDForNameplateinfo(request.inspectiontemplate_asset_class_id);

            get_asset_class.form_nameplate_info = request.form_nameplate_info;
            get_asset_class.modified_at = DateTime.UtcNow;
            var update = await _UoW.BaseGenericRepository<InspectionTemplateAssetClass>().Update(get_asset_class);
            if (update)
            {
                _UoW.SaveChanges();
                response = (int)ResponseStatusNumber.Success;
            }*/
            return response;
        }

        public GetFormNameplateInfobyClassIdResponsemodel GetFormNameplateInfobyClassId(Guid inspectiontemplate_asset_class_id)
        {
            GetFormNameplateInfobyClassIdResponsemodel response = new GetFormNameplateInfobyClassIdResponsemodel();
            var get_asset_class = _UoW.FormIOAssetClassRepository.GetAssetclassbyIDForNameplateinfo(inspectiontemplate_asset_class_id);
            response.form_nameplate_info = get_asset_class.form_nameplate_info;
            response.pdf_report_template_url = get_asset_class.pdf_report_template_url;
            response.inspectiontemplate_asset_class_id = get_asset_class.inspectiontemplate_asset_class_id;
            return response;
        }
    }
}
