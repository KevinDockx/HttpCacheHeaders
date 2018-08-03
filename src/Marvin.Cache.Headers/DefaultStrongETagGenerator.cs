// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Text;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Extensions;

namespace Marvin.Cache.Headers
{
    public interface IETagGenerator
    {
        Task<ETag> GenerateETag(
            StoreKey storeKey,
            string responseBodyContent);
    }

    public class DefaultStrongETagGenerator : IETagGenerator
    {
        // Key = generated from request URI & headers (if VaryBy is set, only use those headers)
        // ETag itself is generated from the key + response body (strong ETag)
        public Task<ETag> GenerateETag(
            StoreKey storeKey,
            string responseBodyContent)
        {
            var requestKeyAsBytes = Encoding.UTF8.GetBytes(storeKey.ToString());
            var responseBodyContentAsBytes = Encoding.UTF8.GetBytes(responseBodyContent);

            // combine both to generate an etag
            var combinedBytes = Combine(requestKeyAsBytes, responseBodyContentAsBytes);

            return Task.FromResult(new ETag(ETagType.Strong, combinedBytes.GenerateMD5Hash()));
        }

        private static byte[] Combine(byte[] a, byte[] b)
        {
            var c = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, c, 0, a.Length);
            Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }
    }
}
