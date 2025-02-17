using Jarvis.db.Models;
using Jarvis.Repo.Abstract;
using Jarvis.Shared.StatusEnums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Repo.Concrete
{
    public class InspectionFormRepository : BaseGenericRepository<InspectionForms>,IInspectionFormRepository
    {
       
        public InspectionFormRepository(DBContextFactory dataContext) : base(dataContext)
        {

        }

        public bool Insert(InspectionForms entity)
        {
            bool IsSuccess = false;
            try
            {

                if (entity == null)
                {
                    return IsSuccess;
                }
                else
                {
                    var response = Add(entity);
                    IsSuccess = true;
                }
            }
            catch (Exception)
            {
                return IsSuccess;
            }
            return IsSuccess;
           
        }
        public async Task<int> Update(InspectionForms entity)
        {
            int IsSuccess = 0;
            try
            {
                var inspectionForms = context.InspectionForms.Where(x => x.inspection_form_id == entity.inspection_form_id).FirstOrDefault();
                if (inspectionForms != null && inspectionForms.inspection_form_id != Guid.Empty || inspectionForms.inspection_form_id != null)
                {
                    inspectionForms.modified_at = DateTime.UtcNow;
                    inspectionForms.modified_by = entity.modified_by;
                    inspectionForms.form_attributes = entity.form_attributes;
                    inspectionForms.name = entity.name;
                    inspectionForms.company_id = entity.company_id;
                    inspectionForms.site_id = entity.site_id;

                    dbSet.Update(inspectionForms);
                    IsSuccess = await context.SaveChangesAsync();
                }
                else
                {
                    IsSuccess = (int)ResponseStatusNumber.NotFound;
                }
            }
            catch (Exception e)
            {
                IsSuccess = (int)ResponseStatusNumber.Error;
                throw e;
            }
            return IsSuccess;
        }

        public InspectionAttributeCategory GetCategoryByID( int category_id)
        {
            return context.InspectionAttributeCategory.Where(x=>x.category_id==category_id).FirstOrDefault();
        }

        public async Task<InspectionForms> GetInspectionFromByAssetId(Guid asset_id)
        {
            InspectionForms inspectionForms = new InspectionForms();

            var inspectionform = await context.Assets.Where(x => x.asset_id == asset_id).Select(x => x.inspectionform_id).FirstOrDefaultAsync();

            if(inspectionform != null && inspectionform != Guid.Empty)
            {
                inspectionForms = await context.InspectionForms.Where(x => x.inspection_form_id == inspectionform).FirstOrDefaultAsync();
            }

            return inspectionForms;
        }

        public async Task<List<InspectionForms>> GetAllInspectionFormByCompanyId(Guid company_id)
        {
            List<InspectionForms> forms = new List<InspectionForms>();
            forms = await context.InspectionForms.Where(x => x.status == (int)Status.Active && x.company_id == company_id).ToListAsync();
            return forms;
        }

    }
}
