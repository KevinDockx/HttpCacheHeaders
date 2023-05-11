using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Cache.Headers.DistributedStore.Interfaces;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Stores
{
    public class RedisDistributedCacheKeyRetriever : IRetrieveDistributedCacheKeys
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

        public IAsyncEnumerable<string> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase =true)
        {
            if (valueToMatch == null)
            {
                throw new ArgumentNullException(nameof(valueToMatch));
            }
            else if (valueToMatch.Length is 0)
            {
                throw new ArgumentException(nameof(valueToMatch));
            }

            var servers = _connectionMultiplexer.GetServers();
            if (_redisDistributedCacheKeyRetrieverOptions.OnlyUseReplicas)
            {
                servers = servers.Where(x =>x.IsReplica).ToArray();
            }

            if (!servers.Any())
            {
                return AsyncEnumerable.Empty<string>();
            }
                throw new NotImplementedException();
        }
    }
}