using AutoMapper;
using Jarvis.db.Models;
using Jarvis.Repo;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels.RequestResponseViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Services
{
    public class SyncRecord
    {
        private readonly IMapper _mapper;

        public readonly IJarvisUOW _UoW;

        public SyncRecord(IMapper mapper)
        {
            _UoW = new JarvisUOW();
            _mapper = mapper;
        }

        public async Task<bool> Insert(InsertSyncRecordRequestModel requestModel)
        {
            int result = (int)ResponseStatusNumber.Error;
            RecordSyncInformation record = _mapper.Map<RecordSyncInformation>(requestModel);
            record.created_at = DateTime.UtcNow;
            bool insert = await _UoW.BaseGenericRepository<RecordSyncInformation>().Insert(record);
            if (insert)
            {
                _UoW.SaveChanges();
                result = (int)ResponseStatusNumber.Success;
            }
            return result > 0;
        }
    }
}
