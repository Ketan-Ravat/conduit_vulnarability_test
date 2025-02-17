using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Filter;
using Jarvis.ViewModels.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(ValidationFilterAttribute))]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET: api/student
        [HttpGet]
        public IEnumerable<string> Index()
        {
            //throw new InvalidCastException();
            return new string[] { "value1", "value2" };
        }

        // GET: api/student/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
    }
}