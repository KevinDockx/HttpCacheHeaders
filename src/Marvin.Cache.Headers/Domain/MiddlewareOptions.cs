// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System.Collections.Generic;
using System.Linq;

namespace Marvin.Cache.Headers;

/// <summary>
/// Options that have to do with the HttpCacheHeadersMiddleware, mainly related to ignoring header generation in some cases.
/// </summary>
public class MiddlewareOptions
{
    /// <summary>
    /// Set this to true if you don't want to automatically generate headers for all endpoints.  
    /// The value "true" is typically used in accordance with [HttpCacheValidation] and [HttpCacheExpiration] attributes to only enable it for a few endpoints.
    /// 
    /// Defaults to false.
    /// </summary>
    public bool DisableGlobalHeaderGeneration { get; set; } = false;
    
    /// <summary>
    /// Ignore header generation for responses with specific status codes.  
    /// Often used when you don't want to generate headers for error responses.
    ///
    /// Defaults to none.
    /// </summary>
    public IEnumerable<int> IgnoredStatusCodes { get; set; } = Enumerable.Empty<int>();
}