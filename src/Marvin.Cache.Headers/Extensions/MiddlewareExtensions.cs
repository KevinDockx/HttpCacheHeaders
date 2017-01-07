// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for the HttpCache middleware.
    /// </summary>
    public static class HttpCacheServicesExtensions
    { 
        public static IServiceCollection AddHttpCacheHeaders(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(ServiceDescriptor.Singleton<IValidationValueStore, InMemoryValidationValueStore>());

            return services;
        }
 
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
