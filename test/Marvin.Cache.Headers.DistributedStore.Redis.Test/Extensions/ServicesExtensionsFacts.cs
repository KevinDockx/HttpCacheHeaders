using Marvin.Cache.Headers.DistributedStore.Redis.Extensions;
using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Test.Extensions;

public class ServicesExtensionsFacts
{
    [Fact]
    public void AddRedisKeyRetriever_Throws_An_Argument_Null_Exception_When_The_Services_Parameter_Passed_In_Is_Null()
    {
        IServiceCollection? services = null;
        Action<IOptions<RedisDistributedCacheKeyRetrieverOptions>> redisDistributedCacheKeyRetrieverOptionsAction =o =>{};
        Assert.Throws<ArgumentNullException>(() => services.AddRedisKeyRetriever(redisDistributedCacheKeyRetrieverOptionsAction));
    }
}