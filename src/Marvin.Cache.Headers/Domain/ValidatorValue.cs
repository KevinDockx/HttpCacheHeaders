// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Globalization;

namespace Marvin.Cache.Headers
{
    public class ValidatorValue
    {
        public ETag ETag { get; }
        public DateTimeOffset LastModified { get; }

        public ValidatorValue(ETag eTag, DateTimeOffset lastModified)
        {
            ETag = eTag;
            LastModified = lastModified;
        }

        public override string ToString()
        {
            return $"ETag: {ETag} Last-Modified: {LastModified.ToString("r", CultureInfo.InvariantCulture)}";
        }
    }
}
