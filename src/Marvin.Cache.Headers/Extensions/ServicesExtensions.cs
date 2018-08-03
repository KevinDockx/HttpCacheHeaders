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
            IDateParser dateParser = null,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null,
            IETagGenerator eTagGenerator = null)
        {
            AddModularParts(
                services,
                dateParser,
                store,
                storeKeyGenerator,
                eTagGenerator);

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> configureExpirationModelOptions,
            IDateParser dateParser = null,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null,
            IETagGenerator eTagGenerator = null)
        {
            AddConfigureExpirationModelOptions(services, configureExpirationModelOptions);

            AddModularParts(
                services,
                dateParser,
                store,
                storeKeyGenerator,
                eTagGenerator);

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ValidationModelOptions> configureValidationModelOptions,
            IDateParser dateParser = null,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null,
            IETagGenerator eTagGenerator = null)
        {
            AddConfigureValidationModelOptions(services, configureValidationModelOptions);

            AddModularParts(
                services,
                dateParser,
                store,
                storeKeyGenerator,
                eTagGenerator);

            return services;
        }

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> configureExpirationModelOptions,
            Action<ValidationModelOptions> configureValidationModelOptions,
            IDateParser dateParser = null,
            IValidationValueStore store = null,
            IStoreKeyGenerator storeKeyGenerator = null,
            IETagGenerator eTagGenerator = null)
        {
            AddConfigureExpirationModelOptions(services, configureExpirationModelOptions);
            AddConfigureValidationModelOptions(services, configureValidationModelOptions);

            AddModularParts(
                services,
                dateParser,
                store,
                storeKeyGenerator,
                eTagGenerator);

            return services;
        }

        private static void AddModularParts(
            IServiceCollection services,
            IDateParser dateParser,
            IValidationValueStore store,
            IStoreKeyGenerator storeKeyGenerator,
            IETagGenerator eTagGenerator)
        {
            AddDateParser(services, dateParser);
            AddValidationValueStore(services, store);
            AddStoreKeyGenerator(services, storeKeyGenerator);
            AddETagGenerator(services, eTagGenerator);
        }

        private static void AddDateParser(
            IServiceCollection services,
            IDateParser dateParser)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (dateParser == null)
            {
                dateParser = new DefaultDateParser();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IDateParser), dateParser));
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
                storeKeyGenerator = new DefaultStoreKeyGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IStoreKeyGenerator), storeKeyGenerator));
        }

        private static void AddETagGenerator(
            IServiceCollection services,
            IETagGenerator eTagGenerator)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (eTagGenerator == null)
            {
                eTagGenerator = new DefaultStrongETagGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IETagGenerator), eTagGenerator));
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
