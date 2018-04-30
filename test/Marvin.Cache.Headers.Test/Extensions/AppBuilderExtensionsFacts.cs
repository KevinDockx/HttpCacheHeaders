// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marvin.Cache.Headers.Test.Extensions
{
    public class AppBuilderExtensionsFacts
    {
        [Fact]
        public void Correctly_register_HttpCacheHeadersMiddleware()
        {
            var hostBuilder = new WebHostBuilder().Configure(app => app.UseHttpCacheHeaders()).ConfigureServices(service => service.AddHttpCacheHeaders());
            var testServer = new TestServer(hostBuilder);

            // not sure this is the correct way to test if the middleware is registered
            var middleware = testServer.Host.Services.GetService(typeof(IValidationValueStore));
            Assert.NotNull(middleware);
        }

        [Fact]
        public void When_no_ApplicationBuilder_expect_ArgumentNullException()
        {
            IApplicationBuilder appBuilder = null;

            Assert.Throws<ArgumentNullException>(() => appBuilder.UseHttpCacheHeaders());
        }
    }
}