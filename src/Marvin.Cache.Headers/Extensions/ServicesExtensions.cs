// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers.Stores;
using Microsoft.Extensions.DependencyInjection;
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
            services.Add(ServiceDescriptor.Singleton(new ExpirationModelOptions()));
            services.Add(ServiceDescriptor.Singleton(new ValidationModelOptions()));

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

            services.Add(ServiceDescriptor.Singleton<IValidationValueStore, InMemoryValidationValueStore>());
            services.Add(ServiceDescriptor.Singleton(configureExpirationModelOptions));
            services.Add(ServiceDescriptor.Singleton(configureValidationModelOptions));

            return services;
        }         
    }
}
