# Http Cache Headers Middleware for ASP.NET Core
ASP.NET Core middleware that adds HttpCache headers to responses (Cache-Control, Expires, ETag, Last-Modified), and implements cache expiration &amp; validation models.  It can be used to ensure caches correctly cache responses and/or to implement concurrency for REST-based APIs using ETags.

The middleware itself does not store responses.  Looking at [this description]( http://2ndscale.com/rtomayko/2008/things-caches-do "Things Caches Do"), this middleware handles the "backend"-part: it generates the correct cache-related headers, and ensures a cache can check for expiration (304 Not Modified) & preconditions (412 Precondition Failed) (often used for concurrency checks).

It can be used together with a shared cache (eg: Microsoft.AspNetCore.ResponseCaching - to be injected in the request pipeline before this component), a private cache or both.  In the sample, the Microsoft.AspNetCore.ResponseCaching cache store is used to effectively cache the responses.  

[![NuGet version](https://badge.fury.io/nu/marvin.cache.headers.svg)](https://badge.fury.io/nu/marvin.cache.headers)
 
# Installation (NuGet)
```
Install-Package Marvin.Cache.Headers
```

# Usage 

First, register the services with ASP.NET Core's dependency injection container (in the ConfigureServices method on the Startup class)

```
services.AddHttpCacheHeaders();
```

Then, add the middleware to the request pipeline.  Add this before the MVC middleware, as the HttpCacheHeaders middleware will sometimes avoid continuing with the MVC delegate (to avoid unnecessarily generating response bodies).

```
app.UseHttpCacheHeaders();

app.UseMvc(); 
```

# Configuring options

The middleware allows customization of how headers are generated.  The AddHttpCacheHeaders() method has overloads for configuring options related to expiration, validation or both.  

For example, this code will set the max-age directive to 600 seconds, and will add the must-revalidate directive.

```
services.AddHttpCacheHeaders(
    (expirationModelOptions) =>
    {
        expirationModelOptions.MaxAge = 600;
    },
    (validationModelOptions) =>
    {
        validationModelOptions.MustRevalidate = true;
    });
```

# Action (Resource) and Controller-level Header Configuration

For anything but the simplest of cases having one global cache policy isn't sufficient: configuration at level of each resource (action/controller) is required.  For those cases, use the HttpCacheExpiration and/or HttpCacheValidation attributes at action or controller level.  

```
[HttpGet]
[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 99999)]
[HttpCacheValidation(MustRevalidate = true)]
public IEnumerable<string> Get()
{
    return new[] { "value1", "value2" };
}
```
Both override the global options.  Controller-level configuration overrides action-level configuration.

# Extensibility

The middleware is very extensible. If you have a look at the AddHttpCacheHeaders method you'll notice it allows injecting custom implementations of 
IValidatorValueStore, IStoreKeyGenerator, IETagGenerator and/or IDateParser (via actions). 


## IStoreKeyGenerator
The StoreKey, as used by the IValidatorValueStore as key, can be customized as well.  To do so, implement the IStoreKeyGenerator interface.  The default implementation (DefaultStoreKeyGenerator) generates a key from the request path, request query string and request header values (taking VaryBy into account). Through StoreKeyContext you can access all applicable values that can be useful for generating such a key. 

```
/// <summary>
/// Contract for a key generator, used to generate a <see cref="StoreKey" /> ```
/// </summary>
public interface IStoreKeyGenerator
{
    /// <summary>
    /// Generate a key for storing a <see cref="ValidatorValue"/> in a <see cref="IValidatorValueStore"/>.
    /// </summary>
    /// <param name="context">The <see cref="StoreKeyContext"/>.</param>         
    /// <returns></returns>
    Task<StoreKey> GenerateStoreKey(
        StoreKeyContext context);
}
```

## IETagGenerator

You can inject an IETagGenerator-implementing class to modify how ETags are generated (ETags are part of a ValidatorValue). The default implementation (DefaultStrongETagGenerator) generates strong Etags from the request key + response body (MD5 hash from combined bytes). 

```
/// <summary>
/// Contract for an E-Tag Generator, used to generate the unique weak or strong E-Tags for cache items
/// </summary>
public interface IETagGenerator
{
    Task<ETag> GenerateETag(
        StoreKey storeKey,
        string responseBodyContent);
}
```

## ILastModifiedInjector

You can inject an ILastModifiedInjector-implementing class to modify how LastModified values are provided. The default implementation (DefaultLastModifiedInjector) injects the current UTC. 

```
/// <summary>
/// Contract for a LastModifiedInjector, which can be used to inject custom last modified dates for resources
/// of which you know when they were last modified (eg: a DB timestamp, custom logic, ...)
/// </summary>
public interface ILastModifiedInjector
{
    Task<DateTimeOffset> CalculateLastModified(
        ResourceContext context);
}
```

## IDateParser 

Through IDateParser you can inject a custom date parser in case you want to override the default way dates are stringified.  The default implementation (DefaultDateParser) uses the RFC1123 pattern (https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx). 

```
/// <summary>
/// Contract for a date parser, used to parse Last-Modified, Expires, If-Modified-Since and If-Unmodified-Since headers.
/// </summary>
public interface IDateParser
{
    Task<string> LastModifiedToString(DateTimeOffset lastModified);

    Task<string> ExpiresToString(DateTimeOffset lastModified);

    Task<DateTimeOffset?> IfModifiedSinceToDateTimeOffset(string ifModifiedSince);

    Task<DateTimeOffset?> IfUnmodifiedSinceToDateTimeOffset(string ifUnmodifiedSince);
}
```