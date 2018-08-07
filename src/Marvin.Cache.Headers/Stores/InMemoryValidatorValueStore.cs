// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Stores
{
    /// <summary>
    /// In-memory implementation of <see cref="IValidatorValueStore"/>.
    /// </summary>
    public class InMemoryValidatorValueStore : IValidatorValueStore
    {
        private readonly ConcurrentDictionary<string, ValidatorValue> _store
            = new ConcurrentDictionary<string, ValidatorValue>();

        public Task<ValidatorValue> GetAsync(StoreKey key) => GetAsync(key.ToString());

        public Task SetAsync(StoreKey key, ValidatorValue eTag) => SetAsync(key.ToString(), eTag);

        private Task<ValidatorValue> GetAsync(string key)
        {
            return _store.ContainsKey(key) && _store[key] is ValidatorValue eTag
                ? Task.FromResult(eTag)
                : Task.FromResult<ValidatorValue>(null);
        }

        private Task SetAsync(string key, ValidatorValue eTag)
        {
            _store[key] = eTag;
            return Task.FromResult(0);
        }
    }
}
