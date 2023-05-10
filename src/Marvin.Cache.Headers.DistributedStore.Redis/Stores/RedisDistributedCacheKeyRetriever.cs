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
            if (connectionMultiplexer == null)
            {
                throw new ArgumentNullException(nameof(connectionMultiplexer));
            }

            if (redisDistributedCacheKeyRetrieverOptions == null)
            {
                throw new ArgumentNullException(nameof(redisDistributedCacheKeyRetrieverOptions));
            }
            else if (redisDistributedCacheKeyRetrieverOptions.Value == null)
            {
                throw new ArgumentNullException(nameof(redisDistributedCacheKeyRetrieverOptions.Value));
            }
            
            throw new NotImplementedException();
        }
    }
}