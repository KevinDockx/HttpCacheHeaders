using System;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores
{
    public class DistributedCacheValidatorValueStore
    {
        public DistributedCacheValidatorValueStore(IDistributedCache distributedCache, IRetrieveDistributedCacheKeys distributedCacheKeyRetriever)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (distributedCacheKeyRetriever == null)
            {
                throw new ArgumentNullException(nameof(distributedCacheKeyRetriever));
            }
            
            throw new NotImplementedException();
        }
    }
}