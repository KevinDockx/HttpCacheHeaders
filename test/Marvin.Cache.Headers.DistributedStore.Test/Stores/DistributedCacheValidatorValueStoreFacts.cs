using System;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Marvin.Cache.Headers.DistributedStore.Test.Stores;

public class DistributedCacheValidatorValueStoreFacts
{
    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_DistributedCache_Is_Passed_in()
    {
        IDistributedCache distributedCache = null;
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        Assert.Throws<ArgumentNullException>(() => new DistributedCacheValidatorValueStore(distributedCache, distributedCacheKeyRetriever.Object));
    }

    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_DistributedCacheKeyRetriever_Is_Passed_in2()
    {
        var distributedCache = new Mock<IDistributedCache>();
        IRetrieveDistributedCacheKeys distributedCacheKeyRetriever = null;
        Assert.Throws<ArgumentNullException>(() => new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever));
    }
}