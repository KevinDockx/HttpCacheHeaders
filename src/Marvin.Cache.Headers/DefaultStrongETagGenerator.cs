// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Text;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Extensions;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers
{
    public class DefaultStrongETagGenerator : IETagGenerator
    {
        // By default using the HTTP context to acquire an ETag is not available. This
        // method must always be provided by the consumer of this package.
        public virtual Task<ETag> GenerateETag(HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey("ETag"))
            {
                return Task.FromResult(new ETag(httpContext.Items["ETag"] as string));
            }
            else
            {
                return Task.FromResult(default(ETag));
            }
        }

        // Key = generated from request URI & headers (if VaryBy is set, only use those headers)
        // ETag itself is generated from the key + response body (strong ETag)
        public virtual Task<ETag> GenerateETag(
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
