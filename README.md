# HttpCacheHeaders
ASP.NET Core middleware that adds HttpCache headers to responses (Cache-Control, Expires, ETag, Last-Modified), and implements cache expiration &amp; validation models.  

Note that this is not a cache store: the middleware does not store responses.  Looking at [link]( http://2ndscale.com/rtomayko/2008/things-caches-do "this description"), this middleware handles the "backend"-part: it generates the correct cache-related headers, and ensures a cache can check for expiration (304 Not Modified) & preconditions (412 Precondition Failed) (often used for concurrency checks).

It can be used together with a shared cache (eg: Microsoft.AspNetCore.ResponseCaching - to be injected in the request pipeline before this component), a private cache or both.

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
        validationModelOptions.AddMustRevalidate = true;
    });
```
