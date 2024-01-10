using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers.Interfaces;

/// <summary>
///     Contract for a ETagInjector, which can be used to inject custom eTags for resources
///     of which may be injected in the request pipeline (eg: based on existing calculated eTag on resource and store key)
/// </summary>
/// <remarks>
///     This injector will wrap the <see cref="IETagGenerator" /> to allow for eTag source to be swapped out
///     based on the <see cref="HttpContext" /> (rather than extend the interface of <see cref="IETagInjector" /> to
///     to extended including the <see cref="HttpContext" />
/// </remarks>
public interface IETagInjector
{
    Task<ETag> RetrieveETag(ETagContext eTagContext);
}