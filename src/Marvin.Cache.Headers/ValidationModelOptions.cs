// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;

namespace Marvin.Cache.Headers
{
    /// <summary>
    /// Options that have to do with the validation model, mainly related to ETag generation, Last-Modified on the response,
    /// but also to the Cache-Control header (as that is used for both expiration & validation requirements)
    /// </summary>
    public class ValidationModelOptions
    {
        /// <summary>
        /// A case-insensitive list of headers from the request to take into account as differentiator
        /// between requests (eg: for generating ETags)
        /// 
        /// Defaults to Accept, Accept-Language.  * indicates all headers will be taken into account.
        /// </summary>
        public IEnumerable<string> Vary { get; set; } = new List<string>() { "Accept", "Accept-Language" };

        /// <summary>
        /// When true, the no-cache directive is added to the Cache-Control header.
        /// This indicates to a cache that the response should not be used for subsequent requests 
        /// without successful revalidation with the origin server.
        /// 
        /// Defaults to false.
        /// </summary>
        public bool AddNoCache { get; set; } = false;

        /// <summary>
        /// When true, the must-revalidate directive is added to the Cache-Control header.
        /// This tells a cache that if a response becomes stale, ie: it's expired, revalidation has to happen. 
        /// By adding this directive, we can force revalidation by the cache even if the client 
        /// has decided that stale responses are for a specified amount of time (which a client can 
        /// do by setting the max-stale directive).
        /// 
        /// Defaults to false.
        /// </summary>
        public bool AddMustRevalidate { get; set; } = false;

        /// <summary>
        /// When true, the proxy-revalidate directive is added to the Cache-Control header.
        /// Exactly the same as must-revalidate, but only only for shared caches.  
        /// So: this tells a shared cache that if a response becomes stale, ie: it's expired, revalidation has to happen. 
        /// By adding this directive, we can force revalidation by the cache even if the client 
        /// has decided that stale responses are for a specified amount of time (which a client can 
        /// do by setting the max-stale directive).
        /// 
        /// Defaults to false.
        /// </summary>
        public bool AddProxyRevalidate { get; set; } = false;

    }
}
