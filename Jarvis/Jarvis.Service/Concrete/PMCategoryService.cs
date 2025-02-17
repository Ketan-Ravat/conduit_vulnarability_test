using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class PMCategoryService : BaseService, IPMCategoryService
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public PMCategoryService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<PMCategoryService>();
        }

        public async Task<PMCategoryResponseModel> AddUpdatePMCategory(PMCategoryRequestModel pmCategoryRequest)
        {
            PMCategoryResponseModel pMCategoryResponse = new PMCategoryResponseModel();
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    if (pmCategoryRequest.pm_category_id != null && pmCategoryRequest.pm_category_id != Guid.Empty)
                    {
                        var categoryDetails = await _UoW.PMCategoryRepository.GetPMCategoryById(pmCategoryRequest.pm_category_id);
                        {
                            categoryDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            categoryDetails.modified_at = DateTime.UtcNow;
                            categoryDetails.category_name = pmCategoryRequest.category_name;
                            result = _UoW.PMCategoryRepository.Update(categoryDetails).Result;
                            if (result > 0)
                            {
                                pMCategoryResponse = _mapper.Map<PMCategoryResponseModel>(categoryDetails);
                                _dbtransaction.Commit();
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                        pMCategoryResponse.response_status = result;
                    }
                    else
                    {
                        pmCategoryRequest.company_id = UpdatedGenericRequestmodel.CurrentUser.company_id.ToString();
                        pmCategoryRequest.created_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                        pmCategoryRequest.created_at = DateTime.UtcNow;
                        var category_code = GenerateRandomString.RandomString(8);
                        pmCategoryRequest.category_code = category_code;
                        pmCategoryRequest.status = (int)Status.Active;
                        var pmCategory = _mapper.Map<PMCategory>(pmCategoryRequest);
                        result = await _UoW.PMCategoryRepository.Insert(pmCategory);
                        if (result > 0)
                        {
                            pMCategoryResponse = _mapper.Map<PMCategoryResponseModel>(pmCategory);
                            pMCategoryResponse.response_status = result;
                            _UoW.SaveChanges();
                            _dbtransaction.Commit();
                        }
                        else
                        {
                            pMCategoryResponse.response_status = result;
                        }
                    }

                }
                catch (Exception)
                {
                    _dbtransaction.Rollback();
                    pMCategoryResponse.response_status = (int)ResponseStatusNumber.Error;
                }
            }

            return pMCategoryResponse;
        }

        public async Task<ListViewModel<PMCategoryResponseModel>> GetAllPMCategory()
        {
            ListViewModel<PMCategoryResponseModel> pMCategoryResponse = new ListViewModel<PMCategoryResponseModel>();
            try
            {
                var categoryDetails = await _UoW.PMCategoryRepository.GetAllPMCategories();
                if (categoryDetails?.Count > 0)
                {
                    pMCategoryResponse.list = _mapper.Map<List<PMCategoryResponseModel>>(categoryDetails);
                   /* pMCategoryResponse.list.ForEach(x =>
                    {
                        x.PMPlans = x.PMPlans.Where(x => x.status == (int)Status.Active).ToList();
                        x.PMPlans.ForEach(y =>
                        {
                            y.PMs = y.PMs.Where(pm => !pm.is_archive).ToList();
                        });
                    });*/
                    pMCategoryResponse.listsize = pMCategoryResponse.list.Count;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return pMCategoryResponse;
        }

        public async Task<PMCategoryResponseModel> GetPMCategoryByID(Guid id)
        {
            PMCategoryResponseModel pMCategoryResponse = new PMCategoryResponseModel();
            try
            {
                var categoryDetails = await _UoW.PMCategoryRepository.GetPMCategoryById(id);
                if (categoryDetails?.pm_category_id != null)
                {
                    pMCategoryResponse = _mapper.Map<PMCategoryResponseModel>(categoryDetails);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return pMCategoryResponse;
        }

        public async Task<int> DeletePMCategoryByID(Guid id)
        {
            int result = (int)ResponseStatusNumber.Error;
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                try
                {
                    var categoryDetails = await _UoW.PMCategoryRepository.GetPMCategoryById(id);
                    if (categoryDetails?.pm_category_id != null)
                    {
                        categoryDetails.PMPlans = categoryDetails.PMPlans.Where(x => x.status == (int)Status.Active).ToList();
                        if (categoryDetails.PMPlans?.Count > 0)
                        {
                            result = (int)ResponseStatusNumber.PMPlansExist;
                        }
                        else
                        {
                            categoryDetails.status = (int)Status.Deactive;
                            categoryDetails.modified_at = DateTime.UtcNow;
                            categoryDetails.modified_by = UpdatedGenericRequestmodel.CurrentUser.requested_by.ToString();
                            result = _UoW.PMCategoryRepository.Update(categoryDetails).Result;
                            if (result > 0)
                            {
                                _dbtransaction.Commit();
                                result = (int)ResponseStatusNumber.Success;
                            }
                            else
                            {
                                _dbtransaction.Rollback();
                            }
                        }
                    }
                    else
                    {
                        result = (int)ResponseStatusNumber.NotFound;
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                    throw e;
                }
            }

            return result;
        }
    }
}
