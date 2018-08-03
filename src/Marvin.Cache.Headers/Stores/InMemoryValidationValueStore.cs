// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Stores
{
    public class InMemoryValidationValueStore : IValidationValueStore
    {
        private readonly ConcurrentDictionary<string, ValidationValue> _store
            = new ConcurrentDictionary<string, ValidationValue>();

        public Task<ValidationValue> GetAsync(string key)
        {
            return _store.ContainsKey(key) && _store[key] is ValidationValue eTag
                ? Task.FromResult(eTag)
                : Task.FromResult<ValidationValue>(null);
        }

        public Task SetAsync(string key, ValidationValue eTag)
        {
            _store[key] = eTag;
            return Task.FromResult(0);
        }

        public Task<ValidationValue> GetAsync(RequestKey key) => GetAsync(key.ToString());

        public Task SetAsync(RequestKey key, ValidationValue eTag) => SetAsync(key.ToString(), eTag);
    }
}
