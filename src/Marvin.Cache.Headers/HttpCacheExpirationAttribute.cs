using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marvin.Cache.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpCacheExpirationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ExpirationModelOptions _expirationModelOptions;

        /// <param name="maxAge">
        /// Maximum age, in seconds, after which a response expires. Has an effect on Expires & on the max-age directive
        /// of the Cache-Control header.
        ///
        /// Defaults to 60.
        /// </param>
        /// <param name="sharedMaxAge">
        /// Maximum age, in seconds, after which a response expires for shared caches.  If included,
        /// a shared cache should use this value rather than the max-age value. (s-maxage directive)
        ///
        /// Not set by default.
        /// </param>
        /// <param name="cacheLocation">
        /// The location where a response can be cached.  Public means it can be cached by both
        /// public (shared) and private (client) caches.  Private means it can only be cached by
        /// private (client) caches. (public or private directive)
        ///
        /// Defaults to public.
        /// </param>
        /// <param name="addNoStoreDirective">
        /// When true, the no-store directive is included in the Cache-Control header.
        /// When this directive is included, a cache must not store any part of the message,
        /// mostly for confidentiality reasonse.
        ///
        /// Defaults to false.
        /// </param>
        /// <param name="addNoTransformDirective">
        /// When true, the no-transform directive is included in the Cache-Control header.
        /// When this directive is included, a cache must not convert the media type of the
        /// response body.
        ///
        /// Defaults to false.
        /// </param>
        public HttpCacheExpirationAttribute(
            int maxAge = 60,
            int sharedMaxAge = -1,
            CacheLocation cacheLocation = CacheLocation.Public,
            bool addNoStoreDirective = false,
            bool addNoTransformDirective = false)
        {
            _expirationModelOptions = new ExpirationModelOptions
            {
                MaxAge = maxAge,
                SharedMaxAge = sharedMaxAge == -1 ? new int?() : sharedMaxAge,
                CacheLocation = cacheLocation,
                AddNoStoreDirective = addNoStoreDirective,
                AddNoTransformDirective = addNoTransformDirective
            };
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();

            context.HttpContext.Items[HttpCacheHeadersMiddleware.ContextItemsExpirationModelOptions] = _expirationModelOptions;
        }
    }
}