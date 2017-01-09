// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the HttpCache middleware (on IServiceCollection)
    /// </summary>
    public static class ServicesExtensions
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

        public static IServiceCollection AddHttpCacheHeaders(this IServiceCollection services,
      Action<ExpirationModelOptions> configureExpirationModelOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureExpirationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(configureExpirationModelOptions));
            }

            services.Configure(configureExpirationModelOptions);
            services.AddHttpCacheHeaders();

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(this IServiceCollection services,
         Action<ValidationModelOptions> configureValidationModelOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureValidationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(configureValidationModelOptions));
            }

            services.Configure(configureValidationModelOptions);
            services.AddHttpCacheHeaders();

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(this IServiceCollection services,
         Action<ExpirationModelOptions> configureExpirationModelOptions,
         Action<ValidationModelOptions> configureValidationModelOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureExpirationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(configureExpirationModelOptions));
            }

            if (configureValidationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(configureValidationModelOptions));
            }

            services.Configure(configureExpirationModelOptions);
            services.Configure(configureValidationModelOptions);
            services.AddHttpCacheHeaders();

            return services;
        }
    }
}
