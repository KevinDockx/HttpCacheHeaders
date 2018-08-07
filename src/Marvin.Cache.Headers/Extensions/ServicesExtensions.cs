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

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>An <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddHttpCacheHeaders(
          this IServiceCollection services)
        {
            AddModularParts(
                services,
                null,
                null,
                null,
                null);

            return services;
        } 

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        { 
            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

            return services;
        } 

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="expirationModelOptionsAction">Action to provide custom <see cref="ExpirationModelOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> expirationModelOptionsAction,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddConfigureExpirationModelOptions(services, expirationModelOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

            return services;
        }

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="validationModelOptionsAction">Action to provide custom <see cref="ValidationModelOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ValidationModelOptions> validationModelOptionsAction,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddConfigureValidationModelOptions(services, validationModelOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

            return services;
        }

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="expirationModelOptionsAction">Action to provide custom <see cref="ExpirationModelOptions" /></param>
        /// <param name="validationModelOptionsAction">Action to provide custom <see cref="ValidationModelOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> expirationModelOptionsAction,
            Action<ValidationModelOptions> validationModelOptionsAction,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null)
        {
            AddConfigureExpirationModelOptions(services, expirationModelOptionsAction);
            AddConfigureValidationModelOptions(services, validationModelOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc);

            return services;
        }

        private static void AddModularParts(
            IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc)
        {
            AddDateParser(services, dateParserFunc);
            AddValidatorValueStore(services, validatorValueStoreFunc);
            AddStoreKeyGenerator(services, storeKeyGeneratorFunc);
            AddETagGenerator(services, eTagGeneratorFunc);
        }
        
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
        
        private static void AddValidatorValueStore(
            IServiceCollection services,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (validatorValueStoreFunc == null)
            {
                validatorValueStoreFunc = _ => new InMemoryValidatorValueStore();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IValidatorValueStore), validatorValueStoreFunc));
        }

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
