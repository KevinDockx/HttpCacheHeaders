// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Domain;
using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers.Interfaces
{
    /// <summary>
    /// Contract for a key generator, used to generate a <see cref="StoreKey" /> 
    /// </summary>
    public interface IStoreKeyGenerator
    {
        /// <summary>
        /// Generate a key for storing a <see cref="ValidatorValue"/> in a <see cref="IValidatorValueStore"/>.
        /// </summary>
        /// <param name="context">The <see cref="StoreKeyContext"/>.</param>         
        /// <returns></returns>
        Task<StoreKey> GenerateStoreKey(
            StoreKeyContext context);
    }
}
