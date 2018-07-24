using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Sample.Controllers
{
    public class ExceptionsController : Controller
    {
        [HttpGet("api/exception")]
        public IEnumerable<string> TestException(int id)
        {
            // test for issue https://github.com/KevinDockx/HttpCacheHeaders/issues/43
            throw new Exception("Test exception");
        }
    }
}
