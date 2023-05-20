using Marvin.Cache.Headers.DistributedStore.Redis.Extensions;
using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.DistributedStore.Redis.Stores;
using Moq;
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
    
    [Fact]
    public void AddRedisKeyRetriever_Throws_An_Argument_Null_Exception_When_The_RedisDistributedCacheKeyRetrieverOptionsAction_Parameter_Passed_In_Is_Null()
    {
        var services = new Mock<IServiceCollection>();
        Action<IOptions<RedisDistributedCacheKeyRetrieverOptions>>? redisDistributedCacheKeyRetrieverOptionsAction = null;
        Assert.Throws<ArgumentNullException>(() => services.Object.AddRedisKeyRetriever(redisDistributedCacheKeyRetrieverOptionsAction));
    }

    [Fact]
    public void AddRedisKeyRetriever_Successfully_Registers_All_Required_Services()
    {
        var services = new Mock<IServiceCollection>();
        var hasTheRedisDistributedCacheKeyRetrieverOptionsActionBeenCalled = false;
        Action<IOptions<RedisDistributedCacheKeyRetrieverOptions>> redisDistributedCacheKeyRetrieverOptionsAction = o => hasTheRedisDistributedCacheKeyRetrieverOptionsActionBeenCalled = true;
        services.Object.AddRedisKeyRetriever(redisDistributedCacheKeyRetrieverOptionsAction);
        Assert.True(hasTheRedisDistributedCacheKeyRetrieverOptionsActionBeenCalled);
        services.Verify(x =>x.Add(It.Is< ServiceDescriptor>(x =>VerifyTheServiceDescriptorProperties(x))), Times.Exactly(1));
    }

    private static bool VerifyTheServiceDescriptorProperties(ServiceDescriptor serviceDescriptor) =>
        serviceDescriptor.Lifetime == ServiceLifetime.Singleton 
        && serviceDescriptor.ServiceType == typeof(IRetrieveDistributedCacheKeys) 
        && serviceDescriptor.ImplementationType ==typeof(RedisDistributedCacheKeyRetriever);
}