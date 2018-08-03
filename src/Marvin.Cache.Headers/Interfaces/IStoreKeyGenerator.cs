// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers.Interfaces
{
    public interface IStoreKeyGenerator
    {
        Task<RequestKey> GenerateStoreKey(
            HttpRequest request,
            ValidationModelOptions validationModelOptions);
    }
}
