using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores
{
    public class DistributedCacheValidatorValueStore : IValidatorValueStore
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRetrieveDistributedCacheKeys _distributedCacheKeyRetriever;

        public DistributedCacheValidatorValueStore(IDistributedCache distributedCache, IRetrieveDistributedCacheKeys distributedCacheKeyRetriever)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _distributedCacheKeyRetriever = distributedCacheKeyRetriever ?? throw new ArgumentNullException(nameof(distributedCacheKeyRetriever));
        }

        public async Task<ValidatorValue> GetAsync(StoreKey key)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync(StoreKey key, ValidatorValue validatorValue)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(StoreKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StoreKey>> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase)
        {
            throw new NotImplementedException();
        }
    }
}