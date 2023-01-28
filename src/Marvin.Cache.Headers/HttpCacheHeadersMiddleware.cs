// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Marvin.Cache.Headers.Extensions;
using Marvin.Cache.Headers.Domain;

namespace Marvin.Cache.Headers
{
    public class HttpCacheHeadersMiddleware
    {
        // RequestDelegate is the middleware delegate that should be called
        // after this middleware delegate is finished
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        private readonly IDateParser _dateParser;
        private readonly IValidatorValueStore _store;
        private readonly IStoreKeyGenerator _storeKeyGenerator;
        private readonly IETagGenerator _eTagGenerator;
        private readonly ILastModifiedInjector _lastModifiedInjector;
        private readonly MiddlewareOptions _middlewareOptions;
        private readonly ValidationModelOptions _validationModelOptions;
        private readonly ExpirationModelOptions _expirationModelOptions;

        private IValidatorValueInvalidator _validatorValueInvalidator;

        public HttpCacheHeadersMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IDateParser dateParser,
            IValidatorValueStore store,
            IStoreKeyGenerator storeKeyGenerator,
            IETagGenerator eTagGenerator,
            ILastModifiedInjector lastModifiedInjector,
            IOptions<MiddlewareOptions> middlewareOptions,
            IOptions<ExpirationModelOptions> expirationModelOptions, 
            IOptions<ValidationModelOptions> validationModelOptions)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (middlewareOptions ==null)
            {
                throw new ArgumentNullException(nameof(middlewareOptions));
            }

