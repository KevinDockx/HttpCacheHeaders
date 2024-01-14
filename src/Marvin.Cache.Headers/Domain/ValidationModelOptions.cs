﻿// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using Marvin.Cache.Headers.Interfaces;

namespace Marvin.Cache.Headers
{
    /// <summary>
    /// Options that have to do with the validation model, mainly related to ETag generation, Last-Modified on the response,
    /// but also to the Cache-Control header (as that is used for both expiration & validation requirements)
    /// </summary>
    public class ValidationModelOptions : IModelOptions
    {
        /// <summary>
        /// A case-insensitive list of headers from the request to take into account as differentiator
        /// between requests (eg: for generating ETags)
        ///
        /// Defaults to Accept, Accept-Language, Accept-Encoding.  * indicates all request headers can be taken into account.
        /// </summary>
        public IEnumerable<string> Vary { get; set; }
            = new List<string> { "Accept", "Accept-Language", "Accept-Encoding" };

        /// <summary>
        /// Indicates that all request headers are taken into account as differentiator.
        /// When set to true, this is the same as Vary *.  The Vary list will thus be ignored.
        ///
        /// Note that a Vary field value of "*" implies that a cache cannot determine
        /// from the request headers of a subsequent request whether this response is
        /// the appropriate representation.  This should thus only be set to true for
        /// exceptional cases.
        ///
        /// Defaults to false.
        /// </summary>
        public bool VaryByAll { get; set; } = false;

        /// <summary>
        /// When true, the no-cache directive is added to the Cache-Control header.
        /// This indicates to a cache that the response should not be used for subsequent requests
        /// without successful revalidation with the origin server.
        ///
        /// Defaults to false.
        /// </summary>
        public bool NoCache { get; set; } = false;

        /// <summary>
        /// When true, the must-revalidate directive is added to the Cache-Control header.
        /// This tells a cache that if a response becomes stale, ie: it's expired, revalidation has to happen.
        /// By adding this directive, we can force revalidation by the cache even if the client
        /// has decided that stale responses are for a specified amount of time (which a client can
        /// do by setting the max-stale directive).
        ///
        /// Defaults to false.
        /// </summary>
        public bool MustRevalidate { get; set; } = false;

        /// <summary>
        /// When true, the proxy-revalidate directive is added to the Cache-Control header.
        /// Exactly the same as must-revalidate, but only for shared caches.
        /// So: this tells a shared cache that if a response becomes stale, ie: it's expired, revalidation has to happen.
        /// By adding this directive, we can force revalidation by the cache even if the client
        /// has decided that stale responses are for a specified amount of time (which a client can
        /// do by setting the max-stale directive).
        ///
        /// Defaults to false.
        /// </summary>
        public bool ProxyRevalidate { get; set; } = false;

    }
}
