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

        public async IAsyncEnumerable<string> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase =true)
        {
            if (valueToMatch == null)
            {
                throw new ArgumentNullException(nameof(valueToMatch));
            }
            else if (valueToMatch.Length == 0)
            {
                throw new ArgumentException(nameof(valueToMatch));
            }

            var servers = _connectionMultiplexer.GetServers();
            if (_redisDistributedCacheKeyRetrieverOptions.OnlyUseReplicas)
            {
                servers = servers.Where(x => x.IsReplica).ToArray();
            }

            RedisValue valueToMatchWithRedisPattern = ignoreCase ? $"pattern: {valueToMatch.ToLower()}" : $"pattern: {valueToMatch}";
            List<string> foundKeys =new List<string>();
            foreach (var server in servers)
            {
                var keys = server.KeysAsync(_redisDistributedCacheKeyRetrieverOptions.Database, valueToMatchWithRedisPattern);
                await foreach (var key in keys)
                {
                    yield return key;
                }
                }
            }
    }
}