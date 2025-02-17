using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class FormIOAssetClassRepository : BaseGenericRepository<InspectionTemplateAssetClass>, IFormIOAssetClassRepository
    {
        public FormIOAssetClassRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public (List<InspectionTemplateAssetClass>, int) GetAllAssetClass(GetAllAssetClassRequestmodel request)
        {

            IQueryable<InspectionTemplateAssetClass> query = context.InspectionTemplateAssetClass.Where(x=>!x.isarchive);

            if(request.company_id!=null&& request.company_id != Guid.Empty)
            {
                query = query.Where(x => x.company_id == request.company_id);
            }

            if (!String.IsNullOrEmpty(request.search_string))
            {
                request.search_string = request.search_string.ToLower().Trim();
                query = query.Where(x=>x.asset_class_code.Trim().ToLower().Contains(request.search_string)
                                    || x.asset_class_name.Trim().ToLower().Contains(request.search_string));
            }
            int total_count = query.Count();

            if (request.page_index > 0 && request.page_size > 0)
            {
                query = query.OrderBy(x => x.asset_class_code).Skip((request.page_index - 1) * request.page_size).Take(request.page_size);
            }
            else
            {
                query = query.OrderBy(x => x.asset_class_code);
            }

            return (query.Include(x => x.FormIOType).Include(x => x.PMCategory.PMPlans).ToList() , total_count);
        }

        public List<InspectionTemplateFormIoExclude> GetFormsByAssetclassID(Guid inspectiontemplate_asset_class_id)
        {
            IQueryable<AssetClassFormIOMapping> query = context.AssetClassFormIOMapping.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id && !x.isarchive);
            var response = query
                .Select(x => new InspectionTemplateFormIoExclude
                {
                    form_id = x.form_id,
                    form_description = x.InspectionsTemplateFormIO.form_description,
                    form_name = x.InspectionsTemplateFormIO.form_name,
                    work_procedure = x.InspectionsTemplateFormIO.work_procedure,
                    form_type_id = x.InspectionsTemplateFormIO.form_type_id.Value,
                    form_type_name = x.InspectionsTemplateFormIO.FormIOType.form_type_name,
                    asset_class_form_properties = x.InspectionsTemplateFormIO.asset_class_form_properties,
                    asset_class_formio_mapping_id = x.asset_class_formio_mapping_id,
                    wo_type = x.wo_type
                })
                .ToList();
            return response;
        }
        public List<InspectionTemplateFormIoExclude> GetFormListtoAddByAssetclassID(Guid inspectiontemplate_asset_class_id)
        {
            IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO.Where(x =>  x.status == (int)Status.Active && x.company_id == Guid.Parse(GenericRequestModel.company_id));
            var response = query
                .Select(x => new InspectionTemplateFormIoExclude
                {
                    form_id = x.form_id,
                    form_description = x.form_description,
                    form_name = x.form_name,
                    work_procedure = x.work_procedure,
                    form_type_id = x.form_type_id.Value,
                    form_type_name = x.FormIOType.form_type_name,
                    asset_class_form_properties = x.asset_class_form_properties
                })
                .ToList();
            return response;
        }

        public List<InspectionTemplateFormIoExclude> GetFormListtoAddByAssetclassID_V2(GetFormListtoAddByAssetclassIDRequestModel requestModel)
        {
            IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO.Where(x => x.status == (int)Status.Active && x.company_id == Guid.Parse(GenericRequestModel.company_id));
            if(requestModel.inpsection_form_type != null && requestModel.inpsection_form_type >0)
            {
                query = query.Where(x => x.inpsection_form_type == requestModel.inpsection_form_type);
            }
            var response = query
                .Select(x => new InspectionTemplateFormIoExclude
                {
                    form_id = x.form_id,
                    form_description = x.form_description,
                    form_name = x.form_name,
                    work_procedure = x.work_procedure,
                    form_type_id = x.form_type_id.Value,
                    form_type_name = x.FormIOType.form_type_name,
                    asset_class_form_properties = x.asset_class_form_properties
                })
                .ToList();
            return response;
        }



        public InspectionsTemplateFormIO GetFormIOFormById(Guid form_id)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.form_id == form_id)
                                                    .Include(x=>x.StatusMaster)
                                                    .FirstOrDefault();
        }


        public List<InspectionTemplateFormIoExclude> GetFormPropertiesByAssetclassID(Guid inspectiontemplate_asset_class_id)
        {
            return context.AssetClassFormIOMapping.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id)
                .Select((x => new InspectionTemplateFormIoExclude {
                    form_id = x.form_id,
                    form_description = x.InspectionsTemplateFormIO.form_description,
                    form_name = x.InspectionsTemplateFormIO.form_name,
                    work_procedure = x.InspectionsTemplateFormIO.work_procedure,
                    form_type_id = x.InspectionsTemplateFormIO.form_type_id.Value,
                    form_type_name = x.InspectionsTemplateFormIO.FormIOType.form_type_name,
                    asset_class_form_properties = x.InspectionsTemplateFormIO.asset_class_form_properties
                })).ToList();
        }

        public List<InspectionTemplateAssetClass> GetAssetclassByAssetclassCodes(List<string> assetclass_codes)
        {
            return context.InspectionTemplateAssetClass.Where(x => assetclass_codes.Contains(x.asset_class_code.ToLower().Trim()) && x.company_id.ToString() == GenericRequestModel.company_id
            && !x.isarchive
            )
                .Include(x=>x.FormIOType)
                .ToList();
        }

        public List<InspectionTemplateAssetClass> GetAllAssetClassForList()
        {
            return context.InspectionTemplateAssetClass.Where(x => x.company_id.ToString() == GenericRequestModel.company_id && !x.isarchive)
                .Include(x => x.FormIOType)
                .ToList();

        }

        public PMPlans GetDefaultPmPlan(Guid class_id)
        {
            return context.InspectionTemplateAssetClass
                .Where(x => x.inspectiontemplate_asset_class_id == class_id)
                .SelectMany(x => x.PMCategory.PMPlans) // Flatten PMPlans
                .Where(plan => plan.is_default_pm_plan)
                .FirstOrDefault(); // Return the default PM plan (or null if none found)
        }


        public InspectionTemplateAssetClass GetAssetclassbyID(DeleteAssetClassRequestmodel request)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == request.inspectiontemplate_asset_class_id)
                .Include(x=>x.WOInspectionsTemplateFormIOAssignment)
                
                .FirstOrDefault();
        }
        public InspectionTemplateAssetClass GetAssetclassbyIDForNameplateinfo(Guid inspectiontemplate_asset_class_id)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id)
                .FirstOrDefault();
        }
        public WOOnboardingAssets GetWolinebyAssetclassCode(string asset_class_code)
        {
            return context.WOOnboardingAssets.Where(x => x.asset_class_code.ToLower().Trim() == asset_class_code && !x.is_deleted
            && x.Sites.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) ).FirstOrDefault();
        }

        public bool GetIsAssetClassEnableFlagBySiteId()
        {
            return context.Sites.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Select(x=>x.isAddAssetClassEnabled)
                .FirstOrDefault();
        }
    }
}
