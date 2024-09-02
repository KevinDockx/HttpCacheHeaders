// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Security.Cryptography;

namespace Marvin.Cache.Headers.Extensions;

public static class ByteExtensions
{
    // from http://jakzaprogramowac.pl/pytanie/20645,implement-http-cache-etag-in-aspnet-core-web-api
    public static string GenerateMD5Hash(this byte[] data)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(data);
            var hex = BitConverter.ToString(hash);
            return hex.Replace("-", "");
        }
    }
}
