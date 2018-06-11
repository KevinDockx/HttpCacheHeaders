using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marvin.Cache.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpCacheValidationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ValidationModelOptions _validationModelOptions;

        /// <summary>
        ///
        /// </summary>
        /// <param name="vary">
        /// A case-insensitive list of headers from the request to take into account as differentiator
        /// between requests (eg: for generating ETags)
        ///
        /// Defaults to Accept, Accept-Language, Accept-Encoding.  * indicates all request headers can be taken into account.
        /// </param>
        /// <param name="varyByAll">
        /// Indicates that all request headers are taken into account as differentiator.
        /// When set to true, this is the same as Vary *.  The Vary list will thus be ignored.
        ///
        /// Note that a Vary field value of "*" implies that a cache cannot determine
        /// from the request headers of a subsequent request whether this response is
        /// the appropriate representation.  This should thus only be set to true for
        /// exceptional cases.
        ///
        /// Defaults to false.
        /// </param>
        /// <param name="addNoCache">
        /// When true, the no-cache directive is added to the Cache-Control header.
        /// This indicates to a cache that the response should not be used for subsequent requests
        /// without successful revalidation with the origin server.
        ///
        /// Defaults to false.
        /// </param>
        /// <param name="addMustRevalidate">
        /// When true, the must-revalidate directive is added to the Cache-Control header.
        /// This tells a cache that if a response becomes stale, ie: it's expired, revalidation has to happen.
        /// By adding this directive, we can force revalidation by the cache even if the client
        /// has decided that stale responses are for a specified amount of time (which a client can
        /// do by setting the max-stale directive).
        ///
        /// Defaults to false.
        /// </param>
        /// <param name="addProxyRevalidate">
        /// When true, the proxy-revalidate directive is added to the Cache-Control header.
        /// Exactly the same as must-revalidate, but only only for shared caches.
        /// So: this tells a shared cache that if a response becomes stale, ie: it's expired, revalidation has to happen.
        /// By adding this directive, we can force revalidation by the cache even if the client
        /// has decided that stale responses are for a specified amount of time (which a client can
        /// do by setting the max-stale directive).
        ///
        /// Defaults to false.
        /// </param>
        public HttpCacheValidationAttribute(
            string vary = null,
            bool varyByAll = false,
            bool addNoCache = false,
            bool addMustRevalidate = false,
            bool addProxyRevalidate = false)
        {
            _validationModelOptions = new ValidationModelOptions
            {
                Vary = string.IsNullOrWhiteSpace(vary)
                    ? new List<string> { "Accept", "Accept-Language", "Accept-Encoding" }
                    : vary.Split(new []{","}, StringSplitOptions.RemoveEmptyEntries).ToList(),
                VaryByAll = varyByAll,
                AddNoCache = addNoCache,
                AddMustRevalidate = addMustRevalidate,
                AddProxyRevalidate = addProxyRevalidate
            };
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();

            context.HttpContext.Items[HttpCacheHeadersMiddleware.ContextItemsValidationModelOptions] = _validationModelOptions;
        }
    }
}