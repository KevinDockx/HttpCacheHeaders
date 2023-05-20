using System;
using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddRedisKeyRetriever(this IServiceCollection services, Action<IOptions<RedisDistributedCacheKeyRetrieverOptions>> redisDistributedCacheKeyRetrieverOptionsAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            throw new NotImplementedException();
        }
    }
}