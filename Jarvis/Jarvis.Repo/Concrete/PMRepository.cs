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

namespace Jarvis.Repo.Concrete {
    public class PMRepository : BaseGenericRepository<PMs>, IPMRepository {
        public PMRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(PMs entity)
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
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> InsertList(IEnumerable<PMs> entity)
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
                    AddRange(entity);
                    IsSuccess = (int)ResponseStatusNumber.Success;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(PMs entity)
        {
            int IsSuccess = 0;
            try
            {
                dbSet.Update(entity);
                IsSuccess = await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public async Task<PMs> GetPMsById(Guid pm_id)
        {
            return await context.PMs.Where(u => u.pm_id == pm_id)
                .Include(x => x.PMTasks).ThenInclude(x => x.Tasks)
                .Include(x => x.StatusMaster).Include(x => x.PMByStatus)
                .Include(x => x.PMTypeStatus).Include(x => x.PMDateTimeRepeatTypeStatus)
                .Include(x => x.PMAttachments)
                .Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                .Include(x=>x.PMsTriggerConditionMapping)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PMs>> GetAllPMsByPlan(Guid pm_plan_id)
        {
            return await context.PMs.Where(u => u.pm_plan_id == pm_plan_id && u.is_archive != true)
               // .Include(x => x.PMTasks).ThenInclude(x => x.Tasks)
               // .Include(x => x.StatusMaster).Include(x => x.PMByStatus)
              //  .Include(x => x.PMTypeStatus).Include(x => x.PMDateTimeRepeatTypeStatus)
                .Include(x => x.PMsTriggerConditionMapping)
               // .Include(x => x.PMAttachments)
               //    .Include(x => x.ServiceDealers).ThenInclude(x => x.StatusMaster)
                .OrderBy(x => x.created_at).ToListAsync();
        }
        public List<Issue> get_cvs_issue()
        {
            return context.Issue.Where(x => x.site_id == Guid.Parse("01c64534-ae74-43c8-8902-ff8386cd48f4")).ToList();
        }
        public List<Asset> Getallassetstovalidatejson()
        {
            return context.Assets.Where(x => x.form_retrived_nameplate_info != null).ToList();
        }

        public List<PMs> GetPMsListByAssetClassId(GetPMsListByAssetClassIdRequestModel requestModel)
        {
            Guid? inspectiontemplate_asset_class_id = null;
            List<Guid> asset_planned_pms = new List<Guid>();
            if (requestModel.asset_id != null)
            {
                var asset = context.Assets.Where(x => x.asset_id == requestModel.asset_id).FirstOrDefault();
                inspectiontemplate_asset_class_id = asset.inspectiontemplate_asset_class_id;

                var planned_pm_plan = context.AssetPMPlans.Where(x => x.asset_id == requestModel.asset_id
                                                                                && !x.is_pm_plan_inspection_manual
                                                                                && x.status == (int)Status.Active
                                                                                ).Include(x=>x.AssetPMs)
                                                                                .FirstOrDefault(); ;
                asset_planned_pms = planned_pm_plan.AssetPMs.Select(x=>x.pm_id.Value).ToList();

            }
            if (requestModel.inspectiontemplate_asset_class_id != null)
            {
                inspectiontemplate_asset_class_id = requestModel.inspectiontemplate_asset_class_id;
            }
            IQueryable<PMs> query = context.PMs.Where(x => x.PMPlans.PMCategory.inspectiontemplate_asset_class_id == inspectiontemplate_asset_class_id &&
            x.status == (int)Status.Active && !x.is_archive);

            if (asset_planned_pms.Count > 0)
            {
                query = query.Where(x => !asset_planned_pms.Contains(x.pm_id));
            }

            // search string
            if (!string.IsNullOrEmpty(requestModel.search_string))
            {
                var searchstring = requestModel.search_string.ToLower().ToString();
                query = query.Where(x => (x.PMPlans.plan_name.ToLower().Contains(searchstring) 
                || x.title.ToLower().Contains(searchstring) ));
            }

            return query.Include(x=>x.PMPlans).Include(x => x.PMPlans.PMCategory).ToList();
        }

        public List<PMPlans> GetPMPlansListByPMIds(List<Guid> pm_ids)
        {
            var plan_ids = context.PMs.Where(u => pm_ids.Contains(u.pm_id) && !u.is_archive).Select(x=>x.pm_plan_id).Distinct().ToList();

            return context.PMPlans.Where(u => plan_ids.Contains(u.pm_plan_id))
                .Include(x => x.PMs).ThenInclude(x=>x.PMAttachments)
                .ToList();
        }

        public AssetPMPlans CheckForPMPlanIsAny(Guid pm_plan_id, Guid asset_id)
        {
            return context.AssetPMPlans.Where(x => x.pm_plan_id == pm_plan_id && x.asset_id == asset_id && x.status ==(int)Status.Active).FirstOrDefault();
        }
        public string GetAssetClassCodeByAssetId(GetPMsListByAssetClassIdRequestModel requestModel)
        {
            string asset_class_code = null;
            if (requestModel.asset_id != null)
            {
                var asset = context.Assets.Where(x => x.asset_id == requestModel.asset_id).Include(x=>x.InspectionTemplateAssetClass).FirstOrDefault();
                asset_class_code = asset.InspectionTemplateAssetClass.asset_class_code;
            }
            if (requestModel.inspectiontemplate_asset_class_id != null)
            {
                asset_class_code = context.InspectionTemplateAssetClass.Where(x => x.inspectiontemplate_asset_class_id == requestModel.inspectiontemplate_asset_class_id)
                    .Select(x=>x.asset_class_code).FirstOrDefault();
            }
            return asset_class_code;
        }
    }
}
