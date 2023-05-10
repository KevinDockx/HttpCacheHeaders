using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Stores
{
    public class RedisDistributedCacheKeyRetriever
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisDistributedCacheKeyRetrieverOptions _redisDistributedCacheKeyRetrieverOptions;

        public RedisDistributedCacheKeyRetriever(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisDistributedCacheKeyRetrieverOptions> redisDistributedCacheKeyRetrieverOptions)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            if (redisDistributedCacheKeyRetrieverOptions == null)
            {
                throw new ArgumentNullException(nameof(redisDistributedCacheKeyRetrieverOptions));
            }

            if (redisDistributedCacheKeyRetrieverOptions.Value == null)
            {
                throw new ArgumentNullException(nameof(redisDistributedCacheKeyRetrieverOptions.Value));
            }

            _redisDistributedCacheKeyRetrieverOptions = redisDistributedCacheKeyRetrieverOptions.Value;
        }
    }
}