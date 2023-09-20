// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Stores
{
    /// <summary>
    /// In-memory implementation of <see cref="IValidatorValueStore"/>.
    /// </summary>
    public class InMemoryValidatorValueStore : IValidatorValueStore
    {
        // store for validatorvalues
        private readonly IMemoryCache _store;
        
        // store for storekeys - different store to speed up search 
        private readonly HashSet<string> _storeKeyStore;
            
        //Serializer for StoreKeys.
        private readonly IStoreKeySerializer _storeKeySerializer;

        public InMemoryValidatorValueStore(IStoreKeySerializer storeKeySerializer, IMemoryCache store, HashSet<string>storeKeyStore =null)
        {
            _storeKeySerializer =storeKeySerializer ?? throw new ArgumentNullException(nameof(storeKeySerializer));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _storeKeyStore = storeKeyStore ?? new HashSet<string>();
        }

        public Task<ValidatorValue> GetAsync(StoreKey key)
        {
            var keyJson = _storeKeySerializer.SerializeStoreKey(key);
            return Task.FromResult(!_store.TryGetValue(keyJson, out ValidatorValue eTag) ? null : eTag);
        }

        /// <summary>
        /// Add an item to the store (or update it)
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/>.</param>
        /// <param name="eTag">The <see cref="ValidatorValue"/>.</param>
        /// <returns></returns>
        public Task SetAsync(StoreKey key, ValidatorValue eTag)
        {
            // store the validator value
            var keyJson = _storeKeySerializer.SerializeStoreKey(key);
            _store.Set(keyJson, eTag);
            
            // save the key itself as well, with an easily searchable stringified key
            _storeKeyStore.Add(keyJson);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Remove an item from the store
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/>.</param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(StoreKey key)
        {
            var keyJson = _storeKeySerializer.SerializeStoreKey(key);
            if (!_storeKeyStore.Contains(keyJson))
            {
                return Task.FromResult(false);
            }
            
            _store.Remove(keyJson);
            _storeKeyStore.Remove(keyJson);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Find store keys
        /// </summary>
        /// <param name="valueToMatch">The value to match as (part of) the key</param>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously - disabled, in-memory implementation doesn't need await.
        public async IAsyncEnumerable<StoreKey> FindStoreKeysByKeyPartAsync(string valueToMatch,
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            bool ignoreCase)
        {
            var lstStoreKeysToReturn = new List<StoreKey>();

            // search for keys that contain valueToMatch
            if (ignoreCase)
            {
                valueToMatch = valueToMatch.ToLowerInvariant();
            }

            foreach (var key in _storeKeyStore)
                {
var deserializedKey =_storeKeySerializer.DeserializeStoreKey(key);
var deserializedKeyValues = String.Join(',', ignoreCase ? deserializedKey.Values.Select(x => x.ToLower()) : deserializedKey.Values);
if (deserializedKeyValues.Contains(valueToMatch))
{
yield return deserializedKey;
}
                }
        }
    }
    }