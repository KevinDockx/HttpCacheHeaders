using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Stores
{
    public class RedisDistributedCacheKeyRetriever
    {
        public RedisDistributedCacheKeyRetriever(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisDistributedCacheKeyRetrieverOptions> redisDistributedCacheKeyRetrieverOptions)
        {
            throw new NotImplementedException();
        }
    }
}