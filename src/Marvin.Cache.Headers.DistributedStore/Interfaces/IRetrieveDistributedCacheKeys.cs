using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.DistributedStore.Interfaces
{
    public interface IRetrieveDistributedCacheKeys
    {
        IAsyncEnumerable<string> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase);
    }
}