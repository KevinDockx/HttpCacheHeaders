using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers.Domain;

/// <summary>
/// Context containing information on a specific resource
/// </summary>
public sealed class ResourceContext
{
    /// <summary>
    /// The current <see cref="HttpRequest"/>
    /// </summary>
    public HttpRequest HttpRequest { get; }

    /// <summary>
    /// The current <see cref="StoreKey"/> for the resource, if available
    /// </summary>
    public StoreKey StoreKey { get; }

    /// <summary>
    /// The current <see cref="ValidatorValue"/> for the resource, if available
    /// </summary>
    public ValidatorValue ValidatorValue { get; }


    public ResourceContext(
        HttpRequest httpRequest,
        StoreKey storeKey)
    {
        HttpRequest = httpRequest;
        StoreKey = storeKey;
    }

    public ResourceContext(
        HttpRequest httpRequest,
        StoreKey storeKey,
        ValidatorValue validatorValue) : this(httpRequest, storeKey)
    {
        ValidatorValue = validatorValue;
    }
}
