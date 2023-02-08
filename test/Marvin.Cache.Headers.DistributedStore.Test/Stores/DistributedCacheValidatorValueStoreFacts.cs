using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores;

public class DistributedCacheValidatorValueStoreFacts
{
    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_DistributedCache_Is_Passed_in()
    {
        IDistributedCache distributedCache = null;
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var exception = Record.Exception(() => new DistributedCacheValidatorValueStore(distributedCache, distributedCacheKeyRetriever.Object));
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_DistributedCacheKeyRetriever_Is_Passed_in2()
    {
        var distributedCache = new Mock<IDistributedCache>();
        IRetrieveDistributedCacheKeys distributedCacheKeyRetriever = null;
        var exception = Record.Exception(() => new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever));
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructs_An_DistributedCacheValidatorValueStore_When_All_The_Parameters_Passed_In_Are_Not_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        DistributedCacheValidatorValueStore distributedCacheValidatorValueStore = null;
        var exception = Record.Exception(() => distributedCacheValidatorValueStore =new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object));
        Assert.Null(exception);
        Assert.NotNull(distributedCacheValidatorValueStore);
    }

    [Fact]
    public async Task GetAsync_Throws_An_ArgumentNullException_When_The_Key_Is_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        StoreKey key = null;
        var distributedCacheValidatorValueStore =new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var exception = await Record.ExceptionAsync(() => distributedCacheValidatorValueStore.GetAsync(key));
        Assert.IsType<ArgumentNullException>(exception);
        distributedCache.Verify(x =>x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task GetAsync_Returns_Null_When_The_Key_Is_Not_In_The_Cache()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var key = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var keyString = key.ToString();
        distributedCache.Setup(x => x.GetAsync(keyString, CancellationToken.None)).Returns(Task.FromResult<byte[]?>(null));
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var result = await distributedCacheValidatorValueStore.GetAsync(key);
        Assert.Null(result);
        distributedCache.Verify(x => x.GetAsync(It.Is<string>(x =>x.Equals(keyString, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_Returns_The_Value_From_The_Cache_When_The_Key_Is_Found_In_The_Cache()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var key = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var keyString = key.ToString();
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        var eTagString = $"{ETagType.Strong} Value=\"Test\" LastModified={referenceTime.ToString(CultureInfo.InvariantCulture)}";
        var eTagBytes = Encoding.UTF8.GetBytes(eTagString);
        distributedCache.Setup(x => x.GetAsync(keyString, CancellationToken.None)).ReturnsAsync(eTagBytes);
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var result = await distributedCacheValidatorValueStore.GetAsync(key);
        Assert.Equal(result.LastModified, eTag.LastModified);
        Assert.Equal(result.ETag.ETagType, eTag.ETag.ETagType);
        Assert.Equal(result.ETag.Value, eTag.ETag.Value);
        distributedCache.Verify(x => x.GetAsync(It.Is<string>(x => x.Equals(keyString, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Once);
    }
}