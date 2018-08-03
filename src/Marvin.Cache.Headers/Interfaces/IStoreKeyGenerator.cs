// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Marvin.Cache.Headers.Interfaces
{
    public interface IStoreKeyGenerator
    {
        Task<RequestKey> GenerateStoreKey(
            HttpRequest request,
            ValidationModelOptions validationModelOptions);
    }
}

namespace Marvin.Cache.Header
{
    public class StoreKeyGenerator : IStoreKeyGenerator
    {
        public Task<RequestKey> GenerateStoreKey(
            HttpRequest request,
            ValidationModelOptions validationModelOptions)
        {
            // generate a key to store the entity tag with in the entity tag store
            List<string> requestHeaderValues;

            // get the request headers to take into account (VaryBy) & take
            // their values
            if (validationModelOptions.VaryByAll)
            {
                requestHeaderValues = request
                    .Headers
                    .SelectMany(h => h.Value)
                    .ToList();
            }
            else
            {
                requestHeaderValues = request
                    .Headers
                    .Where(x => validationModelOptions.Vary.Any(h => h.Equals(x.Key, StringComparison.CurrentCultureIgnoreCase)))
                    .SelectMany(h => h.Value)
                    .ToList();
            }

            // get the resoure path
            var resourcePath = request.Path.ToString();

            // get the query string
            var queryString = request.QueryString.ToString();

            // combine these
            return Task.FromResult(new RequestKey
            {
                { nameof(resourcePath), resourcePath },
                { nameof(queryString), queryString },
                { nameof(requestHeaderValues), string.Join("-", requestHeaderValues)}
            });
        }
    }
}