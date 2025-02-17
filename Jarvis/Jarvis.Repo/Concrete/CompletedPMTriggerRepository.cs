using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class CompletedPMTriggerRepository : BaseGenericRepository<CompletedPMTriggers>, ICompletedPMTriggerRepository
    {
        public CompletedPMTriggerRepository(DBContextFactory dataContext) : base(dataContext)
        {
            this.context = dataContext;
        }
        public virtual async Task<int> Insert(CompletedPMTriggers entity)
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

        public virtual async Task<int> Update(CompletedPMTriggers entity)
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
    }
}
