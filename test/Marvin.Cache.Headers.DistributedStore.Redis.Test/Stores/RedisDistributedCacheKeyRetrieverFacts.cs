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

    [Fact]
    public void Throws_ArgumentNullException_When_The_Value_Property_Of_The_Passed_In_RedisDistributedCacheKeyRetrieverOptions_Is_Null()
    {
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>(); 
        redisDistributedCacheKeyRetrieverOptions.SetupGet(x =>x.Value).Returns((RedisDistributedCacheKeyRetrieverOptions)null);
        var exception = Record.Exception(() => new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions.Object));
        Assert.IsType<ArgumentNullException>(exception);
        redisDistributedCacheKeyRetrieverOptions.VerifyGet(x =>x.Value, Times.Exactly(1));
    }

    [Fact]
    public void Constructs_A_RedisDistributedCacheKeyRetriever_When_All_The_Passed_In_Parameters_Are_Valid()
    {
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>();
        redisDistributedCacheKeyRetrieverOptions.SetupGet(x => x.Value).Returns(new RedisDistributedCacheKeyRetrieverOptions());
        var redisDistributedCacheKeyRetriever = new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions.Object);
        Assert.NotNull(redisDistributedCacheKeyRetriever);
        redisDistributedCacheKeyRetrieverOptions.VerifyGet(x => x.Value, Times.Exactly(2));
    }
}