using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.ViewModels;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class InspectionFormService : BaseService, IInspectionFormService
    {
        public readonly IMapper _mapper;

        public InspectionFormService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public async Task<bool> AddInspectionForm(InspectionFormRequestViewModel inspectionform)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var inspectionForms = _mapper.Map<InspectionForms>(inspectionform);
                    if (inspectionForms.inspection_form_id != Guid.Empty && inspectionForms.inspection_form_id != null)
                    {
                        inspectionForms.modified_at = DateTime.UtcNow;
                        inspectionForms.modified_by = inspectionform.uid;

                        var update_result = await _UoW.InspectionFormRepository.Update(inspectionForms);
                        if (update_result > 0)
                        {
                            _dbtransaction.Commit();
                        }
                        else
                        {
                            _dbtransaction.Rollback();
                        }
                    }
                    else
                    {
                        inspectionForms.modified_at = DateTime.UtcNow;
                        inspectionForms.created_at = DateTime.UtcNow;
                        inspectionForms.created_by = inspectionform.uid;
                        result = _UoW.InspectionFormRepository.Insert(inspectionForms);
                        if (result)
                        {
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                        }
                        else
                        {
                            _dbtransaction.Rollback();
                        }
                    }

                }
                catch (Exception e)
                {
                    Logger.Log("Exception to add inspection forms ", e.Message);
                    _dbtransaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> AddInspectionFormAttributes(InspectionFormAttributesRequestModel inspectionFormAttributes)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var inspectionFormsAttributes = _mapper.Map<InspectionFormAttributes>(inspectionFormAttributes);
                    result = await _UoW.BaseGenericRepository<InspectionFormAttributes>().Insert(inspectionFormsAttributes);
                    if (result)
                    {
                        _UoW.SaveChanges();
                        _dbtransaction.Commit();
                    }
                    else
                    {
                        _dbtransaction.Rollback();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Exception to add inspection forms ", e.Message);
                    _dbtransaction.Rollback();
                }
            }
            return result;
        }

        public InspectionAttributeCategoryViewModel GetAttributesCategoryByID(int category_id)
        {
            InspectionAttributeCategoryViewModel responseModel = new InspectionAttributeCategoryViewModel();
            try
            {
                var response = _UoW.InspectionFormRepository.GetCategoryByID(category_id);
                if (response != null && response.category_id > 0)
                {
                    responseModel = _mapper.Map<InspectionAttributeCategoryViewModel>(response);
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error in Get All Inspections " + e.Message);
                throw e;
            }
            return responseModel;
        }

        public async Task<ListViewModel<InspectionFormDataViewModel>> GetAllInspectionFormByCompanyId(Guid company_id)
        {
            ListViewModel<InspectionFormDataViewModel> response = new ListViewModel<InspectionFormDataViewModel>();
            try
            {
                var forms = await _UoW.InspectionFormRepository.GetAllInspectionFormByCompanyId(company_id);
                if (forms.Count > 0)
                {
                    response.list = _mapper.Map<List<InspectionFormDataViewModel>>(forms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
    }
}