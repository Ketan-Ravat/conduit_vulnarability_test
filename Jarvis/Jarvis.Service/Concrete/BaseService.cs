using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Repo;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Concrete
{
    public class BaseService : IBaseService
    {
        private readonly IMapper _mapper;
        public readonly IJarvisUOW _UoW;

        public BaseService(IMapper mapper)
        {
            _mapper = mapper;
            _UoW = new JarvisUOW();
        }

        public async Task<UsersitesResponseModel> UserSites(UsersitesRequestModel requestModel)
        {
            UsersitesResponseModel responseModel = new UsersitesResponseModel();
            using (var _dbtransaction = _UoW.BeginTransaction())
            {
                bool result = false;
                Guid usid = Guid.Empty;
                try
                {
                    var usersitesmodel = _mapper.Map<UserSites>(requestModel);
                    if (usersitesmodel.usersite_id == Guid.Empty)
                    {
                        usersitesmodel.created_at = DateTime.UtcNow;
                        result = await _UoW.BaseGenericRepository<UserSites>().Insert(usersitesmodel);
                        if (result)
                        {
                            _UoW.SaveChanges();
                            usid = usersitesmodel.usersite_id;
                        }
                    }
                    else
                    {
                        usersitesmodel.modified_at = DateTime.UtcNow;
                        result = _UoW.BaseGenericRepository<UserSites>().Update(usersitesmodel).Result;
                        usid = usersitesmodel.usersite_id;
                    }
                    _dbtransaction.Commit();

                    if (result)
                    {
                        var resp = await _UoW.UserRepository.FindUserSitesDetails(usid);

                        responseModel = _mapper.Map<UsersitesResponseModel>(resp);
                        responseModel.result = (int)ResponseStatusNumber.Success;
                    }
                    else
                    {
                        responseModel.result = (int)ResponseStatusNumber.False;
                    }
                }
                catch (Exception e)
                {
                    _dbtransaction.Rollback();
                }
                return responseModel;
            }
        }

    }
}
