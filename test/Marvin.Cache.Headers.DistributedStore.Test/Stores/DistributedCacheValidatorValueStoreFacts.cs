using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.DistributedStore.Stores;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores;

public class DistributedCacheValidatorValueStoreFacts
{
    [Fact]
    public void Ctor_ExpectArgumentNullException_WhenDistributedCacheIsNull()
    {
        IDistributedCache distributedCache = null;
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        Assert.Throws<ArgumentNullException>(() => new DistributedCacheValidatorValueStore(distributedCache, distributedCacheKeyRetriever.Object,storeKeySerializer.Object));
    }

    [Fact]
    public void Ctor_ExpectArgumentNullException_WhenDistributedCacheKeyRetrieverIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        IRetrieveDistributedCacheKeys distributedCacheKeyRetriever = null;
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        Assert.Throws<ArgumentNullException>(() => new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever, storeKeySerializer.Object));
        }

    [Fact]
    public void Ctor_ExpectArgumentNullException_WhenStoreKeySerializerIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        IStoreKeySerializer storeKeySerializer = null;
        Assert.Throws<ArgumentNullException>(() => new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer));
    }

    [Fact]
    public void Constructs_An_DistributedCacheValidatorValueStore_When_All_The_Parameters_Passed_In_Are_Not_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
                var storeKeySerializer = new Mock<IStoreKeySerializer>();
        DistributedCacheValidatorValueStore distributedCacheValidatorValueStore = null;
        var exception = Record.Exception(() => distributedCacheValidatorValueStore =new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object));
        Assert.Null(exception);
        Assert.NotNull(distributedCacheValidatorValueStore);
    }

    [Fact]
    public async Task GetAsync_ExpectArgumentNullException_WhenSoteKeyIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        StoreKey key = null;
        var distributedCacheValidatorValueStore =new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => distributedCacheValidatorValueStore.GetAsync(key));
        storeKeySerializer.Verify(x =>x.SerializeStoreKey(It.IsAny<StoreKey>()), Times.Never);
        distributedCache.Verify(x =>x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task GetAsync_ExpectNull_WhenTheKeyIsNotInTheCache()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var storeKey = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var storeKeyJson = JsonSerializer.Serialize(storeKey);
        storeKeySerializer.Setup(x =>x.SerializeStoreKey(storeKey)).Returns(storeKeyJson);
        distributedCache.Setup(x => x.GetAsync(storeKeyJson, CancellationToken.None)).Returns(Task.FromResult<byte[]?>(null));
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        var result = await distributedCacheValidatorValueStore.GetAsync(storeKey);
        Assert.Null(result);
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Exactly(1));
        distributedCache.Verify(x => x.GetAsync(It.Is<string>(x =>x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ExpectTheValueToBeReturned_WhenTheKeyIsInTheCache()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var storeKey = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var storeKeyJson = JsonSerializer.Serialize(storeKey);
        storeKeySerializer.Setup(x => x.SerializeStoreKey(storeKey)).Returns(storeKeyJson);
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        var eTagString = $"{ETagType.Strong} Value=\"Test\" LastModified={referenceTime.ToString(CultureInfo.InvariantCulture)}";
        var eTagBytes = Encoding.UTF8.GetBytes(eTagString);
        distributedCache.Setup(x => x.GetAsync(storeKeyJson, CancellationToken.None)).ReturnsAsync(eTagBytes);
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        var result = await distributedCacheValidatorValueStore.GetAsync(storeKey);
        Assert.Equal(result.LastModified, eTag.LastModified);
        Assert.Equal(result.ETag.ETagType, eTag.ETag.ETagType);
        Assert.Equal(result.ETag.Value, eTag.ETag.Value);
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Exactly(1));
        distributedCache.Verify(x => x.GetAsync(It.Is<string>(x => x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetAsync_ExpectArgumentNullException_WhenStoreKeyIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);

        StoreKey storeKey = null;
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        await Assert.ThrowsAsync<ArgumentNullException>(() => distributedCacheValidatorValueStore.SetAsync(storeKey, eTag));
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Never);
        distributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetAsync_ExpectArgumentNullException_WhenValidatorValueIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        var storeKey = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        ValidatorValue eTag = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() =>distributedCacheValidatorValueStore.SetAsync(storeKey, eTag));
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Never);
        distributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetAsync_ExpectTheValueToBeAddedToTheCache_WhenTheStoreKeyAndValidatorValueAreBothNotNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        var storeKey = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var storeKeyJson = JsonSerializer.Serialize(storeKey);
        storeKeySerializer.Setup(x => x.SerializeStoreKey(storeKey)).Returns(storeKeyJson);
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        var eTagString =$"{eTag.ETag.ETagType} Value=\"{eTag.ETag.Value}\" LastModified={eTag.LastModified.ToString(CultureInfo.InvariantCulture)}";
        var eTagBytes = Encoding.UTF8.GetBytes(eTagString);
        distributedCache.Setup(x =>x.SetAsync(storeKeyJson, eTagBytes, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        await distributedCacheValidatorValueStore.SetAsync(storeKey, eTag);
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Exactly(1));
        distributedCache.Verify(x => x.SetAsync(It.Is<string>(x =>x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.Is<byte[]>(x =>eTagBytes.SequenceEqual(x)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task RemoveAsync_ExpectArgumentNullException_WhenStoreKeyIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        StoreKey storeKey =null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => distributedCacheValidatorValueStore.RemoveAsync(storeKey));
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Never);
        distributedCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveAsync_ExpectFalseToBeReturned_WhenTheProvidedKeyIsNotInTheCache()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        var storeKey = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var storeKeyJson = JsonSerializer.Serialize(storeKey);
        storeKeySerializer.Setup(x => x.SerializeStoreKey(storeKey)).Returns(storeKeyJson);
        distributedCache.Setup(x => x.GetAsync(It.Is<string>(x => x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null);

        var result = await distributedCacheValidatorValueStore.RemoveAsync(storeKey);
            Assert.False(result);
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Exactly(1));
        distributedCache.Verify(x => x.GetAsync(It.Is<string>(x => x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Exactly(1));
        distributedCache.Verify(x => x.RemoveAsync(It.Is<string>(x => x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveAsync_ExpectTrueToBeReturned_WhenTheKeyIsInTheCacheAndHasBeenRemoved()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);        
        var storeKey = new StoreKey
        {
            { "resourcePath", "/v1/gemeenten/11057" },
            { "queryString", string.Empty },
            { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
        };
        var storeKeyJson = JsonSerializer.Serialize(storeKey);
        var referenceTime = new DateTimeOffset(2022, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var eTag = new ValidatorValue(new ETag(ETagType.Strong, "Test"), referenceTime);
        var eTagString = $"{ETagType.Strong} Value=\"Test\" LastModified={referenceTime.ToString(CultureInfo.InvariantCulture)}";
        var eTagBytes = Encoding.UTF8.GetBytes(eTagString);
        storeKeySerializer.Setup(x => x.SerializeStoreKey(storeKey)).Returns(storeKeyJson);
        distributedCache.Setup(x => x.GetAsync(It.Is<string>(x => x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>())).ReturnsAsync(eTagBytes);
        var result = await distributedCacheValidatorValueStore.RemoveAsync(storeKey);
        Assert.True(result);
        storeKeySerializer.Verify(x => x.SerializeStoreKey(storeKey), Times.Exactly(1));
        distributedCache.Verify(x => x.GetAsync(It.Is<string>(x => x.Equals(storeKeyJson, StringComparison.InvariantCulture)), It.IsAny<CancellationToken>()), Times.Exactly(1));
        distributedCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task FindStoreKeysByKeyPartAsync_ExpectAnArgumentNullException_WhenValueToMatchIsNull()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        string valueToMatch = null;
        var ignoreCase = false;
        var exception = await CaptureTheExceptionIfOneIsThrownFromAnIAsyncEnumerable(() =>distributedCacheValidatorValueStore.FindStoreKeysByKeyPartAsync(valueToMatch, ignoreCase));
        Assert.IsType<ArgumentNullException>(exception);
distributedCacheKeyRetriever.Verify(x =>x.FindStoreKeysByKeyPartAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
storeKeySerializer.Verify(x => x.DeserializeStoreKey(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task FindStoreKeysByKeyPartAsync_ExpectArgumentException_WhenTheValueToMatchIsAnEmptyString()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        var storeKeySerializer = new Mock<IStoreKeySerializer>();
        var distributedCacheValidatorValueStore = new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object, storeKeySerializer.Object);
        string valueToMatch = String.Empty;
        var ignoreCase = false;
        var exception = await CaptureTheExceptionIfOneIsThrownFromAnIAsyncEnumerable(() => distributedCacheValidatorValueStore.FindStoreKeysByKeyPartAsync(valueToMatch, ignoreCase));
        Assert.IsType<ArgumentException>(exception);
        distributedCacheKeyRetriever.Verify(x => x.FindStoreKeysByKeyPartAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        storeKeySerializer.Verify(x => x.DeserializeStoreKey(It.IsAny<string>()), Times.Never);
    }

    [Theory(Skip ="Reenable later.")]
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
            await foreach (var _ in sequenceGenerator())
            {
            }
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }
}