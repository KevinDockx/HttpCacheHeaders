// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    /// <summary>
    /// Contract for an Validator Value Generator, used to generate the unique weak or strong E-Tags for cache items and Last Modified Time.
    /// </summary>
    public interface IValidatorValueGenerator
    {
        Task<ValidatorValue> Generate(
            StoreKey storeKey,
            HttpContext httpContext,
            IETagGenerator eTagGenerator = null);
    }
}
