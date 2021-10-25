// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Routing;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for the HttpCache middleware (on IApplicationBuilder)
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Add HttpCacheHeaders to the request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpCacheHeaders(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Check whether the EndpointDataSource class has been registered on the container (they are 
            // required for getting endpoint metadata)
            if (builder.ApplicationServices.GetService(typeof(EndpointDataSource)) == null)
            {
                throw new InvalidOperationException("Cannot resolve required routing services on the container.  ");
            }

            return builder.UseMiddleware<HttpCacheHeadersMiddleware>();
        }
    }
}
