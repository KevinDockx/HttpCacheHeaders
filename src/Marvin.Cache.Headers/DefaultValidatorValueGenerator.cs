// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.IO;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Marvin.Cache.Headers
{
    public class DefaultValidatorValueGenerator : IValidatorValueGenerator
    {
        public virtual async Task<ValidatorValue> Generate(StoreKey storeKey, HttpContext httpContext, IETagGenerator eTagGenerator = null)
        {
            ETag eTag = await GetETagFromHttpContext(httpContext);
            DateTimeOffset? lastModified = await GetLastModifiedFromHttpContext(httpContext);

            // If we do not have an etag value at this point, and we can read the response body,
            // initiate the process of generating an etag.
            if ((eTag == null) && (httpContext.Response.Body.CanRead))
            {
                // if no ETag generator has been supplied, use the default etag generator.
                if (eTagGenerator == null)
                {
                    eTagGenerator = new DefaultStrongETagGenerator();
                }

                // get the response bytes
                if (httpContext.Response.Body.CanSeek)
                {
                    httpContext.Response.Body.Position = 0;
                }

                var responseBodyContent = new StreamReader(httpContext.Response.Body).ReadToEnd();

                // Calculate the ETag to store in the store.
                eTag = await eTagGenerator.GenerateETag(storeKey, responseBodyContent);
            }

            if (eTag != null)
            {
                // Ensure we have a valid Last-Modified value.
                if (!lastModified.HasValue)
                {
                    lastModified = GetUtcNowWithoutMilliseconds();
                }
                else
                {
                    lastModified = GetUtcNowWithoutMilliseconds(lastModified.Value);
                }

                return new ValidatorValue(eTag, lastModified.Value);
            }

            // if no etag has been generated, it should never be cached.
            return null;
        }
        
        // By default using the HTTP context to acquire an ETag is not available. This
        // method must always be provided by the consumer of this package.
        protected virtual Task<ETag> GetETagFromHttpContext(HttpContext httpContext, string key = HeaderNames.ETag)
        {
            if ((httpContext != null) && (httpContext.Items.ContainsKey(key)))
            {
                return Task.FromResult(new ETag(httpContext.Items[key] as string));
            }

            return Task.FromResult(default(ETag));
        }

        // By default using the HTTP context to acquire an ETag is not available. This
        // method must always be provided by the consumer of this package.
        protected virtual Task<DateTimeOffset?> GetLastModifiedFromHttpContext(HttpContext httpContext, string key = HeaderNames.LastModified)
        {
            if ((httpContext != null) && (httpContext.Items.ContainsKey(key)))
            {
                var lastModified = httpContext.Items[key];

                if (lastModified is DateTimeOffset)
                {
                    return Task.FromResult((DateTimeOffset?)lastModified);
                }
                else if (lastModified is DateTime)
                {
                    return Task.FromResult((DateTimeOffset?)new DateTimeOffset((DateTime)lastModified));

                }
                else if (lastModified is string)
                {
                    if (DateTimeOffset.TryParse((string)lastModified, out var parsedLastModified))
                    {
                        return Task.FromResult((DateTimeOffset?)parsedLastModified);
                    }
                }
            }

            return Task.FromResult((DateTimeOffset?)null);
        }

        protected static DateTimeOffset GetUtcNowWithoutMilliseconds()
        {
            return GetUtcNowWithoutMilliseconds(DateTimeOffset.UtcNow);
        }

        protected static DateTimeOffset GetUtcNowWithoutMilliseconds(DateTimeOffset dt)
        {
            return new DateTimeOffset(
                dt.Year,
                dt.Month,
                dt.Day,
                dt.Hour,
                dt.Minute,
                dt.Second,
                dt.Offset);
        }
    }
}
