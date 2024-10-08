﻿// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Stores;

/// <summary>
/// In-memory implementation of <see cref="IValidatorValueStore"/>.
/// </summary>
public class InMemoryValidatorValueStore : IValidatorValueStore
{
    // store for validatorvalues
    private readonly IMemoryCache _store;

    // store for storekeys - different store to speed up search.  
    //
    // A ConcurrentList or ConcurrentHashSet would be slightly better, but they don't 
    // exist out of the box.  ConcurrentBag doesn't safely allow removing a specific
    // item, so: ConcurrentDictionary it is.
    private readonly ConcurrentDictionary<string, string> _storeKeyStore;

    //Serializer for StoreKeys.
    private readonly IStoreKeySerializer _storeKeySerializer;

    public InMemoryValidatorValueStore(IStoreKeySerializer storeKeySerializer, IMemoryCache store, ConcurrentDictionary<string, string> storeKeyStore = null)
    {
        _storeKeySerializer = storeKeySerializer ?? throw new ArgumentNullException(nameof(storeKeySerializer));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _storeKeyStore = storeKeyStore ?? new ConcurrentDictionary<string, string>();
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
        _storeKeyStore[keyJson] = keyJson;
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

        if (!_storeKeyStore.ContainsKey(keyJson))
        {
            return Task.FromResult(false);
        }             

        _store.Remove(keyJson);
        _ = _storeKeyStore.TryRemove(keyJson, out string _);
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

        foreach (var keyValuePair in _storeKeyStore)
        {
            var deserializedKey = _storeKeySerializer.DeserializeStoreKey(keyValuePair.Key);
            var deserializedKeyValues = String.Join(',', ignoreCase ? deserializedKey.Values.Select(x => x.ToLower()) : deserializedKey.Values);
            if (deserializedKeyValues.Contains(valueToMatch))
            {
                yield return deserializedKey;
            }
        }
    }
}