using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IPMCategoryRepository
    {
        Task<int> Insert(PMCategory entity);
        Task<int> Update(PMCategory entity);
        Task<PMCategory> GetPMCategoryById(Guid pm_category_id);
        Task<List<PMCategory>> GetAllPMCategories();
    }
}
