// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using Marvin.Cache.Headers.Interfaces;

namespace Marvin.Cache.Headers.Serialization
{
    /// <summary>
    /// Serializes a <see cref="StoreKey"/> to JSON./// </summary>
    public class DefaultStoreKeySerializer : IStoreKeySerializer
    {
        ///<inheritDoc/>
        public string SerializeStoreKey(StoreKey keyToSerialize)
        {
            throw new NotImplementedException();
        }
    }
}