using Microsoft.AspNetCore.Mvc;

namespace Marvin.Cache.Headers.Sample.NET6.Controllers
{

    [Route("api/values")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 99999)]
        [HttpCacheValidation(MustRevalidate = true)]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 1337)]
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