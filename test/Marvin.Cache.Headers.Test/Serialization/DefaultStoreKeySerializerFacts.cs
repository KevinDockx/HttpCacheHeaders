using System;
using System.Text.Json;
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
    public void SerializeStoreKey_ReturnsTheKeyToSerializeAsJson_WhenStoreKeyIsNotNull()
    {
        var keyToSerialize = new StoreKey
        {
            { "testKey", "TestValue" }
        };
        const string expectedStoreKeyJson = "{\"testKey\":\"TestValue\"}";

        var serializedStoreKey = _storeKeySerializer.
            SerializeStoreKey(keyToSerialize);

        JsonAssert.Equal(expectedStoreKeyJson, serializedStoreKey);
    }

    [Fact]
    public void DeserializeStoreKey_ThrowsArgumentNullException_WhenStoreKeyJsonIsNull()
    {
        string storeKeyJson = null;
        Assert.Throws<ArgumentNullException>(() => _storeKeySerializer.DeserializeStoreKey(storeKeyJson));
    }

    [Fact]
    public void DeserializeStoreKey_ThrowsArgumentException_WhenStoreKeyJsonIsAnEmptyString()
    {
        var storeKeyJson = String.Empty;
        Assert.Throws<ArgumentException>(() => _storeKeySerializer.DeserializeStoreKey(storeKeyJson));
    }
    [Fact]
    public void DeserializeStoreKey_ThrowsJsonException_WhenStoreKeyJsonIsInvalid()
    {
        const string storeKeyJson = "{";
        Assert.Throws<JsonException>(() => _storeKeySerializer.DeserializeStoreKey(storeKeyJson));
    }

    [Fact]
    public void DeserializeStoreKey_ReturnsTheStoreKeyJsonAsAStoreKey_WhenTheStoreKeyJsonIsValidJson()
    {
        var expectedStoreKey = new StoreKey
        {
            { "testKey", "TestValue" }
        };
        const string storeKeyJson = "{\"testKey\":\"TestValue\"}";

        var deserializedStoreKey = _storeKeySerializer.DeserializeStoreKey(storeKeyJson);

        Assert.Equal(expectedStoreKey, deserializedStoreKey);
    }
}