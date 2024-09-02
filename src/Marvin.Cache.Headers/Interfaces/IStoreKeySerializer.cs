// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders


using System;
using System.Text.Json;

namespace Marvin.Cache.Headers.Interfaces;

/// <summary>
/// Contract for a key serializer, used to serialize a <see cref="StoreKey" /> 
/// </summary>
public interface IStoreKeySerializer
{
    /// <summary>
    /// Serialize a <see cref="StoreKey"/>.
    /// </summary>
    /// <param name="keyToSerialize">The <see cref="StoreKey"/> to be serialized.</param>
    /// <returns>The <param name="keyToSerialize"/> serialized to a <see cref="string"/>.</returns>
    ///<exception cref="ArgumentNullException">thrown when the <paramref name="keyToSerialize"/> passed in is <c>null</c>.</exception>
    string SerializeStoreKey(StoreKey keyToSerialize);

    /// <summary>
    /// Deserialize a <see cref="StoreKey"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="storeKeyJson">The Json representation of a <see cref="StoreKey"/> to be deserialized.</param>
    /// <returns>The <param name="storeKeyJson"/> deserialized to a <see cref="StoreKey"/>.</returns>
    ///<exception cref="ArgumentNullException">thrown when the <paramref name="storeKeyJson"/> passed in is <c>null</c>.</exception>
    ///<exception cref="ArgumentException">thrown when the <paramref name="storeKeyJson"/> passed in is an empty string.</exception>
    ///<exception cref="JsonException">thrown when the <paramref name="storeKeyJson"/> passed in cannot be deserialized to a <see cref="StoreKey"/>.</exception>
    StoreKey DeserializeStoreKey(string storeKeyJson);
}