// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Xunit;

namespace Marvin.Cache.Headers.Test.Domain
{
    public class ETagFacts
    {
        [Fact]
        public void Correctly_Build_Strong_ETag()
        {
            var eTag = new ETag("\"THISISSTRONG\"");

            Assert.Equal("THISISSTRONG", eTag.Value);
            Assert.Equal(ETagType.Strong, eTag.ETagType);
        }

        [Fact]
        public void Correctly_Build_Weak_ETag()
        {
            var eTag = new ETag("W\"THISISWEAK\"");

            Assert.Equal("THISISWEAK", eTag.Value);
            Assert.Equal(ETagType.Weak, eTag.ETagType);
        }

        [Fact]
        public void Correctly_Build_Unknown_NoDefault_ETag()
        {
            var eTag = new ETag("THISISUNKNOWN");

            Assert.Equal("THISISUNKNOWN", eTag.Value);
            Assert.Equal(ETagType.Weak, eTag.ETagType);
        }

        [Fact]
        public void Correctly_Build_Unknown_WithDefault_ETag()
        {
            var eTag = new ETag("THISISUNKNOWN", ETagType.Strong);

            Assert.Equal("THISISUNKNOWN", eTag.Value);
            Assert.Equal(ETagType.Strong, eTag.ETagType);
        }

        [Fact]
        public void Correctly_Build_NullValue_ETag()
        {
            var eTag = new ETag(null);

            Assert.Null(eTag.Value);
            Assert.Equal(ETagType.Weak, eTag.ETagType);
        }

        [Fact]
        public void Correctly_Build_NoValue_ETag()
        {
            var eTag = new ETag(" ");

            Assert.Null(eTag.Value);
            Assert.Equal(ETagType.Weak, eTag.ETagType);
        }
    }
}
