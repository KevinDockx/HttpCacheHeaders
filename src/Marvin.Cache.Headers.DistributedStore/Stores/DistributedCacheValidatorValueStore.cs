using System;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores
{
    public class DistributedCacheValidatorValueStore
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRetrieveDistributedCacheKeys _distributedCacheKeyRetriever;

        public DistributedCacheValidatorValueStore(IDistributedCache distributedCache, IRetrieveDistributedCacheKeys distributedCacheKeyRetriever)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _distributedCacheKeyRetriever = distributedCacheKeyRetriever ?? throw new ArgumentNullException(nameof(distributedCacheKeyRetriever));
        }
    }
}