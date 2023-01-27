// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using System.Linq;

namespace Marvin.Cache.Headers
{
    /// <summary>
    /// Options that have to do with the HttpCacheHeadersMiddleware, mainly with ignoring caching globally.
    /// </summary>
    public class HttpCacheHeadersMiddlewareOptions
    {
        /// <summary>
        /// Ignore caching on a global level
        ///
        /// Defaults to false.
        /// </summary>
        public bool IgnoreCaching { get; set; } = false;
        
        /// <summary>
        /// Ignore caching on responses with specific status codes.
        ///
        /// Defaults to none.
        /// </summary>
        public IEnumerable<int> IgnoredStatusCodes { get; set; } = Enumerable.Empty<int>();
    }
}