using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.DistributedStore.Interfaces
{
    public interface IRetrieveDistributedCacheKeys
    {
        Task<IEnumerable<string>> FindStoreKeysByKeyPartAsync(string valueToMatch, bool ignoreCase);
    }
}