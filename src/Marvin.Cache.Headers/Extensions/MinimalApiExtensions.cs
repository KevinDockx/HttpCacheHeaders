using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Marvin.Cache.Headers.Extensions;

public static class MinimalApiExtensions
{
  /// <summary>
  /// Adds the HttpCacheExpiration attribute to the minimal API endpoint.
  /// </summary>
  /// <typeparam name="TBuilder">The type of builder (e.g. group or individual)</typeparam>
  /// <param name="builder">The builder to use to add the attribute.</param>
  /// <param name="cacheLocation">The location where a response can be cached.</param>
  /// <param name="maxAge">Maximum age, in seconds, after which a response expires.</param>
  /// <param name="noStore">When true, the no-store directive is included in the Cache-Control header.</param>
  /// <param name="noTransform">When true, the no-transform directive is included in the Cache-Control header.</param>
  /// <param name="sharedMaxAge">Maximum age, in seconds, after which a response expires for shared caches.</param>
  /// <seealso cref="Marvin.Cache.Headers.HttpCacheExpirationAttribute"/>
  /// <returns>The builder for chaining of commands.</returns>
  public static TBuilder AddHttpCacheExpiration<TBuilder>(this TBuilder builder,
      CacheLocation? cacheLocation = null,
      int? maxAge = null,
      bool? noStore = null,
      bool? noTransform = null,
      int? sharedMaxAge = null) 
    where TBuilder : IEndpointConventionBuilder
  {
    ArgumentNullException.ThrowIfNull(builder);

    var attribute = new HttpCacheExpirationAttribute();

    attribute.CacheLocation = cacheLocation ?? attribute.CacheLocation;
    attribute.MaxAge = maxAge ?? attribute.MaxAge;
    attribute.NoStore = noStore ?? attribute.NoStore;
    attribute.NoTransform = noTransform ?? attribute.NoTransform;
    attribute.SharedMaxAge = sharedMaxAge ?? attribute.SharedMaxAge;

    builder.Add(endpointBuilder =>
    {
      endpointBuilder.Metadata.Add(attribute);
    });
    return builder;
  }

  /// <summary>
  /// Adds the HttpCacheValidation attribute to the minimal API endpoint.
  /// </summary>
  /// <typeparam name="TBuilder">The type of builder (e.g. group or individual)</typeparam>
  /// <param name="builder">The builder to use to add the attribute.</param>
  /// <param name="vary">A case-insensitive list of headers from the request to take into account as differentiator
  /// between requests.</param>
  /// <param name="varyByAll">Indicates that all request headers are taken into account as differentiator.</param>
  /// <param name="noCache">When true, the no-cache directive is added to the Cache-Control header.</param>
  /// <param name="mustRevalidate">When true, the must-revalidate directive is added to the Cache-Control header.</param>
  /// <param name="proxyRevalidate">When true, the proxy-revalidate directive is added to the Cache-Control header.</param>
  /// <seealso cref="Marvin.Cache.Headers.HttpCacheValidationAttribute"/>
  /// <returns>The builder for chaining of commands.</returns>
  public static TBuilder AddHttpCacheValidation<TBuilder>(this TBuilder builder,
    string[] vary = null, 
    bool? varyByAll = null, 
    bool? noCache = null,
    bool? mustRevalidate = null,
    bool? proxyRevalidate = null)
  where TBuilder : IEndpointConventionBuilder
  {
    ArgumentNullException.ThrowIfNull(builder);

    var attribute = new HttpCacheValidationAttribute();

    attribute.Vary = vary ?? attribute.Vary;
    attribute.VaryByAll = varyByAll ?? attribute.VaryByAll;
    attribute.NoCache = noCache ?? attribute.NoCache;
    attribute.MustRevalidate = mustRevalidate ?? attribute.MustRevalidate;
    attribute.ProxyRevalidate = proxyRevalidate ?? attribute.ProxyRevalidate;

    builder.Add(endpointBuilder =>
    {
      endpointBuilder.Metadata.Add(attribute);
    });
    return builder;
  }

  /// <summary>
  /// Adds the HttpCacheIgnore attribute to the endpoints
  /// </summary>
  /// <typeparam name="TBuilder">The type of builder (e.g. group or individual)</typeparam>
  /// <param name="builder">The builder to use to add the attribute.</param>
  /// <returns>The builder for chaining of commands.</returns>
  public static TBuilder IgnoreHttpCache<TBuilder>(this TBuilder builder)
    where TBuilder : IEndpointConventionBuilder
  {
    ArgumentNullException.ThrowIfNull(builder);

    builder.Add(endpointBuilder =>
    {
      endpointBuilder.Metadata.Add(new HttpCacheIgnoreAttribute());
    });

    return builder;
  }

}
