// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Marvin.Cache.Headers.Test;

public class MvcConfigurationFacts
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory =new WebApplicationFactory<Program>();

    [Fact]
    public async Task Adds_Default_Validation_And_ExpirationHeaders()
    {
        using (var client = _webApplicationFactory.CreateDefaultClient())
        {
            var response = await client.GetAsync("/api/values");

            Assert.True(response.IsSuccessStatusCode);

            Assert.Contains(response.Headers, pair => pair.Key == HeaderNames.CacheControl && pair.Value.First() == "public, must-revalidate, max-age=99999");

            var response2 = await client.GetAsync("/api/values/1");

            Assert.True(response2.IsSuccessStatusCode);

            Assert.Contains(response2.Headers, pair => pair.Key == HeaderNames.CacheControl && pair.Value.First() == "max-age=1337, private");
        }
    }

    [Fact]
    public async Task Method_Level_Validation_And_ExpirationHeaders_Override_Class_Level()
    {
        using (var client = _webApplicationFactory.CreateDefaultClient())
        {
            var response = await client.GetAsync("/api/morevalues");

            Assert.True(response.IsSuccessStatusCode);

            Assert.Contains(response.Headers, pair => pair.Key == HeaderNames.CacheControl && pair.Value.First() == "must-revalidate, max-age=99999, private");
        }
    }
}