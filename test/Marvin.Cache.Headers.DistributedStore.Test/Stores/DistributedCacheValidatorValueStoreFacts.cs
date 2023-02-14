using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.DistributedStore.Stores;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    [Fact]
    public async Task SetAsync_Throws_An_ArgumentNullException_When_The_Key_Is_null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        StoreKey key = null;
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        var exception = await Record.ExceptionAsync(() =>distributedCacheValidatorValueStore.SetAsync(key, eTag));
        Assert.IsType<ArgumentNullException>(exception);
        distributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetAsync_Throws_An_ArgumentNullException_When_The_ValidatorValue_Is_null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var key = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        ValidatorValue eTag = null;
        var exception = await Record.ExceptionAsync(() => distributedCacheValidatorValueStore.SetAsync(key, eTag));
        Assert.IsType<ArgumentNullException>(exception);
        distributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetAsync_Adds_The_ValidatorValue_To_The_Cache_Using_The_Key()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var key = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var keyString = key.ToString();
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        var eTagString =$"{eTag.ETag.ETagType} Value=\"{eTag.ETag.Value}\" LastModified={eTag.LastModified.ToString(CultureInfo.InvariantCulture)}";
        var eTagBytes = Encoding.UTF8.GetBytes(eTagString);
        distributedCache.Setup(x =>x.SetAsync(keyString, eTagBytes, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        await distributedCacheValidatorValueStore.SetAsync(key, eTag);
        distributedCache.Verify(x => x.SetAsync(It.Is<string>(x =>x.Equals(keyString, StringComparison.InvariantCulture)), It.Is<byte[]>(x =>eTagBytes.SequenceEqual(x)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_Throws_An_ArgumentNullException_When_The_Key_Passed_In_Is_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        StoreKey key =null;
        var exception = await Record.ExceptionAsync(() =>distributedCacheValidatorValueStore.RemoveAsync(key));
        Assert.IsType<ArgumentNullException>(exception);
        distributedCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveAsync_Attempts_To_Remove_The_Item_From_The_Cache_With_The_Passed_In_Key()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var key = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var keyString = key.ToString();
        var exception = await Record.ExceptionAsync(() => distributedCacheValidatorValueStore.RemoveAsync(key));
        Assert.Null(exception);
        distributedCache.Verify(x => x.RemoveAsync(It.Is<string>(x =>x.Equals(keyString, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FindStoreKeysByKeyPartAsync_Throws_An_ArgumentNullException_WhenTheValue_To_Match_Parameter_Is_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        string valueToMatch = null;
        var ignoreCase = false;
        var exception = await CaptureTheExceptionIfOneIsThrownFromAnIAsyncEnumerable(() =>distributedCacheValidatorValueStore.FindStoreKeysByKeyPartAsync(valueToMatch, ignoreCase));
        Assert.IsType<ArgumentNullException>(exception);
distributedCacheKeyRetriever.Verify(x =>x.FindStoreKeysByKeyPartAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }
    
    [Fact]
    public async Task FindStoreKeysByKeyPartAsync_Throws_An_ArgumentException_WhenTheValue_To_Match_Parameter_Is_An_Empty_String()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        string valueToMatch = String.Empty;
        var ignoreCase = false;
        var exception = await CaptureTheExceptionIfOneIsThrownFromAnIAsyncEnumerable(() => distributedCacheValidatorValueStore.FindStoreKeysByKeyPartAsync(valueToMatch, ignoreCase));
        Assert.IsType<ArgumentException>(exception);
        distributedCacheKeyRetriever.Verify(x => x.FindStoreKeysByKeyPartAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Theory]
    [InlineData("Test", true, "test")]
    [InlineData("Test", false, "Test")]
    public async Task FindStoreKeysByKeyPartAsync_AttemptsToFindTheKeysThatStartWithThePassedInKeyPrefix(string keyPrefix, bool ignoreCase, string expectedKeyPrefix)
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        distributedCacheKeyRetriever.Setup(x => x.FindStoreKeysByKeyPartAsync(keyPrefix, ignoreCase)).Returns(AsyncEnumerable.Empty<string>());
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var exception = await CaptureTheExceptionIfOneIsThrownFromAnIAsyncEnumerable(() => distributedCacheValidatorValueStore.FindStoreKeysByKeyPartAsync(keyPrefix, ignoreCase));
        Assert.Null(exception);
        distributedCacheKeyRetriever.Verify(x => x.FindStoreKeysByKeyPartAsync(keyPrefix, ignoreCase), Times.Once);
    }

    private static async Task<Exception> CaptureTheExceptionIfOneIsThrownFromAnIAsyncEnumerable<T>(Func<IAsyncEnumerable<T>> sequenceGenerator)
    {
        try
        {
            await foreach (var item in sequenceGenerator())
            {
            }

            return null;
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }
}