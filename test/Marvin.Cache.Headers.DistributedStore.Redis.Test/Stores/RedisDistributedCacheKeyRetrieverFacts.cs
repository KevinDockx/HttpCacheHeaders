using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Marvin.Cache.Headers.DistributedStore.Redis.Stores;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using Xunit;

namespace Marvin.Cache.Headers.DistributedStore.Redis.Test.Stores;

public class RedisDistributedCacheKeyRetrieverFacts
{
    [Fact]
    public void Throws_ArgumentNullException_When_A_Null_Connection_Multiplexer_Is_Passed_in()
    {
        IConnectionMultiplexer connectionMultiplexer = null;
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>();
        var exception = Record.Exception(() => new RedisDistributedCacheKeyRetriever(connectionMultiplexer, redisDistributedCacheKeyRetrieverOptions.Object));
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Throws_ArgumentNullException_When_A_NullRedis_Distributed_Cache_Key_Retriever_Options_Is_Passed_in()
    {
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        IOptions<RedisDistributedCacheKeyRetrieverOptions> redisDistributedCacheKeyRetrieverOptions = null;
        var exception = Record.Exception(() => new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions));
        Assert.IsType<ArgumentNullException>(exception);
    }
}