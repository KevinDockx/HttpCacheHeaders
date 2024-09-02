using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Marvin.Cache.Headers.Domain;

/// <summary>
/// Context containing information that might be useful when generating a custom store key.
/// </summary>
public class StoreKeyContext
{
    /// <summary>
    /// The current <see cref="HttpRequest"/>
    /// </summary>
    public HttpRequest HttpRequest { get;  }

    /// <summary>
    /// The Vary header keys as set on <see cref="ValidationModelOptions"/> or through <see cref="HttpCacheValidationAttribute"/>
    /// </summary>
    public IEnumerable<string> Vary { get;  }

    /// <summary>
    /// The VaryByAll option as set on <see cref="ValidationModelOptions"/> or through <see cref="HttpCacheValidationAttribute"/>
    /// </summary>
    public bool VaryByAll { get;   }

    public StoreKeyContext(
        HttpRequest httpRequest, 
        IEnumerable<string> vary, 
        bool varyByAll)
    {
        HttpRequest = httpRequest;
        Vary = vary;
        VaryByAll = varyByAll;
    }
}
