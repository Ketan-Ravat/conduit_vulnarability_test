using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class FormIORepository : BaseGenericRepository<InspectionsTemplateFormIO>, IFormIORepository
    {
        public FormIORepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }

        public (List<FormIOFormsExcludedProprties>, int) GetAllForms(int page_size, int page_index, string search_string)
        {
            int total_count = 0;
            IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO.Where(
                x => ( x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id || x.is_master_form) && (x.status==(int)Status.Active || x.status == (int)Status.Draft)
            );
            if (!String.IsNullOrEmpty(search_string))
            {
                search_string = search_string.Replace(" ", "").ToString().ToLower().Trim();
                query = query.Where(x => x.form_name.Replace(" ", "").ToLower().Trim().Contains(search_string)
                || x.work_procedure.Replace(" ", "").ToLower().Trim().Contains(search_string)
                || x.FormIOType.form_type_name.Replace(" ", "").ToLower().Trim().Contains(search_string)
                );
            }
            query = query
                        .Include(x => x.StatusMaster).OrderBy(x => x.form_name);
            total_count = query.Count();
            if(page_size>0 && page_index > 0)
            {
                query = query.Skip((page_index - 1) * page_size).Take(page_size);
            }
            var response = query
              .Select(x => new FormIOFormsExcludedProprties
              {
                  form_id = x.form_id,
                  form_name = x.form_name,
                  form_type = x.FormIOType.form_type_name,
                  form_description = x.form_description,
                  status = x.status,
                  status_name = x.StatusMaster.status_name,
                  form_type_id = x.form_type_id,
                  work_procedure = x.work_procedure
              })
              .ToList();
            return (response, total_count);
        }
        public List<InspectionsTemplateFormIO> GetFormsByIds(List<Guid> form_ids)
        {
            return context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id)).Include(x=>x.FormIOType).ToList();
        }
        public List<FormIOFormsExcludedProprties> GetFormsExcludedByIds(List<Guid> form_ids)
        {
            var test =  context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id)).Include(x => x.FormIOType);

            var response = test
             .Select(x => new FormIOFormsExcludedProprties
             {
                 form_id = x.form_id,
                 form_name = x.form_name,
                 form_type = x.FormIOType.form_type_name,
                 form_description = x.form_description,
                 status = x.status,
                 status_name = x.StatusMaster.status_name,
                 form_type_id = x.form_type_id,
                 work_procedure = x.work_procedure
             }).ToList();
            return response;
        }

        public List<WorkOrders> GetWorkOrdersByIds(List<Guid> workOrderIds)
        {
            var result = context.WorkOrders.Where(x => workOrderIds.Contains(x.wo_id));
            return result.ToList();
        }

        public (List<InspectionsTemplateFormIO>, int) GetAllFormsforOffline(DateTime? sync_time)
        {
            int total_count = 0;
            IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO.Where(x => x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id
            && (x.status == (int)Status.Active || x.status == (int)Status.Draft)
            ).OrderBy(x => x.form_name);
            if (sync_time != null)
            {
                query = query.Where(x => x.created_at.Value >= sync_time.Value || x.modified_at.Value >= sync_time.Value);
            }
            total_count = query.Count();
            return (query.Include(x => x.FormIOType).Include(x=>x.StatusMaster).ToList(), total_count);
        }

        public List<Asset> GetAssetsByCompanyID(string company_id , string userid)
        {
            List<Asset> response = new List<Asset>();
            if (userid != null)
            {
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites =  context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites =  context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites =  context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                response =  context.Assets.Where(x => x.company_id == company_id && x.status == (int)Status.AssetActive && usersites.Contains(x.site_id)).ToList();
            }
            return response;
        }
        public List<MaintenanceRequests> GetMRsByCompanyID(string company_id, string userid)
        {
            List<MaintenanceRequests> response = new List<MaintenanceRequests>();
            if (userid != null)
            {
                List<Guid> usersites = new List<Guid>();
                if (String.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.site_id))
                {
                    usersites = context.User.Where(x => x.uuid.ToString() == userid && x.status == (int)Status.Active).Select(x => x.ac_default_site.Value).ToList();
                }
                else if (UpdatedGenericRequestmodel.CurrentUser.site_status == (int)Status.AllSiteType)
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active).Select(x => x.site_id).ToList();
                }
                else
                {
                    usersites = context.UserSites.Where(x => x.User.status == (int)Status.Active && x.user_id.ToString() == userid && x.status == (int)Status.Active && x.site_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.site_id).Select(x => x.site_id).ToList();
                }
                //response = context.Assets.Where(x => x.company_id == company_id && usersites.Contains(x.site_id)).ToList();
                response = context.MaintenanceRequests.Where(x => x.Sites.company_id.ToString() == company_id && usersites.Contains(x.site_id)).ToList();
            }
            return response;
            //return context.MaintenanceRequests.Where(x => x.Sites.company_id.ToString() ==  company_id).ToList();
        }

        public virtual async Task<int> Insert(InspectionsTemplateFormIO entity)
        {
            int IsSuccess;
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    Add(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw ex;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(InspectionsTemplateFormIO entity)
        {
            int IsSuccess = 0;
            try
            {
                dbSet.Update(entity);
                IsSuccess = await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw ex;
            }
            return IsSuccess;
        }

        public async Task<InspectionsTemplateFormIO> GetFormIOById(Guid form_id)
        {
            return await context.InspectionsTemplateFormIO.Where(u => u.form_id == form_id).Include(x=>x.FormIOType).Include(x=>x.Tasks).FirstOrDefaultAsync();
        }
        public async Task<InspectionsTemplateFormIO> GetFormIOByIdForDelete(Guid form_id)
        {
            return await context.InspectionsTemplateFormIO.Where(u => u.form_id == form_id)
                .Include(x => x.FormIOType)
                .Include(x=>x.Tasks)
                 .ThenInclude(x=>x.WOcategorytoTaskMapping)
                .FirstOrDefaultAsync();
        }
        public (List<InspectionsTemplateFormIO>, int) GetAllFormNames(int page_size, int page_index)
        {
            int total_count = 0;
            IQueryable<InspectionsTemplateFormIO> query = context.InspectionsTemplateFormIO;
            var list = query.Include(x=>x.FormIOType).Where(x=>x.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id).Select(x => new InspectionsTemplateFormIO{ form_id = x.form_id, form_name = x.form_name , work_procedure = x.work_procedure , form_type_id = x.form_type_id , form_type = x.FormIOType.form_type_name , FormIOType = x.FormIOType}).ToList();
            total_count = query.Count();
            if (page_size > 0 && page_index > 0)
            {
                query = query.Skip((page_index - 1) * page_size).Take(page_size);
            }
            return (list, total_count);
        }

        public bool IsValidWorkProcedure(string work_procedure ,  Guid form_id)
        {
            if (form_id!=null && form_id != Guid.Empty)
            {
                return context.InspectionsTemplateFormIO.Where(x => x.work_procedure == work_procedure && x.form_id != form_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.status == (int)Status.Active).FirstOrDefault() != null ? false : true;
            }
            else
            {
                return context.InspectionsTemplateFormIO.Where(x => x.work_procedure == work_procedure && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.status == (int)Status.Active).FirstOrDefault() != null ? false : true;
            }
            //return context.InspectionsTemplateFormIO.Where(x => x.work_procedure == work_procedure).FirstOrDefault()!=null ? false : true;
            
        }
        public bool IsValidFormName(string form_name, Guid form_id)
        {
            if (form_id != null && form_id != Guid.Empty)
            {
                return context.InspectionsTemplateFormIO.Where(x => x.form_name.ToLower().Trim() == form_name && x.form_id != form_id && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.status == (int)Status.Active).FirstOrDefault() != null ? false : true;
            }
            else
            {
                return context.InspectionsTemplateFormIO.Where(x => x.form_name.ToLower().Trim() == form_name && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id) && x.status == (int)Status.Active).FirstOrDefault() != null ? false : true;
            }
            //return context.InspectionsTemplateFormIO.Where(x => x.work_procedure == work_procedure).FirstOrDefault()!=null ? false : true;

        }

        public async Task<List<FormIOType>> GetAllFormTypes(string searchstring)
        {
            string rolename = string.Empty;
            if (!string.IsNullOrEmpty(UpdatedGenericRequestmodel.CurrentUser.role_id))
            {
                rolename = context.UserRoles.Where(x => x.role_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.role_id && x.status == (int)Status.Active && x.user_id == UpdatedGenericRequestmodel.CurrentUser.requested_by).Select(x => x.Role.name).FirstOrDefault();
            }
            IQueryable<FormIOType> query = context.FormIOType;
            if (!string.IsNullOrEmpty(rolename))
            {
                if (rolename != GlobalConstants.Admin)
                {
                    query = query.Where(x => (!x.isarchive) && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));
                }
                else
                {
                    query = query.Where(x => (!x.isarchive));
                }
            }

            if (!String.IsNullOrEmpty(searchstring))
            {
                searchstring = searchstring.ToLower();
                query = query.Where(x => (x.form_type_name.ToLower().Contains(searchstring)));
            }
            return query.OrderBy(x => x.form_type_name).ToList();
        }

        public bool IsFormMappedWithAssetclass(Guid form_id, Guid inspectiontemplate_asset_class_id)
        {
            var mapping = context.AssetClassFormIOMapping.Where(x => x.form_id == form_id && x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id && !x.isarchive).FirstOrDefault();
            if (mapping != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public AssetClassFormIOMapping GetFormMappedWithAssetclass(Guid form_id, Guid inspectiontemplate_asset_class_id)
        {
            return context.AssetClassFormIOMapping.Where(x => x.form_id == form_id && x.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id).FirstOrDefault();
        }
        public AssetClassFormIOMapping GetAssetclassFormIOmappingByID(Guid asset_class_formio_mapping_id)
        {
            return context.AssetClassFormIOMapping.Where(x => x.asset_class_formio_mapping_id == asset_class_formio_mapping_id).FirstOrDefault();
        }
        public AssetFormIO ReplaceAssetformIOJson(Guid assetformid)
        {
            return context.AssetFormIO.Where(x => x.asset_form_id == assetformid).FirstOrDefault();
        }

        public List<AssetFormIO> ReplaceAssetformIOJsonAll(Guid siteId)
        {
            List<string> sites = new List<string>() {
           "00b64855-4b65-4b80-ae32-bfdd2a14aaa1",
            "8b4be1a5-5b4b-427a-9aa1-44e8e96c6251",
            "7816cea7-88eb-44a5-ad6a-3e077dfd20a2",
            "62307cf5-a54c-4a8b-ad83-cb16f6302cf0",
            "8ea74841-a31c-4007-8bde-c6f78746c8b2",
            "3912377c-44bb-4c44-894e-dc9692c40e32",
            "99f6c9d4-57ea-46e6-92d0-4997a33fc726"
            };
            return context.AssetFormIO.Where(x => 
            x.wo_id == Guid.Parse("ef97a1c4-3816-4adf-9e2b-3c7cd6b40e0c")
            //sites.Contains(x.site_id.ToString()) 
            && x.status != 2 
            //&& x.status!= (int)Status.open 
            //&& x.asset_form_id == Guid.Parse("fdcdf424-e16a-4d51-867e-e368834c428e")
            )
               .Include(x=>x.Sites)
            .ToList();
            
        }
        public List<Asset> ReplaceAssetLocationScript(Guid siteId)
        {
            ListViewModel<Asset> assets = new ListViewModel<Asset>();

            List<string> sites = new List<string>() {
           "00b64855-4b65-4b80-ae32-bfdd2a14aaa1",
            "8b4be1a5-5b4b-427a-9aa1-44e8e96c6251",
            "7816cea7-88eb-44a5-ad6a-3e077dfd20a2",
            "62307cf5-a54c-4a8b-ad83-cb16f6302cf0",
            "8ea74841-a31c-4007-8bde-c6f78746c8b2",
            "3912377c-44bb-4c44-894e-dc9692c40e32",
            "99f6c9d4-57ea-46e6-92d0-4997a33fc726"
            };

            IQueryable<Asset> query = context.Assets
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOBuildings)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOFloors)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIORooms)
                    .Include(x => x.AssetFormIOBuildingMappings.FormIOSections)
                    .Where(x => x.site_id == siteId);

            assets.list = query.ToList();

            //return context.AssetFormIO.Where(x => sites.Contains(x.site_id.ToString()) && x.status != 2 && x.status!= (int)Status.open 
            //&& x.asset_form_id == Guid.Parse("fdcdf424-e16a-4d51-867e-e368834c428e")
            //)
            //    .Include(x=>x.Sites)
            //.ToList();
            return assets.list;
        }

        public int InspectionRFRCount()
        {
            return context.AssetFormIO.Where(x => x.status == (int)Status.Ready_for_review && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Count();

        }
        public int InspectionCompletedCount()
        {
            return context.AssetFormIO.Where(x => x.status == (int)Status.Completed && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Count();

        }
        public InspectionsTemplateFormIO GetFormDataTemplateByFormId(Guid form_id)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.form_id == form_id).FirstOrDefault();
        }
        public int OpenAssetIssuesCount()
        {
            return context.AssetIssue.Where(x => x.issue_status == (int)Status.open && !x.is_deleted &&  x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Count();
        }

        public List<AssetIssue> GetAssetIssuesForDashboardCount()
        {
            return context.AssetIssue.Where(x=>!x.is_deleted && x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && 
            (x.issue_status == (int)Status.open || x.issue_status == (int)Status.InProgress || x.issue_status == (int)Status.Schedule)
            ).ToList(); 
        }


    }
}
