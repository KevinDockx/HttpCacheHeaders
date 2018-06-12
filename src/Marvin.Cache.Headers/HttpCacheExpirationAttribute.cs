using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marvin.Cache.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpCacheExpirationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly Lazy<ExpirationModelOptions> _expirationModelOptions;

        /// <summary>
        /// Maximum age, in seconds, after which a response expires. Has an effect on Expires & on the max-age directive
        /// of the Cache-Control header.
        ///
        /// Defaults to 60.
        /// </summary>
        public int MaxAge { get; set; } = 60;

        /// <summary>
        /// Maximum age, in seconds, after which a response expires for shared caches.  If included,
        /// a shared cache should use this value rather than the max-age value. (s-maxage directive)
        ///
        /// Not set by default.
        /// </summary>
        public int? SharedMaxAge { get; set; }

        /// <summary>
        /// The location where a response can be cached.  Public means it can be cached by both
        /// public (shared) and private (client) caches.  Private means it can only be cached by
        /// private (client) caches. (public or private directive)
        ///
        /// Defaults to public.
        /// </summary>
        public CacheLocation CacheLocation { get; set; } = CacheLocation.Public;

        /// <summary>
        /// When true, the no-store directive is included in the Cache-Control header.
        /// When this directive is included, a cache must not store any part of the message,
        /// mostly for confidentiality reasonse.
        ///
        /// Defaults to false.
        /// </summary>
        public bool AddNoStoreDirective { get; set; } = false;

        /// <summary>
        /// When true, the no-transform directive is included in the Cache-Control header.
        /// When this directive is included, a cache must not convert the media type of the
        /// response body.
        ///
        /// Defaults to false.
        /// </summary>
        public bool AddNoTransformDirective { get; set; } = false;

        public HttpCacheExpirationAttribute()
        {
            _expirationModelOptions = new Lazy<ExpirationModelOptions>(() => new ExpirationModelOptions
            {
                MaxAge = MaxAge,
                SharedMaxAge = SharedMaxAge,
                CacheLocation = CacheLocation,
                AddNoStoreDirective = AddNoStoreDirective,
                AddNoTransformDirective = AddNoTransformDirective
            });
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();

            context.HttpContext.Items[HttpCacheHeadersMiddleware.ContextItemsExpirationModelOptions] = _expirationModelOptions.Value;
        }
    }
}
