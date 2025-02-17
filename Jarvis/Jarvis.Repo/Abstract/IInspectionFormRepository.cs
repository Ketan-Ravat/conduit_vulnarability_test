using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Abstract
{
    public interface IInspectionFormRepository
    {
        bool Insert(InspectionForms entity);
        Task<int> Update(InspectionForms entity);
        InspectionAttributeCategory GetCategoryByID(int category_id);

        Task<InspectionForms> GetInspectionFromByAssetId(Guid asset_id);

        Task<List<InspectionForms>> GetAllInspectionFormByCompanyId(Guid company_id);
    }
}
