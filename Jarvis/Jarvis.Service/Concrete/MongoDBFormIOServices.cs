using AutoMapper;
using Jarvis.Service.Abstract;
using Jarvis.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Service.Concrete
{
    public class MongoDBFormIOServices : BaseService,IMongoDBFormIOServices
    {
        public readonly IMapper _mapper;
        private Logger _logger;

        public MongoDBFormIOServices(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
            _logger = Logger.GetInstance<MongoDBFormIOServices>();
        }

    }
}
