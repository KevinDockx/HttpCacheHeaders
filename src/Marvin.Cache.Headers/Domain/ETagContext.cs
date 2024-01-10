// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers;

/// <summary>
///     Context containing information that might be useful when generating or retrieving an e-Tag
/// </summary>
public class ETagContext
{
    public ETagContext(StoreKey storeKey, HttpContext httpContext)
    {
        StoreKey = storeKey;
        HttpContext = httpContext;
    }

    /// <summary>
    ///     The current <see cref="StoreKey" /> for the resource
    /// </summary>
    public StoreKey StoreKey { get; private set; }

    /// <summary>
    ///     The current <see cref="HttpContext" /> allowing access to the <see cref="HttpResponse" />
    /// </summary>
    public HttpContext HttpContext { get; private set; }
}