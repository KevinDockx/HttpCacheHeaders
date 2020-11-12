// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    /// <summary>
    /// Contract for the <see cref="ValidatorValueInvalidator" />
    /// </summary>
    public interface IValidatorValueInvalidator
    {
        /// <summary>
        /// Get the list of <see cref="StoreKey" /> of items marked for invalidation
        /// </summary>
        List<StoreKey> KeysMarkedForInvalidation { get; }

        /// <summary>
        /// Mark an item stored with a <see cref="StoreKey" /> for invalidation
        /// </summary>
        /// <param name="storeKey">The <see cref="StoreKey" /></param>
        /// <returns></returns>
        Task MarkForInvalidation(StoreKey storeKey);

        /// <summary>
        /// Mark a set of items for invalidation by their collection of <see cref="StoreKey" /> 
        /// </summary>
        /// <param name="storeKeys">The collection of <see cref="StoreKey" /></param>
        /// <returns></returns>
        Task MarkForInvalidation(IEnumerable<StoreKey> storeKeys);
    }
}