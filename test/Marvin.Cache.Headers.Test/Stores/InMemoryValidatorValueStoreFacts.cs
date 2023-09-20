// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers.Stores;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Marvin.Cache.Headers.Test.Stores
{
    public class InMemoryValidatorValueStoreFacts
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenStoreKeySerializerIsNull()
        {
            IStoreKeySerializer storeKeySerializer = null;
            var cache = new Mock<IMemoryCache>();
            Assert.Throws<ArgumentNullException>(() =>new InMemoryValidatorValueStore(storeKeySerializer, cache.Object));
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenCacheIsNull()
        {
            var storeKeySerializer = new Mock<IStoreKeySerializer>();
            IMemoryCache cache = null;
            Assert.Throws<ArgumentNullException>(() => new InMemoryValidatorValueStore(storeKeySerializer.Object, cache));
        }

        [Fact]
        public async Task GetAsync_Returns_Stored_ValidatorValue()
        {
            // arrange
            var referenceTime = DateTimeOffset.UtcNow;
            object validatorValue = new ValidatorValue(new ETag(ETagType.Strong, "test"), referenceTime);
            var requestKey = new StoreKey
            {
                { "resourcePath", "/v1/gemeenten/11057" },
                { "queryString", string.Empty },
                { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
            };
            
            var requestKeyJson =JsonSerializer.Serialize(requestKey);
            var storeKeySerializer =new Mock<IStoreKeySerializer>();
storeKeySerializer.Setup(x =>x.SerializeStoreKey(requestKey)).Returns(requestKeyJson);
var cache = new Mock<IMemoryCache>();
cache.Setup(x => x.TryGetValue(requestKeyJson, out validatorValue)).Returns(true);
var target = new InMemoryValidatorValueStore(storeKeySerializer.Object, cache.Object);
            
// act
            var result = await target.GetAsync(requestKey);

            // assert
            Assert.NotNull(result);
            Assert.Equal(ETagType.Strong, result.ETag.ETagType);
            Assert.Equal("test", result.ETag.Value);
            Assert.Equal(result.LastModified, referenceTime);
            storeKeySerializer.Verify(x => x.SerializeStoreKey(requestKey), Times.Exactly(1));
            cache.Verify(x =>x.TryGetValue(requestKeyJson, out validatorValue), Times.Exactly(1));
        }

        [Fact]
        public async Task GetAsync_DoesNotReturn_Unknown_ValidatorValue()
        {
            // arrange
            var referenceTime = DateTimeOffset.UtcNow;
            object validatorValue = new ValidatorValue(new ETag(ETagType.Strong, "test"), referenceTime);
            var requestKey = new StoreKey
            {
                { "resourcePath", "/v1/gemeenten/11057" },
                { "queryString", string.Empty },
                { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
            };

            var storeKeySerializer = new Mock<IStoreKeySerializer>();
            var requestKeyJson =JsonSerializer.Serialize(requestKey);
            storeKeySerializer.Setup(x =>x.SerializeStoreKey(requestKey)).Returns(requestKeyJson);
            var cache = new Mock<IMemoryCache>();
            cache.Setup(x => x.TryGetValue(requestKeyJson, out validatorValue)).Returns(false);
            var target = new InMemoryValidatorValueStore(storeKeySerializer.Object, cache.Object);
            
            // act
            var result = await target.GetAsync(requestKey);

            // assert
            Assert.Null(result);
            storeKeySerializer.Verify(x => x.SerializeStoreKey(requestKey), Times.Exactly(1));
            cache.Verify(x => x.TryGetValue(requestKeyJson, out validatorValue), Times.Exactly(1));
        }
    }
}
