// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    public interface IValidationValueStore
    {
        Task<ValidationValue> GetAsync(string key);
        Task SetAsync(string key, ValidationValue eTag);
    }
}
