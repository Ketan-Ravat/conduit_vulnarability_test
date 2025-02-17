using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class FormAttributesRepository<T> : IDisposable, IFormAttributesRepository<T> where T : class
    {
        private readonly DBContextFactory context;
        private DbSet<T> dbSet;

        public FormAttributesRepository(DBContextFactory context)
        {
            this.context = context;
            //this.userRepository = userRepository;
            dbSet = context.Set<T>();
        }

        public bool Insert(T entity)
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
                    var response = dbSet.Add(entity);
                    IsSuccess = true;
                }
            }
            catch (Exception)
            {
                
            }
            return IsSuccess;
        }

        public List<InspectionFormAttributes> GetAll()
        {
            return context.InspectionFormAttributes.Include(x=>x.InspectionAttributeCategory).ToListAsync().Result;
        }

        public List<InspectionAttributeCategory> GetAllInspectionAttributeCategory()
        {
            return context.InspectionAttributeCategory.ToListAsync().Result;
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        private bool disposed = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
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
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

       
    }
}
