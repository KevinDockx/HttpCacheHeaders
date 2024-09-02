// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Microsoft.AspNetCore.Mvc;

namespace Marvin.Cache.Headers.Sample.Controllers;

[Route("api/morevalues")]
[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 11111)]
[HttpCacheValidation(MustRevalidate = false)]
public class MoreValuesController : Controller
{
    [HttpGet]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Private, MaxAge = 99999)]
    [HttpCacheValidation(MustRevalidate = true, Vary = new[] { "Test" })]
    public IEnumerable<string> Get()
    {
        return new[] { "anothervalue1", "anothervalue2" };
    }

    // no expiration/validation attributes: must take controller level config
    [HttpGet("{id}")]
    public string GetOne(string id)
    {
        return "somevalue";
    }
}
