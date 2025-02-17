using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jarvis.ViewModels.RequestResponseViewModel;

namespace Jarvis.Repo.Concrete
{
    public class PMCategoryRepository : BaseGenericRepository<PMCategory>, IPMCategoryRepository
    {
        public PMCategoryRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(PMCategory entity)
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
                    var alreadyregister = context.PMCategory.Where(x => x.category_code == entity.category_code).FirstOrDefault();
                    if (alreadyregister != null)
                    {
                        if (alreadyregister.pm_category_id == null || alreadyregister.pm_category_id == Guid.Empty)
                        {
                            Add(entity);
                            IsSuccess = (int)ResponseStatusNumber.Success;
                        }
                        else
                        {
                            IsSuccess = (int)ResponseStatusNumber.AlreadyExists;
                        }
                    }
                    else
                    {
                        Add(entity);
                        IsSuccess = (int)ResponseStatusNumber.Success;
                    }
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<int> Update(PMCategory entity)
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

        public async Task<PMCategory> GetPMCategoryById(Guid pm_category_id)
        {
            return await context.PMCategory.Where(u => u.pm_category_id == pm_category_id)
                .Include(x => x.PMPlans).Include(x => x.StatusMaster).FirstOrDefaultAsync();
        }

        public async Task<List<PMCategory>> GetAllPMCategories()
        {
            return await context.PMCategory.Where(u => u.status == (int)Status.Active 
                 && (u.company_id.ToString() == UpdatedGenericRequestmodel.CurrentUser.company_id || u.company_id == null))
                .Include(x => x.PMPlans)
                //.ThenInclude(x => x.PMs)
                .OrderBy(x=>x.category_name).ToListAsync();
        }
    }
}
