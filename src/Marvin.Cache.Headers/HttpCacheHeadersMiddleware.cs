// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers
{
    public class HttpCacheHeadersMiddleware
    {
        // RequestDelegate is the middleware delegate that should be called
        // after this middleware delegate is finished
        private readonly RequestDelegate _next;
 
        private IEnumerable<string> VaryBy = new List<string>() { "Accept", "Accept-Language" };
        private readonly IValidationValueStore _store;
        private readonly ILogger _logger;
        private readonly ValidationModelOptions _validationModelOptions;
        private readonly ExpirationModelOptions _expirationModelOptions;

        public HttpCacheHeadersMiddleware(
            RequestDelegate next, 
            IValidationValueStore store,
            ILoggerFactory loggerFactory,
            ValidationModelOptions validationModelOptions,
            ExpirationModelOptions expirationModelOptions)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (validationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(validationModelOptions));
            }

            if (expirationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(expirationModelOptions));
            }

            _next = next;
            _store = store;
            _expirationModelOptions = expirationModelOptions;
            _validationModelOptions = validationModelOptions;
            _logger = loggerFactory.CreateLogger<HttpCacheHeadersMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // delegate invoked just before the response headers 
            // are sent to the client
            httpContext.Response.OnStarting(state =>
            {
                var currentHttpContext = (HttpContext)state;

                // Handle expiration: Expires & Cache-Control headers
                // (these are also added for 304 / 412 responses)
                GenerateExpirationHeadersOnResponse(currentHttpContext);

                // Handle validation: ETag and Last-Modified headers
                GenerateValidationHeadersOnResponse(currentHttpContext);

                return Task.FromResult(0);
            }, httpContext);

            // check request ETag headers 

            // GET & If-None-Match: returns 304 when the resource hasn't
            // been modified
            if (await ConditionalGETIsValid(httpContext))
            {
                // still valid. Return 304, and update the Last-Modified date.
                Generate304NotModifiedResponse(httpContext);

                // don't continue with the rest of the flow, we don't want
                // to generate the response.
                return;

            }

            // check for If-Match on PUT/PATCH
            if (!(await PreconditionIsValid(httpContext)))
            {
                // not valid anymore.  Return a 412 response
                Generate412PreconditionFailedResponse(httpContext);

                // don't continue with the rest of the flow, we don't want
                // to generate the response.
                return;
            }

            // We treat dates as weak tags.  There is no backup to IfUnmodifiedSince 
            // for 412 responses. 


            // work with an in-between memory buffer for the response body,
            // otherwise we are unable to read it out (and thus cannot generate strong etags
            // correctly)
            // cfr: http://stackoverflow.com/questions/35458737/implement-http-cache-etag-in-asp-net-core-web-api

            using (var buffer = new MemoryStream())
            {
                // replace the context response with a temporary buffer
                var stream = httpContext.Response.Body;
                httpContext.Response.Body = buffer;

                // Call the next middleware delegate in the pipeline 
                await _next.Invoke(httpContext);

                // reset the buffer and read out the contents
                buffer.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(buffer);
                using (var bufferReader = new StreamReader(buffer))
                {
                    var body = await bufferReader.ReadToEndAsync();

                    // reset to the start of the stream
                    buffer.Seek(0, SeekOrigin.Begin);

                    // Copy the buffer content to the original stream.
                    // This will invoke Response.OnStarting (which can now
                    // read our the body & generate the ETag if necessary)  
                    await buffer.CopyToAsync(stream);

                    // set the response body back to the original stream
                    httpContext.Response.Body = stream;
                }
            }
        }

        private void Generate412PreconditionFailedResponse(HttpContext httpContext)
        {
            var headers = httpContext.Response.Headers;

            httpContext.Response.StatusCode = StatusCodes.Status412PreconditionFailed;

            // set the ETag & Last-Modified date.
            // remove any other ETag and Last-Modified headers (could be set
            // by other pieces of code)
            headers.Remove(HeaderNames.ETag);
            headers.Remove(HeaderNames.LastModified);

            // generate key, ETag and LastModified
            var requestKey = GenerateRequestKey(httpContext.Request);
            // strong eTags by default!
            var eTag = new ETag(ETagType.Strong, httpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString());
            var lastModified = DateTimeOffset.UtcNow;

            // store (overwrite)
            _store.SetAsync(requestKey, new ValidationValue(eTag, lastModified));

            // set headers
            headers[HeaderNames.ETag] = eTag.Value;
            headers[HeaderNames.LastModified] = lastModified.ToString("r", CultureInfo.InvariantCulture);
        }

        private void Generate304NotModifiedResponse(HttpContext httpContext)
        {
            var headers = httpContext.Response.Headers;

            httpContext.Response.StatusCode = StatusCodes.Status304NotModified;

            // set the ETag & Last-Modified date.
            // remove any other ETag and Last-Modified headers (could be set
            // by other pieces of code)
            headers.Remove(HeaderNames.ETag);
            headers.Remove(HeaderNames.LastModified);

            // generate key, ETag and LastModified
            var requestKey = GenerateRequestKey(httpContext.Request);
            // strong eTags by default!
            var eTag = new ETag(ETagType.Strong, httpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString());
            var lastModified = DateTimeOffset.UtcNow;

            // store (overwrite)
            _store.SetAsync(requestKey, new ValidationValue(eTag, lastModified));

            // set headers
            headers[HeaderNames.ETag] = eTag.Value;
            headers[HeaderNames.LastModified] = lastModified.ToString("r", CultureInfo.InvariantCulture);
        }

        private async Task<bool> ConditionalGETIsValid(HttpContext httpContext)
        {
            // if the header value to check is missing, we should
            // always return false - we don't need to check anything, and 
            // can never return a 304 response
            if (!httpContext.Request.Headers.Keys.Contains(HeaderNames.IfNoneMatch))
            {
                return false;
            }

            // generate the request key
            var requestKey = GenerateRequestKey(httpContext.Request);

            // find the validationValue with this key in the store
            var validationValue = await _store.GetAsync(requestKey);

            // if there is no validation value in the store, always 
            // return false - we have nothing to compare to, and can
            // never return a 304 response
            if (validationValue == null || validationValue.ETag == null)
            {
                return false;
            }

            // for GET, this must be on If-None-Match + weak comparison
            if (httpContext.Request.Method == HttpMethods.Get)
            {
                return
                    ETagsMatch(validationValue.ETag,
                    httpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString(),
                    false);
            }

            // for all the other methods, return false (no 304 response)
            return false;
        }

        private async Task<bool> PreconditionIsValid(HttpContext httpContext)
        {
            // if the header value to check is missing, we return true
            // (no 412 response)
            if (!httpContext.Request.Headers.Keys.Contains(HeaderNames.IfMatch))
            {
                return true;
            }

            // generate the request key
            var requestKey = GenerateRequestKey(httpContext.Request);

            // find the validationValue with this key in the store
            var validationValue = await _store.GetAsync(requestKey);

            // if there is no validation value in the store, we return false: 
            // there is nothing to compare to, so the precondition can 
            // never be ok - return a 412 response
            if (validationValue == null || validationValue.ETag == null)
            {
                return false;
            }

            // Preconditional checks are used for concurrency checks only.
            // for PUT or PATCH, this must be on If-Match + strong comparison
            if (httpContext.Request.Method == HttpMethods.Put
                || httpContext.Request.Method == HttpMethods.Patch)
            {
                return
                   ETagsMatch(validationValue.ETag,
                   httpContext.Request.Headers[HeaderNames.IfMatch].ToString(),
                   true);
            }

            // for all the other methods, return true (no 412 response)
            return true;
        }

        private bool ETagsMatch(ETag eTag, string eTagToCompare, bool useStrongComparisonFunction)
        {
            // for If-None-Match (cache) checks, weak comparison should be used.
            // for If-Match (concurrency) check, strong comparison should be used.

            //The example below shows the results for a set of entity-tag pairs and
            //both the weak and strong comparison function results:

            //+--------+--------+-------------------+-----------------+
            //| ETag 1 | ETag 2 | Strong Comparison | Weak Comparison |
            //+--------+--------+-------------------+-----------------+
            //| W/"1"  | W/"1"  | no match          | match           |
            //| W/"1"  | W/"2"  | no match          | no match        |
            //| W/"1"  | "1"    | no match          | match           |
            //| "1"    | "1"    | match             | match           |
            //+--------+--------+-------------------+-----------------+

            if (useStrongComparisonFunction)
            {
                // to match, both eTags must be strong & be an exact match.

                var eTagToCompareIsStrong = !eTagToCompare.StartsWith("W/");

                return (eTagToCompareIsStrong &&
                    eTag.ETagType == ETagType.Strong &&
                    string.Equals(eTag.Value, eTagToCompare, StringComparison.OrdinalIgnoreCase));
            }

            // for weak comparison, we only compare the parts of the eTags after the "W/"
            var firstValueToCompare = (eTag.ETagType == ETagType.Weak) ? eTag.Value.Substring(2) : eTag.Value;
            var secondValueToCompare = eTagToCompare.StartsWith("W/") ? eTagToCompare.Substring(2) : eTagToCompare;

            return string.Equals(firstValueToCompare, secondValueToCompare, StringComparison.OrdinalIgnoreCase);
        }

        private void GenerateValidationHeadersOnResponse(HttpContext httpContext)
        {
            // don't generate these for 304 - that's taken care of at the
            // start of the request
            if (httpContext.Response.StatusCode == StatusCodes.Status304NotModified)
            {
                return;
            }

            // This takes care of storing new tags, also after a succesful PUT/POST/PATCH. 
            // Other PUT/POST/PATCH requests must thus include the new ETag as If-Match, 
            // otherwise the precondition will fail. 
            //
            // If an API returns a 204 No Content after PUT/PATCH, the ETag will be generated
            // from an empty response - any other user/cache must GET the response again
            // before updating it.  Getting it will result in a new ETag value being generated.  
            //
            // If an API returns a 200 Ok after PUT/PATCH, the ETag will be generated from 
            // that response body - if the update was succesful but nothing was changed,
            // in those cases the original ETag for other users/caches will still be sufficient.

            // if the response body cannot be read, we can never
            // generate correct ETags (and it should never be cached)

            if (!httpContext.Response.Body.CanRead)
            {
                return;
            }

            var headers = httpContext.Response.Headers;

            // remove any other ETag and Last-Modified headers (could be set
            // by other pieces of code)
            headers.Remove(HeaderNames.ETag);
            headers.Remove(HeaderNames.LastModified);

            // Save the ETag in a store.  
            // Key = generated from request URI & headers (if VaryBy is 
            // set, only use those headers)
            // ETag itself is generated from the key + response body 
            // (strong ETag)

            // get the request key
            var requestKey = GenerateRequestKey(httpContext.Request);
            var requestKeyAsBytes = Encoding.UTF8.GetBytes(requestKey);

            // get the response bytes
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Position = 0;
            }

            var responseBodyContent = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var responseBodyContentAsBytes = Encoding.UTF8.GetBytes(responseBodyContent);

            // combine both to generate an etag
            var combinedBytes = Combine(requestKeyAsBytes, responseBodyContentAsBytes);

            var eTag = new ETag(ETagType.Strong, GenerateETag(combinedBytes));
            var lastModified = DateTimeOffset.UtcNow;

            // store the ETag & LastModified date with the request key as key in the ETag store
            _store.SetAsync(requestKey, new ValidationValue(eTag, lastModified));

            // set the ETag header
            headers[HeaderNames.ETag] = eTag.Value;
            // set the LastModified header
            headers[HeaderNames.LastModified] = lastModified.ToString("r", CultureInfo.InvariantCulture);
        }

        private void GenerateExpirationHeadersOnResponse(HttpContext httpContext)
        {
            var headers = httpContext.Response.Headers;

            // remove current Expires & Cache-Control headers
            headers.Remove(HeaderNames.Expires);
            headers.Remove(HeaderNames.CacheControl);

            // set expiration header (remove milliseconds)
            headers[HeaderNames.Expires] =
                 DateTimeOffset.UtcNow.AddSeconds(600).ToString("r", CultureInfo.InvariantCulture);

            var cacheControlHeaderValue = string.Format(
                   CultureInfo.InvariantCulture,
                   "{0}{1}max-age={2}{3}s-maxage={4}",
                   "public",
                   // cacheControlHeaderValue != null ? "," : null,
                   ",",
                   600,
                   ",",
                   600);

            headers[HeaderNames.CacheControl] = cacheControlHeaderValue;
        }

        private string GenerateRequestKey(HttpRequest request)
        {
            // generate a key to store the entity tag with in the entity tag store

            // get the request headers to take into account (VaryBy) & take 
            // their values
            var requestHeaderValues = request.Headers.Where(x =>
               VaryBy.Any(h => h.Equals(x.Key, StringComparison.CurrentCultureIgnoreCase)))
               .SelectMany(h => h.Value);

            // get the resoure path
            var resourcePath = request.Path.ToString();

            // combine these two
            return string.Format("{0}-{1}", resourcePath, string.Join("-", requestHeaderValues));
        }

        // from http://jakzaprogramowac.pl/pytanie/20645,implement-http-cache-etag-in-aspnet-core-web-api
        private string GenerateETag(byte[] data)
        {
            string ret = string.Empty;

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                string hex = BitConverter.ToString(hash);
                ret = hex.Replace("-", "");
            }
            return ret;
        }

        byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }        
    }
}
