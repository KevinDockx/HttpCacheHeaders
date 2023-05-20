using System;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Marvin.Cache.Headers.DistributedStore.Redis.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Extensions
{
    /// <summary>
    /// Extension methods for the HttpCache middleware (on IServiceCollection)
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        /// Add redis key retriever services to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="redisDistributedCacheKeyRetrieverOptionsAction">Action to provide custom <see cref="RedisDistributedCacheKeyRetrieverOptions" /></param>
        public static IServiceCollection AddRedisKeyRetriever(this IServiceCollection services, Action<IOptions<RedisDistributedCacheKeyRetrieverOptions>> redisDistributedCacheKeyRetrieverOptionsAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (redisDistributedCacheKeyRetrieverOptionsAction == null)
            {
                throw new ArgumentNullException(nameof(redisDistributedCacheKeyRetrieverOptionsAction));
            }

            services.Configure(redisDistributedCacheKeyRetrieverOptionsAction);
            services.Add(ServiceDescriptor.Singleton(typeof(IRetrieveDistributedCacheKeys), typeof(RedisDistributedCacheKeyRetriever)));
            return services;
        }
    }
}