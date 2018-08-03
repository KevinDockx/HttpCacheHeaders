// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers.Stores;
using System;
using Marvin.Cache.Header;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the HttpCache middleware (on IServiceCollection)
    /// </summary>
    public static class ServicesExtensions
    {
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null)
        {
            AddValidationValueStore(services, store);
            AddStoreKeyGenerator(services, storeKeyGenerator);

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> configureExpirationModelOptions,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null)
        {
            AddConfigureExpirationModelOptions(services, configureExpirationModelOptions);

            AddValidationValueStore(services, store);
            AddStoreKeyGenerator(services, storeKeyGenerator);

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ValidationModelOptions> configureValidationModelOptions,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null)
        {
            AddConfigureValidationModelOptions(services, configureValidationModelOptions);

            AddValidationValueStore(services, store);
            AddStoreKeyGenerator(services, storeKeyGenerator);

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> configureExpirationModelOptions,
            Action<ValidationModelOptions> configureValidationModelOptions,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null)
        {
            AddConfigureExpirationModelOptions(services, configureExpirationModelOptions);
            AddConfigureValidationModelOptions(services, configureValidationModelOptions);

            AddValidationValueStore(services, store);
            AddStoreKeyGenerator(services, storeKeyGenerator);

            return services;
        }

        private static void AddValidationValueStore(
            IServiceCollection services,
            IValidationValueStore store)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (store == null)
            {
                store = new InMemoryValidationValueStore();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IValidationValueStore), store));
        }

        private static void AddStoreKeyGenerator(
            IServiceCollection services,
            IStoreKeyGenerator storeKeyGenerator)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (storeKeyGenerator == null)
            {
                storeKeyGenerator = new StoreKeyGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IStoreKeyGenerator), storeKeyGenerator));
        }

        private static void AddConfigureExpirationModelOptions(
            IServiceCollection services,
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
        }

        private static void AddConfigureValidationModelOptions(
            IServiceCollection services,
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
        }
    }
}
