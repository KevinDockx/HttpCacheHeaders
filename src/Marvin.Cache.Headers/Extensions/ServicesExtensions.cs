// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Marvin.Cache.Headers.Serialization;
using Marvin.Cache.Headers.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        /// <param name="expirationModelOptionsAction">Action to provide custom <see cref="ExpirationModelOptions" /></param>
        /// <param name="validationModelOptionsAction">Action to provide custom <see cref="ValidationModelOptions" /></param>
        /// <param name="middlewareOptionsAction">Action to provide custom <see cref="MiddlewareOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="storeKeySerializerFunc">Func to provide a custom <see cref="IStoreKeySerializer" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        /// <param name="lastModifiedInjectorFunc">Func to provide a custom <see cref="ILastModifiedInjector" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> expirationModelOptionsAction = null,
            Action<ValidationModelOptions> validationModelOptionsAction = null,
            Action<MiddlewareOptions> middlewareOptionsAction = null,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IStoreKeySerializer> storeKeySerializerFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc = null)
        {
            if(expirationModelOptionsAction != null)
                AddConfigureExpirationModelOptions(services, expirationModelOptionsAction);
            if(validationModelOptionsAction != null)
                AddConfigureValidationModelOptions(services, validationModelOptionsAction);
            if(middlewareOptionsAction != null)
                AddConfigureMiddlewareOptions(services, middlewareOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                storeKeySerializerFunc,
                eTagGeneratorFunc,
                lastModifiedInjectorFunc);

            return services;
        }

        private static void AddModularParts(IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc,
            Func<IServiceProvider, IStoreKeySerializer> storeKeySerializerFunc,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc)
        {
            AddDateParser(services, dateParserFunc);
            AddValidatorValueStore(services, validatorValueStoreFunc);
            AddStoreKeyGenerator(services, storeKeyGeneratorFunc);
            AddStoreKeySerializer(services, storeKeySerializerFunc);
            AddETagGenerator(services, eTagGeneratorFunc);
            AddLastModifiedInjector(services, lastModifiedInjectorFunc);

            // register dependencies for required services
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // add required additional services
            services.AddScoped<IValidatorValueInvalidator, ValidatorValueInvalidator>();
            services.AddTransient<IStoreKeyAccessor, StoreKeyAccessor>();
        }

        private static void AddLastModifiedInjector(
            IServiceCollection services, 
            Func<IServiceProvider, 
                ILastModifiedInjector> lastModifiedInjectorFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (lastModifiedInjectorFunc == null)
            {
                lastModifiedInjectorFunc = _ => new DefaultLastModifiedInjector();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(ILastModifiedInjector), lastModifiedInjectorFunc));
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
                validatorValueStoreFunc = services => new InMemoryValidatorValueStore(services.GetRequiredService<IStoreKeySerializer>());
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

        private static void AddStoreKeySerializer(
            IServiceCollection services,
            Func<IServiceProvider, IStoreKeySerializer> storeKeySerializerFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (storeKeySerializerFunc == null)
            {
                storeKeySerializerFunc = _ => new DefaultStoreKeySerializer();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IStoreKeySerializer), storeKeySerializerFunc));
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
        
        private static void AddConfigureMiddlewareOptions(
            IServiceCollection services,
            Action<MiddlewareOptions> configureExpirationModelOptions)
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
