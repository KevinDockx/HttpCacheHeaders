// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Globalization;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Interfaces;

namespace Marvin.Cache.Headers
{
    public class DefaultDateParser : IDateParser
    {
        // r = RFC1123 pattern (https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx)

        public Task<string> LastModifiedToString(DateTimeOffset lastModified) => DateTimeOffsetToString(lastModified);

        public Task<string> ExpiresToString(DateTimeOffset expires) => DateTimeOffsetToString(expires);

        public Task<DateTimeOffset?> IfModifiedSinceToDateTimeOffset(string ifModifiedSince) => StringToDateTimeOffset(ifModifiedSince);

        public Task<DateTimeOffset?> IfUnmodifiedSinceToDateTimeOffset(string ifUnmodifiedSince) => StringToDateTimeOffset(ifUnmodifiedSince);

        private static Task<string> DateTimeOffsetToString(DateTimeOffset lastModified) => Task.FromResult(lastModified.ToString("r", CultureInfo.InvariantCulture));

        private static Task<DateTimeOffset?> StringToDateTimeOffset(string @string)
        {
            return Task.FromResult(
                DateTimeOffset.TryParseExact(
                    @string,
                    "r",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AdjustToUniversal,
                    out var parsedIfModifiedSince)
                    ? parsedIfModifiedSince
                    : new DateTimeOffset?());
        }
    }
}