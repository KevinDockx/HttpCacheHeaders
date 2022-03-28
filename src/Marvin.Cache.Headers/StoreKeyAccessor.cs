// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers
{
    /// <summary>
    /// An accessor for finding <see cref="StoreKey" />(s)
    /// </summary>    
    public class StoreKeyAccessor : IStoreKeyAccessor
    {
        private readonly IValidatorValueStore _validatorValueStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreKeyAccessor(IValidatorValueStore validatorValueStore,
            IStoreKeyGenerator storeKeyGenerator,
            IHttpContextAccessor httpContextAccessor)
        {
            _validatorValueStore = validatorValueStore
                ?? throw new ArgumentNullException(nameof(validatorValueStore));
            _httpContextAccessor = httpContextAccessor 
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Find a  <see cref="StoreKey" /> by part of the key
        /// </summary>
        /// <param name="valueToMatch">The value to match as part of the key</param>
        /// <returns></returns>
        public async Task<IEnumerable<StoreKey>> FindByKeyPart(string valueToMatch, bool ignoreCase = true)
        {
            return await _validatorValueStore.FindStoreKeysByKeyPartAsync(valueToMatch, ignoreCase); 
        }

        /// <summary>
        /// Find a  <see cref="StoreKey" /> of which the current resource path is part of the key
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<StoreKey>> FindByCurrentResourcePath(bool ignoreCase = true)
        {
            string path = _httpContextAccessor.HttpContext.Request.Path.ToString();
            return await _validatorValueStore.FindStoreKeysByKeyPartAsync(path, ignoreCase); 
        }         
    }
}
