using Marvin.Cache.Headers.DistributedStore.Redis.Options;
using Marvin.Cache.Headers.DistributedStore.Redis.Stores;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    [Fact]
    public void FindStoreKeysByKeyPartAsync_Throws_An_Argument_Null_Exception_When_The_valueToMatch_Passed_in_Is_null()
    {
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>();
        redisDistributedCacheKeyRetrieverOptions.SetupGet(x => x.Value).Returns(new RedisDistributedCacheKeyRetrieverOptions());
        var redisDistributedCacheKeyRetriever = new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions.Object);
        string? valueToMatch = null;
        var exception = Record.Exception(() => redisDistributedCacheKeyRetriever.FindStoreKeysByKeyPartAsync(valueToMatch));
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void FindStoreKeysByKeyPartAsync_Throws_An_Argument_Exception_When_The_valueToMatch_Passed_in_Is_an_empty_string()
    {
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>();
        redisDistributedCacheKeyRetrieverOptions.SetupGet(x => x.Value).Returns(new RedisDistributedCacheKeyRetrieverOptions());
        var redisDistributedCacheKeyRetriever = new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions.Object);
        var valueToMatch = String.Empty;
        var exception = Record.Exception(() => redisDistributedCacheKeyRetriever.FindStoreKeysByKeyPartAsync(valueToMatch));
        Assert.IsType<ArgumentException>(exception);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FindStoreKeysByKeyPartAsync_Returns_An_Empty_Collection_Of_Keys_When_No_Servers_Are_available(bool onlyUseReplicas)
    {
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        connectionMultiplexer.Setup(x => x.GetServers()).Returns(Array.Empty<IServer>());
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>();
        var redisDistributedCacheKeyRetrieverOptionsValue = new RedisDistributedCacheKeyRetrieverOptions
            {
                OnlyUseReplicas = onlyUseReplicas
            };
        redisDistributedCacheKeyRetrieverOptions.SetupGet(x => x.Value).Returns(redisDistributedCacheKeyRetrieverOptionsValue);
        var redisDistributedCacheKeyRetriever = new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions.Object);
        var valueToMatch = "test";
        var result = redisDistributedCacheKeyRetriever.FindStoreKeysByKeyPartAsync(valueToMatch);
        var hasKeys = await result.AnyAsync();
        Assert.False(hasKeys);
        connectionMultiplexer.Verify(x => x.GetServers(), Times.Exactly(1));
    }

    [Theory, CombinatorialData]
    public async Task FindStoreKeysByKeyPartAsync_Returns_An_Empty_Collection_Of_Keys_When_At_Least_One_Server_Is_Available_But_No_Keys_Exist_On_Any_Of_The_Available_Servers_That_Match_The_Past_in_Value_To_Match_In_The_Passed_In_Database(bool onlyUseReplicas, bool ignoreCase, [CombinatorialRange(1, 2)] int numberOfServers)
    {
        var valueToMatch = GetValueToMatch(ignoreCase);
        var redisDistributedCacheKeyRetrieverOptionsValue = new RedisDistributedCacheKeyRetrieverOptions
        {
            OnlyUseReplicas = onlyUseReplicas,
            Database = 0
        };

        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var servers = new List<Mock<IServer>>(numberOfServers);
        for (int i = 0; i < numberOfServers; i++)
        {
            var server = new Mock<IServer>();
            server.SetupGet(x => x.IsReplica).Returns(onlyUseReplicas);
            server.Setup(x => x.KeysAsync(It.Is<int>(v =>v ==redisDistributedCacheKeyRetrieverOptionsValue.Database), It.Is<RedisValue>(v =>v.Equals(valueToMatch)), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>())).Returns(AsyncEnumerable.Empty<RedisKey>());
            servers.Add(server);
        }
        connectionMultiplexer.Setup(x => x.GetServers()).Returns(servers.Select(x =>x.Object).ToArray);
        var redisDistributedCacheKeyRetrieverOptions = new Mock<IOptions<RedisDistributedCacheKeyRetrieverOptions>>();
        redisDistributedCacheKeyRetrieverOptions.SetupGet(x => x.Value).Returns(redisDistributedCacheKeyRetrieverOptionsValue);
        var redisDistributedCacheKeyRetriever = new RedisDistributedCacheKeyRetriever(connectionMultiplexer.Object, redisDistributedCacheKeyRetrieverOptions.Object);

        var result = redisDistributedCacheKeyRetriever.FindStoreKeysByKeyPartAsync(valueToMatch);
        var hasKeys = await result.AnyAsync();
        Assert.False(hasKeys);
        connectionMultiplexer.Verify(x => x.GetServers(), Times.Exactly(numberOfServers));
        
        foreach (var server in servers)
        {
            server.VerifyGet(x => x.IsReplica);
            server.Verify(x => x.KeysAsync(It.Is<int>(v => v == redisDistributedCacheKeyRetrieverOptionsValue.Database), It.Is<RedisValue>(v => v.Equals(valueToMatch)), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()), Times.Exactly(1));
        }
    }

    private static RedisValue GetValueToMatch(bool ignoreCase) => ignoreCase ? $"pattern: {"TestKey".ToLower()}" : "pattern: TestKey";
}