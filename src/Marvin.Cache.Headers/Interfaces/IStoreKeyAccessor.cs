// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;

namespace Marvin.Cache.Headers;

/// <summary>
/// Contract for finding (a) <see cref="StoreKey" />(s)
/// </summary>    
public interface IStoreKeyAccessor
{
    /// <summary>
    /// Find a  <see cref="StoreKey" /> by part of the key
    /// </summary>
    /// <param name="valueToMatch">The value to match as part of the key</param>
    /// <param name="ignoreCase">Ignore case when matching (default = true)</param>
    /// <returns></returns>
    IAsyncEnumerable<StoreKey> FindByKeyPart(string valueToMatch, bool ignoreCase = true);

    /// <summary>
    /// Find a  <see cref="StoreKey" /> of which the current resource path is part of the key
    /// </summary>
    /// <param name="ignoreCase">Ignore case when matching (default = true)</param>
    /// <returns></returns>
    IAsyncEnumerable<StoreKey> FindByCurrentResourcePath(bool ignoreCase = true);
}