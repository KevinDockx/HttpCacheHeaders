using System;
using System.IO;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers;

/// <summary>
///     Default E-Tag injector generates an eTag each-and-every time based on the resource payload (ie <see cref="HttpContext"/>.Response.Body)
/// </summary>
public class DefaultETagInjector : IETagInjector
{
    private readonly IETagGenerator _eTagGenerator;
    
    public DefaultETagInjector(IETagGenerator eTagGenerator)
    {
        _eTagGenerator = eTagGenerator ?? throw new ArgumentNullException(nameof(eTagGenerator));
    }

    public async Task<ETag> RetrieveETag(ETagContext eTagContext)
    {
        // get the response bytes
        if (eTagContext.HttpContext.Response.Body.CanSeek)
        {
            eTagContext.HttpContext.Response.Body.Position = 0;
        }

        var responseBodyContent = await new StreamReader(eTagContext.HttpContext.Response.Body).ReadToEndAsync();

        // Calculate the ETag to store in the store.
        return await _eTagGenerator.GenerateETag(eTagContext.StoreKey, responseBodyContent);
    }
}