            if (validationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(validationModelOptions));
            }

            if (expirationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(expirationModelOptions));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));

            _dateParser = dateParser ?? throw new ArgumentNullException(nameof(dateParser));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _storeKeyGenerator = storeKeyGenerator ?? throw new ArgumentNullException(nameof(storeKeyGenerator));
            _eTagGenerator = eTagGenerator ?? throw new ArgumentNullException(nameof(eTagGenerator));
            _lastModifiedInjector = lastModifiedInjector ?? throw new ArgumentNullException(nameof(lastModifiedInjector));
            _middlewareOptions = middlewareOptions.Value;
            _expirationModelOptions = expirationModelOptions.Value;
            _validationModelOptions = validationModelOptions.Value;

            _logger = loggerFactory.CreateLogger<HttpCacheHeadersMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext, IValidatorValueInvalidator validatorValueInvalidator)
        {
            // Use method injection for IValidatorValueInvalidator, as it's a scoped service.  Only singleton
            // services can be injected via constructor injection when working with middleware

            _validatorValueInvalidator = validatorValueInvalidator
                ?? throw new ArgumentNullException(nameof(validatorValueInvalidator));

            if (ShouldCachingBeIgnored(httpContext))
            {                
                // continue with the next delegate in line
                await _next.Invoke(httpContext);
                return;
            }

            // check request ETag headers & dates
            if (await GetOrHeadIndicatesResourceStillValid(httpContext))
            {
                // don't continue with the next delegate in line, a response (304) has been generated
                return;
            }

            if (await PutOrPostIndicatesResourceHasChanged(httpContext))
            {                
                // don't continue with the next delegate in line, a response (412) has been generated
                return;
            }

            await HandleResponse(httpContext);
        }

        private bool ShouldCachingBeIgnored(HttpContext httpContext)
        {

            var hasHttpCacheIgnoreAttribute = httpContext.GetEndpoint()?.Metadata
                .Any(m => m is HttpCacheIgnoreAttribute);

            if (hasHttpCacheIgnoreAttribute.HasValue && hasHttpCacheIgnoreAttribute.Value)
            {
                return true;
            }

            if (_middlewareOptions.IgnoreCaching)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> GetOrHeadIndicatesResourceStillValid(HttpContext httpContext)
        {
            // GET & If-None-Match / IfModifiedSince:
            // returns 304 when the resource hasn't been modified
            if (await ConditionalGetOrHeadIsValid(httpContext))
            {
                // still valid. Return 304
                await Generate304NotModifiedResponse(httpContext);

                // don't continue with the rest of the flow, we don't want
                // to generate the response.
                return true;
            }

            _logger.LogInformation("Don't generate 304 - Not Modified.  Continue.");
            return false;
        }

        private async Task<bool> PutOrPostIndicatesResourceHasChanged(HttpContext httpContext)
        {
            // Check for If-Match / IfUnModifiedSince on PUT/PATCH.  Even though
            // dates aren't guaranteed to be strong validators, the standard allows
            // using these.  It's up to the server to ensure they are strong
            // if they want to allow using them.
            if (!await ConditionalPutOrPatchIsValid(httpContext))
            {
                // not valid anymore.  Return a 412 response
                await Generate412PreconditionFailedResponse(httpContext);

                // don't continue with the rest of the flow, we don't want
                // to generate the response.
                return true;
            }

            _logger.LogInformation("Don't generate 412 - Precondition Failed.  Continue.");
            return false;
        }

        private async Task HandleResponse(HttpContext httpContext)
        {
            // We treat dates as weak tags.  There is no backup to IfUnmodifiedSince
            // for 412 responses.

            // work with an in-between memory buffer for the response body,
            // otherwise we are unable to read it out (and thus cannot generate strong etags
            // correctly)
            // cfr: http://stackoverflow.com/questions/35458737/implement-http-cache-etag-in-asp-net-core-web-api

            // ensure additional middleware can write to the response (cfr fix by MarinkoSpasojevic)
            var buffer = new MemoryStream();
            Stream stream = null;
            bool isContinuation = false;
            try
            {
                // replace the context response with a temporary buffer
                stream = httpContext.Response.Body;
                httpContext.Response.Body = buffer;

                // Call the next middleware delegate in the pipeline
                await _next.Invoke(httpContext);

                isContinuation = true;

                if (!ShouldResponseBeIgnored(httpContext))
                {
                    // Grab possible cache overrides from the method
                    var expirationModelOptions = httpContext.ExpirationModelOptionsOrDefault(_expirationModelOptions);
                    var validationModelOptions = httpContext.ValidationModelOptionsOrDefault(_validationModelOptions);

                    // Handle the response (expiration, validation, vary headers)

                    // Handle expiration: Expires & Cache-Control headers
                    // (these are also added for 304 / 412 responses)
                    await GenerateExpirationHeadersOnResponse(httpContext, expirationModelOptions, validationModelOptions);

                    // Handle validation: ETag and Last-Modified headers
                    await GenerateValidationHeadersOnResponse(httpContext);

                    // Generate Vary headers on the response
                    GenerateVaryHeadersOnResponse(httpContext, validationModelOptions);
                }
                
                // reset the buffer, read out the contents & copy it to the original stream.  This
                // will ensure our changes to the buffer are applied to the original stream.
                buffer.Seek(0, SeekOrigin.Begin);
                using (var bufferReader = new StreamReader(buffer))
                {
                    await bufferReader.ReadToEndAsync();

                    // reset to the start of the stream
                    buffer.Seek(0, SeekOrigin.Begin);

                    // Copy the buffer content to the original stream.
                    // This invokes Response.OnStarting (not used)
                    // Do not write to the response body if there is no data (204 No Content, etc.)
                    if (buffer.Length > 0)
                    {
                        await buffer.CopyToAsync(stream);
                    }

                    // set the response body back to the original stream
                    httpContext.Response.Body = stream;
                }
            }
            finally
            {
                if (!isContinuation)
                {
                    httpContext.Response.Body = stream;
                }
                if (buffer != null)
                {
                    buffer.Dispose();
                }
            }

            // Remove the items marked for invalidation.  It's important
            // to do this here, AFTER the response has been generated/set, as to
            // not interfere with that process for those cases where a user 
            // wants to immediately remove the current response ValidatorValue
            if (_validatorValueInvalidator.KeysMarkedForInvalidation.Any())
            {
                foreach (var key in _validatorValueInvalidator.KeysMarkedForInvalidation)
                {
                    await _store.RemoveAsync(key);
                }
            }
        }

        private bool ShouldResponseBeIgnored(HttpContext httpContext)
        {
            var isResponseStatusCodeIgnored = _middlewareOptions.IgnoredStatusCodes.Contains(httpContext.Response.StatusCode);

            if (isResponseStatusCodeIgnored)
                return true;

            return false;
        }

        private async Task<bool> ConditionalGetOrHeadIsValid(HttpContext httpContext)
        {
            _logger.LogInformation("Checking for conditional GET/HEAD.");

            if (httpContext.Request.Method != HttpMethod.Get.ToString() &&
                httpContext.Request.Method != HttpMethod.Head.ToString())
            {
                _logger.LogInformation("Not valid - method isn't GET or HEAD.");
                return false;
            }

            // we should check ALL If-None-Match values (can be multiple eTags) (if available),
            // and the If-Modified-Since date (if available AND an eTag matches).  See issue #2 @Github.
            // So, this is a valid conditional GET/HEAD (304) if one of the ETags match and, if it's
            // available, the If-Modified-Since date is larger than what's saved.

            // if both headers are missing, we should
            // always return false - we don't need to check anything, and
            // can never return a 304 response
            if (!httpContext.Request.Headers.Keys.Contains(HeaderNames.IfNoneMatch) &&
                !httpContext.Request.Headers.Keys.Contains(HeaderNames.IfModifiedSince))
            {
                _logger.LogInformation("Not valid - no If-None-Match or If-Modified-Since headers.");
                return false;
            }

            // generate the request key
            var storeKey = await _storeKeyGenerator.GenerateStoreKey(
                ConstructStoreKeyContext(httpContext.Request, _validationModelOptions));

            // find the validatorValue with this key in the store
            var validatorValue = await _store.GetAsync(storeKey);

            // if there is no validation value in the store, always
            // return false - we have nothing to compare to, and can
            // never return a 304 response
            if (validatorValue?.ETag == null)
            {
                _logger.LogInformation("No saved response found in store.");
                return false;
            }

            if (httpContext.Request.Headers.Keys.Contains(HeaderNames.IfNoneMatch))
            {
                // check the ETags. If the Etag is present we should not check the If-Modified-Since header
                // see issue https://github.com/KevinDockx/HttpCacheHeaders/issues/82

                // cfr: "If none of the entity tags match, then the server MAY perform the requested method as if the
                // If-None-Match header field did not exist, but MUST also ignore any If-Modified-Since header field(s)
                // in the request. That is, if no entity tags match, then the server MUST NOT return a 304(Not Modified) response."

                return CheckIfNoneMatchIsValid(httpContext, validatorValue);
            }

            _logger.LogInformation("No If-None-Match header, don't check ETag.");
            return await CheckIfModifiedSinceIsValid(httpContext, validatorValue);
        }

        private bool CheckIfNoneMatchIsValid(HttpContext httpContext, ValidatorValue validatorValue)
        {
            _logger.LogInformation("Checking If-None-Match.");
            var ifNoneMatchHeaderValue = httpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString().Trim();
            _logger.LogInformation($"Checking If-None-Match: {ifNoneMatchHeaderValue}.");

            // if the value is *, the check is valid.
            if (ifNoneMatchHeaderValue == "*")
            {
                return true;
            }

            var eTagsFromIfNoneMatchHeader = ifNoneMatchHeaderValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // check the ETag.  If one of the ETags matches, we're good to
            // go and can return a 304 Not Modified.
            // For conditional GET/HEAD, we use weak comparison.
            if (eTagsFromIfNoneMatchHeader.Any(eTag => ETagsMatch(validatorValue.ETag, eTag.Trim(), false)))
            {
                _logger.LogInformation($"ETag valid: {validatorValue.ETag}.");
                return true;
            }

            _logger.LogInformation("Not valid. No match found for ETag.");
            return false;
        }

        private async Task<bool> CheckIfModifiedSinceIsValid(HttpContext httpContext, ValidatorValue validatorValue)
        {
            if (httpContext.Request.Headers.Keys.Contains(HeaderNames.IfModifiedSince))
            {
                // if the LastModified date is smaller than the IfModifiedSince date,
                // we can return a 304 Not Modified (If there's also a matching ETag).
                // By adding an If-Modified-Since date
                // to a GET/HEAD request, the consumer is stating that (s)he only wants the resource
                // to be returned if has been modified after that.
                var ifModifiedSinceValue = httpContext.Request.Headers[HeaderNames.IfModifiedSince].ToString();
                _logger.LogInformation($"Checking If-Modified-Since: {ifModifiedSinceValue}");

                var parsedIfModifiedSince = await _dateParser.IfModifiedSinceToDateTimeOffset(ifModifiedSinceValue);

                if (parsedIfModifiedSince.HasValue)
                {
                    return validatorValue.LastModified.CompareTo(parsedIfModifiedSince.Value) <= 0;
                }

                // can only check if we can parse it. Invalid values must be ignored.
                _logger.LogInformation("Cannot parse If-Modified-Since value as date, header is ignored.");
                return true;
            }

            // if there is no IfModifiedSince header, the check is valid.
            _logger.LogInformation("No If-Modified-Since header.");
            return true;
        }

        private async Task<bool> ConditionalPutOrPatchIsValid(HttpContext httpContext)
        {
            _logger.LogInformation("Checking for conditional PUT/PATCH.");

            // Preconditional checks are used for concurrency checks only,
            // on updates: PUT or PATCH
            if (httpContext.Request.Method != HttpMethod.Put.ToString() &&
                httpContext.Request.Method != "PATCH")
            {
                _logger.LogInformation("Not valid - method isn't PUT or PATCH.");
                // for all the other methods, return true (no 412 response)
                return true;
            }

            // the precondition is valid if one of the ETags submitted through
            // IfMatch matches with the saved ETag, AND if the If-UnModified-Since
            // value is smaller than the saved date.  Both must be valid if both
            // are submitted.

            // If both headers are missing, we should
            // always return true (the precondition is missing, so it's valid)
            // We don't need to check anything, and can never return a 412 response
            if (!httpContext.Request.Headers.Keys.Contains(HeaderNames.IfMatch) &&
                !httpContext.Request.Headers.Keys.Contains(HeaderNames.IfUnmodifiedSince))
            {
                _logger.LogInformation("Not valid - no If Match or If Unmodified-Since headers.");
                return true;
            }

            // generate the request key
            var storeKey = await _storeKeyGenerator.GenerateStoreKey(
                ConstructStoreKeyContext(httpContext.Request, _validationModelOptions));

            // find the validatorValue with this key in the store
            var validatorValue = await _store.GetAsync(storeKey);

            // if there is no validation value in the store, we return false:
            // there is nothing to compare to, so the precondition can
            // never be OK - return a 412 response
            if (validatorValue?.ETag == null)
            {
                _logger.LogInformation("No saved response found in store.");
                return false;
            }

            // check the ETags
            // return the combined result of all validators.
            return CheckIfMatchIsValid(httpContext, validatorValue) &&
                   await CheckIfUnmodifiedSinceIsValid(httpContext, validatorValue);
        }

        private bool CheckIfMatchIsValid(HttpContext httpContext, ValidatorValue validatorValue)
        {
            if (!httpContext.Request.Headers.Keys.Contains(HeaderNames.IfMatch))
            {
                // if there is no IfMatch header, the tag precondition is valid.
                _logger.LogInformation("No If-Match header, don't check ETag.");
                return true;
            }

            _logger.LogInformation("Checking If-Match.");
            var ifMatchHeaderValue = httpContext.Request.Headers[HeaderNames.IfMatch].ToString().Trim();
            _logger.LogInformation($"Checking If-Match: {ifMatchHeaderValue}.");

            // if the value is *, the check is valid.
            if (ifMatchHeaderValue == "*")
            {
                return true;
            }

            // otherwise, check the actual ETag(s)
            var eTagsFromIfMatchHeader = ifMatchHeaderValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // check the ETag.  If one of the ETags matches, the
            // ETag precondition is valid.

            // for concurrency checks, we use the strong
            // comparison function.
            if (eTagsFromIfMatchHeader.Any(eTag => ETagsMatch(validatorValue.ETag, eTag.Trim(), true)))
            {
                _logger.LogInformation($"ETag valid: {validatorValue.ETag}.");
                return true;
            }

            // if there is an IfMatch header but none of the ETags match,
            // the precondition is already invalid.  We don't have to
            // continue checking.

            _logger.LogInformation("Not valid. No match found for ETag.");
            return false;
        }

        private async Task<bool> CheckIfUnmodifiedSinceIsValid(HttpContext httpContext, ValidatorValue validatorValue)
        {
            // Either the ETag matches (or one of them), or there was no IfMatch header.
            // Continue with checking the IfUnModifiedSince header, if it exists.
            if (httpContext.Request.Headers.Keys.Contains(HeaderNames.IfUnmodifiedSince))
            {
                // if the LastModified date is smaller than the IfUnmodifiedSince date,
                // the precondition is valid.
                var ifUnmodifiedSinceValue = httpContext.Request.Headers[HeaderNames.IfUnmodifiedSince].ToString();
                _logger.LogInformation($"Checking If-Unmodified-Since: {ifUnmodifiedSinceValue}");

                var parsedIfUnmodifiedSince = await _dateParser.IfUnmodifiedSinceToDateTimeOffset(ifUnmodifiedSinceValue);

                if (parsedIfUnmodifiedSince.HasValue)
                {
                    // If the LastModified date is smaller than the IfUnmodifiedSince date,
                    // the precondition is valid.
                    return validatorValue.LastModified.CompareTo(parsedIfUnmodifiedSince.Value) < 0;
                }

                // can only check if we can parse it. Invalid values must be ignored.
                _logger.LogInformation("Cannot parse If-Unmodified-Since value as date, header is ignored.");
                return true;
            }

            // if there is no IfUnmodifiedSince header, the check is valid.
            _logger.LogInformation("No If-Unmodified-Since header.");
            return true;
        }

        private async Task Generate304NotModifiedResponse(HttpContext httpContext)
        {
            _logger.LogInformation("Generating 304 - Not Modified.");
            httpContext.Response.StatusCode = StatusCodes.Status304NotModified;
            await GenerateResponseFromStore(httpContext, false);
        }

        private async Task Generate412PreconditionFailedResponse(HttpContext httpContext)
        {
            _logger.LogInformation("Generating 412 - Precondition Failed.");
            httpContext.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
            await GenerateResponseFromStore(httpContext);
        }

        private async Task GenerateResponseFromStore(HttpContext httpContext, bool updateLastModified = true)
        {
            var logInformation = string.Empty;

            var headers = httpContext.Response.Headers;

            // set the ETag & Last-Modified date.
            // remove any other ETag and Last-Modified headers (could be set
            // by other pieces of code)
            headers.Remove(HeaderNames.ETag);
            headers.Remove(HeaderNames.LastModified);

            // generate key
            var storeKey = await _storeKeyGenerator.GenerateStoreKey(
                ConstructStoreKeyContext(httpContext.Request, _validationModelOptions));

            // take ETag value from the store (if it's found)
            var savedResponse = await _store.GetAsync(storeKey);
            if (savedResponse?.ETag != null)
            {
                var eTag = new ETag(savedResponse.ETag.ETagType, savedResponse.ETag.Value);
                headers[HeaderNames.ETag] = savedResponse.ETag.ToString();
                logInformation = $"ETag: {eTag.ETagType}, {eTag}, ";
            }

            DateTimeOffset lastModified;
            if (updateLastModified)
            {
                // set LastModified
                lastModified = await _lastModifiedInjector.CalculateLastModified(
                    new ResourceContext(httpContext.Request, storeKey, savedResponse));
            }
            else
            {
                lastModified = savedResponse.LastModified;
            }

            var lastModifiedValue = await _dateParser.LastModifiedToString(lastModified);
            headers[HeaderNames.LastModified] = lastModifiedValue;
            logInformation += $"Last-Modified: {lastModifiedValue}.";

            _logger.LogInformation($"Generation done. {logInformation}");
        }

        private async Task GenerateValidationHeadersOnResponse(HttpContext httpContext)
        {
            // only generate validation headers (and only store them) when the
            // response is a level 200 status code.  Invalid responses should 
            // not result in an ETag being stored.  
            if (httpContext.Response.StatusCode != StatusCodes.Status200OK &&
                httpContext.Response.StatusCode != StatusCodes.Status201Created &&
                httpContext.Response.StatusCode != StatusCodes.Status202Accepted &&
                httpContext.Response.StatusCode != StatusCodes.Status203NonAuthoritative &&
                httpContext.Response.StatusCode != StatusCodes.Status204NoContent &&
                httpContext.Response.StatusCode != StatusCodes.Status205ResetContent &&
                httpContext.Response.StatusCode != StatusCodes.Status206PartialContent &&
                httpContext.Response.StatusCode != StatusCodes.Status207MultiStatus &&
                httpContext.Response.StatusCode != StatusCodes.Status208AlreadyReported &&
                httpContext.Response.StatusCode != StatusCodes.Status226IMUsed)
            {
                _logger.LogInformation("Not generating Validation headers as the response status code does not indicate success.");
                return;
            }

            // This takes care of storing new tags, also after a successful PUT/POST/PATCH.
            // Other PUT/POST/PATCH requests must thus include the new ETag as If-Match,
            // otherwise the precondition will fail.
            //
            // If an API returns a 204 No Content after PUT/PATCH, the ETag will be generated
            // from an empty response - any other user/cache must GET the response again
            // before updating it.  Getting it will result in a new ETag value being generated.
            //
            // If an API returns a 200 OK after PUT/PATCH, the ETag will be generated from
            // that response body - if the update was successful but nothing was changed,
            // in those cases the original ETag for other users/caches will still be sufficient.

            // if the response body cannot be read, we can never
            // generate correct ETags (and it should never be cached)
            if (!httpContext.Response.Body.CanRead)
            {
                return;
            }

            _logger.LogInformation("Generating Validation headers.");

            var headers = httpContext.Response.Headers;

            // remove any other ETag and Last-Modified headers (could be set by other pieces of code)
            headers.Remove(HeaderNames.ETag);
            headers.Remove(HeaderNames.LastModified);

            // get the request key
            var storeKey = await _storeKeyGenerator.GenerateStoreKey(
                ConstructStoreKeyContext(httpContext.Request, _validationModelOptions));

            // get the response bytes
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Position = 0;
            }

            var responseBodyContent = new StreamReader(httpContext.Response.Body).ReadToEnd();

            // Calculate the ETag to store in the store.
            var eTag = await _eTagGenerator.GenerateETag(storeKey, responseBodyContent);

            var lastModified = await _lastModifiedInjector.CalculateLastModified(
                new ResourceContext(httpContext.Request, storeKey));
            var lastModifiedValue = await _dateParser.LastModifiedToString(lastModified);

            // store the ETag & LastModified date with the request key as key in the ETag store
            await _store.SetAsync(storeKey, new ValidatorValue(eTag, lastModified));

            // set the ETag and LastModified header
            headers[HeaderNames.ETag] = eTag.ToString();
            headers[HeaderNames.LastModified] = lastModifiedValue;

            _logger.LogInformation($"Validation headers generated. ETag: {eTag.ETagType}, {eTag}. Last-Modified: {lastModifiedValue}");
        }

        private void GenerateVaryHeadersOnResponse(HttpContext httpContext, ValidationModelOptions validationModelOptions)
        {
            // cfr: https://tools.ietf.org/html/rfc7231#section-7.1.4
            // Generate Vary header for response
            // The "Vary" header field in a response describes what parts of a
            // request message, aside from the method, Host header field, and
            // request target, might influence the origin server's process for
            // selecting and representing this response. The value consists of
            // either a single asterisk ("*") or a list of header field names
            // (case-insensitive).

            _logger.LogInformation("Generating Vary header.");

            var headers = httpContext.Response.Headers;

            headers.Remove(HeaderNames.Vary);

            var varyHeaderValue = validationModelOptions.VaryByAll
                ? "*"
                : string.Join(", ", validationModelOptions.Vary);

            headers[HeaderNames.Vary] = varyHeaderValue;

            _logger.LogInformation($"Vary header generated: {varyHeaderValue}.");
        }

        private async Task GenerateExpirationHeadersOnResponse(
            HttpContext httpContext,
            ExpirationModelOptions expirationModelOptions,
            ValidationModelOptions validationModelOptions)
        {
            _logger.LogInformation("Generating expiration headers.");

            var headers = httpContext.Response.Headers;

            // remove current Expires & Cache-Control headers
            headers.Remove(HeaderNames.Expires);
            headers.Remove(HeaderNames.CacheControl);

            // set expiration header (remove milliseconds)
            var expires = DateTimeOffset
                .UtcNow
                .AddSeconds(expirationModelOptions.MaxAge);

            var expiresValue = await _dateParser.ExpiresToString(expires);

            headers[HeaderNames.Expires] = expiresValue;

            var cacheControlHeaderValue = string.Format(
                CultureInfo.InvariantCulture,
                "{0},max-age={1}{2}{3}{4}{5}{6}{7}{8}",
                expirationModelOptions.CacheLocation.ToString().ToLowerInvariant(),
                expirationModelOptions.MaxAge,
                expirationModelOptions.SharedMaxAge == null ? null : ",s-maxage=",
                expirationModelOptions.SharedMaxAge,
                expirationModelOptions.NoStore ? ",no-store" : null,
                expirationModelOptions.NoTransform ? ",no-transform" : null,
                validationModelOptions.NoCache ? ",no-cache" : null,
                validationModelOptions.MustRevalidate ? ",must-revalidate" : null,
                validationModelOptions.ProxyRevalidate ? ",proxy-revalidate" : null);

            headers[HeaderNames.CacheControl] = cacheControlHeaderValue;

            _logger.LogInformation($"Expiration headers generated. Expires: {expiresValue}. Cache-Control: {cacheControlHeaderValue}.");
        }

        private StoreKeyContext ConstructStoreKeyContext(HttpRequest request,
            ValidationModelOptions validationModelOptions)
        {
            return new StoreKeyContext(request, validationModelOptions.Vary, validationModelOptions.VaryByAll);
        }

        private static bool ETagsMatch(
            ETag eTag,
            string eTagToCompare,
            bool useStrongComparisonFunction)
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

                return eTagToCompareIsStrong &&
                       eTag.ETagType == ETagType.Strong &&
                       string.Equals(eTag.ToString(), eTagToCompare, StringComparison.OrdinalIgnoreCase);
            }

            // for weak comparison, we only compare the parts of the eTags after the "W/"
            var firstValueToCompare = eTag.ETagType == ETagType.Weak ? eTag.ToString()[2..] : eTag.ToString();
            var secondValueToCompare = eTagToCompare.StartsWith("W/") ? eTagToCompare[2..] : eTagToCompare;

            return string.Equals(firstValueToCompare, secondValueToCompare, StringComparison.OrdinalIgnoreCase);
        }
    }
}
