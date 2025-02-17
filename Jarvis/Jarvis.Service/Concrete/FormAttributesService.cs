using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class FormAttributesService : BaseService, IFormAttributesService
    {
        public readonly IMapper _mapper;

        public FormAttributesService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public async Task<bool> AddFormAttributes(FormAttributesViewModel attribute)
        {
            bool result = false;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var AddAttributerequest = _mapper.Map<InspectionFormAttributes>(attribute);
                    result = _UoW.FormAttributesRepository<InspectionFormAttributes>().Insert(AddAttributerequest);
                    if (result)
                    {
                        _UoW.SaveChanges();
                        _dbtransaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Exception to add attributes ", e.Message);
                    _dbtransaction.Rollback();
                }
            }
            return result;
        }

        public List<FormAttributesViewModel> GetAllInspectionAttributes()
        {
            List<FormAttributesViewModel> listofAttributes = new List<FormAttributesViewModel>();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var result = _UoW.FormAttributesRepository<InspectionFormAttributes>().GetAll();
                    if (result!=null)
                    {
                        listofAttributes = _mapper.Map<List<FormAttributesViewModel>>(result);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Exception to add attributes ", e.Message);
                    _dbtransaction.Rollback();
                }
            }
            return listofAttributes;
        }

        public List<InspectionAttributeCategoryViewModel> GetAllInspectionAttributsCategory()
        {
            List<InspectionAttributeCategoryViewModel> listofinspectionAttributeCategory = new List<InspectionAttributeCategoryViewModel>();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var result = _UoW.FormAttributesRepository<InspectionAttributeCategory>().GetAllInspectionAttributeCategory();
                    if (result != null)
                    {
                        listofinspectionAttributeCategory = _mapper.Map<List<InspectionAttributeCategoryViewModel>>(result);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Exception to add attributes ", e.Message);
                    _dbtransaction.Rollback();
                }
            }
            return listofinspectionAttributeCategory;

        }

    }
}
