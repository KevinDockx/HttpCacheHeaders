// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Marvin.Cache.Headers.Test.Stores;

public class ETagInjectorFacts
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenETagGeneratorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultETagInjector(null));
    }

    [Theory]
    [InlineData("payload")]
    [InlineData("")]
    public async Task RetrieveETag_Returns_ETag_BasedOnResponseBody(string payload)
    {
        // arrange
        var eTagGenerator = new Mock<IETagGenerator>();

        eTagGenerator
            .Setup(x => x.GenerateETag(It.IsAny<StoreKey>(), It.IsAny<string>()))
            .ReturnsAsync(new ETag(ETagType.Strong, "B56"));

        var httpContext = new Mock<IHttpContextAccessor>();
        httpContext.Setup(x => x.HttpContext.Response.Body)
            .Returns(new MemoryStream(Encoding.UTF8.GetBytes(payload)));

        var target = new DefaultETagInjector(eTagGenerator.Object);

        // act
        var result = await target.RetrieveETag(new ETagContext(new StoreKey(), httpContext.Object.HttpContext));

        // assert
        Assert.NotNull(result);
        Assert.Equal(ETagType.Strong, result.ETagType);
        Assert.Equal("B56", result.Value);
        eTagGenerator.Verify(x => x.GenerateETag(It.IsAny<StoreKey>(), payload), Times.Exactly(1));
        httpContext.Verify(x => x.HttpContext.Response.Body, Times.AtLeastOnce());
    }
}