// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Marvin.Cache.Headers.Sample.Controllers
{
    [Route("api/values")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [HttpCacheExpiration(cacheLocation: CacheLocation.Public, maxAge: 99999)]
        [HttpCacheValidation(addMustRevalidate: true)]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [HttpCacheExpiration(cacheLocation: CacheLocation.Private, maxAge: 1337)]
        [HttpCacheValidation]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
