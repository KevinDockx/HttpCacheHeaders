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
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidationValueStore> storeFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddModularParts(
                services,
                dateParserFunc,
                storeFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

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
            Action<ExpirationModelOptions> configureExpirationModelOptions,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidationValueStore> storeFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddConfigureExpirationModelOptions(services, configureExpirationModelOptions);

            AddModularParts(
                services,
                dateParserFunc,
                storeFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

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
            Action<ValidationModelOptions> configureValidationModelOptions,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidationValueStore> storeFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddConfigureValidationModelOptions(services, configureValidationModelOptions);

            AddModularParts(
                services,
                dateParserFunc,
                storeFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

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

        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> configureExpirationModelOptions,
            Action<ValidationModelOptions> configureValidationModelOptions,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidationValueStore> storeFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddConfigureExpirationModelOptions(services, configureExpirationModelOptions);
            AddConfigureValidationModelOptions(services, configureValidationModelOptions);

            AddModularParts(
                services,
                dateParserFunc,
                storeFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

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

        private static void AddModularParts(
            IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc,
            Func<IServiceProvider, IValidationValueStore> storeFunc,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc)
        {
            AddDateParser(services, dateParserFunc);
            AddValidationValueStore(services, storeFunc);
            AddStoreKeyGenerator(services, storeKeyGeneratorFunc);
            AddETagGenerator(services, eTagGeneratorFunc);
        }

        private static void AddDateParser(IServiceCollection services, IDateParser dateParser)
            => AddDateParser(services, _ => dateParser ?? new DefaultDateParser());

        private static void AddDateParser(
            IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (dateParserFunc == null)
            {
                dateParserFunc = _ => new DefaultDateParser();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IDateParser), dateParserFunc));
        }

        private static void AddValidationValueStore(IServiceCollection services, IValidationValueStore store)
            => AddValidationValueStore(services, _ => store ?? new InMemoryValidationValueStore());

        private static void AddValidationValueStore(
            IServiceCollection services,
            Func<IServiceProvider, IValidationValueStore> validationValueStoreFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (validationValueStoreFunc == null)
            {
                validationValueStoreFunc = _ => new InMemoryValidationValueStore();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IValidationValueStore), validationValueStoreFunc));
        }

        private static void AddStoreKeyGenerator(IServiceCollection services, IStoreKeyGenerator storeKeyGenerator)
            => AddStoreKeyGenerator(services, _ => storeKeyGenerator ?? new DefaultStoreKeyGenerator());

        private static void AddStoreKeyGenerator(
            IServiceCollection services,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (storeKeyGeneratorFunc == null)
            {
                storeKeyGeneratorFunc = _ => new DefaultStoreKeyGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IStoreKeyGenerator), storeKeyGeneratorFunc));
        }

        private static void AddETagGenerator(IServiceCollection services, IETagGenerator eTagGenerator)
            => AddETagGenerator(services, _ => eTagGenerator ?? new DefaultStrongETagGenerator());

        private static void AddETagGenerator(
            IServiceCollection services,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (eTagGeneratorFunc == null)
            {
                eTagGeneratorFunc = _ => new DefaultStrongETagGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IETagGenerator), eTagGeneratorFunc));
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
