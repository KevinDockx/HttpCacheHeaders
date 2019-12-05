// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers
{

    /// <summary>
    /// Invalidator for the values from the <see cref="IValidatorValueStore" />
    /// </summary>
    public sealed class ValidatorValueInvalidator : IValidatorValueInvalidator
    {
        // ValidatorValueInvalidator is registered with a scoped lifetime.  It'll thus
        // only be accessed by one thread at a time - no need for concurrent collection implementations

        /// <summary>
        /// Get the list of <see cref="StoreKey" /> of items marked for invalidation
        /// </summary>
        public List<StoreKey> KeysMarkedForInvalidation { get; } = new List<StoreKey>();

        private readonly IValidatorValueStore _validatorValueStore;

        public ValidatorValueInvalidator(IValidatorValueStore validatorValueStore)
        {
            _validatorValueStore = validatorValueStore
                ?? throw new ArgumentNullException(nameof(validatorValueStore));
        }
        
        /// <summary>
        /// Mark an item stored with a <see cref="StoreKey" /> for invalidation
        /// </summary>
        /// <param name="storeKey">The <see cref="StoreKey" /></param>
        /// <returns></returns>
        public Task MarkForInvalidation(StoreKey storeKey)
        {
            if (storeKey == null)
            {
                throw new ArgumentNullException(nameof(storeKey));
            }

            KeysMarkedForInvalidation.Add(storeKey);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Mark a set of items for invlidation by their collection of <see cref="StoreKey" /> 
        /// </summary>
        /// <param name="storeKeys">The collection of <see cref="StoreKey" /></param>
        /// <returns></returns>
        public Task MarkForInvalidation(IEnumerable<StoreKey> storeKeys)
        {
            if (storeKeys == null)
            {
                throw new ArgumentNullException(nameof(storeKeys));
            }

            KeysMarkedForInvalidation.AddRange(storeKeys);

            return Task.CompletedTask;
        }
    }
}
