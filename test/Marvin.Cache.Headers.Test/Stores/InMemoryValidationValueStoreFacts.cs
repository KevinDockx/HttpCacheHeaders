// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Stores;
using Xunit;

namespace Marvin.Cache.Headers.Test.Stores
{
    public class InMemoryValidationValueStoreFacts
    {
        [Fact]
        public async Task GetAsync_Returns_Stored_ValidationValue()
        {
            // arrange
            var referenceTime = DateTimeOffset.UtcNow;
            var requestKey = new StoreKey
            {
                { "resourcePath", "/v1/gemeenten/11057" },
                { "queryString", string.Empty },
                { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
            };

            var target = new InMemoryValidationValueStore();
            await target.SetAsync(requestKey, new ValidationValue(new ETag(ETagType.Strong, "test"), referenceTime));

            // act
            var result = await target.GetAsync(requestKey);

            // assert
            Assert.NotNull(result);
            Assert.Equal(ETagType.Strong, result.ETag.ETagType);
            Assert.Equal("test", result.ETag.Value);
            Assert.Equal(result.LastModified, referenceTime);
        }

        [Fact]
        public async Task GetAsync_DoesNotReturn_Unknown_ValidationValue()
        {
            // arrange
            var referenceTime = DateTimeOffset.UtcNow;
            var requestKey = new StoreKey
            {
                { "resourcePath", "/v1/gemeenten/11057" },
                { "queryString", string.Empty },
                { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
            };
            var requestKey2 = new StoreKey
            {
                { "resourcePath", "/v1/gemeenten/1" },
                { "queryString", string.Empty },
                { "requestHeaderValues", string.Join("-", new List<string> {"text/plain", "gzip"})}
            };

            var target = new InMemoryValidationValueStore();
            await target.SetAsync(requestKey, new ValidationValue(new ETag(ETagType.Strong, "test"), referenceTime));

            // act
            var result = await target.GetAsync(requestKey2);

            // assert
            Assert.Null(result);
        }
    }
}
