// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

namespace Marvin.Cache.Headers
{
    public class ETag
    {
        public ETagType ETagType { get; private set; }
        public string Value { get; private set; }

        public ETag(ETagType eTagType, string value)
        {
            ETagType = eTagType;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
