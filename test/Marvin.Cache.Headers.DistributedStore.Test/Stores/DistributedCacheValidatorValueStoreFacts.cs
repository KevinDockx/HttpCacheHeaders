using System;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores;

public class DistributedCacheValidatorValueStoreFacts
{
    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_DistributedCache_Is_Passed_in()
    {
        IDistributedCache distributedCache = null;
        Assert.Throws<ArgumentNullException>(() => new DistributedCacheValidatorValueStore(distributedCache));
    }
}