using System;
using Microsoft.AspNetCore.Builder;
using Moq;
using Xunit;

namespace Marvin.Cache.Headers.Test.Extensions
{
    public class AppBuilderExtensionsFacts
    {
        [Fact(Skip = "The Verify throws an exception because UseMiddleware is an extension function as well and can't be mocked, need to find ")]
        public void Correctly_register_HttpCacheHeadersMiddleware()
        {
            var appBuilderMock = new Mock<IApplicationBuilder>();
            appBuilderMock.Object.UseHttpCacheHeaders();

            appBuilderMock.Verify(x => x.UseMiddleware<HttpCacheHeadersMiddleware>(), "Application builder isn't registering the middleware.");
        }

        [Fact]
        public void When_no_ApplicationBuilder_expect_ArgumentNullException()
        {
            IApplicationBuilder appBuilder = null;

            Assert.Throws<ArgumentNullException>(() => appBuilder.UseHttpCacheHeaders());

        }
    }
}