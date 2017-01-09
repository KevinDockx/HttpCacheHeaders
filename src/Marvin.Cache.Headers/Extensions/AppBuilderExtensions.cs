// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for the HttpCache middleware (on IApplicationBuilder)
    /// </summary>
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseHttpCacheHeaders(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<HttpCacheHeadersMiddleware>();
        }
    }
}
