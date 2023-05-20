using System;
using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Test.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddRedisKeyRetriever(this IServiceCollection services,
        Action<IOptions<RedisDistributedCacheKeyRetrieverOptions>> redisDistributedCacheKeyRetrieverOptionsAction)
    {
        throw new NotImplementedException();
    }
}