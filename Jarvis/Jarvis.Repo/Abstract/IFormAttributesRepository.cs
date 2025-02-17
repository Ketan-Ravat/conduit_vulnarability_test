using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IFormAttributesRepository<T> where T : class
    {
        bool Insert(T entity);
        List<InspectionFormAttributes> GetAll();

        List<InspectionAttributeCategory> GetAllInspectionAttributeCategory();
    }
}
