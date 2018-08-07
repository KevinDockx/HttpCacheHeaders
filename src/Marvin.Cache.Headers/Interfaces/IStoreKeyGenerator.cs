// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

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
        /// <param name="validationModelOptions">The <see cref="ValidationModelOptions"/> to take into account for this resource.</param>
        /// <returns></returns>
        Task<StoreKey> GenerateStoreKey(
            HttpRequest httpRequest,
            ValidationModelOptions validationModelOptions);
    }
}
