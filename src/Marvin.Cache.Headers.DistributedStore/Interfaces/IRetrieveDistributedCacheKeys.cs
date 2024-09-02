using System.Collections.Generic;

namespace Marvin.Cache.Headers.DistributedStore.Interfaces;

public interface IRetrieveDistributedCacheKeys
{
    IAsyncEnumerable<string> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase =true);
}