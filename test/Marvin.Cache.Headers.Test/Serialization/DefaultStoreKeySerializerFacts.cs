using System;
using Marvin.Cache.Headers.Serialization;
using Quibble.Xunit;
using Xunit;

namespace Marvin.Cache.Headers.Test.Serialization;

public class DefaultStoreKeySerializerFacts
{
    private readonly DefaultStoreKeySerializer _storeKeySerializer =new();
    
    [Fact]
    public void SerializeStoreKey_ThrowsArgumentNullException_WhenKeyToSerializeIsNull()
    {
        StoreKey keyToSerialize = null;
        Assert.Throws<ArgumentNullException>(() =>_storeKeySerializer.SerializeStoreKey(keyToSerialize));
    }

    [Fact]
    public void SerializeStoreKey_ReturnsTheKeyToSerializeAsJSon_WhenStoreKeyIsNotNull()
    {
        var keyToSerialize = new StoreKey
        {
            { "testKey", "TestValue" }
        };
        const string expectedStoreKeyJson = "{\"testKey\":\"TestValue\"}";

        var serializedStoreKey = _storeKeySerializer.SerializeStoreKey(keyToSerialize);

        JsonAssert.Equal(expectedStoreKeyJson, serializedStoreKey);
    }
}