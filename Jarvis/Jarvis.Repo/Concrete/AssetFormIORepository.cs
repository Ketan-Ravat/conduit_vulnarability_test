using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class AssetFormIORepository : BaseGenericRepository<AssetFormIO>, IAssetFormIORepository
    {
        public AssetFormIORepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }

        public virtual async Task<int> Insert(AssetFormIO entity)
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

        public virtual async Task<int> Update(AssetFormIO entity)
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

        public async Task<AssetFormIO> GetAssetFormIOById(Guid asset_form_id)
        {
            return await context.AssetFormIO.Where(u => u.asset_form_id == asset_form_id)
                .Include(x=>x.WOcategorytoTaskMapping)
                .Include(x=>x.WorkOrders)
                .Include(x=>x.Sites)
                .Include(x => x.AssetPMs)
                .FirstOrDefaultAsync();
        }
        public AssetFormIO GetAssetFormIOByIdMobile(Guid asset_form_id)
        {
            return  context.AssetFormIO.Where(u => u.asset_form_id == asset_form_id).Include(x=>x.Sites.Company)
                .FirstOrDefault();
        }
        public async Task<AssetFormIO> GetAssetFormIOByAssetId(Guid asset_id)
        {
            return await context.AssetFormIO.Where(u => u.asset_id == asset_id).FirstOrDefaultAsync();
        }

        public  (List<AssetFormIO>, int) GetAllAssetTemplateList(GetAllAssetInspectionListByAssetIdRequestModel request)
        {
            int total_size = 0;
            List<AssetFormIO> tempList = new List<AssetFormIO>();
            IQueryable<AssetFormIO> query = context.AssetFormIO;


            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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

            query = query.Where(x => usersites.Contains(x.site_id.Value));
            if (request.assetid != null && !String.IsNullOrEmpty(request.assetid))
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => x.asset_id.ToString() == request.assetid && (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted))
                                                .OrderByDescending(x => x.created_at);
            }
            else
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted)
                                                              // && x.requested_by != null 
                                                               )
                                                .OrderByDescending(x => x.created_at);
            }
            if (request.initial_start_date_time != null  && request.initial_end_date_time != null)  // if both date comes then give in between records
            {
                query = query.Where(x => x.intial_form_filled_date.Value.Date >= request.initial_start_date_time.Value.Date &&
                 x.intial_form_filled_date.Value.Date <= request.initial_end_date_time.Value.Date);
            }
            else if(request.initial_start_date_time != null) // if only start date comes then give only start date records
            {
                query = query.Where(x => x.intial_form_filled_date.Value.Date == request.initial_start_date_time.Value.Date);
            }
            if (request.filter_asset_name.Count > 0)
            {
                var list = request.filter_asset_name.ConvertAll(x => x.Trim().ToLower());
                query = query.Where(x => list.Contains(x.form_retrived_asset_name.ToLower().Trim()));
            }
            if (request.WO_ids.Count > 0)
            {
                query = query.Where(x => request.WO_ids.Contains(x.wo_id.Value));
            }
            if (request.inspected_by.Count > 0)
            {
                query = query.Where(x => request.inspected_by.Contains(x.requested_by.Value));
            }
            if (request.accepted_by.Count > 0)
            {
                query = query.Where(x => request.accepted_by.Contains(x.accepted_by));
            }
            if (request.status.Count > 0)
            {
                query = query.Where(x => request.status.Contains(x.status));
            }
            if (request.service_type.Count > 0)
            {
                query = query.Where(x => request.service_type.Contains(x.WorkOrders.wo_type));
            }
            if (!request.is_wo_completed) // this will be for filter of bulk report and bulk status changes
            {
                query = query.Where(x => x.WorkOrders.status!= (int)Status.Completed);
            }
            if(request.is_for_asset_inspection_tab == true)
            {
                query = query.Where(x => x.status == (int)Status.Completed || x.status == (int)Status.Submitted && x.WorkOrders.status == (int)Status.Completed);
            }

            if (!String.IsNullOrEmpty(request.search_string))
            {
                request.search_string = request.search_string.Trim().ToLower();
                query = query.Where(x => x.form_retrived_asset_name.ToLower().Trim().Contains(request.search_string) ||
                                        //x.WorkOrders.wo_number.ToString().Contains(request.search_string) || 
                                        x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(request.search_string));
            }

            total_size = query.Count();

            if (request.pagesize > 0 && request.pageindex > 0)
            {
                query = query.Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }

            var response  = query
                            .Include(x => x.WorkOrders)
                            .ThenInclude(x=>x.StatusMaster)
                            .Include(x=>x.WorkOrders).ThenInclude(x=>x.Sites)
                            .Include(x => x.Asset)
                             .ThenInclude(x => x.Sites)
                             .Include(x=>x.PDFReportStatusMaster)
                             .Include(x=>x.StatusMaster)
                             .Include(x=>x.WOcategorytoTaskMapping)
                             .AsNoTracking()
                             .ToList();

            var asset_ids = response.Select(q => q.asset_id).ToList();
            var asset_details = context.Assets.Where(q => asset_ids.Contains(q.asset_id))
                .Include(q=>q.Sites)
                .ToList();
            response.ForEach(q =>
            {
                q.Asset = asset_details.Where(a => a.asset_id == q.asset_id).FirstOrDefault();
            });
            

            return (response, total_size);
        }

        public (List<AssetFormIOExcludeNew>, int total_size) GetAllAssetTemplateListNew(GetAllAssetInspectionListByAssetIdRequestModel request)
        {
            int total_size = 0;
            //List<AssetFormIOExlude> tempList = new List<AssetFormIOExlude>();
            IQueryable<AssetFormIO> query = context.AssetFormIO;


            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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

           // query = query.Where(x => usersites.Contains(x.site_id.Value));
            query = query.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id));
            if (request.assetid != null && !String.IsNullOrEmpty(request.assetid) && !request.is_requested_for_otherthan_completed)
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => x.asset_id.ToString() == request.assetid && (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted))
                                                .OrderByDescending(x => x.created_at);
            }
            else if (request.assetid != null && !String.IsNullOrEmpty(request.assetid) && request.is_requested_for_otherthan_completed)
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => x.asset_id.ToString() == request.assetid && (x.status != (int)Status.Submitted && x.status != (int)Status.Completed))
                                                .OrderByDescending(x => x.created_at);
            }
            else
            {
                query = query.Include(x => x.Asset).ThenInclude(x => x.Sites).Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed || x.status == (int)Status.Submitted)
                                                               // && x.requested_by != null 
                                                               )
                                                .OrderByDescending(x => x.created_at);
            }
            if (request.initial_start_date_time != null && request.initial_end_date_time != null)  // if both date comes then give in between records
            {
                query = query.Where(x => x.intial_form_filled_date.Value.Date >= request.initial_start_date_time.Value.Date &&
                 x.intial_form_filled_date.Value.Date <= request.initial_end_date_time.Value.Date);
            }
            else if (request.initial_start_date_time != null) // if only start date comes then give only start date records
            {
                query = query.Where(x => x.intial_form_filled_date.Value.Date == request.initial_start_date_time.Value.Date);
            }
            if (request.filter_asset_name!=null && request.filter_asset_name.Count > 0)
            {
                var list = request.filter_asset_name.ConvertAll(x => x.Trim().ToLower());
                query = query.Where(x => list.Contains(x.form_retrived_asset_name.ToLower().Trim()));
            }
            if (request.WO_ids != null && request.WO_ids.Count > 0)
            {
                query = query.Where(x => request.WO_ids.Contains(x.wo_id.Value));
            }
            if (request.wo_type != null && request.wo_type.Count > 0)
            {
                query = query.Include(x => x.WorkOrders).Where(x => request.wo_type.Contains(x.WorkOrders.wo_type));
            }
            if (request.inspected_by != null && request.inspected_by.Count > 0)
            {
                query = query.Where(x => request.inspected_by.Contains(x.requested_by.Value));
            }
            if (request.accepted_by != null && request.accepted_by.Count > 0)
            {
                query = query.Where(x => request.accepted_by.Contains(x.accepted_by));
            }
            if (request.status != null && request.status.Count > 0)
            {
                query = query.Where(x => request.status.Contains(x.status));
            }
            if (request.service_type != null && request.service_type.Count > 0)
            {
                query = query.Where(x => request.service_type.Contains(x.WorkOrders.wo_type));
            }
            if (!request.is_wo_completed) // this will be for filter of bulk report and bulk status changes
            {
                query = query.Where(x => x.WorkOrders.status != (int)Status.Completed);
            }

            if (!String.IsNullOrEmpty(request.search_string))
            {
                request.search_string = request.search_string.Trim().ToLower();
                query = query.Where(x => x.form_retrived_asset_name.ToLower().Trim().Contains(request.search_string) ||
                                        //x.WorkOrders.wo_number.ToString().Contains(request.search_string) || 
                                        x.WorkOrders.manual_wo_number.ToLower().Trim().Contains(request.search_string));
            }
            // serach with qr_code
            if (!String.IsNullOrEmpty(request.qr_code))
            {
                request.qr_code = request.qr_code.Trim().ToLower();
                query = query.Where(x => x.form_retrived_asset_id.ToLower().Trim().Contains(request.qr_code));
            }

            total_size = query.Count();

            if (request.pagesize > 0 && request.pageindex > 0)
            {
                query = query.Skip((request.pageindex - 1) * request.pagesize).Take(request.pagesize);
            }

            var response = query
                .Select(x => new AssetFormIOExcludeNew
                {
                    asset_form_id = x.asset_form_id,
                    asset_id = x.asset_id,
                    site_id = x.site_id,
                    form_id = x.form_id,
                    wo_number = x.WorkOrders.wo_number,
                    manual_wo_number = x.WorkOrders.manual_wo_number,
                    asset_form_name = x.asset_form_name,
                    asset_form_type = x.asset_form_type,
                    asset_form_description = x.asset_form_description,
                    // asset_form_data = x.asset_form_data,
                    requested_by = x.requested_by,
                    created_at = x.created_at,
                    created_by = x.created_by,
                    modified_at = x.modified_at,
                    modified_by = x.modified_by,
                    accepted_by = x.accepted_by,
                    status = x.status,
                    wo_id = x.wo_id,
                    WOcategorytoTaskMapping_id = x.WOcategorytoTaskMapping_id,
                    pdf_report_status = x.pdf_report_status,
                    pdf_report_url = x.pdf_report_url,
                    form_retrived_asset_name = x.form_retrived_asset_name,
                    form_retrived_asset_id = x.form_retrived_asset_id,
                    form_retrived_location = x.form_retrived_location,
                    // form_retrived_data = x.form_retrived_data,
                    intial_form_filled_date = x.intial_form_filled_date,
                    form_retrived_nameplate_info = x.form_retrived_nameplate_info,
                    inspected_at = x.inspected_at,
                    accepted_at = x.accepted_at,
                    export_pdf_at = x.export_pdf_at,
                    asset_name = x.Asset.name,
                    status_name = x.StatusMaster.status_name,
                    timezone = x.Asset.Sites.timezone,
                    wo_inspectionsTemplateFormIOAssignment_id = x.WOcategorytoTaskMapping.wo_inspectionsTemplateFormIOAssignment_id,
                    workOrderStatus = x.WorkOrders.status,
                    inspection_verdict = x.inspection_verdict.Value,
                    form_retrived_workOrderType = x.form_retrived_workOrderType,
                    asset_class_code = x.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment.InspectionTemplateAssetClass.asset_class_code,
                    asset_class_name = x.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment.InspectionTemplateAssetClass.asset_class_name
                })
                .ToList();

            return (response, total_size);
        }

        public AssetFormIO GetAssetFormIOBytId(Guid asset_form_id)
        {
            return context.AssetFormIO.Where(x => x.asset_form_id == asset_form_id)
                .Include(x=>x.WorkOrders)
                .ThenInclude(x=>x.WOTypeStatusMaster)
                .Include(x => x.Sites)
                .ThenInclude(x => x.ClientCompany)
                .Include(x => x.Sites.Company)
                .Include(x => x.WOcategorytoTaskMapping.WOInspectionsTemplateFormIOAssignment.InspectionTemplateAssetClass)
                .FirstOrDefault();
        }

        public List<AssetFormIO> ExractandStoreOnlydatafromOldForm(int skip , int take)
        {
            return context.AssetFormIO.Where(x =>
            !String.IsNullOrEmpty(x.asset_form_data) && x.asset_form_data.Contains("components")
           ).Skip(skip).Take(take).ToList();
        }
        public int ExractandStoreOnlydatafromOldFormcount()
        {
            return context.AssetFormIO.Where(x =>
            !String.IsNullOrEmpty(x.asset_form_data) && x.asset_form_data.Contains("components")).Count();
        }
        public  Asset GetAssetForUpdate(Guid asset_id)
        {
            return context.Assets.Where(x => x.asset_id == asset_id).FirstOrDefault();

        }

        public FormIOInsulationResistanceTestMapping GetFormIOInsulationResistanceTestMappingByAssetFormID(Guid asset_form_id)
        {
            return context.FormIOInsulationResistanceTestMapping.Where(x => x.asset_form_id == asset_form_id).FirstOrDefault();
        }
        public (List<FormIOInsulationResistanceTestMapping> , int) GetFormIOInsulationResistanceTest(FormIOInsulationResistanceTestRequestModel request)
        {
            IQueryable<FormIOInsulationResistanceTestMapping> query = context.FormIOInsulationResistanceTestMapping.Where(x=>x.AssetFormIO.asset_id == request.asset_id);
            if (request.start_date != null)
            {
                query = query.Where(x => x.created_at.Value.Date >= request.start_date.Value.Date);
            }
            if ( request.end_date != null)
            {
                query = query.Where(x => x.created_at.Value.Date<= request.end_date.Value.Date);
            }
            int total_size = query.Count();
            if (request.page_size > 0 && request.page_index > 0)
            {
                query = query.Skip((request.page_index.Value - 1) * request.page_size.Value).Take(request.page_size.Value);
            }
            return (query.ToList(), total_size);
        }
        public AssetFormIO GetAssetFormIOByIdForStatusShange(Guid asset_form_id)
        {
            return context.AssetFormIO.Where(x => x.asset_form_id == asset_form_id).Include(x => x.WOcategorytoTaskMapping).Include(x => x.WorkOrders).FirstOrDefault();

        }
        public List<AssetFormIO> GetAssetFormIOByIdForStatusShangeFormultiple(List<Guid> asset_form_id)
        {
            return context.AssetFormIO.Where(x => asset_form_id.Contains(x.asset_form_id)).Include(x => x.WOcategorytoTaskMapping).Include(x => x.WorkOrders).ToList();
        }
        public bool IsWOCompleted(List<Guid> asset_form_id)
        {
            var wo = context.AssetFormIO.Where(x => asset_form_id.Contains(x.asset_form_id) && x.WorkOrders.status == (int)Status.Completed).FirstOrDefault();
            if (wo != null)
            {
                return true;
            }
            return false;

        }
        public bool is_form_completed(List<Guid> asset_form_id)
        {
            var form = context.AssetFormIO.Where(x => asset_form_id.Contains(x.asset_form_id) && (x.status != (int)Status.Completed && x.status != (int)Status.Submitted && x.status != (int)Status.Ready_for_review)).FirstOrDefault();
            if (form != null)
            {
                return false;
            }
            return true;

        }
        public List<string> GetAssetsForSubmittedFilterOptions(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel)
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO;
            //.Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed) && !String.IsNullOrEmpty(x.form_retrived_asset_name));


            if (requestmodel.status != null && requestmodel.status.Count > 0)
            {
                query = query.Where(x => requestmodel.status.Contains(x.status));
            }
            else
            {
                query = query.Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed) && !String.IsNullOrEmpty(x.form_retrived_asset_name));
            }

            if (requestmodel.wo_type != null && requestmodel.wo_type.Count > 0)
            {
                query = query.Include(x => x.WorkOrders).Where(x => requestmodel.wo_type.Contains(x.WorkOrders.wo_type));
            }

            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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

            query = query.Where(x => usersites.Contains(x.site_id.Value));

            return query.Select(x => x.form_retrived_asset_name).ToList();
        }
        public List<WorkOrders> GetWorkOrdersForSubmittedFilterOptions(GetAssetsForSubmittedFilterOptionsByStatusRequestModel requestmodel)
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO;//.Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed));


            if (requestmodel.status != null && requestmodel.status.Count > 0)
            {
                query = query.Where(x => requestmodel.status.Contains(x.status));
            }
            else
            {
                query = query.Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed));
            }

            if (requestmodel.wo_type != null && requestmodel.wo_type.Count > 0)
            {
                query = query.Include(x => x.WorkOrders).Where(x => requestmodel.wo_type.Contains(x.WorkOrders.wo_type));
            }

            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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
            query = query.Where(x => usersites.Contains(x.site_id.Value));
            return query.Select(x => x.WorkOrders).ToList();
        }
        public List<Guid> GetInspectedForSubmittedFilterOptions()
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO.Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed)
                                                                         && x.requested_by != null);

            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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
            query = query.Where(x => usersites.Contains(x.site_id.Value));

            return query.Select(x => x.requested_by.Value).ToList();
        }
        public List<string> GetApprovedForSubmittedFilterOptions()
        {
            IQueryable<AssetFormIO> query = context.AssetFormIO.Where(x => (x.status == (int)Status.Ready_for_review || x.status == (int)Status.Completed)
                                                                         && x.requested_by != null);

            List<Guid> usersites = new List<Guid>();
            string userid = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
            if (String.IsNullOrEmpty(ViewModels.RequestResponseViewModel.UpdatedGenericRequestmodel.CurrentUser.site_id))
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
            query = query.Where(x => usersites.Contains(x.site_id.Value));

            return query.Select(x => x.accepted_by).ToList();
        }

        public List<AssetFormIO> Getallassetformios()
        {
            List<string> sites = new List<string>() {
            "f68826db-3248-4e10-aff2-ea67e1153277",
            "8ea74841-a31c-4007-8bde-c6f78746c8b2",
            "3912377c-44bb-4c44-894e-dc9692c40e32",
            "99f6c9d4-57ea-46e6-92d0-4997a33fc726",
            "ee5f8679-f260-43a4-85ee-b7b99610f76c",
            "c2ebb0a0-08a5-46d0-aeff-acd4828e8442",
            "6d597a43-20eb-494e-8529-d57d63b98a6c",
            "61d9d32b-5bd8-4ea8-8405-e67e9ad1a068",
            "5a33deb0-1ee0-4e79-93b7-7560042a7ed0",
            "021dfe0b-ebb9-4e1a-be08-cf25831231cc",
            "60b09fbb-e166-43a7-af6e-2feabfe4b41d",
            "557b8cd7-e10a-4792-a8e6-a767d36e18b4",
            "e2536f11-d080-4e78-a6b1-d48ae9b5d50a",
            "ec433e29-17dc-445b-84c3-e9d511dd9a1e",
            "892bbd1d-61fa-401d-a028-dda4ada7b419",
            "bb2ab181-b0da-4e14-ad22-af23e079d741",
            "652a352a-4607-49a9-b7dc-4f41da6ae5b1",
            "da0ba1be-f26f-426d-8766-cdd8fbee2017",
            "67ec9bad-42b1-476f-aac2-a6aa82429596",
            "fc2d1514-6ec8-4fe5-b36a-e74039e4a17d",
            "07c72b9f-65fe-41fc-95b8-7b879b412255",
            "5663d207-b1a9-40ee-8d82-44fec8fbd868",
            "62307cf5-a54c-4a8b-ad83-cb16f6302cf0",
            "35d3c07f-1ae3-43cf-8e07-1e4f572c3857"

            };
           
            return context.AssetFormIO.Where(x=>x.status!=2  && sites.Contains(x.site_id.Value.ToString())).ToList();
        }

        public List<AssetFormIO> GetFormtoUpdate()
        {
            List<string> wo_ids = new List<string>()
            {
                    "7807d7da-2223-43ba-9aaf-c71b467549bb",
                    "7807d7da-2223-43ba-9aaf-c71b467549bb",
                    "3ef0a9a6-5257-4823-897a-1a733243cce1",
                    "7807d7da-2223-43ba-9aaf-c71b467549bb",
                    "befae47f-4e0f-4891-8f06-d735f344bad7",
                    "befae47f-4e0f-4891-8f06-d735f344bad7",
                    "befae47f-4e0f-4891-8f06-d735f344bad7",
                    "ce9ce3a0-2f80-48ee-a3ec-b3762b061700",
                    "5c20bb95-1932-4091-89bf-de3c248c2137",
                    "aa22df2e-78e5-48ee-b0ac-e6334e284486",
                    "b70909bf-461c-4f6c-a550-2c2d8c0975f3",
                    "ec377926-6f37-45b0-9f3c-aea6bf59644c",
                    "befae47f-4e0f-4891-8f06-d735f344bad7",
                    "b70909bf-461c-4f6c-a550-2c2d8c0975f3",
                    "c84bb85d-d51f-4849-a1ea-33fadef71a0b",
                    "e4468374-083d-4b7b-b1f2-d54e939c30c4",
                    "fe82ed19-55df-4a65-8ad9-f15874b69772",
                    "b70909bf-461c-4f6c-a550-2c2d8c0975f3",
                    "b15bfb21-02a3-43c0-9656-a3d2337c2a81",
                    "f2c0b27f-bcdf-4c71-8a15-ab3fe77f0870",
                    "befae47f-4e0f-4891-8f06-d735f344bad7",
                    "9b696bde-f194-43d9-822d-815f37b59c2e",
                    "e508b1f5-f3ac-4a21-ab24-89118695cc64",
                    "93675e2b-ed45-4e10-8b43-c14edb33f054",
                    "ec377926-6f37-45b0-9f3c-aea6bf59644c",
                    "93675e2b-ed45-4e10-8b43-c14edb33f054",
                    "ec377926-6f37-45b0-9f3c-aea6bf59644c",
                    "1c8a371a-efde-4e05-a55b-40e0e88bf850",
                    "b3f94072-8f41-49d5-99c3-31c6ed535aa7",
                    "f8d2d165-26cb-42be-9cdf-f8550c449e54",
                    "327527e1-4a46-4f87-b978-a46be5e5770d",
                    "609dc822-faae-412c-9186-b35d665cbb42",
                    "f8d2d165-26cb-42be-9cdf-f8550c449e54",
                    "9dd170e7-ff87-4815-9a65-8667d5542337",
                    "9dd170e7-ff87-4815-9a65-8667d5542337",
                    "f1d7af1a-0411-4931-a96b-906702adce81",
                    "7807d7da-2223-43ba-9aaf-c71b467549bb",
                    "449bdf49-4b01-4e2b-9c8f-11c0e6e8fb87",
                    "85b4ba1b-0d44-4e43-926e-08d5a0363ed2",
                    "3ef0a9a6-5257-4823-897a-1a733243cce1",
                    "7807d7da-2223-43ba-9aaf-c71b467549bb",
                    "3627b0d1-a7f4-42b2-a752-a637bdc416c0",
                    "ec377926-6f37-45b0-9f3c-aea6bf59644c",
                    "3627b0d1-a7f4-42b2-a752-a637bdc416c0",
                    "9b5ca534-bd45-486c-aa3a-a17b2eab0ffb",
                    "5c20bb95-1932-4091-89bf-de3c248c2137"

            };
            
            var date = DateTime.Parse("2023-01-30 00:00:00.000");

            return context.AssetFormIO.Where(x => x.form_id == Guid.Parse("2cc61c02-7e66-4ba9-b0a5-a7169ff190e3") && x.status!=2 && x.status!= 68 &&
            wo_ids.Contains(x.wo_id.ToString())
            && x.created_at.Value < date
            ).ToList();
        }

        public InspectionsTemplateFormIO GetMasterFormByformID(Guid form_id)
        {
            return context.InspectionsTemplateFormIO.Where(x => x.form_id == form_id).FirstOrDefault();
        }
        public List<AssetFormIO> GetAssetformsByIds(List<Guid> asset_form_ids)
        {
            return context.AssetFormIO.Where(x => asset_form_ids.Contains(x.asset_form_id)).ToList();
        }
        public List<InspectionsTemplateFormIO> GetMasterFormDataByIds(List<Guid> form_ids)
        {
            return context.InspectionsTemplateFormIO.Where(x => form_ids.Contains(x.form_id)).ToList();
        }
        public AssetFormIO GetAssetFormIOByIdForTempIssue(Guid asset_form_id)
        {
            return context.AssetFormIO.Where(u => u.asset_form_id == asset_form_id)
                .Include(x=>x.WOLineIssue)
                .FirstOrDefault();
        }
        public List<WOLineIssue> GetNECTempIssueByAssetFormid(Guid asset_form_id)
        {
            return context.WOLineIssue.Where(x => x.asset_form_id == asset_form_id && !x.is_deleted && x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.NEC_Violation).ToList();
        }
        public List<WOLineIssue> GetOSHATempIssueByAssetFormid(Guid asset_form_id)
        {
            return context.WOLineIssue.Where(x => x.asset_form_id == asset_form_id && !x.is_deleted && x.issue_caused_id == (int)WOLine_Temp_Issue_Caused.OSha_Violation).ToList();
        }

        public (List<Equipment>, int) GetAllEquipmentListData(GetAllEquipmentListRequestmodel request)
        {

            IQueryable<Equipment> query = context.Equipment.Where(x=>!x.isarchive && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id));

           // if (request.site_id != null && request.site_id != Guid.Empty)
           // {
          //      query = query.Where(x => x.site_id == request.site_id);
          //  }

            
            // add equipment_number Filter
            if (request.equipment_number?.Count > 0)
            {
                query = query.Where(x => request.equipment_number.Contains(x.equipment_number));
            }
            // add manufacturer Filter
            if (request.manufacturer != null && request.manufacturer.Count > 0)
            {
                query = query.Where(x => request.manufacturer.Contains(x.manufacturer));
            }
            // add model_number Filter
            if (request.model_number != null && request.model_number.Count > 0)
            {
                query = query.Where(x => request.model_number.Contains(x.model_number));
            }
            //calibration_status Filter
            if (request.calibration_status?.Count > 0)
            {
                query = query.Where(x => request.calibration_status.Contains(x.calibration_status));
            }

            // search string
            if (!string.IsNullOrEmpty(request.search_string))
            {

                var searchstring = request.search_string.ToLower().ToString();
                query = query.Where(x => (x.equipment_number.ToLower().Contains(searchstring) || x.manufacturer.ToLower().Contains(searchstring) || x.equipment_name.ToLower().Contains(searchstring) ||
                x.model_number.ToLower().Contains(searchstring) || x.serial_number.ToLower().Contains(searchstring)));
            }

            int total_count = query.Count();

            if (request.page_index > 0 && request.page_size > 0)
            {
                query = query.OrderByDescending(x => x.created_at).Skip((request.page_index - 1) * request.page_size).Take(request.page_size);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_at);
            }

            return (query.Include(x => x.Sites).ToList(), total_count);
        }

        public Equipment GetEquipmentDataByID(Guid equipmentId)
        {
            return context.Equipment.Where(x=>x.equipment_id == equipmentId).FirstOrDefault();
        }

        public List<string> GetDropdownEquipmentNumber()
        {
            return context.Equipment.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.isarchive).Select(x => x.equipment_number).Distinct().ToList();
        }
        public List<string> GetDropdownEquipmentManufacturer()
        {
            return context.Equipment.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.isarchive).Select(x => x.manufacturer).Distinct().ToList();
        }
        public List<string> GetDropdownEquipmentModelNumber()
        {
            return context.Equipment.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.isarchive).Select(x => x.model_number).Distinct().ToList();
        }

        public List<int?> GetDropdownEquipmentCalibrationStatus()
        {
            return context.Equipment.Where(x => x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.isarchive).Select(x => x.calibration_status).Distinct().ToList();
        }

        public bool CheckForDuplicateEquipmentNumber(string equipmentNumber)
        {
            return context.Equipment.Where(x => !x.isarchive && x.equipment_number.ToLower() == equipmentNumber.ToLower()
            && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
            ).Any();
        }

        public bool CheckForDuplicateEquipmentNumberForUpdate(string equipmentNumber , Guid equipment_id)
        {
            return context.Equipment.Where(x => !x.isarchive && x.equipment_number.ToLower() == equipmentNumber.ToLower()
            && x.equipment_id != equipment_id

            && x.company_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.company_id)
            ).Any();
        }

        public Company GetCompanyById(Guid company_id)
        {
            return context.Company.Where(x => x.company_id == company_id).FirstOrDefault();
        }


        public AssetFormIO GetAssetFormIOByAssetID(Guid asset_id)
        {
            return context.AssetFormIO.Where(x => x.asset_id == asset_id)
                    .Include(x => x.WOcategorytoTaskMapping).Include(x => x.WorkOrders).FirstOrDefault();
        }

        public List<Asset> GetAllAssetsToAddPMs()
        {
            int count = context.Assets.Include(x => x.InspectionTemplateAssetClass.PMCategory.PMPlans).Include(x => x.AssetPMs)//.ThenInclude(x=>x.AssetPMPlans)
                .Where(x => x.company_id != "f1c579ce-1571-47fd-8d4b-6e3e35df3eff" && x.company_id != "22a8170a-f97c-432b-976a-28c4d7abf3ca" && x.inspectiontemplate_asset_class_id != null && x.status == (int)Status.AssetActive
                && x.InspectionTemplateAssetClass.PMCategory == null)
                //&& x.InspectionTemplateAssetClass.PMCategory.status != (int)Status.Deactive && x.InspectionTemplateAssetClass.PMCategory.PMPlans.Select(y=>y.plan_name).Contains("NFPA-70B"))
                .Count();

            int count2 = context.Assets.Include(x => x.InspectionTemplateAssetClass.PMCategory.PMPlans).Include(x => x.AssetPMs)//.ThenInclude(x=>x.AssetPMPlans)
                .Where(x => x.company_id != "f1c579ce-1571-47fd-8d4b-6e3e35df3eff" && x.company_id != "22a8170a-f97c-432b-976a-28c4d7abf3ca" && x.inspectiontemplate_asset_class_id != null && x.status == (int)Status.AssetActive
                && x.InspectionTemplateAssetClass.PMCategory.status != (int)Status.Deactive)
                .Count();

            return context.Assets.Include(x => x.InspectionTemplateAssetClass.PMCategory.PMPlans).Include(x => x.AssetPMs)//.ThenInclude(x=>x.AssetPMPlans)
                .Where(x => x.company_id != "f1c579ce-1571-47fd-8d4b-6e3e35df3eff" && x.company_id != "22a8170a-f97c-432b-976a-28c4d7abf3ca" && x.inspectiontemplate_asset_class_id != null && x.status == (int)Status.AssetActive
                && x.InspectionTemplateAssetClass.PMCategory.status != (int)Status.Deactive)
                .ToList();
        }

        public List<Asset> GetAllAssetsToAddPMs2(List<string> list)
        {
            return context.Assets//.ThenInclude(x=>x.AssetPMPlans)
                .Where(x => list.Contains(x.asset_id.ToString()))
                .Include(x=>x.AssetPMPlans)
                .ToList();
        }

        public List<AssetPMPlans> GetAssetPMPlanByAssetId(Guid asset_id)
        {
            return context.AssetPMPlans.Where(x => x.asset_id == asset_id && x.status == (int)Status.Active).ToList();
        }

        public Guid GetassetclassbyClassCode(string code,string name)
        {
            return context.InspectionTemplateAssetClass.Where(x=>x.asset_class_code == code && x.asset_class_name==name && !x.isarchive).Select(x=>x.inspectiontemplate_asset_class_id).FirstOrDefault();
        }
        public Guid GetassetclassbyClassCode(string code)
        {
            return context.InspectionTemplateAssetClass.Where(x => x.asset_class_code.ToLower().Trim() == code.ToLower().Trim() && !x.isarchive
            && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            ).Select(x => x.inspectiontemplate_asset_class_id).FirstOrDefault();
        }
        public FormIOBuildings GetFormIOBuildingByName(string building_name, Guid site_id)
        {
            return context.FormIOBuildings.Where(x => x.formio_building_name.Trim().ToLower() == building_name.Trim().ToLower()
            && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.site_id == site_id).FirstOrDefault();
        }
        public FormIOFloors GetFormIOFloorByName(string floor_name, int building_id, Guid site_id)
        {
            return context.FormIOFloors.Where(x => x.formio_floor_name.Trim().ToLower() == floor_name.Trim().ToLower()
            && x.formiobuilding_id == building_id
            && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.site_id == site_id).FirstOrDefault();
        }
        public FormIORooms GetFormIORoomByName(string room_name, int floor_id, Guid site_id)
        {
            return context.FormIORooms.Where(x => x.formio_room_name.Trim().ToLower() == room_name.Trim().ToLower() && x.formiofloor_id == floor_id
             && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.site_id == site_id).FirstOrDefault();
        }
        public FormIOSections GetFormIOSectionByName(string section_name, int room_id, Guid site_id)
        {
            return context.FormIOSections.Where(x => x.formio_section_name.Trim().ToLower() == section_name.Trim().ToLower() && x.formioroom_id == room_id
            && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            && x.site_id == site_id).FirstOrDefault();
        }
        public ClientCompany GetCCbyname(string cc_name)
        {
            return context.ClientCompany.Where(x =>
            (x.client_company_name.ToLower().Trim() == cc_name.ToLower().Trim() ||
            x.clientcompany_code.ToLower().Trim() == cc_name.ToLower().Trim())
            && x.parent_company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")).FirstOrDefault();
        }
        public Sites Getsitebyname(string site_name, Guid cc_id)
        {
            return context.Sites.Where(x => x.site_name.ToLower().Trim() == site_name.ToLower().Trim()
            && x.client_company_id == cc_id
            && x.status == 1
            && x.company_id == Guid.Parse("47aa8c4d-684a-4da2-9aad-cfbb851d3f6d")
            ).FirstOrDefault();
        }
        public List<Equipment> GetAllEquipmentListForUpdateCalStatus()
        {
            return context.Equipment.Where(x=>!x.isarchive && x.company_id != null).ToList();
        }   
        public (List<NetaInspectionBulkReportTracking>,int) GetAllNetaInspectionsReportList(GetAllNetaInspectionBulkReportTrackingListRequestModel requestModel)
        {
            IQueryable<NetaInspectionBulkReportTracking> query = context.NetaInspectionBulkReportTracking;

            query = query.Where(x=> x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id) && !x.is_deleted);

            if (requestModel.status != null && requestModel.status>0)
            {
                query = query.Where(x => x.report_status == requestModel.status);
            }

            if (requestModel.report_inspection_type != null && requestModel.report_inspection_type > 0)
            {
                query = query.Where(x => x.report_inspection_type == requestModel.report_inspection_type);
            }

            query = query.OrderByDescending(x=>x.created_at);

            int count = query.Count();

            if(requestModel.pagesize > 0 && requestModel.pageindex > 0)
            {
                query = query.Skip((requestModel.pageindex - 1) * requestModel.pagesize).Take(requestModel.pagesize);
            }

            return (query.ToList(),count);
        }

        public List<string> GetAssetNamesByAssetFormIds(List<string> asset_form_ids_list)
        {
            return context.AssetFormIO.Where(x=>asset_form_ids_list.Contains(x.asset_form_id.ToString()))
                .Select(x=>x.form_retrived_asset_name != null ? x.form_retrived_asset_name : x.Asset.name).ToList();
        }

        public int GetNetareportCountBySite()
        {
            return context.NetaInspectionBulkReportTracking.Where(x=>x.site_id == Guid.Parse(UpdatedGenericRequestmodel.CurrentUser.site_id)).Count();
        }
        public List<TempAsset> GetAllTempAssetsForScript(Guid site_id)
        {
            return context.TempAsset.Where(x => !x.is_deleted && x.site_id == site_id && x.temp_master_building_id==null)
                .Include(x => x.TempFormIOBuildings)
                .Include(x => x.TempFormIOFloors)
                .Include(x => x.TempFormIORooms)
                .Include(x => x.TempFormIOSections)
                .ToList();
        }
        public List<Guid> GetAllSitesByCompany()
        {
            List<string> site_ids = new List<string>()
                {
                    "c39cd242-1ce3-4c58-9e30-b7346c8420ef", "f4d9250f-f9c9-4ddb-adfc-de56c1584c12", "e6a74669-2303-4602-aa5c-6e794d831ff1", "b51b044c-3448-439f-94ff-8a759f15508d",
                    "928dc041-e36b-4029-a88a-9e2e557e4dff", "3e6b69e8-ed54-4439-ba64-f2009e68fa8c", "a6795a2c-6a16-46a6-8636-ebf2be306488", "94eac91c-2f49-4135-816a-52f3d6455365",
                    "663448e3-6f92-4521-a623-9468bd86107b", "b2e5ed62-90db-4194-b018-8258be4855c4", "ad55cf9c-21e6-4234-80a6-806a65d54ce5", "7fb921d6-f59d-41a9-8add-c9388cc0482f",
                    "57f88a9c-370c-41d9-ac60-569510cb67e2", "738aa5a1-755b-444d-8d71-48fb86faaf47", "11252005-1a8d-478b-83d0-e9a9e333c76b", "1a74361f-00f8-4387-8239-2dbe7e555931", 
                    "4dc3cf39-c81e-40ee-b63d-5491a5d8f181", "70177e70-6fdf-4bec-8ea4-94813de6194e"
                };

            return context.Sites.Where(x => x.status == (int)Status.Active && x.client_company_id != null && x.client_company_id != Guid.Empty
            && site_ids.Contains(x.site_id.ToString()) && (x.company_id != Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff"))
            ).Select(x => x.site_id).ToList();// x.company_id != Guid.Parse("f1c579ce-1571-47fd-8d4b-6e3e35df3eff")
        }
    }
}
