// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    /// <summary>
    /// Contract for a store for validator values.  Each item is stored with a <see cref="StoreKey" /> as key 
    /// and a <see cref="ValidatorValue" /> as value (consisting of an ETag and Last-Modified date).   
    /// </summary>
    public interface IValidatorValueStore
    {
        /// <summary>
        /// Get a value from the store.
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/> of the value to get.</param>
        /// <returns></returns>
        Task<ValidatorValue> GetAsync(StoreKey key);

        /// <summary>
        /// Set a value in the store.
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/> of the value to store.</param>
        /// <param name="validatorValue">The <see cref="ValidatorValue"/> to store.</param>
        /// <returns></returns>
        Task SetAsync(StoreKey key, ValidatorValue validatorValue);

        /// <summary>
        /// Remove a value from the store.
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/> of the value to remove.</param> 
        /// <returns></returns>
        Task<bool> RemoveAsync(StoreKey key);

        /// <summary>
        /// Find one or more keys that contain the inputted valueToMatch 
        /// </summary>
        /// <param name="valueToMatch">The value to match as part of the key</param>
        /// <returns></returns>
        Task<IEnumerable<StoreKey>> FindStoreKeysByKeyPartAsync(string valueToMatch);
    }
}
