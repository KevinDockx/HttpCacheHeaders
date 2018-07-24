using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Sample.Controllers
{
    [Route("api/morevalues")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 11111)]
    [HttpCacheValidation(MustRevalidate = false)]
    public class MoreValuesController : Controller
    {   
        [HttpGet]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 99999)]
        [HttpCacheValidation(MustRevalidate = true)]
        public IEnumerable<string> Get()
        {
            return new[] { "anothervalue1", "anothervalue2" };
        }
    }
}
