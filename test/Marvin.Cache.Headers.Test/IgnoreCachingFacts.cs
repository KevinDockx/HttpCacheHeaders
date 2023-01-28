// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Threading.Tasks;
using Marvin.Cache.Headers.Test.TestStartups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Marvin.Cache.Headers.Test
{
    public class IgnoreCachingFacts
    {
        private readonly IWebHostBuilder _hostBuilder = new WebHostBuilder()
            .UseStartup<DefaultStartup>();

        [Fact]
        public async Task Can_Ignore_Caching_Globally()
        {
            _hostBuilder.ConfigureServices(services => services.AddOptions<HttpCacheHeadersMiddlewareOptions>()
                .Configure(options => options.IgnoreCaching = true));

            var server = new TestServer(_hostBuilder);

            using var client = server.CreateClient();

            var response = await client.GetAsync("/");

            Assert.True(response.IsSuccessStatusCode);
            Assert.Empty(response.Headers);
            Assert.False(string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()));
        }

        [Fact]
        public async Task Dont_Ignore_Caching_Per_Default()
        {
            _hostBuilder.ConfigureServices(services => services.AddOptions<HttpCacheHeadersMiddlewareOptions>());

            var server = new TestServer(_hostBuilder);

            using var client = server.CreateClient();

            var response = await client.GetAsync("/");

            Assert.True(response.IsSuccessStatusCode);
            Assert.Collection(response.Headers,
                pair => Assert.True(pair.Key == HeaderNames.CacheControl),
                pair => Assert.True(pair.Key == HeaderNames.ETag),
                pair => Assert.True(pair.Key == HeaderNames.Vary));
            Assert.False(string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()));
        }
        
        [Fact]
        public async Task Can_Ignore_Status_Codes()
        {
            _hostBuilder.ConfigureServices(services => services.AddOptions<HttpCacheHeadersMiddlewareOptions>()
                .Configure(options => options.IgnoredStatusCodes = new []{ 400, 500 }));

            var server = new TestServer(_hostBuilder);

            using var client = server.CreateClient();

            //Assert ignored status codes
            var badRequestResponse = await client.GetAsync("/bad-request");
            
            Assert.Equal(400, (int) badRequestResponse.StatusCode);
            Assert.Empty(badRequestResponse.Headers);
            Assert.False(string.IsNullOrWhiteSpace(await badRequestResponse.Content.ReadAsStringAsync()));

            var serverErrorResponse = await client.GetAsync("/server-error");

            Assert.Equal(500, (int) serverErrorResponse.StatusCode);
            Assert.Empty(serverErrorResponse.Headers);
            Assert.False(string.IsNullOrWhiteSpace(await serverErrorResponse.Content.ReadAsStringAsync()));

            //Assert other status codes
            var notfoundResult = await client.GetAsync("/not-found");
            
            Assert.Equal(404, (int) notfoundResult.StatusCode);
            Assert.Collection(notfoundResult.Headers,
                pair => Assert.True(pair.Key == HeaderNames.CacheControl),
                pair => Assert.True(pair.Key == HeaderNames.Vary));
            Assert.False(string.IsNullOrWhiteSpace(await notfoundResult.Content.ReadAsStringAsync()));

            var successResponse = await client.GetAsync("/");

            Assert.Equal(200, (int) successResponse.StatusCode);
            Assert.Collection(successResponse.Headers,
                pair => Assert.True(pair.Key == HeaderNames.CacheControl),
                pair => Assert.True(pair.Key == HeaderNames.ETag),
                pair => Assert.True(pair.Key == HeaderNames.Vary));
            Assert.False(string.IsNullOrWhiteSpace(await successResponse.Content.ReadAsStringAsync()));
        }
    }
}