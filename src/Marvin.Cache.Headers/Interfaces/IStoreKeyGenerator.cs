// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <param name="httpRequest">The incoming <see cref="HttpRequest"/>.</param>
        /// <param name="varyByHeaderKeys">The keys of the headers to (potentially) vary by for this resource. 
        /// If VaryByAll is set to true this will contain all the current request header keys.  
        /// If VaryByAll is set to false it will contain the inputted list as configured in <see cref="ValidationModelOptions"/> 
        /// or set via the <see cref="HttpCacheValidationAttribute"/>.</param>
        /// <returns></returns>
        Task<StoreKey> GenerateStoreKey(
            HttpRequest httpRequest,
            IEnumerable<string> varyByHeaderKeys);
    }
}
