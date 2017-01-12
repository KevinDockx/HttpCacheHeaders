using System.Linq;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Test.TestStartups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Marvin.Cache.Headers.Test
{
    public class DefaultConfigurationFacts
    {
        private readonly IWebHostBuilder _hostBuilder = new WebHostBuilder()
            .UseStartup<DefaultStartup>();

        private readonly TestServer _server;

        public DefaultConfigurationFacts()
        {
            _server = new TestServer(_hostBuilder);
        }

        [Fact]
        public async Task Adds_Default_Validation_And_ExpirationHeaders()
        {
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync("/");

                Assert.True(response.IsSuccessStatusCode);

                Assert.Collection(response.Headers,
                    pair => Assert.True(pair.Key == HeaderNames.CacheControl && pair.Value.First() == "public, max-age=60"),
                    pair => Assert.True(pair.Key == HeaderNames.ETag),
                    pair =>
                    {
                        Assert.True(pair.Key == HeaderNames.Vary);
                        Assert.Collection(response.Headers.Vary,
                            vary => Assert.Equal("Accept", vary),
                            vary => Assert.Equal("Accept-Language", vary),
                            vary => Assert.Equal("Accept-Encoding", vary));
                    });
            }
        }

        [Fact]
        public async Task Returns_Same_Etag_For_Same_Request()
        {
            using (var client = _server.CreateClient())
            {
                var response1 = await client.GetAsync("/");
                var response2 = await client.GetAsync("/");

                Assert.True(response1.IsSuccessStatusCode);
                Assert.True(response2.IsSuccessStatusCode);

                Assert.Equal(
                    response1.Headers.GetValues(HeaderNames.ETag).First(),
                    response2.Headers.GetValues(HeaderNames.ETag).First());
            }
        }

        [Fact]
        public async Task Returns_Different_Etag_For_Different_Request()
        {
            using (var client = _server.CreateClient())
            {
                var response1 = await client.GetAsync("/foo");
                var response2 = await client.GetAsync("/bar");

                Assert.True(response1.IsSuccessStatusCode);
                Assert.True(response2.IsSuccessStatusCode);

                Assert.NotEqual(
                    response1.Headers.GetValues(HeaderNames.ETag).First(),
                    response2.Headers.GetValues(HeaderNames.ETag).First());
            }
        }
    }
}