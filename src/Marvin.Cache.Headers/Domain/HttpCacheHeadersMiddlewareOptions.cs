// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

namespace Marvin.Cache.Headers
{
    /// <summary>
    /// Options that have to do with the HttpCacheHeadersMiddleware, mainly to do with ignoring caching globally.
    /// </summary>
    public class HttpCacheHeadersMiddlewareOptions
    {
        /// <summary>
        /// Ignore caching on a global level, defaults to false.
        /// </summary>
        public bool IgnoreCaching { get; set; } = false;
    }
    }