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
            if (_store.ContainsKey(key))
            {
                var eTag = _store[key] as ValidationValue;
                if (eTag != null)
                {
                    return Task.FromResult(eTag);
                }
            }
            // not found
            return Task.FromResult<ValidationValue>(null);
        }

        public Task SetAsync(string key, ValidationValue eTag)
        {
            _store[key] = eTag;
            return Task.FromResult(0);
        }
    }
}
