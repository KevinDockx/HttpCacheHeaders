using Marvin.Cache.Headers.DistributedStore.Interfaces;
using Marvin.Cache.Headers.DistributedStore.Redis.Extensions;
using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Marvin.Cache.Headers.Test.TestStartups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
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
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var host = new WebHostBuilder()
            .UseStartup<DefaultStartup>()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer.Object);
                services.AddRedisKeyRetriever(Xunit => { });
            })
            .Build();
        Assert.NotNull(host.Services.GetService(typeof(IRetrieveDistributedCacheKeys)));
    }
}