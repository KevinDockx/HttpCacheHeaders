// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders


using System;

namespace Marvin.Cache.Headers.Interfaces
{
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
    }
}