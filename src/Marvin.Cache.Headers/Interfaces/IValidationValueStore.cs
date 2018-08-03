// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    public interface IValidationValueStore
    {
        Task<ValidationValue> GetAsync(StoreKey key);
        Task SetAsync(StoreKey key, ValidationValue eTag);
    }
}
