// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    public interface IDateParser
    {
        Task<string> LastModifiedToString(DateTimeOffset lastModified);

        Task<string> ExpiresToString(DateTimeOffset lastModified);

        Task<DateTimeOffset?> IfModifiedSinceToDateTimeOffset(string ifModifiedSince);

        Task<DateTimeOffset?> IfUnmodifiedSinceToDateTimeOffset(string ifUnmodifiedSince);
    }
}
