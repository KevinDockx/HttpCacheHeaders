using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Marvin.Cache.Headers.Sample.Controllers
{ 
    [ApiController]
    public class AttributesController : ControllerBase
    {
        [HttpGet("api/ignoreattribute")]
        [HttpCacheIgnore]
        public ActionResult TestIgnoreAttribute()
        {
            return Ok("Test ignore attribute action");
        }
    }
}
