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

## IValidatorValueStore

A validator value store stores validator values.  A validator value is used by the cache validation model when checking if a cached item is still valid.  It contains ETag and LastModified properties.  The default IValidatorValueStore implementation (InMemoryValidatorValueStore) is an in-memory store that stores items in a ConcurrentDictionary<string, ValidatorValue>. 

```
/// <summary>
/// Contract for a store for validator values.  Each item is stored with a <see cref="StoreKey" /> as key```
/// and a <see cref="ValidatorValue" /> as value (consisting of an ETag and Last-Modified date).   
/// </summary>
public interface IValidatorValueStore
{
    /// <summary>
    /// Get a value from the store.
    /// </summary>
    /// <param name="key">The <see cref="StoreKey"/> of the value to get.</param>
    /// <returns></returns>
    Task<ValidatorValue> GetAsync(StoreKey key);

    /// <summary>
    /// Set a value in the store.
    /// </summary>
    /// <param name="key">The <see cref="StoreKey"/> of the value to store.</param>
    /// <param name="validatorValue">The <see cref="ValidatorValue"/> to store.</param>
    /// <returns></returns>
    Task SetAsync(StoreKey key, ValidatorValue validatorValue);
}
```

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

You can inject an IETagGenerator-implementing class to modify how ETags are generated (ETags are part of a ValidatorValue). The default implementation (DefaultStrongETagGenerator) generates strong Etags from the request key + response body (MD5 hjsh from combined bytes). 

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

## IValidatorValueGenerator

You can inject an IValidatorValueGenerator-implementing class to modify how the header values 
ETag and Last-Modified are generated. This interface is a superset of the IETagGenerator and 
maintains backword compatibility with previous versions of this package (i.e., IETagGenerator
will always generate the ETag if the IValidatorValueGenerator interface was unable to generate
and ETag).

```
/// <summary>
/// Contract for an Validator Value Generator, used to generate the unique weak or strong E-Tags for cache items and Last Modified Time.
/// </summary>
public interface IValidatorValueGenerator
{
    Task<ValidatorValue> Generate(
        StoreKey storeKey,
        HttpContext httpContext,
        IETagGenerator eTagGenerator = null);
}
```

The default implementation, implemented by the class *DefaultValidatorValueGenerator*, first examines the
*Items* dictionary of the HttpContext to see if it contains values for the keys `ETag` and `Last-Modified` 
(e.g., `httpContext.Items["ETag"]`). If ETag value is present, that value shall be used when creating the 
ValidatorValue. If the ETag value is not acquired, the ETag shall be generated via the defined IETagGenerator.

If the LastModified value is present, that value shall be used when creating the ValidatorValue; otherwise, the
current time is applied.

Below is an example of controller methods for reading a ToDo item and lists from repository using CosmosDB 
as its underlying data storage system. First the method for reading a single ToDo item.

```
/// <summary>
/// Get the ToDo item with the given id.
/// </summary>
[HttpHead("{id}", Name = "HeadToDo")]
[HttpGet("{id}", Name = "GetToDo")]
[ProducesResponseType(typeof(ToDoDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status304NotModified)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetailsExtendedDto), StatusCodes.Status500InternalServerError)]
[ToDoResultFilter()]
public async Task<IActionResult> Get(string id)
{
    var result = await _respository.GetToDoAsync(id);

    if (result == null)
        return NotFound();

    // Place etag and unix time (converted to DateTimeOffset) in httpContext Items dictionary.
    HttpContext.AddETag(result.ETag);
    HttpContext.AddLastModified(result.Resource._ts);

    return Ok(result.Resource);
}
```

The *DefaultValidatorValueGenerator* will discover the ETag and LastModified values in the context's Items 
dictionary and use these values for the ValidatorValue. This provides several benefits. The client of the API
now has the true LastModified value of the resource for which better update decisions can be made. The ETag 
provided can be used by our repository for concurrency checking. 


Below is the controller method for getting a list of ToDos.

```
/// <summary>
/// Get all available ToDos.
/// </summary>
/// <returns>
/// </returns>
[HttpHead(Name = "HeadToDos")]
[HttpGet(Name = "GetToDos")]
[ProducesResponseType(typeof(IEnumerable<ToDoDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status304NotModified)]
[ProducesResponseType(typeof(ProblemDetailsExtendedDto), StatusCodes.Status500InternalServerError)]
[ToDosResultFilter()]
public async Task<IActionResult> Get()
{
    var result = await _respository.GetToDosAsync();

    // Get the maximum timestamp to provide the last time any entry in this list was modified.
    if (result.Count() > 0)
    {
        var maxLastModified = result.Max(todo => todo._ts);
        HttpContext.AddLastModified(maxLastModified);
    }

    return Ok(result);
}
```
In the case of the list, the ETag will be generated by this package, but the LastModified date will be 
the latest date an item in the list was modified. The *DefaultValidatorValueGenerator* will discover 
LastModified values in the context's Items dictionary, and since to ETag was not discovered, the 
IETagGenerator generates an ETag.


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