using System;
using Microsoft.Extensions.Caching.Distributed;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores
{
    public class DistributedCacheValidatorValueStore
    {
        public DistributedCacheValidatorValueStore(IDistributedCache distributedCache)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }
            
            throw new NotImplementedException();
        }
    }
}