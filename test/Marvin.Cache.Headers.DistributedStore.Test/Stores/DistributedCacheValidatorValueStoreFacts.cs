using System;
using System.Threading.Tasks;
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
        var exception = Record.Exception(() => new DistributedCacheValidatorValueStore(distributedCache, distributedCacheKeyRetriever.Object));
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_DistributedCacheKeyRetriever_Is_Passed_in2()
    {
        var distributedCache = new Mock<IDistributedCache>();
        IRetrieveDistributedCacheKeys distributedCacheKeyRetriever = null;
        var exception = Record.Exception(() => new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever));
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructs_An_DistributedCacheValidatorValueStore_When_All_The_Parameters_Passed_In_Are_Not_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        DistributedCacheValidatorValueStore distributedCacheValidatorValueStore = null;
        var exception = Record.Exception(() => distributedCacheValidatorValueStore =new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object));
        Assert.Null(exception);
        Assert.NotNull(distributedCacheValidatorValueStore);
    }

    [Fact]
    public async Task GetAsync_Throws_An_ArgumentNullException_When_The_Key_Is_Null()
    {
        var distributedCache = new Mock<IDistributedCache>();
        var distributedCacheKeyRetriever = new Mock<IRetrieveDistributedCacheKeys>();
        StoreKey key = null;
        var distributedCacheValidatorValueStore =new DistributedCacheValidatorValueStore(distributedCache.Object, distributedCacheKeyRetriever.Object);
        var exception = await Record.ExceptionAsync(() => distributedCacheValidatorValueStore.GetAsync(key));
        Assert.IsType<ArgumentNullException>(exception);
    }
}