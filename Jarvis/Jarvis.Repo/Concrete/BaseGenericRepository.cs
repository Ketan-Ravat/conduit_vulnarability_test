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
    public class BaseGenericRepository<TEntity> : IDisposable, IBaseGenericRepository<TEntity> where TEntity : class
    {
        internal DBContextFactory context;
        internal DbSet<TEntity> dbSet;
        private bool disposed = false;

        public BaseGenericRepository(DBContextFactory context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        public TEntity Get(Guid id)
        {
            // Here we are working with a DbContext, not PlutoContext. So we don't have DbSets 
            // such as Courses or Authors, and we need to use the generic Set() method to access them.
            return context.Set<TEntity>().Find(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbSet.AsEnumerable();
        }

        public virtual IEnumerable<TEntity> GetUsers()
        {
            return dbSet.AsEnumerable();
        }

        public object Add(TEntity entity)
        {
            var response = context.Set<TEntity>().Add(entity);
            return response;
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().AddRange(entities);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<bool> Insert(TEntity entity)
        {
            bool IsSuccess = false;
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                else
                {
                    dbSet.Add(entity);
                    IsSuccess = true;
                }
            }
            catch (Exception e)
            {
                IsSuccess = false;
                throw e;
            }
            return IsSuccess;
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            bool IsSuccess = false;
            try
            {
                dbSet.Update(entity);
                var response = await context.SaveChangesAsync();
                if (response > 0)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                IsSuccess = false;
                throw e;
            }
            return IsSuccess;
        }

        //public virtual async Task<bool> UpdateList(List<TEntity> entities)
        //{
        //    bool IsSuccess = false;
        //    try
        //    {
        //        dbSet.UpdateRange(entities);
        //        var response = await context.SaveChangesAsync();
        //        if (response > 0)
        //        {
        //            IsSuccess = true;
        //        }
        //        else
        //        {
        //            IsSuccess = false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        IsSuccess = false;
        //        throw e;
        //    }
        //    return IsSuccess;
        //}

        public virtual bool UpdateList(List<TEntity> entities)
        {
            dbSet.UpdateRange(entities);
            var response = context.SaveChanges();
            return response > 0;
        }
        
        public virtual async Task<bool> Delete(IEnumerable<TEntity> entity)
        {
            bool IsSuccess = false;
            try
            {
                dbSet.RemoveRange(entity);
                var response = await context.SaveChangesAsync();
                if (response > 0)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                IsSuccess = false;
                throw e;
            }
            return IsSuccess;
        }

        public virtual string GetPreferLangName(string key_name,int lang_type)
        {
            string name = "";
            
            if(lang_type == (int)Language.spanish)
            {
                name = context.PreferLanguageMaster.Where(x => x.key_name == key_name).Select(x => x.spanish_name).FirstOrDefault();
            }

            return name;
        }

    }
}
