using System;
using System.Text;
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

            var target = new InMemoryValidationValueStore();
            await target.SetAsync("test-key", new ValidationValue(new ETag(ETagType.Strong, "test"), referenceTime));

            // act
            var result = await target.GetAsync("test-key");

            // assert
            Assert.NotNull(result);
            Assert.Equal(result.ETag.ETagType, ETagType.Strong);
            Assert.Equal(result.ETag.Value, "test");
            Assert.Equal(result.LastModified, referenceTime);
        }

        [Fact]
        public async Task GetAsync_DoesNotReturn_Unknown_ValidationValue()
        {
            // arrange
            var referenceTime = DateTimeOffset.UtcNow;

            var target = new InMemoryValidationValueStore();
            await target.SetAsync("test-key", new ValidationValue(new ETag(ETagType.Strong, "test"), referenceTime));

            // act
            var result = await target.GetAsync("test-nonexisting-key");

            // assert
            Assert.Null(result);
        }
    }
}
