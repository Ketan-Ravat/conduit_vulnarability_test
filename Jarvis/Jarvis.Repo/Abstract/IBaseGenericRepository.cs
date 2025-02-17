using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IBaseGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetUsers();
        Task<bool> Insert(TEntity entity);
        Task<bool> Update(TEntity entity);
        //Task<bool> UpdateList(List<TEntity> entities);
        bool UpdateList(List<TEntity> entities);
        Task<bool> Delete(IEnumerable<TEntity> entity);
        string GetPreferLangName(string name, int lang_type);
    }
}
